using AutoMapper;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class CaseTransferService : ICaseTransferService
    {
        private readonly IGenericRepository<CaseTransfer> _caseTransferRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public CaseTransferService(IGenericRepository<CaseTransfer> caseTransferRepository, IPatientRepository patientRepository, IMapper mapper, IAuditLogger auditLogger)
        {
            _caseTransferRepository = caseTransferRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<CaseTransferResponseDTO>> CreateCaseTransferAsync(CaseTransferRequestDTO request, string fromDoctorId)
        {
            try
            {
                // Validate that the patient exists
                var patients = await _patientRepository.FindAsync(p => p.UserId == request.PatientId);
                var patient = patients.FirstOrDefault();
                if (patient == null)
                {
                    return ApiResponse<CaseTransferResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود",
                        new List<string> { $"Patient with UserId '{request.PatientId}' does not exist." }
                    );
                }

                var caseTransfer = _mapper.Map<CaseTransfer>(request);
                caseTransfer.FromDoctorId = fromDoctorId;
                caseTransfer.TransferDate = DateTime.UtcNow;
                caseTransfer.Status = "Pending";

                await _caseTransferRepository.AddAsync(caseTransfer);

                var response = _mapper.Map<CaseTransferResponseDTO>(caseTransfer);
                return ApiResponse<CaseTransferResponseDTO>.SuccessResponse(
                    response,
                    "Case transferred successfully",
                    "تم تحويل الحالة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CaseTransferResponseDTO>.ErrorResponse(
                    "Failed to transfer case",
                    "فشل في تحويل الحالة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<CaseTransferResponseDTO>> UpdateCaseTransferStatusAsync(int id, CaseTransferStatusDTO request)
        {
            try
            {
                var caseTransfer = await _caseTransferRepository.GetByIdAsync(id);
                if (caseTransfer == null)
                {
                    return ApiResponse<CaseTransferResponseDTO>.ErrorResponse(
                        "Case transfer not found",
                        "تحويل الحالة غير موجود"
                    );
                }

                var oldValues = new
                {
                    caseTransfer.Status,
                    caseTransfer.Notes_En,
                    caseTransfer.Notes_Ar
                };

                caseTransfer.Status = request.Status;
                if (request.Notes_En != null) caseTransfer.Notes_En = request.Notes_En;
                if (request.Notes_Ar != null) caseTransfer.Notes_Ar = request.Notes_Ar;

                await _caseTransferRepository.UpdateAsync(caseTransfer);

                var response = _mapper.Map<CaseTransferResponseDTO>(caseTransfer);

                var newValues = new
                {
                    caseTransfer.Status,
                    caseTransfer.Notes_En,
                    caseTransfer.Notes_Ar
                };

                await _auditLogger.LogAsync(
                    "UpdateStatus",
                    nameof(CaseTransfer),
                    caseTransfer.Id.ToString(),
                    oldValues: oldValues,
                    newValues: newValues);

                return ApiResponse<CaseTransferResponseDTO>.SuccessResponse(
                    response,
                    "Case transfer status updated successfully",
                    "تم تحديث حالة تحويل الحالة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CaseTransferResponseDTO>.ErrorResponse(
                    "Failed to update case transfer status",
                    "فشل في تحديث حالة تحويل الحالة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<CaseTransferResponseDTO>> GetCaseTransferByIdAsync(int id)
        {
            try
            {
                var caseTransfer = await _caseTransferRepository.GetByIdAsync(id);
                if (caseTransfer == null)
                {
                    return ApiResponse<CaseTransferResponseDTO>.ErrorResponse(
                        "Case transfer not found",
                        "تحويل الحالة غير موجود"
                    );
                }

                var response = _mapper.Map<CaseTransferResponseDTO>(caseTransfer);
                return ApiResponse<CaseTransferResponseDTO>.SuccessResponse(
                    response,
                    "Case transfer retrieved successfully",
                    "تم استرجاع تحويل الحالة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<CaseTransferResponseDTO>.ErrorResponse(
                    "Failed to retrieve case transfer",
                    "فشل في استرجاع تحويل الحالة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<CaseTransferResponseDTO>>> GetCaseTransfersByPatientAsync(string patientUserId)
        {
            try
            {
                var caseTransfers = await _caseTransferRepository.FindAsync(ct => ct.PatientUserId == patientUserId);
                var response = _mapper.Map<IEnumerable<CaseTransferResponseDTO>>(caseTransfers);

                return ApiResponse<IEnumerable<CaseTransferResponseDTO>>.SuccessResponse(
                    response,
                    "Case transfers retrieved successfully",
                    "تم استرجاع تحويلات الحالة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CaseTransferResponseDTO>>.ErrorResponse(
                    "Failed to retrieve case transfers",
                    "فشل في استرجاع تحويلات الحالة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<CaseTransferResponseDTO>>> GetCaseTransfersByDoctorAsync(string doctorId)
        {
            try
            {
                var caseTransfers = await _caseTransferRepository.FindAsync(ct => 
                    ct.FromDoctorId == doctorId || ct.ToDoctorId == doctorId);
                var response = _mapper.Map<IEnumerable<CaseTransferResponseDTO>>(caseTransfers);

                return ApiResponse<IEnumerable<CaseTransferResponseDTO>>.SuccessResponse(
                    response,
                    "Case transfers retrieved successfully",
                    "تم استرجاع تحويلات الحالة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CaseTransferResponseDTO>>.ErrorResponse(
                    "Failed to retrieve case transfers",
                    "فشل في استرجاع تحويلات الحالة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<CaseTransferResponseDTO>>> GetPendingCaseTransfersAsync(string doctorId)
        {
            try
            {
                var caseTransfers = await _caseTransferRepository.FindAsync(ct => 
                    ct.ToDoctorId == doctorId && ct.Status == "Pending");
                var response = _mapper.Map<IEnumerable<CaseTransferResponseDTO>>(caseTransfers);

                return ApiResponse<IEnumerable<CaseTransferResponseDTO>>.SuccessResponse(
                    response,
                    "Pending case transfers retrieved successfully",
                    "تم استرجاع تحويلات الحالة المعلقة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CaseTransferResponseDTO>>.ErrorResponse(
                    "Failed to retrieve pending case transfers",
                    "فشل في استرجاع تحويلات الحالة المعلقة",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}



