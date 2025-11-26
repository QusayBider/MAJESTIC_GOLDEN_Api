using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text.Json;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.Payment
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Payment")]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckOutService _checkOutService;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IConfiguration _configuration;

        public CheckOutController(
            ICheckOutService checkOutService,
            IInvoiceRepository invoiceRepository,
            IConfiguration configuration)
        {
            _checkOutService = checkOutService;
            _invoiceRepository = invoiceRepository;
            _configuration = configuration;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        
        [HttpPost("process")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> ProcessPayment([FromBody] CheckOutDTORequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _checkOutService.ProcessPaymentAsync(request, userId, Request);

                if (result.Success)
                {
                    return Ok(new ApiResponse<CheckOutDTOResponse>
                    {
                        Success = true,
                        Message_En = result.Message,
                        Message_Ar = "تم إنشاء جلسة الدفع بنجاح. يرجى التوجه إلى رابط الدفع لإتمام عملية الدفع.",
                        Data = result
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<CheckOutDTOResponse>
                    {
                        Success = false,
                        Message_En = result.Message,
                        Message_Ar = "فشل في إنشاء جلسة الدفع",
                        Data = result
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = $"An error occurred: {ex.Message}",
                    Message_Ar = $"حدث خطأ: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

      
        [HttpGet("success")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string session_id, [FromQuery] int invoiceId)
        {
            try
            {
                if (string.IsNullOrEmpty(session_id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Session ID is required",
                        Message_Ar = "معرف الجلسة مطلوب"
                    });
                }

                
                var stripeSecretKey = _configuration["Stripe:SecretKey"] ?? _configuration["StripeSettings:SecretKey"];
                if (string.IsNullOrEmpty(stripeSecretKey))
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Stripe configuration error",
                        Message_Ar = "خطأ في إعدادات Stripe"
                    });
                }

                StripeConfiguration.ApiKey = stripeSecretKey;

                
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(session_id);

                if (session == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Payment session not found",
                        Message_Ar = "لم يتم العثور على جلسة الدفع"
                    });
                }

                
                if (session.PaymentStatus != "paid")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Payment was not completed",
                        Message_Ar = "لم يتم إتمام الدفع"
                    });
                }

                
                var invoiceIdFromSession = session.Metadata?.ContainsKey("InvoiceId") == true
                    ? int.Parse(session.Metadata["InvoiceId"])
                    : invoiceId;

                if (invoiceIdFromSession == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Invoice ID not found",
                        Message_Ar = "معرف الفاتورة غير موجود"
                    });
                }

                
                var amount = (decimal)(session.AmountTotal ?? 0) / 100; // Convert from cents

                var paymentProcessed = await _checkOutService.HandlePaymentSuccessAsync(
                    invoiceIdFromSession,
                    session_id,
                    amount);

                if (paymentProcessed)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message_En = "Payment processed successfully",
                        Message_Ar = "تم معالجة الدفع بنجاح",
                        Data = new
                        {
                            SessionId = session_id,
                            InvoiceId = invoiceIdFromSession,
                            Amount = amount,
                            PaymentStatus = session.PaymentStatus
                        }
                    });
                }
                else
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Failed to process payment",
                        Message_Ar = "فشل في معالجة الدفع"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message_En = $"An error occurred: {ex.Message}",
                    Message_Ar = $"حدث خطأ: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

  
        [HttpGet("cancel")]
        [AllowAnonymous]
        public IActionResult PaymentCancel([FromQuery] int invoiceId)
        {
            return Ok(new ApiResponse<object>
            {
                Success = false,
                Message_En = "Payment was cancelled",
                Message_Ar = "تم إلغاء الدفع",
                Data = new
                {
                    InvoiceId = invoiceId,
                    Message = "You can try again later"
                }
            });
        }


    }
}

