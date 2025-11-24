using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Migrations;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<ApiResponse<InvoiceResponseDTO>> CreateInvoiceAsync(InvoiceRequestDTO request, string doctorId);
        Task<ApiResponse<InvoiceResponseDTO>> UpdateInvoiceAsync(int id, UpdateInvoiceRequestDTO request);
        Task<ApiResponse<List<InvoiceResponseDTO>>> GetAllInvoicesAsync(string UserId, string userRole, DateTime? fromDate = null, DateTime? toDate = null, string? status = null);
        Task<ApiResponse<InvoiceResponseDTO>> GetInvoiceByIdAsync(int id);
        Task<ApiResponse<IEnumerable<InvoiceResponseDTO>>> GetInvoicesByPatientAsync(string patientUserId);
        Task<ApiResponse<IEnumerable<InvoiceResponseDTO>>> GetUnpaidInvoicesAsync(string UserId, string userRole);
        Task<ApiResponse<PaymentResponseDTO>> RecordPaymentAsync(PaymentRequestDTO request, string receivedBy);
        Task<ApiResponse<IEnumerable<PaymentResponseDTO>>> GetPaymentsByInvoiceAsync(int invoiceId);
        Task<ApiResponse<bool>> DeleteInvoiceAsync(int id);
        Task<ApiResponse<int>> GetInvoiceIdByItemId(int item_id);
        Task<ApiResponse<bool>> DeleteItemFromInvoiceAsync(int Item_Id);
    }
}



