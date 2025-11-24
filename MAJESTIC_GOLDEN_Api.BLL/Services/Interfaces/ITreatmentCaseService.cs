using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface ITreatmentCaseService
    {
        Task<ApiResponse<TreatmentCaseResponseDTO>> CreateCaseAsync(TreatmentCaseRequestDTO request, string createdByDoctorId);
        Task<ApiResponse<TreatmentCaseResponseDTO>> UpdateCaseAsync(int id, TreatmentCaseRequestDTO request);
        Task<ApiResponse<TreatmentCaseResponseDTO>> UpdateCaseStatusAsync(int id, UpdateCaseStatusDTO request);
        Task<ApiResponse<TreatmentCaseResponseDTO>> GetCaseByIdAsync(int id);
        Task<ApiResponse<TreatmentCaseDetailedResponseDTO>> GetCaseDetailsByIdAsync(int id);
        Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetCasesByPatientIdAsync(string patientUserId);
        Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetCasesByDoctorIdAsync(string doctorId);
        Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetCasesByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>> GetUpcomingVisitsAsync(DateTime fromDate, DateTime toDate);
        Task<ApiResponse<bool>> DeleteCaseAsync(int id);
        Task<ApiResponse<bool>> AddDoctorToCaseAsync(int caseId, string doctorId, bool isPrimary = false);
        Task<ApiResponse<bool>> RemoveDoctorFromCaseAsync(int caseId, string doctorId);
    }
}


