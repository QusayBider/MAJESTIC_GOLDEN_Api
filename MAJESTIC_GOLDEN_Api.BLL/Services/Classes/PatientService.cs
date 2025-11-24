using AutoMapper;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogger _auditLogger;

        public PatientService(IPatientRepository patientRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IAuditLogger auditLogger)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<PatientResponseDTO>> CreatePatientAsync(PatientRequestDTO request)
        {
            try
            {

                // Check if email is provided and already exists
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

                // Parse gender from string to enum
                Gender gender;
                if (!Enum.TryParse<Gender>(request.Gender, true, out gender))
                {
                    gender = Gender.Male; // Default to Male if parsing fails
                }

                // Create ApplicationUser first
                // Generate unique username by appending timestamp to phone number
                // This allows multiple users to have the same phone number
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
                    EmailConfirmed = true, // Email not confirmed by default
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Parse marital status if provided
                if (!string.IsNullOrEmpty(request.MaritalStatus_En))
                {
                    MaritalStatus maritalStatus;
                    if (Enum.TryParse<MaritalStatus>(request.MaritalStatus_En, true, out maritalStatus))
                    {
                        user.MaritalStatus = maritalStatus;
                    }
                }

                // Create user with a default password (you might want to generate a random one or handle this differently)
                var defaultPassword = $"Patient@{request.Phone}"; // Default password format
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

                // Add user to Patient role
                await _userManager.AddToRoleAsync(user, "Patient");

                // Create Patient record linked to the user
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

                // Get the patient with full details for response
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
                // Use GetPatientForUpdateAsync which includes tracking (no AsNoTracking)
                var patient = await _patientRepository.GetPatientForUpdateAsync(userId);
                if (patient == null)
                {
                    return ApiResponse<PatientResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                // Update ApplicationUser information
                // Use patient.User directly - it's already tracked by the context
                var user = patient.User;
                if (user == null)
                {
                    return ApiResponse<PatientResponseDTO>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    );
                }

                var oldValues = _mapper.Map<PatientResponseDTO>(patient);

                // Parse gender from string to enum
                Gender gender;
                if (Enum.TryParse<Gender>(request.Gender, true, out gender))
                {
                    user.Gender = gender;
                }

                // Update user fields
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

                // Parse marital status if provided
                if (!string.IsNullOrEmpty(request.MaritalStatus_En))
                {
                    MaritalStatus maritalStatus;
                    if (Enum.TryParse<MaritalStatus>(request.MaritalStatus_En, true, out maritalStatus))
                    {
                        user.MaritalStatus = maritalStatus;
                    }
                }

                await _userManager.UpdateAsync(user);

                // Update Patient-specific information
                patient.TreatmentPlan_En = request.TreatmentPlan_En;
                patient.TreatmentPlan_Ar = request.TreatmentPlan_Ar;
                patient.MedicalHistory_En = request.MedicalHistory_En;
                patient.MedicalHistory_Ar = request.MedicalHistory_Ar;
                patient.Allergies_En = request.Allergies_En;
                patient.Allergies_Ar = request.Allergies_Ar;

                await _patientRepository.UpdateAsync(patient);

                // Get updated patient with details
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
    }
}



