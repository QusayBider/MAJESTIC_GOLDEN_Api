using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.Invoices
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Invoices")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoicesController(IInvoiceService invoiceService, IInvoiceRepository invoiceRepository,UserManager<ApplicationUser> userManager)
        {
            _invoiceService = invoiceService;
            _invoiceRepository = invoiceRepository;
            _userManager = userManager;
        }

        private string GetCurrentDoctorId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }
        private async Task<IActionResult> GetInvoiceById(int id)
        {
            var result = await _invoiceService.GetInvoiceByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("Create_a_new_invoice")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Invoices_Admin")]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequestDTO request)
        {
            var doctorId = request.DoctorId;
            var result = await _invoiceService.CreateInvoiceAsync(request, doctorId);
            return result.Success ? CreatedAtAction(nameof(GetInvoiceById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPost("Create_a_new_invoice_subDoctor")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceSubDoctorRequestDTO request)
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            request.DoctorId = doctorId;
            InvoiceRequestDTO requestDTO = request.Adapt<InvoiceRequestDTO>();
            var result = await _invoiceService.CreateInvoiceAsync(requestDTO, doctorId);
            return result.Success ? CreatedAtAction(nameof(GetInvoiceById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPost("Record_payment_for_invoice")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Invoices_Admin")]
        public async Task<IActionResult> RecordPayment([FromBody] PaymentRequestDTO request)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var invoice = _invoiceRepository.GetByIdAsync(request.InvoiceId).Result;
            if (invoice == null) {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to find Invoice",
                    Message_Ar = "فشل في اجاد الفاتوره",
                });
            }
            if (UserId != invoice.DoctorId && userRole== "SubDoctor")
            {
                return BadRequest(ApiResponse<PaymentResponseDTO>.ErrorResponse(
                            "You can only record payments for your own invoices",
                            "يمكنك فقط تسجيل الدفعات للفواتير الخاصة بك"
                        )); 

            }

            var result = await _invoiceService.RecordPaymentAsync(request, UserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

 
        [HttpPut("Update_an_invoice/{id}")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Invoices_Admin,SubDoctor")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] UpdateInvoiceRequestDTO request)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (userRole == "SubDoctor")
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                if (invoice == null)
                {
                    return BadRequest(ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "Invoice not found",
                        "الفاتورة غير موجودة"
                    ));
                }
                if (invoice.DoctorId != doctorId)
                {
                    return BadRequest(ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "You are not authorized to update this invoice",
                        "أنت غير مخول بتعديل هذه الفاتورة"
                    ));
                }
            }
            var result = await _invoiceService.UpdateInvoiceAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("Delete_an_invoice/{id}")]
        [Authorize(Roles = "HeadDoctor,Invoices_Admin")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("Delete_Item_invoice/{id}")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Invoices_Admin,SubDoctor")]
        public async Task<IActionResult> DeleteItemFromInvoice(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (userRole == "SubDoctor")
            {
                var invoice_id = await _invoiceService.GetInvoiceIdByItemId(id);
                var invoice = await _invoiceRepository.GetByIdAsync(invoice_id.Data);
                if (invoice == null)
                {
                    return BadRequest(ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "Invoice or Item not found",
                        "الفاتورة أو العنصر غير موجودة"
                    ));
                }
                if (invoice.DoctorId != doctorId)
                {
                    return BadRequest(ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "You are not authorized to update this invoice",
                        "أنت غير مخول بتعديل هذه الفاتورة"
                    ));
                }
            }

            var result = await _invoiceService.DeleteItemFromInvoiceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetInvoiceByID/{id}")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Invoices_Admin,SubDoctor,Patient")]
        public async Task<IActionResult> GetInvoiceByID(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await _invoiceService.GetInvoiceByIdAsync(id);
            if (userRole == "Patient" && result.Data.PatientUserId != userId)
            {
                return BadRequest(ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                            "You are not authorized to view this invoice",
                            "أنت غير مخول بمشاهدة هذه الفاتورة"
                        ));
            }
            if (userRole == "SubDoctor" && result.Data.DoctorId != userId) { 
                return BadRequest(ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "You are not authorized to view this invoice",
                        "أنت غير مخول بمشاهدة هذه الفاتورة"
                    ));

            }
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("GetInvoiceByPatintId/{patientId}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Invoices_Admin")]
        public async Task<IActionResult> GetInvoicesByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "PatientId is required",
                    "معرف المريض مطلوب"
                ));
            }

            var result = await _invoiceService.GetInvoicesByPatientAsync(patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("payments/{invoiceId}")]
        [Authorize(Roles = "HeadDoctor,Receptionist,SubDoctor,Invoices_Admin")]
        public async Task<IActionResult> GetPaymentsByInvoice(int invoiceId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var invoiceResult = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (userRole == "SubDoctor" && invoiceResult.Data.DoctorId != userId)
            {
                return BadRequest(ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "You are not authorized to view this invoice",
                        "أنت غير مخول بمشاهدة هذه الفاتورة"
                    ));

            }
            var result = await _invoiceService.GetPaymentsByInvoiceAsync(invoiceId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "HeadDoctor,SubDoctor,Patient,Receptionist,Invoices_Admin")]
        [HttpGet("my_invoices")]
        public async Task<IActionResult> GetMyInvoices( [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null, [FromQuery] string? status = null)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var allInvoices = await _invoiceService.GetAllInvoicesAsync(UserId, userRole,fromDate = null,toDate = null, status = null );

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message_En = $"Found {allInvoices.Data.Count} invoices",
                    Message_Ar = $"تم العثور على {allInvoices.Data.Count} فواتير",
                    Data = allInvoices
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve invoices",
                    Message_Ar = "فشل في استرجاع الفواتير",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("Get_all_unpaid_invoices")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Invoices_Admin,Patient")]
        public async Task<IActionResult> GetUnpaidInvoices()
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

                var allInvoices = await _invoiceService.GetUnpaidInvoicesAsync(UserId, userRole);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message_En = $"Found {allInvoices.Data.ToList().Count} unpaid invoices",
                    Message_Ar = $"تم العثور على {allInvoices.Data.ToList().Count} فاتورة غير مدفوعة",
                    Data = allInvoices
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve unpaid invoices",
                    Message_Ar = "فشل في استرجاع الفواتير غير المدفوعة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("Get_all_unpaid_invoices_PatientId/{patientId}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Invoices_Admin")]
        public async Task<IActionResult> GetUnpaidInvoicesByPatientId(string patientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(patientId))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "PatientId is required",
                        "معرف المريض مطلوب"
                    ));
                }

                var invoicesResponse = await _invoiceService.GetInvoicesByPatientAsync(patientId);
                if (!invoicesResponse.Success)
                {
                    return BadRequest(invoicesResponse);
                }

                var invoices = invoicesResponse.Data ?? Enumerable.Empty<InvoiceResponseDTO>();
                var unpaidInvoices = invoices
                    .Where(i => string.Equals(i.Status, InvoiceStatus.Unpaid.ToString(), StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(i.Status, InvoiceStatus.PartiallyPaid.ToString(), StringComparison.OrdinalIgnoreCase))
                    .OrderBy(i => i.InvoiceDate)
                    .ToList();

                var response = ApiResponse<IEnumerable<InvoiceResponseDTO>>.SuccessResponse(
                    unpaidInvoices,
                    $"Found {unpaidInvoices.Count} unpaid invoices for patient",
                    $"تم العثور على {unpaidInvoices.Count} فاتورة غير مدفوعة للمريض"
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Failed to retrieve unpaid invoices for patient",
                    "فشل في استرجاع الفواتير غير المدفوعة للمريض",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpGet("Get_outstanding_debts_for_my_patients_SubDoctor")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetOutstandingDebts()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();

                var allInvoices = await _invoiceRepository.GetAllAsync();

                var myUnpaidInvoices = allInvoices
                    .Where(i => i.DoctorId == doctorId &&
                               (i.Status == InvoiceStatus.Unpaid || i.Status == InvoiceStatus.PartiallyPaid))
                    .ToList();

                var patientDebts = myUnpaidInvoices
                    .GroupBy(i => new { i.PatientUserId, PatientName_En = i.Patient?.User?.FullName_En ?? "", PatientName_Ar = i.Patient?.User?.FullName_Ar ?? "" })
                    .Select(g => new
                    {
                        PatientId = g.Key.PatientUserId,
                        g.Key.PatientName_En,
                        g.Key.PatientName_Ar,
                        TotalDebt = g.Sum(i => i.RemainingAmount),
                        InvoicesCount = g.Count(),
                        OldestInvoiceDate = g.Min(i => i.InvoiceDate),
                        Invoices = g.Select(i => new
                        {
                            i.Id,
                            i.InvoiceNumber,
                            i.InvoiceDate,
                            i.Total,
                            i.PaidAmount,
                            i.RemainingAmount,
                            i.Status,
                            DaysPastDue = (DateTime.Now - i.InvoiceDate).Days
                        }).OrderBy(i => i.InvoiceDate).ToList()
                    })
                    .OrderByDescending(p => p.TotalDebt)
                    .ToList();

                var totalOutstanding = patientDebts.Sum(p => p.TotalDebt);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message_En = $"Found outstanding debts for {patientDebts.Count} patients",
                    Message_Ar = $"تم العثور على ديون مستحقة لـ {patientDebts.Count} مريض",
                    Data = new
                    {
                        TotalOutstandingAmount = totalOutstanding,
                        PatientCount = patientDebts.Count,
                        TotalInvoicesCount = myUnpaidInvoices.Count,
                        PatientDebts = patientDebts
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve outstanding debts",
                    Message_Ar = "فشل في استرجاع الديون المستحقة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("Get_invoices_statistics_subDoctor")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetInvoiceStatistics()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();

                var allInvoices = await _invoiceRepository.GetAllAsync();

                var myInvoices = allInvoices
                    .Where(i => i.DoctorId == doctorId)
                    .ToList();

                if (!myInvoices.Any())
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message_En = "Statistics retrieved successfully",
                        Message_Ar = "تم استرجاع الإحصائيات بنجاح",
                        Data = new
                        {
                            TotalInvoices = 0,
                            TotalRevenue = 0m,
                            PaidAmount = 0m,
                            UnpaidAmount = 0m,
                            PendingInvoices = 0,
                            PaidInvoices = 0,
                            PartiallyPaidInvoices = 0,
                            CancelledInvoices = 0,
                            AverageInvoiceAmount = 0m,
                            CollectionRate = 0m
                        }
                    });
                }

                var totalRevenue = myInvoices.Sum(i => i.Total);
                var totalPaid = myInvoices.Sum(i => i.PaidAmount);
                var totalUnpaid = myInvoices.Sum(i => i.RemainingAmount);

                var statistics = new
                {
                    TotalInvoices = myInvoices.Count,
                    TotalRevenue = totalRevenue,
                    PaidAmount = totalPaid,
                    UnpaidAmount = totalUnpaid,
                    PendingInvoices = myInvoices.Count(i => i.Status == InvoiceStatus.Unpaid),
                    PaidInvoices = myInvoices.Count(i => i.Status == InvoiceStatus.Paid),
                    PartiallyPaidInvoices = myInvoices.Count(i => i.Status == InvoiceStatus.PartiallyPaid),
                    CancelledInvoices = myInvoices.Count(i => i.Status == InvoiceStatus.Cancelled),
                    AverageInvoiceAmount = myInvoices.Average(i => i.Total),
                    CollectionRate = totalRevenue > 0 ? Math.Round((totalPaid / totalRevenue) * 100, 2) : 0m,

                    // Monthly revenue for current year
                    MonthlyRevenue = myInvoices
                        .Where(i => i.InvoiceDate.Year == DateTime.Now.Year)
                        .GroupBy(i => i.InvoiceDate.Month)
                        .Select(g => new
                        {
                            Month = g.Key,
                            MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                            Revenue = g.Sum(i => i.Total),
                            InvoicesCount = g.Count()
                        })
                        .OrderBy(m => m.Month)
                        .ToList(),

                    // Top 5 patients by revenue
                    TopPatients = myInvoices
                        .GroupBy(i => new { i.PatientUserId, PatientName_En = i.Patient?.User?.FullName_En ?? "", PatientName_Ar = i.Patient?.User?.FullName_Ar ?? "" })
                        .Select(g => new
                        {
                            PatientId = g.Key.PatientUserId,
                            g.Key.PatientName_En,
                            g.Key.PatientName_Ar,
                            TotalRevenue = g.Sum(i => i.Total),
                            InvoicesCount = g.Count()
                        })
                        .OrderByDescending(p => p.TotalRevenue)
                        .Take(5)
                        .ToList(),

                    // Recent invoices
                    RecentInvoices = myInvoices
                        .OrderByDescending(i => i.InvoiceDate)
                        .Take(5)
                        .Select(i => new
                        {
                            i.Id,
                            i.InvoiceNumber,
                            i.InvoiceDate,
                            PatientName_En = i.Patient?.User?.FullName_En ?? "",
                            PatientName_Ar = i.Patient?.User?.FullName_Ar ?? "",
                            i.Total,
                            i.PaidAmount,
                            i.RemainingAmount,
                            i.Status
                        })
                        .ToList()
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message_En = "Statistics retrieved successfully",
                    Message_Ar = "تم استرجاع الإحصائيات بنجاح",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve statistics",
                    Message_Ar = "فشل في استرجاع الإحصائيات",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("current-month-summary-subDoctor")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetCurrentMonthSummary()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                var allInvoices = await _invoiceRepository.GetAllAsync();

                var currentMonthInvoices = allInvoices
                    .Where(i => i.DoctorId == doctorId &&
                               i.InvoiceDate.Month == currentMonth &&
                               i.InvoiceDate.Year == currentYear)
                    .ToList();

                var summary = new
                {
                    Month = DateTime.Now.ToString("MMMM yyyy"),
                    Month_Ar = DateTime.Now.ToString("MMMM yyyy", new System.Globalization.CultureInfo("ar-SA")),
                    TotalInvoices = currentMonthInvoices.Count,
                    TotalRevenue = currentMonthInvoices.Sum(i => i.Total),
                    CollectedAmount = currentMonthInvoices.Sum(i => i.PaidAmount),
                    OutstandingAmount = currentMonthInvoices.Sum(i => i.RemainingAmount),
                    PaidInvoices = currentMonthInvoices.Count(i => i.Status == InvoiceStatus.Paid),
                    PendingInvoices = currentMonthInvoices.Count(i => i.Status == InvoiceStatus.Unpaid),
                    PartiallyPaidInvoices = currentMonthInvoices.Count(i => i.Status == InvoiceStatus.PartiallyPaid)
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message_En = "Current month summary retrieved successfully",
                    Message_Ar = "تم استرجاع ملخص الشهر الحالي بنجاح",
                    Data = summary
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve current month summary",
                    Message_Ar = "فشل في استرجاع ملخص الشهر الحالي",
                    Errors = new List<string> { ex.Message }
                });
            }
        }


    }
}
