using AutoMapper;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class InvoiceDocumentService : IInvoiceDocumentService
    {
        private static readonly CultureInfo CurrencyCulture = CultureInfo.GetCultureInfo("en-US");

        static InvoiceDocumentService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public InvoiceDocumentService(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<byte[]>> GenerateInvoicePdfAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetInvoiceWithDetailsAsync(invoiceId);
            if (invoice == null)
            {
                return ApiResponse<byte[]>.ErrorResponse(
                    "Invoice not found",
                    "الفاتورة غير موجودة");
            }

            var invoiceDto = _mapper.Map<InvoiceResponseDTO>(invoice);
            var document = CreateInvoiceDocument(invoiceDto);
            var pdfBytes = document.GeneratePdf();

            return ApiResponse<byte[]>.SuccessResponse(
                pdfBytes,
                "Invoice PDF generated successfully",
                "تم إنشاء ملف الفاتورة بنجاح");
        }

        private static IDocument CreateInvoiceDocument(InvoiceResponseDTO invoice)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Spacing(4);
                        column.Item().Text("Majestic Golden Dental Clinic").FontSize(20).SemiBold();
                        column.Item().Text(text =>
                        {
                            text.Span("Invoice #: ").SemiBold();
                            text.Span(invoice.InvoiceNumber);
                            text.Span("  |  Date: ").SemiBold();
                            text.Span(invoice.InvoiceDate.ToString("dd MMM yyyy"));
                        });
                        
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(16);
                        column.Item().Element(c => ComposePartiesSection(c, invoice));
                        column.Item().Element(c => ComposeItemsTable(c, invoice));
                        column.Item().Element(c => ComposeTotals(c, invoice));
                        column.Item().Element(c => ComposePayments(c, invoice));
                        column.Item().Element(c => ComposeNotes(c, invoice));
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken1));
                        text.Span("Generated on ");
                        text.Span(DateTime.UtcNow.ToString("dd MMM yyyy HH:mm 'UTC'\n"));
                    });
                });
            });
        }

        private static void ComposePartiesSection(IContainer container, InvoiceResponseDTO invoice)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten3).Padding(12).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Spacing(2);
                    column.Item().Text("Bill To").FontSize(12).SemiBold().FontColor(Colors.Grey.Darken2);
                    column.Item().Text(invoice.PatientName_En).FontSize(11);
                    column.Item().Text($"Patient ID: {invoice.PatientUserId}");
                });

                row.RelativeItem().Column(column =>
                {
                    column.Spacing(2);
                    column.Item().Text("Prepared By").FontSize(12).SemiBold().FontColor(Colors.Grey.Darken2);
                    column.Item().Text(string.IsNullOrWhiteSpace(invoice.DoctorName) ? "N/A" : invoice.DoctorName);
                    column.Item().Text($"Status: {invoice.Status}");
                });
            });
        }

        private static void ComposeItemsTable(IContainer container, InvoiceResponseDTO invoice)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1.5f);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Service").SemiBold();
                    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Qty").SemiBold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Unit Price").SemiBold();
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Total").SemiBold();
                });

                if (invoice.Items.Any())
                {
                    foreach (var item in invoice.Items)
                    {
                        var serviceName = !string.IsNullOrWhiteSpace(item.ServiceName_En)
                            ? item.ServiceName_En
                            : !string.IsNullOrWhiteSpace(item.ServiceName_Ar)
                                ? item.ServiceName_Ar
                                : $"Service #{item.ServiceId}";

                        table.Cell().Element(CellStyle).Text(serviceName);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(item.UnitPrice));
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatCurrency(item.Total));
                    }
                }
                else
                {
                    table.Cell().ColumnSpan(4)
                        .Element(CellStyle)
                        .AlignCenter()
                        .Text("No items recorded for this invoice.");
                }
            });
        }

        private static void ComposeTotals(IContainer container, InvoiceResponseDTO invoice)
        {
            container.AlignRight().Width(250).Column(column =>
            {
                column.Spacing(4);
                column.Item().Element(c => TotalLine(c, "Subtotal", invoice.SubTotal));
                column.Item().Element(c => TotalLine(c, "Discount", -invoice.Discount));
                column.Item().Element(c => TotalLine(c, "Tax", invoice.Tax));
                column.Item().Element(c => TotalLine(c, "Grand Total", invoice.Total, emphasize: true));
                column.Item().Element(c => TotalLine(c, "Paid", invoice.PaidAmount));
                column.Item().Element(c => TotalLine(c, "Balance Due", invoice.RemainingAmount, emphasize: true));
            });
        }

        private static void ComposePayments(IContainer container, InvoiceResponseDTO invoice)
        {
            if (invoice.Payments == null || !invoice.Payments.Any())
            {
                return;
            }

            container.Column(column =>
            {
                column.Spacing(6);
                column.Item().Text("Payments").FontSize(12).SemiBold();

                foreach (var payment in invoice.Payments.OrderByDescending(p => p.PaymentDate))
                {
                    column.Item().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(6).Row(row =>
                    {
                        row.RelativeItem().Text(payment.PaymentDate.ToString("dd MMM yyyy"));
                        row.RelativeItem().Text(payment.PaymentMethod);
                        row.ConstantItem(120).AlignRight().Text(FormatCurrency(payment.Amount)).SemiBold();
                    });
                }
            });
        }

        private static void ComposeNotes(IContainer container, InvoiceResponseDTO invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.Notes_En) && string.IsNullOrWhiteSpace(invoice.Notes_Ar))
            {
                return;
            }

            container.Border(1).BorderColor(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Spacing(4);
                if (!string.IsNullOrWhiteSpace(invoice.Notes_En))
                {
                    column.Item().Text("Notes (EN)").SemiBold();
                    column.Item().Text(invoice.Notes_En);
                }

                if (!string.IsNullOrWhiteSpace(invoice.Notes_Ar))
                {
                    column.Item().Text("Notes (AR)").SemiBold();
                    column.Item().Text(invoice.Notes_Ar);
                }
            });
        }

        private static void TotalLine(IContainer container, string label, decimal value, bool emphasize = false)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    var span = text.Span(label);
                    if (emphasize)
                    {
                        span.SemiBold();
                    }
                });

                row.ConstantItem(120).AlignRight().Text(text =>
                {
                    var span = text.Span(FormatCurrency(value));
                    if (emphasize)
                    {
                        span.SemiBold();
                    }
                });
            });
        }

        private static string FormatCurrency(decimal value) =>
            value.ToString("N2", CurrencyCulture);

        private static IContainer HeaderCellStyle(IContainer container) =>
            container.Background(Colors.Grey.Lighten3).PaddingVertical(6).PaddingHorizontal(4);

        private static IContainer CellStyle(IContainer container) =>
            container.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5).PaddingHorizontal(4);
    }
}


