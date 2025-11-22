using Domain.Dtos;
using Domain.Interfaces;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

namespace Infrastructure.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private static string ConvertToBengaliDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var bengaliDigits = new Dictionary<char, char>
            {
                {'0', '০'}, {'1', '১'}, {'2', '২'}, {'3', '৩'}, {'4', '৪'},
                {'5', '৫'}, {'6', '৬'}, {'7', '৭'}, {'8', '৮'}, {'9', '৯'}
            };

            var result = new StringBuilder();
            foreach (char c in input)
            {
                result.Append(bengaliDigits.ContainsKey(c) ? bengaliDigits[c] : c);
            }
            return result.ToString();
        }

        private static string FormatNumberWithBengaliDigits(decimal number, string format = "N2")
        {
            return ConvertToBengaliDigits(number.ToString(format));
        }

        private static string FormatIntegerWithBengaliDigits(int number)
        {
            return ConvertToBengaliDigits(number.ToString());
        }

        private static string GetSalesStatusInBengali(Domain.Enums.SalesStatus status)
        {
            return status switch
            {
                Domain.Enums.SalesStatus.Pending => "মুলতবি",
                Domain.Enums.SalesStatus.PartiallyPaid => "আংশিক পরিশোধিত",
                Domain.Enums.SalesStatus.Paid => "পরিশোধিত",
                Domain.Enums.SalesStatus.Cancelled => "বাতিল",
                _ => "অজানা"
            };
        }

        private static string GetUnitInBengali(Domain.Enums.ProductUnit unit)
        {
            return unit switch
            {
                Domain.Enums.ProductUnit.Box => "বক্স",
                Domain.Enums.ProductUnit.Piece => "পিস",
                Domain.Enums.ProductUnit.SquareFeet => "বর্গফুট",
                Domain.Enums.ProductUnit.Kilogram => "কেজি",
                Domain.Enums.ProductUnit.Gram => "গ্রাম",
                Domain.Enums.ProductUnit.Liter => "লিটার",
                Domain.Enums.ProductUnit.Milliliter => "মিলিলিটার",
                Domain.Enums.ProductUnit.Meter => "মিটার",
                Domain.Enums.ProductUnit.Centimeter => "সেন্টিমিটার",
                Domain.Enums.ProductUnit.Inch => "ইঞ্চি",
                Domain.Enums.ProductUnit.Yard => "গজ",
                Domain.Enums.ProductUnit.Ton => "টন",
                Domain.Enums.ProductUnit.Pack => "প্যাক",
                Domain.Enums.ProductUnit.Dozen => "ডজন",
                Domain.Enums.ProductUnit.Pair => "জোড়া",
                Domain.Enums.ProductUnit.Roll => "রোল",
                Domain.Enums.ProductUnit.Bundle => "বান্ডিল",
                Domain.Enums.ProductUnit.Carton => "কার্টন",
                Domain.Enums.ProductUnit.Bag => "ব্যাগ",
                Domain.Enums.ProductUnit.Set => "সেট",
                Domain.Enums.ProductUnit.Barrel => "ব্যারেল",
                Domain.Enums.ProductUnit.Gallon => "গ্যালন",
                Domain.Enums.ProductUnit.Can => "ক্যান",
                Domain.Enums.ProductUnit.Tube => "টিউব",
                Domain.Enums.ProductUnit.Packet => "প্যাকেট",
                Domain.Enums.ProductUnit.Unit => "ইউনিট",
                _ => "অজানা"
            };
        }

        private static string GetExpenseStatusInBengali(int status)
        {
            return status switch
            {
                1 => "মুলতবি",
                2 => "পরিশোধিত",
                3 => "প্রত্যাখ্যান করা",
                _ => "অজানা"
            };
        }

        public byte[] GenerateSalesPdf(SalesDto sales, string tenantName, string tenantAddress, string tenantPhone, string customerAddress, string customerPhone)
        {
            // Generate QR Code for the sales number (human-readable)
            var qrCodeBytes = GenerateQRCode(sales.SalesNumber);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().AlignCenter().Column(column =>
                {
                    column.Spacing(10);

                    // Main footer row with return policy and signatures
                    column.Item().Row(row =>
                    {
                        // Customer signature section
                        row.RelativeItem().AlignCenter().Column(col =>
                        {
                            col.Item().Height(40).BorderBottom(1).BorderColor(Colors.Black);
                            col.Item().PaddingTop(3).Text("গ্রাহকের স্বাক্ষর").FontSize(9);
                        });

                        // Center return policy notice
                        row.RelativeItem(2).AlignCenter().Column(col =>
                        {
                            col.Item().Height(40).BorderBottom(1).BorderColor(Colors.White);
                            col.Item().PaddingTop(3).Text("বিক্রিত মালামাল ফেরৎ যোগ্য নহে")
                                .FontSize(9).Bold().FontColor(Colors.Red.Darken1);
                        });

                        // Seller signature section
                        row.RelativeItem().AlignCenter().Column(col =>
                        {
                            col.Item().Height(40).BorderBottom(1).BorderColor(Colors.Black);
                            col.Item().PaddingTop(3).Text("বিক্রেতার স্বাক্ষর").FontSize(9);
                        });
                    });

                    // Page number will be handled automatically by QuestPDF
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
                        column.Item().Text($"ফোন: {tenantPhone}").FontSize(9);
                    });

                    row.ConstantItem(150).Column(column =>
                    {
                        column.Item().BorderBottom(1).Padding(2).Text("বিক্রয় চালান").FontSize(12).Bold();
                        column.Item().Text($"বিক্রয় #: {sales.SalesNumber}").FontSize(8);
                        column.Item().Text($"তারিখ: {ConvertToBengaliDigits(sales.CreatedDate.ToString("dd/MM/yyyy"))}").FontSize(8);

                        // Show status with appropriate color
                        var statusText = GetSalesStatusInBengali(sales.Status);
                        var statusColor = sales.Status == Domain.Enums.SalesStatus.Cancelled ? Colors.Red.Darken2 : Colors.Black;
                        column.Item().Text($"অবস্থা: {statusText}").FontSize(8).Bold().FontColor(statusColor);
                    });

                    // QR Code
                    row.ConstantItem(60).Padding(5).Image(qrCodeBytes);
                });
            }

            void ComposeContent(IContainer container)
            {
                // Create a layered container for watermark overlay
                container.Layers(layers =>
                {
                    // Main content layer (primary layer)
                    layers.PrimaryLayer().Column(column =>
                    {
                        column.Spacing(10);

                        // Customer Information
                        column.Item().Element(ComposeCustomerInfo);

                        // Order Details Table
                        column.Item().Element(ComposeTable);

                        // Payment Summary
                        column.Item().Element(ComposePaymentSummary);

                        // Footer Note
                        if (!string.IsNullOrWhiteSpace(sales.Remark))
                        {
                            column.Item().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5)
                                .Text($"নোট: {sales.Remark}").FontSize(9).Italic();
                        }

                        // Cancellation notice
                        if (sales.Status == Domain.Enums.SalesStatus.Cancelled)
                        {
                            column.Item().PaddingTop(10).BorderTop(2).BorderColor(Colors.Red.Medium).PaddingTop(5)
                                .Text("এই অর্ডারটি বাতিল করা হয়েছে। স্টক ইনভেন্টরিতে ফিরিয়ে দেওয়া হয়েছে।")
                                .FontSize(10).FontColor(Colors.Red.Medium).Bold();
                        }
                    });

                    // Watermark layer (only for cancelled orders)
                    if (sales.Status == Domain.Enums.SalesStatus.Cancelled)
                    {
                        layers.Layer().AlignCenter().AlignMiddle().Rotate(-45)
                            .Text("বাতিল")
                            .FontSize(72)
                            .Bold()
                            .FontColor(Colors.Red.Lighten3);
                    }
                });
            }


            void ComposeCustomerInfo(IContainer container)
            {
                container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
                {
                    column.Item().Text("গ্রাহক তথ্য").FontSize(12).Bold();
                    column.Item().Text($"নাম: {sales.CustomerName}").FontSize(10);
                    column.Item().Text($"ঠিকানা: {customerAddress}").FontSize(10);
                    column.Item().Text($"ফোন: {customerPhone}").FontSize(10);
                });
            }

            void ComposeTable(IContainer container)
            {
                container.Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);  // #
                        columns.RelativeColumn(3);    // Product Name
                        columns.RelativeColumn(1);    // Unit
                        columns.RelativeColumn(1);    // Quantity
                        columns.RelativeColumn(1.5f); // Unit Price
                        columns.RelativeColumn(1.5f); // Total
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("ক্রমিক").Bold();
                        header.Cell().Element(CellStyle).Text("পণ্যের নাম").Bold();
                        header.Cell().Element(CellStyle).Text("একক").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("পরিমাণ").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("একক মূল্য").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("সর্বমোট").Bold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5)
                                .BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    // Rows
                    int index = 1;
                    foreach (var item in sales.SalesItems)
                    {
                        var isEvenRow = index % 2 == 0;

                        table.Cell().Element(CellStyle).Text(FormatIntegerWithBengaliDigits(index));
                        table.Cell().Element(CellStyle).Text(item.ProductName);
                        table.Cell().Element(CellStyle).Text(GetUnitInBengali(item.Unit));
                        table.Cell().Element(CellStyle).AlignRight().Text(FormatNumberWithBengaliDigits(item.Quantity));
                        table.Cell().Element(CellStyle).AlignRight().Text($"{FormatNumberWithBengaliDigits(item.UnitPrice)} ৳");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{FormatNumberWithBengaliDigits(item.TotalPrice)} ৳");

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
                        row.ConstantItem(120).Text("উপমোট:").FontSize(11);
                        row.ConstantItem(100).AlignRight().Text($"{FormatNumberWithBengaliDigits(sales.TotalPrice)} ৳").FontSize(11);
                    });

                    // Show discount if there is any discount
                    if (sales.DiscountAmount > 0)
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text($"ডিসকাউন্ট ({FormatNumberWithBengaliDigits(sales.DiscountPercentage, "N2")}%):").FontSize(11).FontColor(Colors.Red.Medium);
                            row.ConstantItem(100).AlignRight().Text($"-{FormatNumberWithBengaliDigits(sales.DiscountAmount)} ৳").FontSize(11).FontColor(Colors.Red.Medium);
                        });

                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("ডিসকাউন্টের পর:").FontSize(11);
                            row.ConstantItem(100).AlignRight().Text($"{FormatNumberWithBengaliDigits(sales.DiscountedPrice)} ৳").FontSize(11);
                        });
                    }

                    // Show tax only if tax percentage is greater than 0
                    if (sales.TaxPercentage > 0)
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text($"কর ({FormatNumberWithBengaliDigits(sales.TaxPercentage, "N2")}%):").FontSize(11);
                            row.ConstantItem(100).AlignRight().Text($"{FormatNumberWithBengaliDigits(sales.TaxAmount)} ৳").FontSize(11);
                        });

                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("সর্বমোট:").FontSize(11).SemiBold();
                            row.ConstantItem(100).AlignRight().Text($"{FormatNumberWithBengaliDigits(sales.GrandTotal)} ৳").FontSize(11).SemiBold();
                        });
                    }
                    else
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("সর্বমোট পরিমাণ:").FontSize(11).SemiBold();
                            row.ConstantItem(100).AlignRight().Text($"{FormatNumberWithBengaliDigits(sales.DiscountedPrice)} ৳").FontSize(11).SemiBold();
                        });
                    }

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("সর্বমোট প্রদত্ত:").FontSize(11).FontColor(Colors.Green.Darken2);
                        row.ConstantItem(100).AlignRight().Text($"{FormatNumberWithBengaliDigits(sales.TotalPaid)} ৳").FontSize(11).FontColor(Colors.Green.Darken2);
                    });

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("বাকি:").FontSize(12).Bold().FontColor(Colors.Red.Medium);
                        row.ConstantItem(100).AlignRight().Text($"{FormatNumberWithBengaliDigits(sales.RemainingAmount)} ৳").FontSize(12).Bold().FontColor(Colors.Red.Medium);
                    });
                });
            }
        }

        public byte[] GenerateExpensesPdf<T>(List<T> expenses, string tenantName, string tenantAddress, string tenantPhone, DateTime? startDate, DateTime? endDate) where T : class
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("পৃষ্ঠা ");
                        x.CurrentPageNumber();
                        x.Span(" এর ");
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
                        column.Item().Text($"ফোন: {tenantPhone}").FontSize(9);
                    });

                    row.RelativeItem().Column(column =>
                    {
                        column.Item().BorderBottom(1).Padding(2).Text("ব্যয় প্রতিবেদন").FontSize(12).Bold();
                        column.Item().Text($"তৈরি: {ConvertToBengaliDigits(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))}").FontSize(8);

                        if (startDate.HasValue && endDate.HasValue)
                        {
                            column.Item().Text($"সময়কাল: {ConvertToBengaliDigits(startDate.Value.ToString("dd/MM/yyyy"))} - {ConvertToBengaliDigits(endDate.Value.ToString("dd/MM/yyyy"))}").FontSize(8);
                        }
                        else if (startDate.HasValue)
                        {
                            column.Item().Text($"থেকে: {ConvertToBengaliDigits(startDate.Value.ToString("dd/MM/yyyy"))}").FontSize(8);
                        }
                        else if (endDate.HasValue)
                        {
                            column.Item().Text($"পর্যন্ত: {ConvertToBengaliDigits(endDate.Value.ToString("dd/MM/yyyy"))}").FontSize(8);
                        }

                        column.Item().Text($"মোট ব্যয়: {FormatIntegerWithBengaliDigits(expenses.Count)}").FontSize(8);
                    });
                });
            }

            void ComposeContent(IContainer container)
            {
                container.Column(column =>
                {
                    column.Spacing(10);

                    // Summary Section
                    column.Item().Element(ComposeSummary);

                    // Expenses Table
                    column.Item().Element(ComposeExpensesTable);
                });
            }

            void ComposeSummary(IContainer container)
            {
                container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
                {
                    column.Item().Text("সারসংক্ষেপ").FontSize(12).Bold();

                    var totalAmount = expenses.Sum(e => GetDecimalProperty(e, "Amount"));
                    var paidAmount = expenses.Where(e => GetIntProperty(e, "Status") == 2).Sum(e => GetDecimalProperty(e, "Amount")); // Assuming status 2 = Paid
                    var pendingAmount = expenses.Where(e => GetIntProperty(e, "Status") == 1).Sum(e => GetDecimalProperty(e, "Amount")); // Assuming status 1 = Pending
                    var rejectedAmount = expenses.Where(e => GetIntProperty(e, "Status") == 3).Sum(e => GetDecimalProperty(e, "Amount")); // Assuming status 3 = Rejected

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"সর্বমোট পরিমাণ:").FontSize(10);
                        row.RelativeItem().AlignRight().Text($"{FormatNumberWithBengaliDigits(totalAmount)} ৳").FontSize(10).Bold();
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("পরিশোধিত:").FontSize(10).FontColor(Colors.Green.Darken2);
                        row.RelativeItem().AlignRight().Text($"{FormatNumberWithBengaliDigits(paidAmount)} ৳").FontSize(10).FontColor(Colors.Green.Darken2);
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("মুলতবি:").FontSize(10).FontColor(Colors.Orange.Darken2);
                        row.RelativeItem().AlignRight().Text($"{FormatNumberWithBengaliDigits(pendingAmount)} ৳").FontSize(10).FontColor(Colors.Orange.Darken2);
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("প্রত্যাখ্যান করা:").FontSize(10).FontColor(Colors.Red.Darken2);
                        row.RelativeItem().AlignRight().Text($"{FormatNumberWithBengaliDigits(rejectedAmount)} ৳").FontSize(10).FontColor(Colors.Red.Darken2);
                    });
                });
            }

            void ComposeExpensesTable(IContainer container)
            {
                container.Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);  // #
                        columns.RelativeColumn(2);    // Date
                        columns.RelativeColumn(3);    // Title
                        columns.RelativeColumn(2);    // Category
                        columns.RelativeColumn(1.5f); // Status
                        columns.RelativeColumn(1.5f); // Payment Method
                        columns.RelativeColumn(1.5f); // Amount
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("ক্রমিক").Bold();
                        header.Cell().Element(CellStyle).Text("তারিখ").Bold();
                        header.Cell().Element(CellStyle).Text("শিরোনাম").Bold();
                        header.Cell().Element(CellStyle).Text("বিভাগ").Bold();
                        header.Cell().Element(CellStyle).Text("অবস্থা").Bold();
                        header.Cell().Element(CellStyle).Text("পেমেন্ট").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("পরিমাণ").Bold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5)
                                .BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    // Rows
                    int index = 1;
                    var sortedExpenses = expenses.OrderByDescending(e => GetDateTimeProperty(e, "ExpenseDate"));
                    foreach (var expense in sortedExpenses)
                    {
                        var status = GetIntProperty(expense, "Status");
                        var statusColor = GetStatusColor(status);
                        var statusText = GetExpenseStatusInBengali(status);

                        table.Cell().Element(CellStyle).Text(FormatIntegerWithBengaliDigits(index)).FontSize(9);
                        table.Cell().Element(CellStyle).Text(ConvertToBengaliDigits(GetDateTimeProperty(expense, "ExpenseDate").ToString("dd/MM/yyyy"))).FontSize(9);
                        table.Cell().Element(CellStyle).Text(GetStringProperty(expense, "Title")).FontSize(9);
                        table.Cell().Element(CellStyle).Text(GetStringProperty(expense, "ExpenseTypeName") ?? "").FontSize(9);
                        table.Cell().Element(CellStyle).Text(statusText).FontColor(statusColor).FontSize(9);
                        table.Cell().Element(CellStyle).Text(GetStringProperty(expense, "PaymentMethodName") ?? "").FontSize(9);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{FormatNumberWithBengaliDigits(GetDecimalProperty(expense, "Amount"))}").FontColor(statusColor).FontSize(9);

                        index++;

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                .PaddingVertical(3);
                        }
                    }
                });
            }
        }

        private Color GetStatusColor(int status)
        {
            return status switch
            {
                1 => Colors.Orange.Darken2, // Pending
                2 => Colors.Green.Darken2,  // Paid
                3 => Colors.Red.Darken2,    // Rejected
                _ => Colors.Grey.Darken1    // Default
            };
        }

        private string GetStringProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj)?.ToString() ?? string.Empty;
        }

        private decimal GetDecimalProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property != null ? Convert.ToDecimal(property.GetValue(obj) ?? 0) : 0;
        }

        private int GetIntProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property != null ? Convert.ToInt32(property.GetValue(obj) ?? 0) : 0;
        }

        private DateTime GetDateTimeProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property != null ? Convert.ToDateTime(property.GetValue(obj) ?? DateTime.MinValue) : DateTime.MinValue;
        }

        private byte[] GenerateQRCode(string salesNumber)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(salesNumber, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(20);
                }
            }
        }
    }
}
