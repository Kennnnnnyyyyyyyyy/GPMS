using Gate_Pass_management.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gate_Pass_management.Services;

public interface IPdfGenerationService
{
    byte[] GenerateGatePassPdf(GatePass gatePass, VisitorAppointment appointment, string qrCodeBase64);
    byte[] GenerateVisitorReportPdf(IEnumerable<VisitorAppointment> appointments, DateTime reportDate);
}

public class PdfGenerationService : IPdfGenerationService
{
    public byte[] GenerateGatePassPdf(GatePass gatePass, VisitorAppointment appointment, string qrCodeBase64)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .PaddingBottom(20)
                    .Text("VISITOR GATE PASS")
                    .FontSize(24)
                    .Bold()
                    .FontColor(Colors.Blue.Medium)
                    .AlignCenter();

                page.Content()
                    .Column(column =>
                    {
                        // Pass Information
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Pass Number: {gatePass.PassNumber}")
                                    .Bold().FontSize(14);
                                col.Item().Text($"Status: {gatePass.Status}")
                                    .FontColor(GetStatusColor(gatePass.Status));
                                col.Item().Text($"Valid From: {gatePass.ValidFromUtc:dd-MM-yyyy}")
                                    .FontSize(10);
                                col.Item().Text($"Valid To: {gatePass.ValidToUtc:dd-MM-yyyy}")
                                    .FontSize(10);
                            });

                            row.ConstantItem(120).Column(col =>
                            {
                                if (!string.IsNullOrEmpty(qrCodeBase64))
                                {
                                    col.Item().Image(Convert.FromBase64String(qrCodeBase64))
                                        .FitWidth();
                                }
                            });
                        });

                        column.Item().PaddingVertical(10).LineHorizontal(1);

                        // Visitor Information
                        column.Item().Text("VISITOR INFORMATION")
                            .Bold()
                            .FontSize(16)
                            .FontColor(Colors.Blue.Medium);

                        column.Item().PaddingVertical(5).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Name: {appointment.VisitorName}")
                                    .SemiBold();
                                col.Item().Text($"Mobile: {appointment.Mobile}");
                                if (!string.IsNullOrEmpty(appointment.Email))
                                    col.Item().Text($"Email: {appointment.Email}");
                                if (!string.IsNullOrEmpty(appointment.Company))
                                    col.Item().Text($"Company: {appointment.Company}");
                            });
                        });

                        column.Item().PaddingVertical(10).LineHorizontal(1);

                        // Visit Information
                        column.Item().Text("VISIT INFORMATION")
                            .Bold()
                            .FontSize(16)
                            .FontColor(Colors.Blue.Medium);

                        column.Item().PaddingVertical(5).Column(col =>
                        {
                            col.Item().Text($"Purpose: {appointment.PurposeOfVisit}");
                            col.Item().Text($"Scheduled Date: {appointment.ScheduledDate:dd-MM-yyyy}");
                            col.Item().Text($"Scheduled Time: {appointment.ScheduledTime:HH:mm}");
                            col.Item().Text($"Duration: {appointment.EstimatedDurationMinutes} minutes");
                        });

                        column.Item().PaddingVertical(10).LineHorizontal(1);

                        // Host Information
                        column.Item().Text("HOST INFORMATION")
                            .Bold()
                            .FontSize(16)
                            .FontColor(Colors.Blue.Medium);

                        column.Item().PaddingVertical(5).Column(col =>
                        {
                            col.Item().Text($"Host Name: {appointment.HostName}")
                                .SemiBold();
                            if (!string.IsNullOrEmpty(appointment.HostDepartment))
                                col.Item().Text($"Department: {appointment.HostDepartment}");
                            if (!string.IsNullOrEmpty(appointment.HostEmployeeId))
                                col.Item().Text($"Employee ID: {appointment.HostEmployeeId}");
                            if (!string.IsNullOrEmpty(appointment.HostContactNumber))
                                col.Item().Text($"Contact: {appointment.HostContactNumber}");
                        });

                        column.Item().PaddingVertical(20).Column(col =>
                        {
                            col.Item().Border(1).Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Text("INSTRUCTIONS FOR VISITOR")
                                .Bold()
                                .FontSize(12)
                                .FontColor(Colors.Blue.Darken2);
                            
                            col.Item().Border(1).BorderTop(0).Padding(10)
                                .Text("• Present this gate pass to security at entry and exit\n" +
                                     "• Keep the pass visible at all times during your visit\n" +
                                     "• Follow all safety and security protocols\n" +
                                     "• Contact your host upon arrival\n" +
                                     "• This pass is valid only for the specified date and time")
                                .FontSize(10);
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on {DateTime.Now:dd-MM-yyyy HH:mm}")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);
            });
        }).GeneratePdf();
    }

    public byte[] GenerateVisitorReportPdf(IEnumerable<VisitorAppointment> appointments, DateTime reportDate)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .PaddingBottom(20)
                    .Column(column =>
                    {
                        column.Item().Text("VISITOR REPORT")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Medium)
                            .AlignCenter();
                        column.Item().Text($"Date: {reportDate:dd-MM-yyyy}")
                            .FontSize(12)
                            .AlignCenter();
                    });

                page.Content()
                    .Table(table =>
                    {
                        // Define columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // S.No
                            columns.RelativeColumn();    // Visitor Name
                            columns.RelativeColumn();    // Company
                            columns.ConstantColumn(80);  // Time
                            columns.RelativeColumn();    // Host
                            columns.ConstantColumn(60);  // Status
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("S.No");
                            header.Cell().Element(CellStyle).Text("Visitor Name");
                            header.Cell().Element(CellStyle).Text("Company");
                            header.Cell().Element(CellStyle).Text("Time");
                            header.Cell().Element(CellStyle).Text("Host");
                            header.Cell().Element(CellStyle).Text("Status");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.Bold())
                                    .PaddingVertical(5)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Black);
                            }
                        });

                        // Data rows
                        var serialNo = 1;
                        foreach (var appointment in appointments)
                        {
                            table.Cell().Element(CellStyle).Text(serialNo++.ToString());
                            table.Cell().Element(CellStyle).Text(appointment.VisitorName);
                            table.Cell().Element(CellStyle).Text(appointment.Company ?? "-");
                            table.Cell().Element(CellStyle).Text(appointment.ScheduledTime.ToString("HH:mm"));
                            table.Cell().Element(CellStyle).Text(appointment.HostName);
                            table.Cell().Element(CellStyle).Text(appointment.Status.ToString())
                                .FontColor(GetStatusColor((GatePassStatus)(int)appointment.Status));

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(3)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on {DateTime.Now:dd-MM-yyyy HH:mm} | Total Visitors: {appointments.Count()}")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);
            });
        }).GeneratePdf();
    }

    private static string GetStatusColor(GatePassStatus status)
    {
        return status switch
        {
            GatePassStatus.PendingApproval => Colors.Orange.Medium,
            GatePassStatus.Active => Colors.Green.Medium,
            GatePassStatus.Used => Colors.Blue.Medium,
            GatePassStatus.Expired => Colors.Red.Medium,
            GatePassStatus.Cancelled => Colors.Red.Darken1,
            _ => Colors.Black
        };
    }
}
