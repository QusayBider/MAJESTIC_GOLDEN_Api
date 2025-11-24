using AutoMapper;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class ServiceManagementService : IServiceManagementService
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public ServiceManagementService(IGenericRepository<Service> serviceRepository, IMapper mapper, IAuditLogger auditLogger)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<ServiceResponseDTO>> CreateServiceAsync(ServiceRequestDTO request)
        {
            try
            {
                var service = _mapper.Map<Service>(request);
                await _serviceRepository.AddAsync(service);

                var response = _mapper.Map<ServiceResponseDTO>(service);
                return ApiResponse<ServiceResponseDTO>.SuccessResponse(
                    response,
                    "Service created successfully",
                    "تم إنشاء الخدمة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceResponseDTO>.ErrorResponse(
                    "Failed to create service",
                    "فشل في إنشاء الخدمة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<ServiceResponseDTO>> UpdateServiceAsync(int id, ServiceRequestDTO request)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    return ApiResponse<ServiceResponseDTO>.ErrorResponse(
                        "Service not found",
                        "الخدمة غير موجودة"
                    );
                }

                var oldValues = _mapper.Map<ServiceResponseDTO>(service);

                _mapper.Map(request, service);
                await _serviceRepository.UpdateAsync(service);

                var response = _mapper.Map<ServiceResponseDTO>(service);

                await _auditLogger.LogAsync(
                    "Update",
                    nameof(Service),
                    service.Id.ToString(),
                    oldValues: oldValues,
                    newValues: response);

                return ApiResponse<ServiceResponseDTO>.SuccessResponse(
                    response,
                    "Service updated successfully",
                    "تم تحديث الخدمة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceResponseDTO>.ErrorResponse(
                    "Failed to update service",
                    "فشل في تحديث الخدمة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteServiceAsync(int id)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Service not found",
                        "الخدمة غير موجودة"
                    );
                }

                var oldValues = _mapper.Map<ServiceResponseDTO>(service);

                service.IsActive = false;
                await _serviceRepository.UpdateAsync(service);

                await _auditLogger.LogAsync(
                    "SoftDelete",
                    nameof(Service),
                    service.Id.ToString(),
                    oldValues: oldValues,
                    newValues: new { service.IsActive });

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Service deleted successfully",
                    "تم حذف الخدمة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete service",
                    "فشل في حذف الخدمة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<ServiceResponseDTO>> GetServiceByIdAsync(int id)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    return ApiResponse<ServiceResponseDTO>.ErrorResponse(
                        "Service not found",
                        "الخدمة غير موجودة"
                    );
                }

                var response = _mapper.Map<ServiceResponseDTO>(service);
                return ApiResponse<ServiceResponseDTO>.SuccessResponse(
                    response,
                    "Service retrieved successfully",
                    "تم استرجاع الخدمة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceResponseDTO>.ErrorResponse(
                    "Failed to retrieve service",
                    "فشل في استرجاع الخدمة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ServiceResponseDTO>>> GetAllServicesAsync()
        {
            try
            {
                var services = await _serviceRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<ServiceResponseDTO>>(services);

                return ApiResponse<IEnumerable<ServiceResponseDTO>>.SuccessResponse(
                    response,
                    "Services retrieved successfully",
                    "تم استرجاع الخدمات بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ServiceResponseDTO>>.ErrorResponse(
                    "Failed to retrieve services",
                    "فشل في استرجاع الخدمات",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ServiceResponseDTO>>> GetActiveServicesAsync()
        {
            try
            {
                var services = await _serviceRepository.FindAsync(s => s.IsActive);
                var response = _mapper.Map<IEnumerable<ServiceResponseDTO>>(services);

                return ApiResponse<IEnumerable<ServiceResponseDTO>>.SuccessResponse(
                    response,
                    "Active services retrieved successfully",
                    "تم استرجاع الخدمات النشطة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ServiceResponseDTO>>.ErrorResponse(
                    "Failed to retrieve active services",
                    "فشل في استرجاع الخدمات النشطة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ServiceResponseDTO>>> GetServicesByCategoryAsync(string category)
        {
            try
            {
                var services = await _serviceRepository.FindAsync(s => 
                    s.IsActive && (s.Category_En == category || s.Category_Ar == category));
                var response = _mapper.Map<IEnumerable<ServiceResponseDTO>>(services);

                return ApiResponse<IEnumerable<ServiceResponseDTO>>.SuccessResponse(
                    response,
                    "Services retrieved successfully",
                    "تم استرجاع الخدمات بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ServiceResponseDTO>>.ErrorResponse(
                    "Failed to retrieve services",
                    "فشل في استرجاع الخدمات",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}







