using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface ICheckOutService
    {
        Task<CheckOutDTOResponse> ProcessPaymentAsync(CheckOutDTORequest request, string UserId, HttpRequest Request);
        Task<bool> HandlePaymentSuccessAsync(int invoiceId, string paymentId, decimal amount);
    }
}
