using System;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class LaboratoryResponseDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserFullNameEn { get; set; }
        public string? UserFullNameAr { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserAddressEn { get; set; }
        public string? UserAddressAr { get; set; }
        public int TotalRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


