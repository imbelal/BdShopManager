using Domain.Dtos;
using Domain.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;

namespace Infrastructure.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public byte[] GenerateOrderPdf(OrderDto order, string tenantName, string tenantAddress, string tenantPhone, string customerAddress, string customerPhone)
        {
            // Generate QR Code for the order number (human-readable)
            var qrCodeBytes = GenerateQRCode(order.OrderNumber);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();

            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text(tenantName).FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        column.Item().Text(tenantAddress).FontSize(9);
                        column.Item().Text($"Phone: {tenantPhone}").FontSize(9);
                    });

                    row.ConstantItem(150).Column(column =>
                    {
                        column.Item().BorderBottom(1).Padding(2).Text("ORDER INVOICE").FontSize(12).Bold();
                        column.Item().Text($"Order #: {order.OrderNumber}").FontSize(8);
                        column.Item().Text($"Date: {order.CreatedDate:dd/MM/yyyy}").FontSize(8);
                        column.Item().Text($"Status: {order.Status}").FontSize(8).Bold();
                    });

                    // QR Code
                    row.ConstantItem(60).Padding(5).Image(qrCodeBytes);
                });
            }

            void ComposeContent(IContainer container)
            {
                container.PaddingVertical(20).Column(column =>
                {
                    column.Spacing(10);

                    // Customer Information
                    column.Item().Element(ComposeCustomerInfo);

                    // Order Details Table
                    column.Item().Element(ComposeTable);

                    // Payment Summary
                    column.Item().Element(ComposePaymentSummary);

                    // Footer Note
                    if (!string.IsNullOrWhiteSpace(order.Remark))
                    {
                        column.Item().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5)
                            .Text($"Note: {order.Remark}").FontSize(9).Italic();
                    }
                });
            }

            void ComposeCustomerInfo(IContainer container)
            {
                container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
                {
                    column.Item().Text("CUSTOMER INFORMATION").FontSize(12).Bold();
                    column.Item().Text($"Name: {order.CustomerName}").FontSize(10);
                    column.Item().Text($"Address: {customerAddress}").FontSize(10);
                    column.Item().Text($"Phone: {customerPhone}").FontSize(10);
                });
            }

            void ComposeTable(IContainer container)
            {
                container.Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(25);  // #
                        columns.RelativeColumn(3);    // Product Name
                        columns.RelativeColumn(1);    // Unit
                        columns.RelativeColumn(1);    // Quantity
                        columns.RelativeColumn(1.5f); // Unit Price
                        columns.RelativeColumn(1.5f); // Total
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("#").Bold();
                        header.Cell().Element(CellStyle).Text("Product Name").Bold();
                        header.Cell().Element(CellStyle).Text("Unit").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Qty").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5)
                                .BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    // Rows
                    int index = 1;
                    foreach (var item in order.OrderDetails)
                    {
                        table.Cell().Element(CellStyle).Text(index.ToString());
                        table.Cell().Element(CellStyle).Text(item.ProductName);
                        table.Cell().Element(CellStyle).Text(item.Unit.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalPrice:N2}");

                        index++;

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                .PaddingVertical(5);
                        }
                    }
                });
            }

            void ComposePaymentSummary(IContainer container)
            {
                container.AlignRight().Column(column =>
                {
                    column.Spacing(5);

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Subtotal:").FontSize(11);
                        row.ConstantItem(100).AlignRight().Text($"{order.TotalPrice:N2} BDT").FontSize(11);
                    });

                    // Show tax only if tax percentage is greater than 0
                    if (order.TaxPercentage > 0)
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text($"Tax ({order.TaxPercentage:N2}%):").FontSize(11);
                            row.ConstantItem(100).AlignRight().Text($"{order.TaxAmount:N2} BDT").FontSize(11);
                        });

                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("Grand Total:").FontSize(11).SemiBold();
                            row.ConstantItem(100).AlignRight().Text($"{order.GrandTotal:N2} BDT").FontSize(11).SemiBold();
                        });
                    }
                    else
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("Total Amount:").FontSize(11).SemiBold();
                            row.ConstantItem(100).AlignRight().Text($"{order.TotalPrice:N2} BDT").FontSize(11).SemiBold();
                        });
                    }

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Total Paid:").FontSize(11).FontColor(Colors.Green.Darken2);
                        row.ConstantItem(100).AlignRight().Text($"{order.TotalPaid:N2} BDT").FontSize(11).FontColor(Colors.Green.Darken2);
                    });

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Remaining:").FontSize(12).Bold().FontColor(Colors.Red.Medium);
                        row.ConstantItem(100).AlignRight().Text($"{order.RemainingAmount:N2} BDT").FontSize(12).Bold().FontColor(Colors.Red.Medium);
                    });
                });
            }
        }

        private byte[] GenerateQRCode(string orderNumber)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(orderNumber, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(20);
                }
            }
        }
    }
}
