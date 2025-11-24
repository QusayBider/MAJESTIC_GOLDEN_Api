using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;

namespace MAJESTIC_GOLDEN_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceManagementService _serviceManagementService;

        public ServicesController(IServiceManagementService serviceManagementService)
        {
            _serviceManagementService = serviceManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var result = await _serviceManagementService.GetAllServicesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveServices()
        {
            var result = await _serviceManagementService.GetActiveServicesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var result = await _serviceManagementService.GetServiceByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetServicesByCategory(string category)
        {
            var result = await _serviceManagementService.GetServicesByCategoryAsync(category);
            return result.Success ? Ok(result) : BadRequest(result);
        }

 
        [HttpPost]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> CreateService([FromBody] ServiceRequestDTO request)
        {
            var result = await _serviceManagementService.CreateServiceAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetServiceById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

 
        [HttpPut("{id}")]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceRequestDTO request)
        {
            var result = await _serviceManagementService.UpdateServiceAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

   
        [HttpDelete("{id}")]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceManagementService.DeleteServiceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}







