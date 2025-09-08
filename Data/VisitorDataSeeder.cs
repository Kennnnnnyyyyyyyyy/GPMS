using Gate_Pass_management.Models;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Data;

public static class VisitorDataSeeder
{
    public static async Task SeedVisitorAppointments(AppDbContext context)
    {
        // Check if we already have visitor appointments
        if (await context.VisitorAppointments.AnyAsync())
        {
            return; // Data already seeded
        }

        // Create some sample visitor appointments
        var appointments = new List<VisitorAppointment>
        {
            new VisitorAppointment
            {
                VisitorName = "John Smith",
                Mobile = "9876543210",
                Email = "john.smith@example.com",
                Company = "Tech Solutions Inc",
                PurposeOfVisit = "Business Meeting - Discuss new project requirements",
                ScheduledDate = DateTime.Today,
                ScheduledTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2)),
                EstimatedDurationMinutes = 60,
                HostName = "Sarah Johnson",
                HostDepartment = "Sales",
                HostEmployeeId = "EMP001",
                HostContactNumber = "9123456789",
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new VisitorAppointment
            {
                VisitorName = "Emily Davis",
                Mobile = "8765432109",
                Email = "emily.davis@consulting.com",
                Company = "Davis Consulting",
                PurposeOfVisit = "Technical consultation and system review",
                ScheduledDate = DateTime.Today,
                ScheduledTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(3)),
                EstimatedDurationMinutes = 90,
                HostName = "Mike Wilson",
                HostDepartment = "IT",
                HostEmployeeId = "EMP002",
                HostContactNumber = "9234567890",
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new VisitorAppointment
            {
                VisitorName = "Robert Brown",
                Mobile = "7654321098",
                Email = "rbrown@logistics.com",
                Company = "Global Logistics",
                PurposeOfVisit = "Supply chain discussion and vendor meeting",
                ScheduledDate = DateTime.Today,
                ScheduledTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(4)),
                EstimatedDurationMinutes = 45,
                HostName = "Lisa Anderson",
                HostDepartment = "Operations",
                HostEmployeeId = "EMP003",
                HostContactNumber = "9345678901",
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.VisitorAppointments.AddRange(appointments);
        await context.SaveChangesAsync();
    }
}
