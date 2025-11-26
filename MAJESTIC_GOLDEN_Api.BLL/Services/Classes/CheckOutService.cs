using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class CheckOutService : ICheckOutService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<PatientDebt> _debtRepository;
        private readonly IConfiguration _configuration;

        public CheckOutService(
            IInvoiceRepository invoiceRepository,
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<PatientDebt> debtRepository,
            IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _debtRepository = debtRepository;
            _configuration = configuration;
        }

        public async Task<CheckOutDTOResponse> ProcessPaymentAsync(CheckOutDTORequest request, string userId, HttpRequest httpRequest)
        {
            try
            {
                var onlinePaymentMethods = new[] { DAL.Enums.PaymentMethod.CreditCard, DAL.Enums.PaymentMethod.DebitCard, DAL.Enums.PaymentMethod.MobilePayment };
                if (!onlinePaymentMethods.Contains(request.PaymentMethod))
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = "Only online payment methods are allowed. Please use CreditCard, DebitCard, or MobilePayment.",
                        URL = null,
                        PaymentId = null
                    };
                }

                var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(request.InvoiceId);
                if (invoice == null)
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = "Invoice not found",
                        URL = null,
                        PaymentId = null
                    };
                }

                if (invoice.PatientUserId != userId)
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = "You are not authorized to pay this invoice",
                        URL = null,
                        PaymentId = null
                    };
                }

                if (invoice.Status == InvoiceStatus.Paid)
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = "This invoice is already paid in full",
                        URL = null,
                        PaymentId = null
                    };
                }

                if (invoice.Status == InvoiceStatus.Cancelled)
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = "This invoice has been cancelled",
                        URL = null,
                        PaymentId = null
                    };
                }

                if (request.Amount <= 0)
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = "Payment amount must be greater than zero",
                        URL = null,
                        PaymentId = null
                    };
                }

                if (request.Amount > invoice.RemainingAmount)
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = $"Payment amount ({request.Amount}) exceeds remaining balance ({invoice.RemainingAmount})",
                        URL = null,
                        PaymentId = null
                    };
                }

                var stripeSecretKey = _configuration["Stripe:SecretKey"] ?? _configuration["StripeSettings:SecretKey"];
                if (string.IsNullOrEmpty(stripeSecretKey))
                {
                    return new CheckOutDTOResponse
                    {
                        Success = false,
                        Message = "Stripe API key is not configured. Please contact administrator.",
                        URL = null,
                        PaymentId = null
                    };
                }

                StripeConfiguration.ApiKey = stripeSecretKey;

                var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}";
                
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = $"{baseUrl}/api/Payment/CheckOut/success?session_id={{CHECKOUT_SESSION_ID}}&invoiceId={invoice.Id}",
                    CancelUrl = $"{baseUrl}/api/Payment/CheckOut/cancel?invoiceId={invoice.Id}",
                    ClientReferenceId = invoice.Id.ToString(),
                    Metadata = new Dictionary<string, string>
                    {
                        { "InvoiceId", invoice.Id.ToString() },
                        { "InvoiceNumber", invoice.InvoiceNumber },
                        { "PatientUserId", userId },
                        { "Amount", request.Amount.ToString("F2") }
                    }
                };

                
                var paymentDescription = request.Amount == invoice.RemainingAmount
                    ? $"Full payment for invoice #{invoice.InvoiceNumber}"
                    : $"Partial payment of {request.Amount:C} for invoice #{invoice.InvoiceNumber} (Remaining: {invoice.RemainingAmount:C})";

                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "ILS", 
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Invoice Payment - {invoice.InvoiceNumber}",
                            Description = paymentDescription,
                        },
                        UnitAmount = (long)(request.Amount * 100), 
                    },
                    Quantity = 1,
                });

               
                var sessionService = new SessionService();
                var session = await sessionService.CreateAsync(options);

                
                var paymentId = session.Id;

                return new CheckOutDTOResponse
                {
                    Success = true,
                    Message = "Payment session created successfully. Redirect to the payment URL to complete payment.",
                    URL = session.Url,
                    PaymentId = paymentId
                };
            }
            catch (Exception ex)
            {
                return new CheckOutDTOResponse
                {
                    Success = false,
                    Message = $"An error occurred while processing payment: {ex.Message}",
                    URL = null,
                    PaymentId = null
                };
            }
        }

        public async Task<bool> HandlePaymentSuccessAsync(int invoiceId, string paymentId, decimal amount)
        {
            try
            {
                var existingPayments = await _paymentRepository.FindAsync(p => p.TransactionReference == paymentId);
                if (existingPayments.Any())
                {
                    return true;
                }

                var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(invoiceId);
                if (invoice == null)
                {
                    return false;
                }

                if (invoice.Status == InvoiceStatus.Paid && invoice.RemainingAmount == 0)
                {
                    return false;
                }

                if (amount > invoice.RemainingAmount)
                {
                    return false;
                }

                var payment = new Payment
                {
                    InvoiceId = invoiceId,
                    Amount = amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = DAL.Enums.PaymentMethod.CreditCard, 
                    TransactionReference = paymentId,
                    Notes_En = $"Online payment processed successfully via Stripe. Session ID: {paymentId}",
                    Notes_Ar = $"تم معالجة الدفع عبر الإنترنت بنجاح عبر Stripe. معرف الجلسة: {paymentId}",
                    ReceivedBy = null 
                };

                await _paymentRepository.AddAsync(payment);

                invoice.PaidAmount += amount;
                invoice.RemainingAmount -= amount;
                invoice.Status = invoice.RemainingAmount == 0 ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
                await _invoiceRepository.UpdateAsync(invoice);

                var debts = await _debtRepository.FindAsync(d => d.InvoiceId == invoiceId);
                var debt = debts.FirstOrDefault();
                if (debt != null)
                {
                    debt.AmountDue = invoice.RemainingAmount;
                    debt.Status = invoice.RemainingAmount == 0 ? PatientDebtStatus.Paid : PatientDebtStatus.Pending;
                    await _debtRepository.UpdateAsync(debt);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
