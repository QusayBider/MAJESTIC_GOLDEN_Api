using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
            Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetInvoiceWithDetailsAsync(int id);
        Task<IEnumerable<Invoice>> GetInvoicesByPatientAsync(string patientUserId);
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
        Task<string> GenerateInvoiceNumberAsync();
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> RemoveItemsByServiceIdAsync(int serviceId);
        Task<ApiResponse<bool>> DeleteAsync(Invoice invoice);
        Task<int> GetInvoiceIdbyItemIdAsync(int item_id);
        Task<bool> RemoveItemByItemIdAsync(int itemId);
    }
}



