using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Gate_Pass_management.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace eTickets.Data
{
    public static class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using var scope = applicationBuilder.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (context == null) return;

            // Apply migrations (creates DB + Identity tables if missing)
            context.Database.Migrate();

            // Seed Identity roles & admin user
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roles = ["Admin", "Reception", "Security", "Employee", "Viewer"];            
            foreach (var role in roles)
            {
                if (!roleManager.Roles.Any(r => r.Name == role))
                {
                    roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                }
            }

            const string adminEmail = "admin@gpms.local";
            const string adminUserName = "admin";
            var adminUser = userManager.FindByNameAsync(adminUserName).GetAwaiter().GetResult();
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                userManager.CreateAsync(adminUser, "Admin@123").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
            }

            // Seed sample employees for the new visitor workflow
            if (!context.Employees.Any())
            {
                var employees = new[]
                {
                    new Employee
                    {
                        FullName = "Director1",
                        Email = "director1@company.com",
                        Department = "Executive Management",
                        Designation = "Executive Director",
                        ContactNumber = "+91-9876543210",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Employee
                    {
                        FullName = "Director2", 
                        Email = "director2@company.com",
                        Department = "Operations",
                        Designation = "Operations Director",
                        ContactNumber = "+91-9876543211",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Employee
                    {
                        FullName = "Director3",
                        Email = "director3@company.com", 
                        Department = "Finance",
                        Designation = "Finance Director",
                        ContactNumber = "+91-9876543212",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Employee
                    {
                        FullName = "Kiran Chougule",
                        Email = "kiran.chougule@company.com",
                        Department = "Administration",
                        Designation = "Manager",
                        ContactNumber = "+91-9876543213",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Employee
                    {
                        FullName = "Rohan Takmoge",
                        Email = "rohan.takmoge@company.com",
                        Department = "IT",
                        Designation = "Senior Developer",
                        ContactNumber = "+91-9876543214",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Employees.AddRange(employees);
            }

            // Legacy VisitorsEntries seeding removed

            // Seed sample visitor appointments for the approval workflow
            if (!context.VisitorAppointments.Any())
            {
                var appointments = new[]
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
                        HostName = "Kiran Chougule",
                        HostDepartment = "Administration",
                        HostEmployeeId = "EMP001",
                        HostContactNumber = "9123456789",
                        Status = Gate_Pass_management.Models.AppointmentStatus.Pending,
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
                        HostName = "Rohan Takmoge",
                        HostDepartment = "IT",
                        HostEmployeeId = "EMP002",
                        HostContactNumber = "9234567890",
                        Status = Gate_Pass_management.Models.AppointmentStatus.Pending,
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
                        HostName = "Director1",
                        HostDepartment = "Executive Management",
                        HostEmployeeId = "EMP003",
                        HostContactNumber = "9345678901",
                        Status = Gate_Pass_management.Models.AppointmentStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.VisitorAppointments.AddRange(appointments);
            }

            context.SaveChanges();
        }
    }
}