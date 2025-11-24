using AutoMapper;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<PatientDebt> _debtRepository;
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<PatientDebt> debtRepository,
            IGenericRepository<Service> serviceRepository,
            IPatientRepository patientRepository,
            IMapper mapper,
            IAuditLogger auditLogger)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _debtRepository = debtRepository;
            _serviceRepository = serviceRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<InvoiceResponseDTO>> CreateInvoiceAsync(InvoiceRequestDTO request, string doctorId)
        {
            try
            {
                // Validate that the patient exists
                var patients = await _patientRepository.FindAsync(p => p.UserId == request.PatientId);
                var patient = patients.FirstOrDefault();
                if (patient == null)
                {
                    return ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود",
                        new List<string> { $"Patient with UserId '{request.PatientId}' does not exist." }
                    );
                }

                var invoice = new Invoice
                {
                    InvoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync(),
                    PatientUserId = request.PatientId,
                    DoctorId = doctorId,
                    InvoiceDate = DateTime.UtcNow,
                    Discount = request.Discount,
                    Tax = request.Tax,
                    Notes_En = request.Notes_En,
                    Notes_Ar = request.Notes_Ar,
                    Items = new List<InvoiceItem>()
                };

                decimal subtotal = 0;
                foreach (var itemDto in request.Items)
                {
                    var service = await _serviceRepository.GetByIdAsync(itemDto.ServiceId);
                    if (service == null)
                    {
                        return ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                            $"Service with ID {itemDto.ServiceId} not found",
                            $"الخدمة رقم {itemDto.ServiceId} غير موجودة"
                        );
                    }

                    decimal unitPrice = itemDto.UnitPrice > 0 ? itemDto.UnitPrice : service.BasePrice;

                    var item = new InvoiceItem
                    {
                        ServiceId = itemDto.ServiceId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = unitPrice,
                        Discount = itemDto.Discount,
                        Total = (unitPrice * itemDto.Quantity) - itemDto.Discount,
                        Notes_En = itemDto.Notes_En,
                        Notes_Ar = itemDto.Notes_Ar
                    };
                    invoice.Items.Add(item);
                    subtotal += item.Total;
                }

                invoice.SubTotal = subtotal;
                invoice.Total = subtotal - invoice.Discount + invoice.Tax;
                invoice.PaidAmount = 0;
                invoice.RemainingAmount = invoice.Total;
                invoice.Status = InvoiceStatus.Unpaid;

                await _invoiceRepository.AddAsync(invoice);

                if (invoice.RemainingAmount > 0)
                {
                    var debt = new PatientDebt
                    {
                        PatientUserId = invoice.PatientUserId,
                        InvoiceId = invoice.Id,
                        AmountDue = invoice.RemainingAmount,
                        Status = PatientDebtStatus.Pending
                    };
                    await _debtRepository.AddAsync(debt);
                }

                var fullInvoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(invoice.Id);
                var response = _mapper.Map<InvoiceResponseDTO>(fullInvoice);

                return ApiResponse<InvoiceResponseDTO>.SuccessResponse(
                    response,
                    "Invoice created successfully",
                    "تم إنشاء الفاتورة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                    "Failed to create invoice",
                    "فشل في إنشاء الفاتورة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<InvoiceResponseDTO>> UpdateInvoiceAsync(int id, UpdateInvoiceRequestDTO request)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);

                if (invoice == null)
                    return ApiResponse<InvoiceResponseDTO>.ErrorResponse("Invoice not found", "الفاتورة غير موجودة");

                var oldValues = _mapper.Map<InvoiceResponseDTO>(invoice);

                if (request.Items != null && request.Items.Any())
                {
                    // Update or Add items
                    foreach (var itemDto in request.Items)
                    {
                        var service = await _serviceRepository.GetByIdNoTrackingAsync(itemDto.ServiceId);
                        if (service == null)
                            return ApiResponse<InvoiceResponseDTO>.ErrorResponse($"Service {itemDto.ServiceId} not found", $"الخدمة رقم {itemDto.ServiceId} غير موجودة");

                        var existingItem = invoice.Items.FirstOrDefault(i => i.ServiceId == itemDto.ServiceId);

                        decimal unitPrice = itemDto.UnitPrice > 0 ? itemDto.UnitPrice : service.BasePrice;
                        decimal total = (unitPrice * itemDto.Quantity) - itemDto.Discount;

                        if (existingItem != null)
                        {
                            existingItem.Quantity = itemDto.Quantity;
                            existingItem.UnitPrice = unitPrice;
                            existingItem.Discount = itemDto.Discount;
                            existingItem.Total = total;
                            existingItem.Notes_En = itemDto.Notes_En;
                            existingItem.Notes_Ar = itemDto.Notes_Ar;
                        }
                        else
                        {
                            invoice.Items.Add(new InvoiceItem
                            {
                                ServiceId = itemDto.ServiceId,
                                Quantity = itemDto.Quantity,
                                UnitPrice = unitPrice,
                                Discount = itemDto.Discount,
                                Total = total,
                                Notes_En = itemDto.Notes_En,
                                Notes_Ar = itemDto.Notes_Ar
                            });
                        }
                    }

                    // Recalculate subtotal
                    invoice.SubTotal = invoice.Items.Sum(i => i.Total);
                }

                // Update discount & tax
                if (request.Discount.HasValue)
                    invoice.Discount = request.Discount.Value;

                if (request.Tax.HasValue)
                    invoice.Tax = request.Tax.Value;

                // Update notes
                if (!string.IsNullOrWhiteSpace(request.Notes_En))
                    invoice.Notes_En = request.Notes_En;

                if (!string.IsNullOrWhiteSpace(request.Notes_Ar))
                    invoice.Notes_Ar = request.Notes_Ar;

                // Recalculate totals
                invoice.Total = invoice.SubTotal - invoice.Discount + invoice.Tax;
                invoice.RemainingAmount = invoice.Total - invoice.PaidAmount;

                // Update status
                invoice.Status = invoice.RemainingAmount == 0 ? InvoiceStatus.Paid :
                                 invoice.PaidAmount > 0 ? InvoiceStatus.PartiallyPaid :
                                 InvoiceStatus.Unpaid;

                await _invoiceRepository.UpdateAsync(invoice);

                // Update debt record
                var debt = (await _debtRepository.FindAsync(d => d.InvoiceId == id)).FirstOrDefault();
                if (debt != null)
                {
                    debt.AmountDue = invoice.RemainingAmount;
                    debt.Status = invoice.RemainingAmount == 0 ? PatientDebtStatus.Paid : PatientDebtStatus.Pending;
                    await _debtRepository.UpdateAsync(debt);
                }

                var updatedInvoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(invoice.Id);
                var response = _mapper.Map<InvoiceResponseDTO>(updatedInvoice);

                await _auditLogger.LogAsync(
                    "Update",
                    nameof(Invoice),
                    invoice.Id.ToString(),
                    oldValues: oldValues,
                    newValues: response);

                return ApiResponse<InvoiceResponseDTO>.SuccessResponse(response, "Invoice updated successfully", "تم تحديث الفاتورة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<InvoiceResponseDTO>.ErrorResponse("Failed to update invoice", "فشل في تحديث الفاتورة", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<InvoiceResponseDTO>>> GetAllInvoicesAsync(string UserId, string userRole,DateTime? fromDate = null, DateTime? toDate = null,  string? status = null ) { 
            
            try
            {
                
                var invoices = await _invoiceRepository.GetAllAsync();
                if (fromDate.HasValue)
                {
                    invoices = invoices.Where(i => i.InvoiceDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    invoices = invoices.Where(i => i.InvoiceDate <= toDate.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    var statusEnum = EnumExtensions.ParseEnum<InvoiceStatus>(status);
                    invoices = invoices.Where(i => i.Status == statusEnum);
                }
                if (userRole == "Patient")
                {
                    invoices = invoices.Where(i => i.PatientUserId == UserId);
                }
                if (userRole == "SubDoctor" || userRole == "HeadDoctor")
                {
                    invoices = invoices.Where(i => i.DoctorId == UserId);
                }
                var result = invoices.OrderByDescending(i => i.InvoiceDate).ToList();
                var response = _mapper.Map<List<InvoiceResponseDTO>>(result);

                return ApiResponse<List<InvoiceResponseDTO>>.SuccessResponse(
                    response,
                    "Invoices retrieved successfully",
                    "تم استرجاع الفواتير بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<InvoiceResponseDTO>>.ErrorResponse(
                    "Failed to retrieve invoices",
                    "فشل في استرجاع الفواتير",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteInvoiceAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);

                if (invoice == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Invoice not found", "الفاتورة غير موجودة");
                }

                var oldValues = _mapper.Map<InvoiceResponseDTO>(invoice);

                var resultMessages = await _invoiceRepository.DeleteAsync(invoice);

                if (!resultMessages.Success)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        resultMessages.Message_En,
                        resultMessages.Message_Ar
                    );
                }

                await _auditLogger.LogAsync(
                    "Delete",
                    nameof(Invoice),
                    invoice.Id.ToString(),
                    oldValues: oldValues);

                return ApiResponse<bool>.SuccessResponse(true, "Invoice deleted successfully", "تم حذف الفاتورة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Failed to delete invoice", "فشل في حذف الفاتورة", new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> DeleteItemFromInvoiceAsync(int Item_Id)
        {
            var invoiceIdResponse = await GetInvoiceIdByItemId(Item_Id);
            if (!invoiceIdResponse.Success)
            {
                return ApiResponse<bool>.ErrorResponse(
                    invoiceIdResponse.Message_En,
                    invoiceIdResponse.Message_Ar,
                    invoiceIdResponse.Errors
                );
            }
            int invoiceId = invoiceIdResponse.Data;
            var invoice =  await _invoiceRepository.GetInvoiceWithDetailsAsync( invoiceId);
            if (invoice == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Invoice not found",
                    "الفاتورة غير موجودة"
                );
            }
            var item = invoice.Items.FirstOrDefault(i => i.Id == Item_Id);
            if (item == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Item not found in invoice",
                    "العنصر غير موجود في الفاتورة"
                );
            }
            var oldInvoiceSnapshot = _mapper.Map<InvoiceResponseDTO>(invoice);
            var removedItemSnapshot = new
            {
                item.Id,
                item.ServiceId,
                item.Quantity,
                item.UnitPrice,
                item.Discount,
                item.Total
            };

            var ItemDelete = await _invoiceRepository.RemoveItemByItemIdAsync(Item_Id);

            if (!ItemDelete) { 
                
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to delete item from invoice",
                    "فشل في حذف العنصر من الفاتورة"
                );
            }
            invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(invoiceId);
            if (invoice == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Invoice not found after deleting the item",
                    "لم يتم العثور على الفاتورة بعد حذف العنصر"
                );
            }
            // Recalculate subtotal
            invoice.SubTotal = invoice.Items.Sum(i => i.Total);
            
            // Recalculate totals
            invoice.Total = invoice.SubTotal - invoice.Discount + invoice.Tax;
            invoice.RemainingAmount = invoice.Total - invoice.PaidAmount;
            
            // Update status
            invoice.Status = invoice.RemainingAmount == 0 ? InvoiceStatus.Paid :
                             invoice.PaidAmount > 0 ? InvoiceStatus.PartiallyPaid :
                             InvoiceStatus.Unpaid;
            await _invoiceRepository.UpdateAsync(invoice);
            // Update debt record
            var debt = (await _debtRepository.FindAsync(d => d.InvoiceId == invoiceId)).FirstOrDefault();
            if (debt != null)
            {
                debt.AmountDue = invoice.RemainingAmount;
                debt.Status = invoice.RemainingAmount == 0 ? PatientDebtStatus.Paid : PatientDebtStatus.Pending;
                await _debtRepository.UpdateAsync(debt);
            }
            var updatedInvoiceSnapshot = _mapper.Map<InvoiceResponseDTO>(invoice);

            await _auditLogger.LogAsync(
                "DeleteItem",
                nameof(InvoiceItem),
                Item_Id.ToString(),
                oldValues: removedItemSnapshot);

            await _auditLogger.LogAsync(
                "UpdateAfterItemDelete",
                nameof(Invoice),
                invoice.Id.ToString(),
                oldValues: oldInvoiceSnapshot,
                newValues: updatedInvoiceSnapshot);

            return ApiResponse<bool>.SuccessResponse(
                true,
                "Item deleted from invoice successfully",
                "تم حذف العنصر من الفاتورة بنجاح"
            );


        }
        public async Task<ApiResponse<InvoiceResponseDTO>> GetInvoiceByIdAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);
                if (invoice == null)
                {
                    return ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                        "Invoice not found",
                        "الفاتورة غير موجودة"
                    );
                }

                var response = _mapper.Map<InvoiceResponseDTO>(invoice);
                return ApiResponse<InvoiceResponseDTO>.SuccessResponse(
                    response,
                    "Invoice retrieved successfully",
                    "تم استرجاع الفاتورة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<InvoiceResponseDTO>.ErrorResponse(
                    "Failed to retrieve invoice",
                    "فشل في استرجاع الفاتورة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<InvoiceResponseDTO>>> GetInvoicesByPatientAsync(string patientUserId)
        {
            try
            {
                var invoices = await _invoiceRepository.GetInvoicesByPatientAsync(patientUserId);
                var response = _mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);

                return ApiResponse<IEnumerable<InvoiceResponseDTO>>.SuccessResponse(
                    response,
                    "Invoices retrieved successfully",
                    "تم استرجاع الفواتير بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<InvoiceResponseDTO>>.ErrorResponse(
                    "Failed to retrieve invoices",
                    "فشل في استرجاع الفواتير",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<InvoiceResponseDTO>>> GetUnpaidInvoicesAsync(string UserId, string userRole)
        {
            try
            {
                var invoices = await _invoiceRepository.GetUnpaidInvoicesAsync();
                
                if (userRole == "Patient")
                {
                    invoices = invoices.Where(i => i.PatientUserId == UserId);
                }
                if (userRole == "SubDoctor" || userRole == "HeadDoctor")
                {
                    invoices = invoices.Where(i => i.DoctorId == UserId);
                }

                invoices = invoices
                .Where(i => (i.Status == InvoiceStatus.Unpaid || i.Status == InvoiceStatus.PartiallyPaid))
                .OrderBy(i => i.InvoiceDate)
                .ToList();

                var response = _mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
                return ApiResponse<IEnumerable<InvoiceResponseDTO>>.SuccessResponse(
                    response,
                    "Unpaid invoices retrieved successfully",
                    "تم استرجاع الفواتير غير المدفوعة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<InvoiceResponseDTO>>.ErrorResponse(
                    "Failed to retrieve unpaid invoices",
                    "فشل في استرجاع الفواتير غير المدفوعة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<PaymentResponseDTO>> RecordPaymentAsync(PaymentRequestDTO request, string receivedBy)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
                if (invoice == null)
                {
                    return ApiResponse<PaymentResponseDTO>.ErrorResponse(
                        "Invoice not found",
                        "الفاتورة غير موجودة"
                    );
                }

                if (request.Amount > invoice.RemainingAmount)
                {
                    return ApiResponse<PaymentResponseDTO>.ErrorResponse(
                        "Payment amount exceeds remaining balance",
                        "مبلغ الدفع يتجاوز الرصيد المتبقي"
                    );
                }

                var payment = _mapper.Map<Payment>(request);
                payment.ReceivedBy = receivedBy;
                await _paymentRepository.AddAsync(payment);

                // Update invoice
                var oldInvoiceValues = new
                {
                    invoice.PaidAmount,
                    invoice.RemainingAmount,
                    invoice.Status
                };

                invoice.PaidAmount += request.Amount;
                invoice.RemainingAmount -= request.Amount;
                invoice.Status = invoice.RemainingAmount == 0 ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
                await _invoiceRepository.UpdateAsync(invoice);

                // Update debt
                var debts = await _debtRepository.FindAsync(d => d.InvoiceId == request.InvoiceId);
                var debt = debts.FirstOrDefault();
                if (debt != null)
                {
                    debt.AmountDue = invoice.RemainingAmount;
                    debt.Status = invoice.RemainingAmount == 0 ? PatientDebtStatus.Paid : PatientDebtStatus.Pending;
                    await _debtRepository.UpdateAsync(debt);
                }

                var response = _mapper.Map<PaymentResponseDTO>(payment);

                var newInvoiceValues = new
                {
                    invoice.PaidAmount,
                    invoice.RemainingAmount,
                    invoice.Status
                };

                await _auditLogger.LogAsync(
                    "RecordPayment",
                    nameof(Payment),
                    payment.Id.ToString(),
                    userId: receivedBy,
                    newValues: response);

                await _auditLogger.LogAsync(
                    "UpdateAfterPayment",
                    nameof(Invoice),
                    invoice.Id.ToString(),
                    userId: receivedBy,
                    oldValues: oldInvoiceValues,
                    newValues: newInvoiceValues);

                return ApiResponse<PaymentResponseDTO>.SuccessResponse(
                    response,
                    "Payment recorded successfully",
                    "تم تسجيل الدفع بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PaymentResponseDTO>.ErrorResponse(
                    "Failed to record payment",
                    "فشل في تسجيل الدفع",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<PaymentResponseDTO>>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            try
            {
                var payments = await _paymentRepository.FindAsync(p => p.InvoiceId == invoiceId);
                var response = _mapper.Map<IEnumerable<PaymentResponseDTO>>(payments);
                if (!response.Any())
                { 
                    return ApiResponse<IEnumerable<PaymentResponseDTO>>.ErrorResponse(
                        "No payments found for the invoice",
                        "لم يتم العثور على مدفوعات للفاتورة"
                    );
                }
                return ApiResponse<IEnumerable<PaymentResponseDTO>>.SuccessResponse(
                    response,
                    "Payments retrieved successfully",
                    "تم استرجاع المدفوعات بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<PaymentResponseDTO>>.ErrorResponse(
                    "Failed to retrieve payments",
                    "فشل في استرجاع المدفوعات",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<int>> GetInvoiceIdByItemId(int item_id)
        {

            try
            {
                 var invoiceId = await _invoiceRepository.GetInvoiceIdbyItemIdAsync(item_id);
                return ApiResponse<int>.SuccessResponse(
                    invoiceId,
                    "Invoice ID retrieved successfully",
                    "تم استرجاع معرف الفاتورة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse(
                    "Failed to retrieve Invoice ID",
                    "فشل في استرجاع معرف الفاتورة",
                    new List<string> { ex.Message }
                );

            }
        }

    }
}


