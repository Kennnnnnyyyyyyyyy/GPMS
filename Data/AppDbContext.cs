using Gate_Pass_management.Models;
using Gate_Pass_management.Domain.Entities;
using Gate_Pass_management.Domain.Scheduler;
using Gate_Pass_management.Infrastructure.Sqlite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.AddInterceptors(new SqlitePragmaInterceptor());
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // GatePass (legacy VisitorsEntry relation removed)
            modelBuilder.Entity<GatePass>(e =>
            {
                e.HasKey(g => g.Id);
                e.Property(g => g.PassNumber).HasMaxLength(50);
                e.Property(g => g.VisitorName).HasMaxLength(100);
                e.Property(g => g.Mobile).HasMaxLength(15);
                e.Property(g => g.ValidFromUtc);
                e.Property(g => g.ValidToUtc);
                e.Property(g => g.Status);
                e.Property(g => g.QRCodeValue).HasMaxLength(500);
            });

            // VisitorAppointment entity configuration (workflow)
            modelBuilder.Entity<VisitorAppointment>(e =>
            {
                e.HasKey(v => v.Id);
                e.Property(v => v.VisitorName).HasMaxLength(100).IsRequired();
                e.Property(v => v.Mobile).HasMaxLength(15).IsRequired();
                e.Property(v => v.Email).HasMaxLength(100);
                e.Property(v => v.Company).HasMaxLength(100);
                e.Property(v => v.PurposeOfVisit).HasMaxLength(500).IsRequired();
                e.Property(v => v.HostName).HasMaxLength(100).IsRequired();
                e.Property(v => v.HostDepartment).HasMaxLength(100);
                e.Property(v => v.HostEmployeeId).HasMaxLength(50);
                e.Property(v => v.HostContactNumber).HasMaxLength(15);
                e.Property(v => v.ApprovedBy).HasMaxLength(100);
                e.Property(v => v.ApprovalNotes).HasMaxLength(500);

                // New optional fields
                e.Property(v => v.VisitLocation).HasMaxLength(100);
                e.Property(v => v.ArrivingWithVehicle);
                e.Property(v => v.VehicleType).HasMaxLength(20);
                e.Property(v => v.VehicleNumber).HasMaxLength(20);
                e.Property(v => v.FoodRequired);
                e.Property(v => v.FoodPreference).HasMaxLength(20);
                e.Property(v => v.ComplimentaryDrink);

                e.HasOne(v => v.GatePass)
                 .WithMany()
                 .HasForeignKey(v => v.GatePassId);

                e.HasMany(v => v.Entries)
                 .WithOne(e => e.Appointment)
                 .HasForeignKey(e => e.AppointmentId);
            });

            // VisitorEntry entity configuration (workflow)
            modelBuilder.Entity<VisitorEntry>(e =>
            {
                e.HasKey(v => v.Id);
                e.Property(v => v.GateNumber).HasMaxLength(20);
                e.Property(v => v.SecurityPersonnel).HasMaxLength(100);
            });

            base.OnModelCreating(modelBuilder);
        }

        // DbSets
        public DbSet<GatePass> GatePasses { get; set; }

        // Other existing tables
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Gate_Pass_management.Models.Room> Rooms { get; set; }
    public DbSet<Gate_Pass_management.Models.Meeting> Meetings { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SmsMessage> SmsMessages { get; set; }

        // Scheduler specific DbSets
        public DbSet<Domain.Scheduler.Room> SchedulerRooms { get; set; }
        public DbSet<Domain.Scheduler.Meeting> SchedulerMeetings { get; set; }
        public DbSet<Domain.Scheduler.Office> Offices { get; set; }
        public DbSet<Domain.Scheduler.MeetingAttendee> MeetingAttendees { get; set; }

        // Enhanced Gate Pass Workflow
        public DbSet<VisitorAppointment> VisitorAppointments { get; set; }
        public DbSet<VisitorEntry> VisitorEntries { get; set; }
    }
}
