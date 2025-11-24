using AutoMapper;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Migrations;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class LabRequestService : ILabRequestService
    {
        private readonly ILabRequestRepository _labRequestRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILaboratoryRepository _laboratoryRepository;
        private readonly IAuditLogger _auditLogger;
        private readonly IMapper _mapper;

        public LabRequestService(
            ILabRequestRepository labRequestRepository,
            IPatientRepository patientRepository,
            ILaboratoryRepository laboratoryRepository,
            IAuditLogger auditLogger,
            IMapper mapper)
        {
            _labRequestRepository = labRequestRepository;
            _patientRepository = patientRepository;
            _laboratoryRepository = laboratoryRepository;
            _auditLogger = auditLogger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<LabRequestResponseDTO>> CreateLabRequestAsync(LabRequestCreateDTO request, string doctorId, string? doctorName)
        {
            try
            {
                var patients = await _patientRepository.FindAsync(p => p.UserId == request.PatientId);
                var patient = patients.FirstOrDefault();
                if (patient == null)
                {
                    return ApiResponse<LabRequestResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود",
                        new List<string> { $"Patient with UserId '{request.PatientId}' does not exist." }
                    );
                }

                var laboratory = await _laboratoryRepository.GetByIdWithUserAsync(request.LaboratoryId);
                if (laboratory == null)
                {
                    return ApiResponse<LabRequestResponseDTO>.ErrorResponse(
                        "Laboratory not found",
                        "المختبر غير موجود",
                        new List<string> { $"Laboratory with Id '{request.LaboratoryId}' does not exist." }
                    );
                }

                var labRequest = _mapper.Map<LabRequest>(request);
               
                labRequest.DoctorId = doctorId;
                labRequest.RequestNumber = await _labRequestRepository.GenerateRequestNumberAsync();
                labRequest.RequestDate = DateTime.UtcNow;
                labRequest.Status = LabRequestStatus.Pending;

                await _labRequestRepository.AddAsync(labRequest);

                var fullRequest = await _labRequestRepository.GetLabRequestWithDetailsAsync(labRequest.Id);
                var response = _mapper.Map<LabRequestResponseDTO>(fullRequest);
                response.LabName= laboratory.User.FullName_En;
                response.PatientId= patient.UserId;
                await LogAuditAsync(
                    "Create",
                    doctorId,
                    doctorName,
                    labRequest.Id.ToString(),
                    null,
                    response);

                return ApiResponse<LabRequestResponseDTO>.SuccessResponse(
                    response,
                    "Lab request created successfully",
                    "تم إنشاء طلب المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<LabRequestResponseDTO>.ErrorResponse(
                    "Failed to create lab request",
                    "فشل في إنشاء طلب المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<LabRequestResponseDTO>> UpdateLabRequestAsync(int id, LabRequestUpdateDTO request, string userId, string? userName)
        {
            try
            {
                var labRequest = await _labRequestRepository.GetByIdAsync(id);
                if (labRequest == null)
                {
                    return ApiResponse<LabRequestResponseDTO>.ErrorResponse(
                        "Lab request not found",
                        "طلب المختبر غير موجود"
                    );
                }

                var oldSnapshot = new
                {
                    labRequest.Id,
                    labRequest.Status,
                    labRequest.ExpectedDate,
                    labRequest.ReceivedDate,
                    labRequest.Cost,
                    labRequest.Notes_En,
                    labRequest.Notes_Ar
                };

                _mapper.Map(request, labRequest);

                if (request.Status == "Received")
                {
                    labRequest.ReceivedDate = DateTime.UtcNow;
                }

                await _labRequestRepository.UpdateAsync(labRequest);

                var fullRequest = await _labRequestRepository.GetLabRequestWithDetailsAsync(id);
                var response = _mapper.Map<LabRequestResponseDTO>(fullRequest);

                await LogAuditAsync(
                    "Update",
                    userId,
                    userName,
                    id.ToString(),
                    oldSnapshot,
                    response);

                return ApiResponse<LabRequestResponseDTO>.SuccessResponse(
                    response,
                    "Lab request updated successfully",
                    "تم تحديث طلب المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<LabRequestResponseDTO>.ErrorResponse(
                    "Failed to update lab request",
                    "فشل في تحديث طلب المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<LabRequestResponseDTO>> GetLabRequestByIdAsync(int id)
        {
            try
            {
                var labRequest = await _labRequestRepository.GetLabRequestWithDetailsAsync(id);
                if (labRequest == null)
                {
                    return ApiResponse<LabRequestResponseDTO>.ErrorResponse(
                        "Lab request not found",
                        "طلب المختبر غير موجود"
                    );
                }

                var response = _mapper.Map<LabRequestResponseDTO>(labRequest);
                return ApiResponse<LabRequestResponseDTO>.SuccessResponse(
                    response,
                    "Lab request retrieved successfully",
                    "تم استرجاع طلب المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<LabRequestResponseDTO>.ErrorResponse(
                    "Failed to retrieve lab request",
                    "فشل في استرجاع طلب المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetLabRequestsByDoctorAsync(string doctorId)
        {
            try
            {
                var labRequests = await _labRequestRepository.GetLabRequestsByDoctorAsync(doctorId);
                var response = _mapper.Map<IEnumerable<LabRequestResponseDTO>>(labRequests);

                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.SuccessResponse(
                    response,
                    "Lab requests retrieved successfully",
                    "تم استرجاع طلبات المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.ErrorResponse(
                    "Failed to retrieve lab requests",
                    "فشل في استرجاع طلبات المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetLabRequestsByPatientAsync(string patientUserId)
        {
            try
            {
                var labRequests = await _labRequestRepository.GetLabRequestsByPatientAsync(patientUserId);
                var response = _mapper.Map<IEnumerable<LabRequestResponseDTO>>(labRequests);

                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.SuccessResponse(
                    response,
                    "Lab requests retrieved successfully",
                    "تم استرجاع طلبات المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.ErrorResponse(
                    "Failed to retrieve lab requests",
                    "فشل في استرجاع طلبات المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetLabRequestsByLaboratoryAsync(int laboratoryId)
        {
            try
            {
                var labRequests = await _labRequestRepository.GetLabRequestsByLaboratoryAsync(laboratoryId);
                var response = _mapper.Map<IEnumerable<LabRequestResponseDTO>>(labRequests);

                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.SuccessResponse(
                    response,
                    "Lab requests retrieved successfully",
                    "تم استرجاع طلبات المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.ErrorResponse(
                    "Failed to retrieve lab requests",
                    "فشل في استرجاع طلبات المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetPendingLabRequestsAsync()
        {
            try
            {
                var labRequests = await _labRequestRepository.GetPendingLabRequestsAsync();
                var response = _mapper.Map<IEnumerable<LabRequestResponseDTO>>(labRequests);

                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.SuccessResponse(
                    response,
                    "Pending lab requests retrieved successfully",
                    "تم استرجاع طلبات المختبر المعلقة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LabRequestResponseDTO>>.ErrorResponse(
                    "Failed to retrieve pending lab requests",
                    "فشل في استرجاع طلبات المختبر المعلقة",
                    new List<string> { ex.Message }
                );
            }
        }

        private async Task LogAuditAsync(string action, string? userId, string? userName, string entityId, object? oldValues, object? newValues)
        {
            await _auditLogger.LogAsync(
                action,
                nameof(LabRequest),
                entityId,
                userId,
                userName,
                oldValues,
                newValues);
        }
    }
}

