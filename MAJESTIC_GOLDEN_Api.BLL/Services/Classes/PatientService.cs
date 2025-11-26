using AutoMapper;
using Azure.Core;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Net.Mail;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogger _auditLogger;

        public PatientService(IPatientRepository patientRepository, IFileRepository fileRepository, IFileService fileService, IMapper mapper, UserManager<ApplicationUser> userManager, IAuditLogger auditLogger)
        {
            _patientRepository = patientRepository;
            _fileRepository = fileRepository;
            _fileService = fileService;
            _mapper = mapper;
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<PatientResponseDTO>> CreatePatientAsync(PatientRequestDTO request)
        {
            try
            {

                if (!string.IsNullOrEmpty(request.Email))
                {
                    var existingUser = await _userManager.FindByEmailAsync(request.Email);
                    if (existingUser != null)
                    {
                        return ApiResponse<PatientResponseDTO>.ErrorResponse(
                            "User with this email already exists",
                            "مستخدم بهذا البريد الإلكتروني موجود بالفعل"
                        );
                    }
                }

                Gender gender;
                if (!Enum.TryParse<Gender>(request.Gender, true, out gender))
                {
                    gender = Gender.Male; 
                }

                
                var uniqueUsername = $"{request.Phone}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                
                var user = new ApplicationUser
                {
                    UserName = uniqueUsername, // Unique username with timestamp
                    Email = request.Email,
                    PhoneNumber = request.Phone,
                    FullName_En = request.FullName_En,
                    FullName_Ar = request.FullName_Ar,
                    Gender = gender,
                    DateOfBirth = request.DateOfBirth,
                    Address_En = request.Address_En,
                    Address_Ar = request.Address_Ar,
                    Occupation_En = request.Occupation_En,
                    Occupation_Ar = request.Occupation_Ar,
                    BranchId = request.BranchId,
                    EmailConfirmed = true, 
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

               
                if (!string.IsNullOrEmpty(request.MaritalStatus_En))
                {
                    MaritalStatus maritalStatus;
                    if (Enum.TryParse<MaritalStatus>(request.MaritalStatus_En, true, out maritalStatus))
                    {
                        user.MaritalStatus = maritalStatus;
                    }
                }

                var defaultPassword = $"Patient@{request.Phone}"; 
                var createUserResult = await _userManager.CreateAsync(user, defaultPassword);

                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<PatientResponseDTO>.ErrorResponse(
                        "Failed to create user account",
                        "فشل في إنشاء حساب المستخدم",
                        errors
                    );
                }
                await _userManager.AddToRoleAsync(user, "Patient");
                var patient = new Patient
                {
                    UserId = user.Id,
                    TreatmentPlan_En = request.TreatmentPlan_En,
                    TreatmentPlan_Ar = request.TreatmentPlan_Ar,
                    MedicalHistory_En = request.MedicalHistory_En,
                    MedicalHistory_Ar = request.MedicalHistory_Ar,
                    Allergies_En = request.Allergies_En,
                    Allergies_Ar = request.Allergies_Ar,
                    User = user
                };

                await _patientRepository.AddAsync(patient);
                var createdPatient = await _patientRepository.GetPatientWithDetailsAsync(user.Id);
                var response = _mapper.Map<PatientResponseDTO>(createdPatient);
                
                return ApiResponse<PatientResponseDTO>.SuccessResponse(
                    response,
                    "Patient created successfully",
                    "تم إضافة المريض بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PatientResponseDTO>.ErrorResponse(
                    "Failed to create patient",
                    "فشل في إضافة المريض",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<PatientResponseDTO>> UpdatePatientAsync(string userId, PatientRequestDTO request)
        {
            try
            {
                var patient = await _patientRepository.GetPatientForUpdateAsync(userId);
                if (patient == null)
                {
                    return ApiResponse<PatientResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                var user = patient.User;
                if (user == null)
                {
                    return ApiResponse<PatientResponseDTO>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    );
                }

                var oldValues = _mapper.Map<PatientResponseDTO>(patient);

                Gender gender;
                if (Enum.TryParse<Gender>(request.Gender, true, out gender))
                {
                    user.Gender = gender;
                }

                user.FullName_En = request.FullName_En;
                user.FullName_Ar = request.FullName_Ar;
                user.PhoneNumber = request.Phone;
                user.Email = request.Email;
                user.DateOfBirth = request.DateOfBirth;
                user.Address_En = request.Address_En;
                user.Address_Ar = request.Address_Ar;
                user.Occupation_En = request.Occupation_En;
                user.Occupation_Ar = request.Occupation_Ar;
                user.BranchId = request.BranchId;

                if (!string.IsNullOrEmpty(request.MaritalStatus_En))
                {
                    MaritalStatus maritalStatus;
                    if (Enum.TryParse<MaritalStatus>(request.MaritalStatus_En, true, out maritalStatus))
                    {
                        user.MaritalStatus = maritalStatus;
                    }
                }

                await _userManager.UpdateAsync(user);

                patient.TreatmentPlan_En = request.TreatmentPlan_En;
                patient.TreatmentPlan_Ar = request.TreatmentPlan_Ar;
                patient.MedicalHistory_En = request.MedicalHistory_En;
                patient.MedicalHistory_Ar = request.MedicalHistory_Ar;
                patient.Allergies_En = request.Allergies_En;
                patient.Allergies_Ar = request.Allergies_Ar;

                await _patientRepository.UpdateAsync(patient);

                var updatedPatient = await _patientRepository.GetPatientWithDetailsAsync(userId);
                var response = _mapper.Map<PatientResponseDTO>(updatedPatient);

                await _auditLogger.LogAsync(
                    "Update",
                    nameof(Patient),
                    patient.UserId,
                    oldValues: oldValues,
                    newValues: response);
                
                return ApiResponse<PatientResponseDTO>.SuccessResponse(
                    response,
                    "Patient updated successfully",
                    "تم تحديث بيانات المريض بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PatientResponseDTO>.ErrorResponse(
                    "Failed to update patient",
                    "فشل في تحديث بيانات المريض",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeletePatientAsync(string userId)
        {
            try
            {
                var patient = await _patientRepository.GetPatientWithDetailsAsync(userId);
                if (patient == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                await _patientRepository.RemoveAsync(patient);
                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Patient deleted successfully",
                    "تم حذف المريض بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete patient",
                    "فشل في حذف المريض",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<PatientDetailedResponseDTO>> GetPatientByIdAsync(string userId)
        {
            try
            {
                var patient = await _patientRepository.GetPatientWithDetailsAsync(userId);
                if (patient == null)
                {
                    return ApiResponse<PatientDetailedResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                var response = _mapper.Map<PatientDetailedResponseDTO>(patient);
                response.Age = patient.User?.Age ?? 0;
                response.TotalAppointments = patient.Appointments?.Count ?? 0;
                response.TotalInvoices = patient.Invoices?.Count ?? 0;
                response.TotalDebt = patient.Debts?.Sum(d => d.AmountDue) ?? 0;
                response.TotalTeethRecords = patient.PatientTeeth?.Count ?? 0;
                response.LastVisit = patient.Appointments?.OrderByDescending(a => a.AppointmentDateTime)
                    .FirstOrDefault()?.AppointmentDateTime;

                return ApiResponse<PatientDetailedResponseDTO>.SuccessResponse(
                    response,
                    "Patient retrieved successfully",
                    "تم استرجاع بيانات المريض بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PatientDetailedResponseDTO>.ErrorResponse(
                    "Failed to retrieve patient",
                    "فشل في استرجاع بيانات المريض",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<PatientResponseDTO>>> GetAllPatientsAsync()
        {
            try
            {
                var patients = await _patientRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<PatientResponseDTO>>(patients);

                return ApiResponse<IEnumerable<PatientResponseDTO>>.SuccessResponse(
                    response,
                    "Patients retrieved successfully",
                    "تم استرجاع المرضى بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<PatientResponseDTO>>.ErrorResponse(
                    "Failed to retrieve patients",
                    "فشل في استرجاع المرضى",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<PatientResponseDTO>>> GetPatientsByBranchAsync(int branchId)
        {
            try
            {
                var patients = await _patientRepository.GetPatientsByBranchAsync(branchId);
                var response = _mapper.Map<IEnumerable<PatientResponseDTO>>(patients);

                return ApiResponse<IEnumerable<PatientResponseDTO>>.SuccessResponse(
                    response,
                    "Patients retrieved successfully",
                    "تم استرجاع المرضى بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<PatientResponseDTO>>.ErrorResponse(
                    "Failed to retrieve patients",
                    "فشل في استرجاع المرضى",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<PatientResponseDTO>>> SearchPatientsAsync(string searchTerm)
        {
            try
            {
                var patients = await _patientRepository.SearchPatientsAsync(searchTerm);
                var response = _mapper.Map<IEnumerable<PatientResponseDTO>>(patients);

                return ApiResponse<IEnumerable<PatientResponseDTO>>.SuccessResponse(
                    response,
                    "Search completed successfully",
                    "تم البحث بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<PatientResponseDTO>>.ErrorResponse(
                    "Failed to search patients",
                    "فشل في البحث عن المرضى",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> AdminResetPasswordAsync(string userId, AdminResetPasswordRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var user_role = await _userManager.GetRolesAsync(user);
                var its_Patient = false;
                foreach (var r in user_role ) {
                    if (r == "Patient") {
                        its_Patient=true; break;
                    }
                }

                if (user == null || !its_Patient)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    );
                }

                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    var errors = removePasswordResult.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Failed to reset password",
                        "فشل في إعادة تعيين كلمة المرور",
                        errors
                    );
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    var errors = addPasswordResult.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Failed to set new password",
                        "فشل في تعيين كلمة المرور الجديدة",
                        errors
                    );
                }

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Password reset successfully",
                    "تم إعادة تعيين كلمة المرور بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to reset password",
                    "فشل في إعادة تعيين كلمة المرور",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<UploadFileResponseDTO>> UploadFileToPatientAsync(string patientUserId, IFormFile file, UploadFileRequestDTO request, string? uploadedById = null)
        {
            try
            {
                var patient = await _patientRepository.GetPatientWithDetailsAsync(patientUserId);
                if (patient == null)
                {
                    return ApiResponse<UploadFileResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                if (file == null || file.Length == 0)
                {
                    return ApiResponse<UploadFileResponseDTO>.ErrorResponse(
                        "File is required and cannot be empty",
                        "الملف مطلوب ولا يمكن أن يكون فارغًا"
                    );
                }

                var fileName = await _fileService.UploadFileAsync(file);

                var fileUrl = "";
                var fileType = file.ContentType ?? "application/octet-stream";
                
                var attachment = new PatientAttachment
                {
                    PatientUserId = patientUserId,
                    FileName = fileName,
                    FileUrl = fileUrl,
                    FileType = fileType,
                    Description_En = request?.Description_En,
                    Description_Ar = request?.Description_Ar,
                    UploadDate = DateTime.UtcNow,
                    UploadedBy = uploadedById
                };
                var uploadedUser = await _userManager.FindByIdAsync(uploadedById);
                attachment.UploadedBy = uploadedUser.FullName != null ? uploadedUser.FullName : "System";
                await _fileRepository.AddAsync(attachment);

                var result = new UploadFileResponseDTO
                {
                    Id = attachment.Id,
                    PatientUserId = attachment.PatientUserId,
                    FileName = attachment.FileName,
                    FileUrl = attachment.FileUrl,
                    FileType = attachment.FileType,
                    Description_En = attachment.Description_En,
                    Description_Ar = attachment.Description_Ar,
                    UploadDate = attachment.UploadDate,
                    UploadedBy = attachment.UploadedBy,
                    CreatedAt = attachment.CreatedAt,
                    UpdatedAt = attachment.UpdatedAt
                };

                await _auditLogger.LogAsync(
                    "Upload",
                    nameof(PatientAttachment),
                    attachment.Id.ToString(),
                    userId: uploadedById,
                    userName: attachment.UploadedBy,
                    newValues: result);

                return ApiResponse<UploadFileResponseDTO>.SuccessResponse(
                    result,
                    "File uploaded successfully",
                    "تم رفع الملف بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<UploadFileResponseDTO>.ErrorResponse(
                    "Failed to upload file",
                    "فشل في رفع الملف",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteFileAsync(int fileId, string? deletedById = null)
        {
            try
            {
                var attachment = await _fileRepository.GetByIdAsync(fileId);
                if (attachment == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "File not found",
                        "الملف غير موجود"
                    );
                }

                var oldValues = new
                {
                    Id = attachment.Id,
                    PatientUserId = attachment.PatientUserId,
                    FileName = attachment.FileName,
                    FileUrl = attachment.FileUrl,
                    FileType = attachment.FileType,
                    Description_En = attachment.Description_En,
                    Description_Ar = attachment.Description_Ar,
                    UploadDate = attachment.UploadDate,
                    UploadedBy = attachment.UploadedBy
                };
                
                var fileName = attachment.FileName;
                if (fileName.StartsWith("/Files/"))
                {
                    fileName = fileName.Substring("/Files/".Length);
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    _fileService.DeleteFileAsync(fileName);
                }

                
                await _fileRepository.RemoveAsync(attachment);
                var UserDeleted = await _userManager.FindByIdAsync(deletedById);
                await _auditLogger.LogAsync(
                    "Delete",
                    nameof(PatientAttachment),
                    fileId.ToString(),
                    userId: deletedById ?? attachment.UploadedBy,
                    userName: UserDeleted != null ? UserDeleted.FullName : "System",
                    oldValues: oldValues);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "File deleted successfully",
                    "تم حذف الملف بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete file",
                    "فشل في حذف الملف",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<UploadFileResponseDTO>>> GetPatientFilesAsync(HttpRequest request ,string patientUserId)
        {
            try
            {
                var patient = await _patientRepository.GetPatientWithDetailsAsync(patientUserId);
                if (patient == null)
                {
                    return ApiResponse<IEnumerable<UploadFileResponseDTO>>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                var attachments = await _fileRepository.FindAsync(a => a.PatientUserId == patientUserId);

                var result = attachments.Select(a => new UploadFileResponseDTO
                {
                    Id = a.Id,
                    PatientUserId = a.PatientUserId,
                    FileName = a.FileName, 
                    FileUrl = $"{request.Scheme}://{request.Host}/Files/{a.FileName}", 
                    FileType = a.FileType,
                    Description_En = a.Description_En,
                    Description_Ar = a.Description_Ar,
                    UploadDate = a.UploadDate,
                    UploadedBy = a.UploadedBy,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList();

                return ApiResponse<IEnumerable<UploadFileResponseDTO>>.SuccessResponse(
                    result,
                    "Files retrieved successfully",
                    "تم استرجاع الملفات بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<UploadFileResponseDTO>>.ErrorResponse(
                    "Failed to retrieve files",
                    "فشل في استرجاع الملفات",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<UploadFileResponseDTO>> GetFileByIdAsync(HttpRequest request, int fileId)
        {
            try
            {
                var attachment = await _fileRepository.GetByIdAsync(fileId);
                if (attachment == null)
                {
                    return ApiResponse<UploadFileResponseDTO>.ErrorResponse(
                        "File not found",
                        "الملف غير موجود"
                    );
                }

                var result = new UploadFileResponseDTO
                {
                    Id = attachment.Id,
                    PatientUserId = attachment.PatientUserId,
                    FileName = attachment.FileName, 
                    FileUrl = $"{request.Scheme}://{request.Host}/Files/{attachment.FileName}", 
                    FileType = attachment.FileType,
                    Description_En = attachment.Description_En,
                    Description_Ar = attachment.Description_Ar,
                    UploadDate = attachment.UploadDate,
                    UploadedBy = attachment.UploadedBy,
                    CreatedAt = attachment.CreatedAt,
                    UpdatedAt = attachment.UpdatedAt
                };

                return ApiResponse<UploadFileResponseDTO>.SuccessResponse(
                    result,
                    "File retrieved successfully",
                    "تم استرجاع الملف بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<UploadFileResponseDTO>.ErrorResponse(
                    "Failed to retrieve file",
                    "فشل في استرجاع الملف",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}



