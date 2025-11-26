using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class UploadFileRequestDTO
    {
        
        public IFormFile File { get; set; }
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
    }
}

