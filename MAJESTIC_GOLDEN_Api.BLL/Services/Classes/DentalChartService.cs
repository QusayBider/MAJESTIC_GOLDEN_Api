using AutoMapper;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class DentalChartService : IDentalChartService
    {
        private readonly IGenericRepository<PatientTooth> _patientToothRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public DentalChartService(
            IGenericRepository<PatientTooth> patientToothRepository,
            IPatientRepository patientRepository,
            IMapper mapper,
            IAuditLogger auditLogger)
        {
            _patientToothRepository = patientToothRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<PatientToothResponseDTO>> AddOrUpdateToothAsync(PatientToothRequestDTO request, string doctorId)
        {
            try
            {
                var patients = await _patientRepository.FindAsync(p => p.UserId == request.PatientId);
                var patient = patients.FirstOrDefault();
                if (patient == null)
                {
                    return ApiResponse<PatientToothResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود",
                        new List<string> { $"Patient with UserId '{request.PatientId}' does not exist." }
                    );
                }

                var existingTeeth = await _patientToothRepository.FindAsync(pt => 
                    pt.PatientUserId == request.PatientId && pt.ToothId == request.ToothId);
                
                var existingTooth = existingTeeth.FirstOrDefault();

                PatientTooth patientTooth;
                if (existingTooth != null)
                {
                    var oldValues = _mapper.Map<PatientToothResponseDTO>(existingTooth);

                    _mapper.Map(request, existingTooth);
                    existingTooth.DoctorId = doctorId;
                    existingTooth.TreatmentDate = request.TreatmentDate ?? DateTime.UtcNow;
                    await _patientToothRepository.UpdateAsync(existingTooth);
                    patientTooth = existingTooth;

                    var newValues = _mapper.Map<PatientToothResponseDTO>(existingTooth);

                    await _auditLogger.LogAsync(
                        "Update",
                        nameof(PatientTooth),
                        BuildToothKey(existingTooth),
                        userId: doctorId,
                        oldValues: oldValues,
                        newValues: newValues);
                }
                else
                {
                    patientTooth = _mapper.Map<PatientTooth>(request);
                    patientTooth.DoctorId = doctorId;
                    patientTooth.TreatmentDate = request.TreatmentDate ?? DateTime.UtcNow;
                    await _patientToothRepository.AddAsync(patientTooth);

                     await _auditLogger.LogAsync(
                        "Create",
                        nameof(PatientTooth),
                        BuildToothKey(patientTooth),
                        userId: doctorId,
                        newValues: _mapper.Map<PatientToothResponseDTO>(patientTooth));
                }

                var response = _mapper.Map<PatientToothResponseDTO>(patientTooth);
                return ApiResponse<PatientToothResponseDTO>.SuccessResponse(
                    response,
                    "Tooth information updated successfully",
                    "تم تحديث معلومات السن بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PatientToothResponseDTO>.ErrorResponse(
                    "Failed to update tooth information",
                    "فشل في تحديث معلومات السن",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<DentalChartResponseDTO>> GetPatientDentalChartAsync(string patientUserId)
        {
            try
            {
                var patient = await _patientRepository.GetPatientWithDetailsAsync(patientUserId);
                if (patient == null)
                {
                    return ApiResponse<DentalChartResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                var teeth = await _patientToothRepository.FindAsync(pt => pt.PatientUserId == patientUserId);
                
                var response = new DentalChartResponseDTO
                {
                    PatientId = patientUserId,
                    PatientName_En = patient.User?.FullName_En ?? "",
                    PatientName_Ar = patient.User?.FullName_Ar ?? "",
                    Teeth = _mapper.Map<List<PatientToothResponseDTO>>(teeth.OrderBy(t => t.ToothId))
                };

                return ApiResponse<DentalChartResponseDTO>.SuccessResponse(
                    response,
                    "Dental chart retrieved successfully",
                    "تم استرجاع مخطط الأسنان بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<DentalChartResponseDTO>.ErrorResponse(
                    "Failed to retrieve dental chart",
                    "فشل في استرجاع مخطط الأسنان",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<PatientToothResponseDTO>> GetToothByCompositeKeyAsync(int toothId, string patientUserId)
        {
            try
            {
                var teeth = await _patientToothRepository.FindAsync(pt => pt.ToothId == toothId && pt.PatientUserId == patientUserId);
                var tooth = teeth.FirstOrDefault();
                
                if (tooth == null)
                {
                    return ApiResponse<PatientToothResponseDTO>.ErrorResponse(
                        "Tooth record not found",
                        "سجل السن غير موجود"
                    );
                }

                var response = _mapper.Map<PatientToothResponseDTO>(tooth);
                return ApiResponse<PatientToothResponseDTO>.SuccessResponse(
                    response,
                    "Tooth record retrieved successfully",
                    "تم استرجاع سجل السن بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PatientToothResponseDTO>.ErrorResponse(
                    "Failed to retrieve tooth record",
                    "فشل في استرجاع سجل السن",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<PatientToothResponseDTO>>> GetTeethByPatientAsync(string patientUserId)
        {
            try
            {
                var teeth = await _patientToothRepository.FindAsync(pt => pt.PatientUserId == patientUserId);
                var response = _mapper.Map<IEnumerable<PatientToothResponseDTO>>(teeth.OrderBy(t => t.ToothId));

                return ApiResponse<IEnumerable<PatientToothResponseDTO>>.SuccessResponse(
                    response,
                    "Patient teeth retrieved successfully",
                    "تم استرجاع أسنان المريض بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<PatientToothResponseDTO>>.ErrorResponse(
                    "Failed to retrieve patient teeth",
                    "فشل في استرجاع أسنان المريض",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteToothRecordAsync(int toothId, string patientUserId)
        {
            try
            {
                var teeth = await _patientToothRepository.FindAsync(pt => pt.ToothId == toothId && pt.PatientUserId == patientUserId);
                var tooth = teeth.FirstOrDefault();
                
                if (tooth == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Tooth record not found",
                        "سجل السن غير موجود"
                    );
                }

                await _patientToothRepository.RemoveAsync(tooth);
                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Tooth record deleted successfully",
                    "تم حذف سجل السن بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete tooth record",
                    "فشل في حذف سجل السن",
                    new List<string> { ex.Message }
                );
            }
        }

        private static string BuildToothKey(PatientTooth tooth)
        {
            return $"{tooth.PatientUserId}_{tooth.ToothId}";
        }
    }
}


