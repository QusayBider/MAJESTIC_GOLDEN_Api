using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IInvoiceDocumentService
    {
        Task<ApiResponse<byte[]>> GenerateInvoicePdfAsync(int invoiceId);
    }
}


