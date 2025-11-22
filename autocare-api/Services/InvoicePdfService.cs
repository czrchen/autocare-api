using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using autocare_api.Models;
using System.Text.Json;

namespace autocare_api.Services
{
    public class InvoicePdfService
    {
        public string GeneratePdf(
            Invoices invoice,
            User customer,
            WorkshopProfile workshop,
            Service service,
            decimal subtotal,
            decimal tax,
            decimal total
        )
        {
            var filePath = Path.Combine("wwwroot", "invoices", $"{invoice.Id}.pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            string street = workshop.Address.Street ?? "";
            string city = workshop.Address.City ?? "";
            string state = workshop.Address.State ?? "";
            string postcode = workshop.Address.Postcode ?? "";
            string country = workshop.Address.Country ?? "";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    // ----------------------------------------------------
                    // HEADER: WORKSHOP INFO
                    // ----------------------------------------------------
                    page.Header().Column(col =>
                    {
                        col.Item().Text(workshop.WorkshopName)
                            .FontSize(24)
                            .Bold();

                        col.Item().Text(street);
                        col.Item().Text($"{postcode} {city}, {state}");
                        col.Item().Text(country);

                        col.Item().PaddingTop(10).LineHorizontal(1);
                    });

                    // ----------------------------------------------------
                    // CONTENT
                    // ----------------------------------------------------
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Invoice Info
                        col.Item().Column(header =>
                        {
                            header.Item().Text($"Invoice Number: {invoice.InvoiceNumber}")
                                .FontSize(16).Bold();

                            header.Item().Text($"Invoice ID: {invoice.Id}");
                            header.Item().Text($"Date: {invoice.CreatedAt:yyyy-MM-dd}");
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(0.5f);

                        // Customer Info
                        col.Item().Column(cust =>
                        {
                            cust.Item().Text("Bill To:")
                                .FontSize(14)
                                .Bold();

                            cust.Item().Text(customer.FullName);
                            cust.Item().Text(customer.Email);
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(0.5f);

                        // Service Table
                        col.Item().PaddingVertical(10).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2); // Category
                                cols.RelativeColumn(5); // Name
                                cols.RelativeColumn(3); // Price
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Category").Bold();
                                header.Cell().Element(CellStyle).Text("Service").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Price").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                }
                            });

                            // Row
                            table.Cell().PaddingVertical(8).Text(service.Category);
                            table.Cell().PaddingVertical(8).Text(service.Name);
                            table.Cell().PaddingVertical(8).AlignRight().Text($"RM {service.Price:F2}");
                        });

                        col.Item().PaddingTop(20).LineHorizontal(0.5f);

                        // Summary
                        col.Item().AlignRight().Column(sum =>
                        {
                            sum.Item().Text($"Subtotal: RM {subtotal:F2}");
                            sum.Item().Text($"Tax (6%): RM {tax:F2}");
                            sum.Item().Text($"Total: RM {total:F2}")
                                .FontSize(14)
                                .Bold();
                        });
                    });

                    // ----------------------------------------------------
                    // FOOTER
                    // ----------------------------------------------------
                    page.Footer()
                        .AlignCenter()
                        .Text("Thank you for choosing our service.")
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            document.GeneratePdf(filePath);
            return $"/invoices/{invoice.Id}.pdf";
        }
    }
}
