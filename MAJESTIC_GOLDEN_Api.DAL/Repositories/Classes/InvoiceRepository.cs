using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Patient.User)
                .Include(i => i.Doctor)
                .Include(i => i.Items)
                .ThenInclude(ii => ii.Service)
                .Include(i => i.Payments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceWithDetailsAsync(int id)
        {
            return await context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Patient.User)
                .Include(i => i.Doctor)
                .Include(i => i.Items)
                    .ThenInclude(ii => ii.Service)
                .Include(i => i.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByPatientAsync(string patientUserId)
        {
            return await context.Invoices
                .Where(i => i.PatientUserId == patientUserId)
                .Include(i => i.Items)
                .ThenInclude(ii => ii.Service)
                .Include(i => i.Payments)
                .AsNoTracking()
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }


        public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
        {
            return await context.Invoices
                .Where(i => i.Status == InvoiceStatus.Unpaid || i.Status == InvoiceStatus.PartiallyPaid)
                .Include(i => i.Patient)
                .Include(i => i.Patient.User)
                .Include(i => i.Items)
                .ThenInclude(ii => ii.Service)
                .Include(i => i.Payments)
                .Include(i=> i.Doctor)
                .AsNoTracking()
                .OrderBy(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Invoice invoice)
        {
            if (invoice.Status == InvoiceStatus.Paid)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Cannot delete a paid invoice.",
                    "لا يمكن حذف فاتورة مدفوعة."
                );
            }

            var existingInvoice = await context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoice.Id);

            if (existingInvoice == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Invoice not found.",
                    "الفاتورة غير موجودة."
                );
            }

            if (existingInvoice.Payments.Any())
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Cannot delete invoice with associated payments.",
                    "لا يمكن حذف الفاتورة التي تحتوي على مدفوعات مرتبطة."
                );
            }

            var debts = await context.PatientDebts
                .Where(d => d.InvoiceId == invoice.Id)
                .ToListAsync();
            if (debts.Any())
                context.PatientDebts.RemoveRange(debts);

            if (existingInvoice.Items.Any())
                context.InvoiceItems.RemoveRange(existingInvoice.Items);

            context.Invoices.Remove(existingInvoice);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                true,
                "Invoice deleted successfully.",
                "تم حذف الفاتورة بنجاح."
            );
        }

        public async Task<int> GetInvoiceIdbyItemIdAsync(int item_id)
        {

            return await context.InvoiceItems
                 .Where(ii => ii.Id == item_id)
                 .Select(ii => ii.InvoiceId)
                 .FirstOrDefaultAsync();
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.Now.Year;
            var lastInvoice = await context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith($"INV-{year}"))
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            if (lastInvoice == null)
            {
                return $"INV-{year}-0001";
            }

            var lastNumber = int.Parse(lastInvoice.InvoiceNumber.Split('-')[2]);
            return $"INV-{year}-{(lastNumber + 1):D4}";
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = context.Invoices.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate <= endDate.Value);
            }

            return await query.SumAsync(i => i.PaidAmount);
        }
        public async Task<bool> RemoveItemsByServiceIdAsync(int serviceId)
        {
            var itemsToRemove = await context.InvoiceItems
                .Where(ii => ii.ServiceId == serviceId)
                .ToListAsync();
            if (itemsToRemove.Count == 0)
                return false;
            context.InvoiceItems.RemoveRange(itemsToRemove);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveItemByItemIdAsync(int itemId) { 
        
            var itemToRemove = await context.InvoiceItems
                .FirstOrDefaultAsync(ii => ii.Id == itemId);
            if (itemToRemove == null)
                return false;
            context.InvoiceItems.Remove(itemToRemove);
            await context.SaveChangesAsync();
            return true;
        }
    }

}


