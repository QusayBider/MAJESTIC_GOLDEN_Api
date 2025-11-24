using AutoMapper;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public BranchService(IBranchRepository branchRepository, IMapper mapper, IAuditLogger auditLogger)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<BranchResponseDTO>> CreateBranchAsync(BranchRequestDTO request)
        {
            try
            {
                var branch = _mapper.Map<Branch>(request);
                await _branchRepository.AddAsync(branch);

                var response = _mapper.Map<BranchResponseDTO>(branch);
                return ApiResponse<BranchResponseDTO>.SuccessResponse(
                    response,
                    "Branch created successfully",
                    "تم إنشاء الفرع بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<BranchResponseDTO>.ErrorResponse(
                    "Failed to create branch",
                    "فشل في إنشاء الفرع",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<BranchResponseDTO>> UpdateBranchAsync(int id, UpdateBranchRequestDTO request)
        {
            try
            {
                var branch = await _branchRepository.GetByIdAsync(id);
                if (branch == null)
                {
                    return ApiResponse<BranchResponseDTO>.ErrorResponse(
                        "Branch not found",
                        "الفرع غير موجود"
                    );
                }

                var oldValues = _mapper.Map<BranchResponseDTO>(branch);

                _mapper.Map(request, branch);
                await _branchRepository.UpdateAsync(branch);

                var response = _mapper.Map<BranchResponseDTO>(branch);

                await _auditLogger.LogAsync(
                    "Update",
                    nameof(Branch),
                    branch.Id.ToString(),
                    oldValues: oldValues,
                    newValues: response);

                return ApiResponse<BranchResponseDTO>.SuccessResponse(
                    response,
                    "Branch updated successfully",
                    "تم تحديث الفرع بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<BranchResponseDTO>.ErrorResponse(
                    "Failed to update branch",
                    "فشل في تحديث الفرع",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteBranchAsync(int id)
        {
            try
            {
                var branch = await _branchRepository.GetByIdAsync(id);
                if (branch == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Branch not found",
                        "الفرع غير موجود"
                    );
                }

                await _branchRepository.RemoveAsync(branch);
                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Branch deleted successfully",
                    "تم حذف الفرع بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete branch",
                    "فشل في حذف الفرع",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<BranchResponseDTO>> GetBranchByIdAsync(int id)
        {
            try
            {
                var branch = await _branchRepository.GetBranchWithDetailsAsync(id);
                if (branch == null)
                {
                    return ApiResponse<BranchResponseDTO>.ErrorResponse(
                        "Branch not found",
                        "الفرع غير موجود"
                    );
                }

                var response = _mapper.Map<BranchResponseDTO>(branch);
                response.TotalPatients = branch.Users?.Count(u => u.PatientProfile != null) ?? 0;
                response.TotalStaff = branch.Users?.Count ?? 0;

                return ApiResponse<BranchResponseDTO>.SuccessResponse(
                    response,
                    "Branch retrieved successfully",
                    "تم استرجاع الفرع بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<BranchResponseDTO>.ErrorResponse(
                    "Failed to retrieve branch",
                    "فشل في استرجاع الفرع",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<BranchResponseDTO>>> GetAllBranchesAsync()
        {
            try
            {
                var branches = await _branchRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<BranchResponseDTO>>(branches);

                return ApiResponse<IEnumerable<BranchResponseDTO>>.SuccessResponse(
                    response,
                    "Branches retrieved successfully",
                    "تم استرجاع الفروع بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<BranchResponseDTO>>.ErrorResponse(
                    "Failed to retrieve branches",
                    "فشل في استرجاع الفروع",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<BranchResponseDTO>>> GetActiveBranchesAsync()
        {
            try
            {
                var branches = await _branchRepository.GetActiveBranchesAsync();
                var response = _mapper.Map<IEnumerable<BranchResponseDTO>>(branches);

                return ApiResponse<IEnumerable<BranchResponseDTO>>.SuccessResponse(
                    response,
                    "Active branches retrieved successfully",
                    "تم استرجاع الفروع النشطة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<BranchResponseDTO>>.ErrorResponse(
                    "Failed to retrieve active branches",
                    "فشل في استرجاع الفروع النشطة",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}



