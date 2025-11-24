using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using System.Security.Claims;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MAJESTIC_GOLDEN_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LabRequestsController : ControllerBase
    {
        private readonly ILabRequestService _labRequestService;
        private readonly ILaboratoryService _laboratoryService;

        public LabRequestsController(ILabRequestService labRequestService, ILaboratoryService laboratoryService)
        {
            _labRequestService = labRequestService;
            _laboratoryService = laboratoryService;
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetLabRequestById(int id)
        {
            var result = await _labRequestService.GetLabRequestByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetLabRequestsByDoctor(string doctorId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "SubDoctor" && userId != doctorId)
            {
                return Forbid();
            }

            var result = await _labRequestService.GetLabRequestsByDoctorAsync(doctorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Laboratory")]
        public async Task<IActionResult> GetLabRequestsByPatient(string patientId)
        {
            var result = await _labRequestService.GetLabRequestsByPatientAsync(patientId);
            if (User.IsInRole("Laboratory")) { 
                foreach (var labRequest in result.Data ?? Enumerable.Empty<LabRequestResponseDTO>())
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var labProfile = await _laboratoryService.GetLaboratoryByUserIdAsync(userId);
                    if (!labProfile.Success || labRequest.LaboratoryId != labProfile.Data?.Id)
                    {
                        (result.Data as List<LabRequestResponseDTO>)?.Remove(labRequest);
                    }
                }

            }
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("laboratory/{laboratoryId}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Laboratory")]
        public async Task<IActionResult> GetLabRequestsByLaboratory(int laboratoryId)
        {
            if (User.IsInRole("Laboratory"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var labProfile = await _laboratoryService.GetLaboratoryByUserIdAsync(userId);
                if (!labProfile.Success || labProfile.Data?.Id != laboratoryId)
                {
                    return Forbid();
                }
            }

            var result = await _labRequestService.GetLabRequestsByLaboratoryAsync(laboratoryId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Laboratory")]
        public async Task<IActionResult> GetPendingLabRequests()
        {
            var userId= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var labProfile = await _laboratoryService.GetLaboratoryByUserIdAsync(userId);
            var result = await _labRequestService.GetPendingLabRequestsAsync();
            if (User.IsInRole("Laboratory"))
            {
                if (!labProfile.Success)
                {
                    return Forbid();
                }
                foreach (var labRequest in result.Data ?? Enumerable.Empty<LabRequestResponseDTO>())
                {
                    if (labRequest.LaboratoryId != labProfile.Data?.Id)
                    {
                        (result.Data as List<LabRequestResponseDTO>)?.Remove(labRequest);
                    }
                }
            }

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> CreateLabRequest([FromBody] LabRequestCreateDTO request)
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(doctorId))
            {
                return Unauthorized();
            }

            var doctorName = GetUserDisplayName();
            var result = await _labRequestService.CreateLabRequestAsync(request, doctorId, doctorName);
            return result.Success ? CreatedAtAction(nameof(GetLabRequestById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Laboratory")]
        public async Task<IActionResult> UpdateLabRequest(int id, [FromBody] LabRequestUpdateDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            if (userRoles.Contains("Laboratory"))
                {
                var labProfile = await _laboratoryService.GetLaboratoryByUserIdAsync(userId);
                if (!labProfile.Success)
                {
                    return Forbid();
                }
                var labRequestResult = await _labRequestService.GetLabRequestByIdAsync(id);
                if (!labRequestResult.Success || labRequestResult.Data?.LaboratoryId != labProfile.Data?.Id)
                {
                    return Forbid();
                }
            }
            var userName = GetUserDisplayName();
            var result = await _labRequestService.UpdateLabRequestAsync(id, request, userId, userName);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private string? GetUserDisplayName()
        {
            return User.FindFirst("FullName")?.Value
                ?? User.FindFirst(ClaimTypes.GivenName)?.Value
                ?? User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.Identity?.Name;
        }
    }
}



