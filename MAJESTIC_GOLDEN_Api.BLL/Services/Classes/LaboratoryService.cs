using AutoMapper;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class LaboratoryService : ILaboratoryService
    {
        private readonly ILaboratoryRepository _laboratoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public LaboratoryService(
            ILaboratoryRepository laboratoryRepository,
            IUserRepository userRepository,
            UserManager<ApplicationUser> _userManager,
            IMapper mapper,
            IAuditLogger auditLogger)
        {
            _laboratoryRepository = laboratoryRepository;
            _userRepository = userRepository;
            this._userManager = _userManager;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<LaboratoryResponseDTO>> CreateLaboratoryAsync(LaboratoryCreateDTO request)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(request.UserId);
                if (user == null)
                {
                    return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    );
                }

                var existingLab = await _laboratoryRepository.GetByUserIdAsync(request.UserId);
                if (existingLab != null)
                {
                    return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                        "Laboratory profile already exists for this user",
                        "يوجد ملف مختبر لهذا المستخدم بالفعل"
                    );
                }

                var laboratory = _mapper.Map<Laboratory>(request);
                await _laboratoryRepository.AddAsync(laboratory);
                await _userManager.AddToRoleAsync(user, "Laboratory");
                var createdLab = await _laboratoryRepository.GetByIdWithUserAsync(laboratory.Id);
                var response = _mapper.Map<LaboratoryResponseDTO>(createdLab);

                return ApiResponse<LaboratoryResponseDTO>.SuccessResponse(
                    response,
                    "Laboratory created successfully",
                    "تم إنشاء ملف المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                    "Failed to create laboratory",
                    "فشل في إنشاء ملف المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<LaboratoryResponseDTO>> UpdateLaboratoryAsync(int id, LaboratoryUpdateDTO request)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetByIdWithUserAsync(id);
                if (laboratory == null)
                {
                    return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                        "Laboratory not found",
                        "ملف المختبر غير موجود"
                    );
                }

                if (!string.IsNullOrWhiteSpace(request.UserId) && request.UserId != laboratory.UserId)
                {
                    var user = await _userRepository.GetUserByIdAsync(request.UserId);
                    if (user == null)
                    {
                        return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                            "User not found",
                            "المستخدم غير موجود"
                        );
                    }

                    var existingLabForUser = await _laboratoryRepository.GetByUserIdAsync(request.UserId);
                    if (existingLabForUser != null && existingLabForUser.Id != laboratory.Id)
                    {
                        return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                            "Laboratory profile already exists for this user",
                            "يوجد ملف مختبر لهذا المستخدم بالفعل"
                        );
                    }

                    laboratory.UserId = request.UserId;

                    if (!await _userManager.IsInRoleAsync(user, "Laboratory"))
                    {
                        await _userManager.AddToRoleAsync(user, "Laboratory");
                    }
                }

                var oldValues = _mapper.Map<LaboratoryResponseDTO>(laboratory);

                await _laboratoryRepository.UpdateAsync(laboratory);

                var updatedLab = await _laboratoryRepository.GetByIdWithUserAsync(id);
                var response = _mapper.Map<LaboratoryResponseDTO>(updatedLab);

                await _auditLogger.LogAsync(
                    "Update",
                    nameof(Laboratory),
                    laboratory.Id.ToString(),
                    oldValues: oldValues,
                    newValues: response);

                return ApiResponse<LaboratoryResponseDTO>.SuccessResponse(
                    response,
                    "Laboratory updated successfully",
                    "تم تحديث ملف المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                    "Failed to update laboratory",
                    "فشل في تحديث ملف المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteLaboratoryAsync(int id)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetByIdWithUserAsync(id);
                if (laboratory == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Laboratory not found",
                        "ملف المختبر غير موجود"
                    );
                }

                await _laboratoryRepository.RemoveAsync(laboratory);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Laboratory deleted successfully",
                    "تم حذف ملف المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete laboratory",
                    "فشل في حذف ملف المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<LaboratoryResponseDTO>> GetLaboratoryByIdAsync(int id)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetByIdWithUserAsync(id);
                if (laboratory == null)
                {
                    return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                        "Laboratory not found",
                        "ملف المختبر غير موجود"
                    );
                }

                var response = _mapper.Map<LaboratoryResponseDTO>(laboratory);
                return ApiResponse<LaboratoryResponseDTO>.SuccessResponse(
                    response,
                    "Laboratory retrieved successfully",
                    "تم استرجاع ملف المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                    "Failed to retrieve laboratory",
                    "فشل في استرجاع ملف المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<LaboratoryResponseDTO>> GetLaboratoryByUserIdAsync(string userId)
        {
            try
            {
                var laboratory = await _laboratoryRepository.GetByUserIdAsync(userId);
                if (laboratory == null)
                {
                    return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                        "Laboratory profile not found for this user",
                        "ملف المختبر غير موجود لهذا المستخدم"
                    );
                }

                var response = _mapper.Map<LaboratoryResponseDTO>(laboratory);
                return ApiResponse<LaboratoryResponseDTO>.SuccessResponse(
                    response,
                    "Laboratory retrieved successfully",
                    "تم استرجاع ملف المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<LaboratoryResponseDTO>.ErrorResponse(
                    "Failed to retrieve laboratory",
                    "فشل في استرجاع ملف المختبر",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<LaboratoryResponseDTO>>> GetAllLaboratoriesAsync()
        {
            try
            {
                var laboratories = await _laboratoryRepository.GetAllWithUsersAsync();
                var response = _mapper.Map<IEnumerable<LaboratoryResponseDTO>>(laboratories);

                return ApiResponse<IEnumerable<LaboratoryResponseDTO>>.SuccessResponse(
                    response,
                    "Laboratories retrieved successfully",
                    "تم استرجاع ملفات المختبر بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LaboratoryResponseDTO>>.ErrorResponse(
                    "Failed to retrieve laboratories",
                    "فشل في استرجاع ملفات المختبر",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}


