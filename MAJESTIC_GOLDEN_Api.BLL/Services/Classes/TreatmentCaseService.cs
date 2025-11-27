using AutoMapper;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class TreatmentCaseService : ITreatmentCaseService
    {
        private readonly ITreatmentCaseRepository _caseRepository;
        private readonly IGenericRepository<CaseTreatment> _caseTreatmentRepository;
        private readonly IGenericRepository<CaseDoctor> _caseDoctorRepository;
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public TreatmentCaseService(
            ITreatmentCaseRepository caseRepository,
            IGenericRepository<CaseTreatment> caseTreatmentRepository,
            IGenericRepository<CaseDoctor> caseDoctorRepository,
            IGenericRepository<Service> serviceRepository,
            IInvoiceService invoiceService,
            IPatientRepository patientRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IAuditLogger auditLogger)
        {
            _caseRepository = caseRepository;
            _caseTreatmentRepository = caseTreatmentRepository;
            _caseDoctorRepository = caseDoctorRepository;
            _serviceRepository = serviceRepository;
            _invoiceService = invoiceService;
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<TreatmentCaseResponseDTO>> CreateCaseAsync(TreatmentCaseRequestDTO request, string createdByDoctorId)
        {
            try
            {
                var patients = await _patientRepository.FindAsync(p => p.UserId == request.PatientId);
                var patient = patients.FirstOrDefault();
                if (patient == null)
                {
                    return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود",
                        new List<string> { $"Patient with UserId '{request.PatientId}' does not exist." }
                    );
                }

                var caseNumber = await _caseRepository.GenerateCaseNumberAsync();

                var treatmentCase = new TreatmentCase
                {
                    CaseNumber = caseNumber,
                    PatientUserId = request.PatientId,
                    BranchId = request.BranchId,
                    Title_En = request.Title_En,
                    Title_Ar = request.Title_Ar,
                    Description_En = request.Description_En ?? "",
                    Description_Ar = request.Description_Ar ?? "",
                    Notes_En = request.Notes_En,
                    Notes_Ar = request.Notes_Ar,
                    NextVisitDate = request.NextVisitDate,
                    Status = TreatmentCaseStatus.Open,
                    CaseDate = DateTime.UtcNow
                };

                await _caseRepository.AddAsync(treatmentCase);

                decimal totalAmount = 0;
                foreach (var treatmentDTO in request.Treatments)
                {
                    var service = await _serviceRepository.GetByIdAsync(treatmentDTO.ServiceId);
                    if (service != null)
                    {
                        var unitPrice = service.BasePrice;
                        var totalPrice = unitPrice * treatmentDTO.Quantity;
                        totalAmount += totalPrice;

                        var caseTreatment = new CaseTreatment
                        {
                            CaseId = treatmentCase.Id,
                            ServiceId = treatmentDTO.ServiceId,
                            DoctorId = treatmentDTO.DoctorId ?? createdByDoctorId,
                            Quantity = treatmentDTO.Quantity,
                            UnitPrice = unitPrice,
                            TotalPrice = totalPrice,
                            Notes_En = treatmentDTO.Notes_En,
                            Notes_Ar = treatmentDTO.Notes_Ar,
                            Status = CaseTreatmentStatus.Pending
                        };

                        await _caseTreatmentRepository.AddAsync(caseTreatment);
                    }
                }

                var doctorIds = new List<string> { createdByDoctorId };
                if (request.DoctorIds != null && request.DoctorIds.Any())
                {
                    doctorIds.AddRange(request.DoctorIds.Where(id => id != createdByDoctorId));
                }

                var validDoctorIds = new List<string>();
                foreach (var doctorId in doctorIds.Distinct())
                {
                    var doctor = await _userRepository.GetUserByIdAsync(doctorId);
                    if (doctor != null)
                    {
                        validDoctorIds.Add(doctorId);
                    }
                }

                if (!validDoctorIds.Any())
                {
                    return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                        "No valid doctors found. Please ensure the doctor IDs are correct.",
                        "لم يتم العثور على أطباء صالحين. يرجى التأكد من صحة معرفات الأطباء."
                    );
                }

                bool isFirst = true;
                foreach (var doctorId in validDoctorIds)
                {
                    var caseDoctor = new CaseDoctor
                    {
                        CaseId = treatmentCase.Id,
                        DoctorId = doctorId,
                        Role_En = isFirst ? "Primary Doctor" : "Treating Doctor",
                        Role_Ar = isFirst ? "طبيب رئيسي" : "طبيب معالج",
                        IsPrimary = isFirst,
                        AssignedDate = DateTime.UtcNow
                    };

                    await _caseDoctorRepository.AddAsync(caseDoctor);
                    isFirst = false;
                }

                if (request.AutoGenerateInvoice && request.Treatments.Any())
                {
                    var invoiceRequest = new InvoiceRequestDTO
                    {
                        PatientId = request.PatientId,
                        DoctorId = createdByDoctorId,
                        Notes_En = $"Auto-generated for case: {caseNumber}",
                        Notes_Ar = $"تم إنشاؤها تلقائياً للحالة: {caseNumber}",
                        Items = request.Treatments.Select(t => new InvoiceItemRequestDTO
                        {
                            ServiceId = t.ServiceId,
                            Quantity = t.Quantity
                        }).ToList()
                    };

                    var invoiceResult = await _invoiceService.CreateInvoiceAsync(invoiceRequest, createdByDoctorId);
                    if (invoiceResult.Success && invoiceResult.Data != null)
                    {
                        treatmentCase.InvoiceId = invoiceResult.Data.Id;
                        await _caseRepository.UpdateAsync(treatmentCase);
                    }
                }

                var createdCase = await _caseRepository.GetByIdWithDetailsAsync(treatmentCase.Id);
                var response = MapToResponseDTO(createdCase!);

                return ApiResponse<TreatmentCaseResponseDTO>.SuccessResponse(
                    response,
                    "Treatment case created successfully",
                    "تم إنشاء الحالة العلاجية بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                    "Failed to create treatment case",
                    "فشل في إنشاء الحالة العلاجية",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<TreatmentCaseResponseDTO>> GetCaseByIdAsync(int id)
        {
            try
            {
                var treatmentCase = await _caseRepository.GetByIdWithDetailsAsync(id);
                if (treatmentCase == null)
                {
                    return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                        "Treatment case not found",
                        "الحالة العلاجية غير موجودة"
                    );
                }

                var response = MapToResponseDTO(treatmentCase);
                return ApiResponse<TreatmentCaseResponseDTO>.SuccessResponse(
                    response,
                    "Treatment case retrieved successfully",
                    "تم استرجاع الحالة العلاجية بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                    "Failed to retrieve treatment case",
                    "فشل في استرجاع الحالة العلاجية",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<TreatmentCaseDetailedResponseDTO>> GetCaseDetailsByIdAsync(int id)
        {
            try
            {
                var treatmentCase = await _caseRepository.GetByIdWithDetailsAsync(id);
                if (treatmentCase == null)
                {
                    return ApiResponse<TreatmentCaseDetailedResponseDTO>.ErrorResponse(
                        "Treatment case not found",
                        "الحالة العلاجية غير موجودة"
                    );
                }

                var response = MapToDetailedResponseDTO(treatmentCase);
                return ApiResponse<TreatmentCaseDetailedResponseDTO>.SuccessResponse(
                    response,
                    "Treatment case details retrieved successfully",
                    "تم استرجاع تفاصيل الحالة العلاجية بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<TreatmentCaseDetailedResponseDTO>.ErrorResponse(
                    "Failed to retrieve treatment case details",
                    "فشل في استرجاع تفاصيل الحالة العلاجية",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetCasesByPatientIdAsync(string patientUserId)
        {
            try
            {
                var cases = await _caseRepository.GetByPatientIdAsync(patientUserId);
                var response = cases.Select(MapToResponseDTO).ToList();
                
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.SuccessResponse(
                    response,
                    "Patient cases retrieved successfully",
                    "تم استرجاع حالات المريض بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.ErrorResponse(
                    "Failed to retrieve patient cases",
                    "فشل في استرجاع حالات المريض",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetCasesByDoctorIdAsync(string doctorId)
        {
            try
            {
                var cases = await _caseRepository.GetByDoctorIdAsync(doctorId);
                var response = cases.Select(MapToResponseDTO).ToList();
                
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.SuccessResponse(
                    response,
                    "Doctor cases retrieved successfully",
                    "تم استرجاع حالات الطبيب بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.ErrorResponse(
                    "Failed to retrieve doctor cases",
                    "فشل في استرجاع حالات الطبيب",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetCasesByStatusAsync(string status)
        {
            try
            {
                var cases = await _caseRepository.GetByStatusAsync(status);
                var response = cases.Select(MapToResponseDTO).ToList();
                
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.SuccessResponse(
                    response,
                    "Cases retrieved successfully",
                    "تم استرجاع الحالات بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.ErrorResponse(
                    "Failed to retrieve cases",
                    "فشل في استرجاع الحالات",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetUpcomingVisitsAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var cases = await _caseRepository.GetUpcomingVisitsAsync(fromDate, toDate);
                var response = cases.Select(MapToResponseDTO).ToList();
                
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.SuccessResponse(
                    response,
                    "Upcoming visits retrieved successfully",
                    "تم استرجاع الزيارات القادمة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>.ErrorResponse(
                    "Failed to retrieve upcoming visits",
                    "فشل في استرجاع الزيارات القادمة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<TreatmentCaseResponseDTO>> UpdateCaseAsync(int id, TreatmentCaseRequestDTO request)
        {
            try
            {
                var treatmentCase = await _caseRepository.GetByIdAsync(id);
                if (treatmentCase == null)
                {
                    return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                        "Treatment case not found",
                        "الحالة العلاجية غير موجودة"
                    );
                }

                var oldValues = new
                {
                    treatmentCase.Title_En,
                    treatmentCase.Title_Ar,
                    treatmentCase.Description_En,
                    treatmentCase.Description_Ar,
                    treatmentCase.Notes_En,
                    treatmentCase.Notes_Ar,
                    treatmentCase.NextVisitDate
                };

                treatmentCase.Title_En = request.Title_En;
                treatmentCase.Title_Ar = request.Title_Ar;
                treatmentCase.Description_En = request.Description_En ?? "";
                treatmentCase.Description_Ar = request.Description_Ar ?? "";
                treatmentCase.Notes_En = request.Notes_En;
                treatmentCase.Notes_Ar = request.Notes_Ar;
                treatmentCase.NextVisitDate = request.NextVisitDate;

                await _caseRepository.UpdateAsync(treatmentCase);

                var updatedCase = await _caseRepository.GetByIdWithDetailsAsync(id);
                var response = MapToResponseDTO(updatedCase!);

                await _auditLogger.LogAsync(
                    "Update",
                    nameof(TreatmentCase),
                    treatmentCase.Id.ToString(),
                    oldValues: oldValues,
                    newValues: response);

                return ApiResponse<TreatmentCaseResponseDTO>.SuccessResponse(
                    response,
                    "Treatment case updated successfully",
                    "تم تحديث الحالة العلاجية بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                    "Failed to update treatment case",
                    "فشل في تحديث الحالة العلاجية",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<TreatmentCaseResponseDTO>> UpdateCaseStatusAsync(int id, UpdateCaseStatusDTO request)
        {
            try
            {
                var treatmentCase = await _caseRepository.GetByIdAsync(id);
                if (treatmentCase == null)
                {
                    return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                        "Treatment case not found",
                        "الحالة العلاجية غير موجودة"
                    );
                }

                var oldValues = new
                {
                    treatmentCase.Status,
                    treatmentCase.Notes_En,
                    treatmentCase.Notes_Ar,
                    treatmentCase.NextVisitDate,
                    treatmentCase.CompletedDate
                };

                treatmentCase.Status = EnumExtensions.ParseEnum<TreatmentCaseStatus>(request.Status);
                treatmentCase.Notes_En = request.Notes_En ?? treatmentCase.Notes_En;
                treatmentCase.Notes_Ar = request.Notes_Ar ?? treatmentCase.Notes_Ar;
                treatmentCase.NextVisitDate = request.NextVisitDate ?? treatmentCase.NextVisitDate;

                if (request.Status == "Completed")
                {
                    treatmentCase.CompletedDate = DateTime.UtcNow;
                }

                await _caseRepository.UpdateAsync(treatmentCase);

                var updatedCase = await _caseRepository.GetByIdWithDetailsAsync(id);
                var response = MapToResponseDTO(updatedCase!);

                await _auditLogger.LogAsync(
                    "UpdateStatus",
                    nameof(TreatmentCase),
                    treatmentCase.Id.ToString(),
                    oldValues: oldValues,
                    newValues: response);

                return ApiResponse<TreatmentCaseResponseDTO>.SuccessResponse(
                    response,
                    "Treatment case status updated successfully",
                    "تم تحديث حالة الحالة العلاجية بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<TreatmentCaseResponseDTO>.ErrorResponse(
                    "Failed to update treatment case status",
                    "فشل في تحديث حالة الحالة العلاجية",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteCaseAsync(int id)
        {
            try
            {
                var treatmentCase = await _caseRepository.GetByIdAsync(id);
                if (treatmentCase == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Treatment case not found",
                        "الحالة العلاجية غير موجودة"
                    );
                }

                await _caseRepository.RemoveAsync(treatmentCase);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Treatment case deleted successfully",
                    "تم حذف الحالة العلاجية بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete treatment case",
                    "فشل في حذف الحالة العلاجية",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> AddDoctorToCaseAsync(int caseId, string doctorId, bool isPrimary = false)
        {
            try
            {
                // Validate that the doctor exists
                var doctor = await _userRepository.GetUserByIdAsync(doctorId);
                if (doctor == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Doctor not found. Please ensure the doctor ID is correct.",
                        "الطبيب غير موجود. يرجى التأكد من صحة معرف الطبيب."
                    );
                }

                // Validate that the case exists
                var treatmentCase = await _caseRepository.GetByIdAsync(caseId);
                if (treatmentCase == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Treatment case not found",
                        "الحالة العلاجية غير موجودة"
                    );
                }

                // Check if doctor is already assigned to this case
                var existingAssignment = await _caseDoctorRepository.FindAsync(cd => cd.CaseId == caseId && cd.DoctorId == doctorId);
                if (existingAssignment.Any())
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Doctor is already assigned to this case",
                        "الطبيب مُعيّن بالفعل لهذه الحالة"
                    );
                }

                var caseDoctor = new CaseDoctor
                {
                    CaseId = caseId,
                    DoctorId = doctorId,
                    Role_En = isPrimary ? "Primary Doctor" : "Treating Doctor",
                    Role_Ar = isPrimary ? "طبيب رئيسي" : "طبيب معالج",
                    IsPrimary = isPrimary,
                    AssignedDate = DateTime.UtcNow
                };

                await _caseDoctorRepository.AddAsync(caseDoctor);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Doctor added to case successfully",
                    "تمت إضافة الطبيب للحالة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to add doctor to case",
                    "فشل في إضافة الطبيب للحالة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> RemoveDoctorFromCaseAsync(int caseId, string doctorId)
        {
            try
            {
                var caseDoctors = await _caseDoctorRepository.FindAsync(cd => cd.CaseId == caseId && cd.DoctorId == doctorId);
                var caseDoctor = caseDoctors.FirstOrDefault();

                if (caseDoctor == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Doctor not found in case",
                        "الطبيب غير موجود في الحالة"
                    );
                }

                await _caseDoctorRepository.RemoveAsync(caseDoctor);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Doctor removed from case successfully",
                    "تمت إزالة الطبيب من الحالة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to remove doctor from case",
                    "فشل في إزالة الطبيب من الحالة",
                    new List<string> { ex.Message }
                );
            }
        }

        private TreatmentCaseResponseDTO MapToResponseDTO(TreatmentCase treatmentCase)
        {
            return new TreatmentCaseResponseDTO
            {
                Id = treatmentCase.Id,
                CaseNumber = treatmentCase.CaseNumber,
                PatientId = treatmentCase.PatientUserId,
                PatientName_En = treatmentCase.Patient?.User?.FullName_En ?? "",
                PatientName_Ar = treatmentCase.Patient?.User?.FullName_Ar ?? "",
                PatientPhone = treatmentCase.Patient?.User?.PhoneNumber ?? "",
                BranchId = treatmentCase.BranchId,
                BranchName_En = treatmentCase.Branch?.Name_En ?? "",
                BranchName_Ar = treatmentCase.Branch?.Name_Ar ?? "",
                Title_En = treatmentCase.Title_En,
                Title_Ar = treatmentCase.Title_Ar,
                Description_En = treatmentCase.Description_En,
                Description_Ar = treatmentCase.Description_Ar,
                Status = treatmentCase.Status.GetDescription(),
                CaseDate = treatmentCase.CaseDate,
                CompletedDate = treatmentCase.CompletedDate,
                NextVisitDate = treatmentCase.NextVisitDate,
                Notes_En = treatmentCase.Notes_En,
                Notes_Ar = treatmentCase.Notes_Ar,
                InvoiceId = treatmentCase.InvoiceId,
                InvoiceNumber = treatmentCase.Invoice?.InvoiceNumber,
                TotalAmount = treatmentCase.Treatments?.Sum(t => t.TotalPrice) ?? 0,
                Treatments = treatmentCase.Treatments?.Select(t => new CaseTreatmentResponseDTO
                {
                    Id = t.Id,
                    CaseId = t.CaseId,
                    ServiceId = t.ServiceId,
                    ServiceName_En = t.Service?.Name_En ?? "",
                    ServiceName_Ar = t.Service?.Name_Ar ?? "",
                    DoctorId = t.DoctorId,
                    DoctorName = t.Doctor?.FullName,
                    Quantity = t.Quantity,
                    UnitPrice = t.UnitPrice,
                    TotalPrice = t.TotalPrice,
                    Notes_En = t.Notes_En,
                    Notes_Ar = t.Notes_Ar,
                    TreatmentDate = t.TreatmentDate,
                    Status = t.Status.GetDescription(),
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList() ?? new List<CaseTreatmentResponseDTO>(),
                Doctors = treatmentCase.CaseDoctors?.Select(cd => new CaseDoctorResponseDTO
                {
                    DoctorId = cd.DoctorId,
                    DoctorName = cd.Doctor?.FullName ?? "",
                    Role_En = cd.Role_En,
                    Role_Ar = cd.Role_Ar,
                    AssignedDate = cd.AssignedDate,
                    IsPrimary = cd.IsPrimary
                }).ToList() ?? new List<CaseDoctorResponseDTO>(),
                CreatedAt = treatmentCase.CreatedAt,
                UpdatedAt = treatmentCase.UpdatedAt
            };
        }

        private TreatmentCaseDetailedResponseDTO MapToDetailedResponseDTO(TreatmentCase treatmentCase)
        {
            var baseResponse = MapToResponseDTO(treatmentCase);
            return new TreatmentCaseDetailedResponseDTO
            {
                Id = baseResponse.Id,
                CaseNumber = baseResponse.CaseNumber,
                PatientId = baseResponse.PatientId,
                PatientName_En = baseResponse.PatientName_En,
                PatientName_Ar = baseResponse.PatientName_Ar,
                PatientPhone = baseResponse.PatientPhone,
                BranchId = baseResponse.BranchId,
                BranchName_En = baseResponse.BranchName_En,
                BranchName_Ar = baseResponse.BranchName_Ar,
                Title_En = baseResponse.Title_En,
                Title_Ar = baseResponse.Title_Ar,
                Description_En = baseResponse.Description_En,
                Description_Ar = baseResponse.Description_Ar,
                Status = baseResponse.Status,
                CaseDate = baseResponse.CaseDate,
                CompletedDate = baseResponse.CompletedDate,
                NextVisitDate = baseResponse.NextVisitDate,
                Notes_En = baseResponse.Notes_En,
                Notes_Ar = baseResponse.Notes_Ar,
                InvoiceId = baseResponse.InvoiceId,
                InvoiceNumber = baseResponse.InvoiceNumber,
                TotalAmount = baseResponse.TotalAmount,
                Treatments = baseResponse.Treatments,
                Doctors = baseResponse.Doctors,
                CreatedAt = baseResponse.CreatedAt,
                UpdatedAt = baseResponse.UpdatedAt,
                PatientDetails = treatmentCase.Patient != null ? _mapper.Map<PatientResponseDTO>(treatmentCase.Patient) : null,
                InvoiceDetails = treatmentCase.Invoice != null ? _mapper.Map<InvoiceResponseDTO>(treatmentCase.Invoice) : null
            };
        }
    }
}

