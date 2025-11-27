using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        public CheckOutService(
            IInvoiceRepository invoiceRepository,
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<PatientDebt> debtRepository,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _debtRepository = debtRepository;
            _configuration = configuration;
            _emailSender = emailSender;
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

                await SendPaymentSuccessEmailAsync(invoice, amount, paymentId);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task SendPaymentSuccessEmailAsync(DAL.Models.Invoice invoice, decimal amount, string transactionId)
        {
            try
            {
                var patientUser = invoice.Patient?.User;
                if (patientUser == null || string.IsNullOrWhiteSpace(patientUser.Email))
                {
                    return;
                }

                var customerName = string.IsNullOrWhiteSpace(patientUser.FullName_En)
                    ? patientUser.FullName ?? "Customer"
                    : patientUser.FullName_En;

                var companyName = _configuration["CompanySettings:Name"] ?? "Majestic Golden Dental Clinic";
                var currency = _configuration["Stripe:Currency"] ?? "ILS";
                var paymentDate = DateTime.UtcNow.ToString("MMMM dd, yyyy");
                var year = DateTime.UtcNow.Year.ToString();

                var body = @"

                    <!DOCTYPE html>

                    <html lang='en'>

                    <head>

                      <meta charset='UTF-8'>

                      <title>Payment Successful</title>

                    </head>

                    <body style='font-family: Arial, sans-serif; background-color:#f8f9fa; margin:0; padding:0;'>

                      <div style='max-width:600px; margin:20px auto; background:#ffffff; border-radius:8px; 

                                  box-shadow:0 4px 8px rgba(0,0,0,0.05); overflow:hidden;'>

                        <!-- Header -->

                        <div style='background:#0d6efd; color:#fff; padding:20px; text-align:center;'>

                          <h2 style='margin:0;'>✅ Payment Successful</h2>

                        </div>

                        <!-- Body -->

                        <div style='padding:20px;'>

                          <h4 style='color:#0d6efd; margin-top:0;'>Hello {customer_name},</h4>

                          <p>Your Visa payment has been processed successfully. Below are the details:</p>

                          <!-- Bootstrap-like card -->

                          <div style='border:1px solid #dee2e6; border-radius:6px; padding:15px; background:#f8f9fa; margin:15px 0;'>

                            <p style='margin:5px 0;'><strong>Transaction ID:</strong> {transaction_id}</p>

                            <p style='margin:5px 0;'><strong>Amount Paid:</strong> {amount} {currency}</p>

                            <p style='margin:5px 0;'><strong>Date:</strong> {payment_date}</p>

                          </div>

                          <p>Thank you for your purchase! If you have any questions, just reply to this email.</p>

                          <p style='margin-bottom:0;'>— The {company_name} Team</p>

                        </div>

                        <!-- Footer -->

                        <div style='background:#f1f1f1; color:#6c757d; text-align:center; padding:10px; font-size:12px;'>

                          &copy; {year} {company_name}. All rights reserved.

                        </div>

                      </div>

                    </body>

                    </html>";

                body = body.Replace("{customer_name}", customerName)
                           .Replace("{transaction_id}", transactionId)
                           .Replace("{amount}", amount.ToString("0.00"))
                           .Replace("{currency}", currency)
                           .Replace("{payment_date}", paymentDate)
                           .Replace("{company_name}", companyName)
                           .Replace("{year}", year);

                var subject = $"Payment Confirmation - Invoice #{invoice.InvoiceNumber}";

                await _emailSender.SendEmailAsync(patientUser.Email, subject, body);
            }
            catch
            {
               
            }
        }
    }
}
