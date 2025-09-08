using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gate_Pass_management.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorWorkflowEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Employees_EmployeeId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Visitors_VisitorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Appointments_AppointmentId",
                table: "EntryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Passes_PassId",
                table: "EntryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Visitors_VisitorId",
                table: "EntryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_GatePasses_Sites_SiteId",
                table: "GatePasses");

            migrationBuilder.DropForeignKey(
                name: "FK_GatePasses_VisitorsEntries_VisitorEntryId",
                table: "GatePasses");

            migrationBuilder.DropForeignKey(
                name: "FK_Passes_Appointments_AppointmentId",
                table: "Passes");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerMeetings_Employees_OrganizerId",
                table: "SchedulerMeetings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerMeetings_Office_OfficeId",
                table: "SchedulerMeetings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerMeetings_SchedulerRooms_RoomId",
                table: "SchedulerMeetings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerRooms_Office_OfficeId",
                table: "SchedulerRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeBlocks_Rooms_RoomId",
                table: "TimeBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeBlocks_Sites_SiteId",
                table: "TimeBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorsEntries_GatePasses_GatePassId",
                table: "VisitorsEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries");

            migrationBuilder.DropIndex(
                name: "IX_VisitorsEntries_GatePassId",
                table: "VisitorsEntries");

            migrationBuilder.DropIndex(
                name: "IX_SchedulerRooms_OfficeId_Name",
                table: "SchedulerRooms");

            migrationBuilder.DropIndex(
                name: "IX_SchedulerMeetings_OfficeId_RoomId_Start",
                table: "SchedulerMeetings");

            migrationBuilder.DropIndex(
                name: "IX_SchedulerMeetings_Start",
                table: "SchedulerMeetings");

            migrationBuilder.DropIndex(
                name: "IX_Punches_VisitorEntryId_TimestampUtc",
                table: "Punches");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_RoomId_StartUtc",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_MeetingAttendees_MeetingId_EmployeeId",
                table: "MeetingAttendees");

            migrationBuilder.DropIndex(
                name: "IX_GatePasses_PassNumber",
                table: "GatePasses");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Department",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmployeeId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_IsActive",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CreatedUtc",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visitors",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_ContactNumber",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_CreatedAt",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_Email",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_IdType_IdNumber",
                table: "Visitors");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_IsBlacklisted",
                table: "Visitors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeBlocks",
                table: "TimeBlocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Passes",
                table: "Passes");

            migrationBuilder.DropIndex(
                name: "IX_Passes_QrToken",
                table: "Passes");

            migrationBuilder.DropIndex(
                name: "IX_Passes_Status_ValidFrom",
                table: "Passes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Office",
                table: "Office");

            migrationBuilder.DropIndex(
                name: "IX_Office_Name",
                table: "Office");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntryLogs",
                table: "EntryLogs");

            migrationBuilder.DropIndex(
                name: "IX_EntryLogs_PassId_Action",
                table: "EntryLogs");

            migrationBuilder.DropIndex(
                name: "IX_EntryLogs_Timestamp",
                table: "EntryLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CreatedAt",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_Status_ScheduledDate",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "Visitors",
                newName: "Visitor");

            migrationBuilder.RenameTable(
                name: "TimeBlocks",
                newName: "TimeBlock");

            migrationBuilder.RenameTable(
                name: "Passes",
                newName: "Pass");

            migrationBuilder.RenameTable(
                name: "Office",
                newName: "Offices");

            migrationBuilder.RenameTable(
                name: "EntryLogs",
                newName: "EntryLog");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "Appointment");

            migrationBuilder.RenameIndex(
                name: "IX_TimeBlocks_SiteId",
                table: "TimeBlock",
                newName: "IX_TimeBlock_SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_TimeBlocks_RoomId",
                table: "TimeBlock",
                newName: "IX_TimeBlock_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Passes_AppointmentId",
                table: "Pass",
                newName: "IX_Pass_AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryLogs_VisitorId",
                table: "EntryLog",
                newName: "IX_EntryLog_VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryLogs_AppointmentId",
                table: "EntryLog",
                newName: "IX_EntryLog_AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_VisitorId",
                table: "Appointment",
                newName: "IX_Appointment_VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_EmployeeId",
                table: "Appointment",
                newName: "IX_Appointment_EmployeeId");

            migrationBuilder.AlterColumn<string>(
                name: "VisitorName",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "VisitorMobileNo",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitDateTime",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "VehicleType",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "VehicleNo",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "PurposeOfVisit",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Sites",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "VisitLocation",
                table: "LocalODs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "TypeOfLocalOD",
                table: "LocalODs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "PurposeOfVisit",
                table: "LocalODs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OutDateTime",
                table: "LocalODs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeNo",
                table: "LocalODs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "LocalODs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visitor",
                table: "Visitor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeBlock",
                table: "TimeBlock",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pass",
                table: "Pass",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Offices",
                table: "Offices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntryLog",
                table: "EntryLog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointment",
                table: "Appointment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "VisitorAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Mobile = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PurposeOfVisit = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScheduledTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    HostName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    HostDepartment = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    HostEmployeeId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    HostContactNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovalNotes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApprovedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: true),
                    GatePassId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitorAppointments_GatePasses_GatePassId",
                        column: x => x.GatePassId,
                        principalTable: "GatePasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VisitorAppointments_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisitorEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppointmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GateNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    SecurityPersonnel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitorEntries_VisitorAppointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "VisitorAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerMeetings_OfficeId",
                table: "SchedulerMeetings",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Punches_VisitorEntryId",
                table: "Punches",
                column: "VisitorEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_RoomId",
                table: "Meetings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_MeetingId",
                table: "MeetingAttendees",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryLog_PassId",
                table: "EntryLog",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAppointments_GatePassId",
                table: "VisitorAppointments",
                column: "GatePassId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAppointments_SiteId",
                table: "VisitorAppointments",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorEntries_AppointmentId",
                table: "VisitorEntries",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Employees_EmployeeId",
                table: "Appointment",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Visitor_VisitorId",
                table: "Appointment",
                column: "VisitorId",
                principalTable: "Visitor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLog_Appointment_AppointmentId",
                table: "EntryLog",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLog_Pass_PassId",
                table: "EntryLog",
                column: "PassId",
                principalTable: "Pass",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLog_Visitor_VisitorId",
                table: "EntryLog",
                column: "VisitorId",
                principalTable: "Visitor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GatePasses_Sites_SiteId",
                table: "GatePasses",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GatePasses_VisitorsEntries_VisitorEntryId",
                table: "GatePasses",
                column: "VisitorEntryId",
                principalTable: "VisitorsEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pass_Appointment_AppointmentId",
                table: "Pass",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerMeetings_Employees_OrganizerId",
                table: "SchedulerMeetings",
                column: "OrganizerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerMeetings_Offices_OfficeId",
                table: "SchedulerMeetings",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerMeetings_SchedulerRooms_RoomId",
                table: "SchedulerMeetings",
                column: "RoomId",
                principalTable: "SchedulerRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerRooms_Offices_OfficeId",
                table: "SchedulerRooms",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeBlock_Rooms_RoomId",
                table: "TimeBlock",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeBlock_Sites_SiteId",
                table: "TimeBlock",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Employees_EmployeeId",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Visitor_VisitorId",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLog_Appointment_AppointmentId",
                table: "EntryLog");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLog_Pass_PassId",
                table: "EntryLog");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryLog_Visitor_VisitorId",
                table: "EntryLog");

            migrationBuilder.DropForeignKey(
                name: "FK_GatePasses_Sites_SiteId",
                table: "GatePasses");

            migrationBuilder.DropForeignKey(
                name: "FK_GatePasses_VisitorsEntries_VisitorEntryId",
                table: "GatePasses");

            migrationBuilder.DropForeignKey(
                name: "FK_Pass_Appointment_AppointmentId",
                table: "Pass");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerMeetings_Employees_OrganizerId",
                table: "SchedulerMeetings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerMeetings_Offices_OfficeId",
                table: "SchedulerMeetings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerMeetings_SchedulerRooms_RoomId",
                table: "SchedulerMeetings");

            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerRooms_Offices_OfficeId",
                table: "SchedulerRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeBlock_Rooms_RoomId",
                table: "TimeBlock");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeBlock_Sites_SiteId",
                table: "TimeBlock");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries");

            migrationBuilder.DropTable(
                name: "VisitorEntries");

            migrationBuilder.DropTable(
                name: "VisitorAppointments");

            migrationBuilder.DropIndex(
                name: "IX_SchedulerMeetings_OfficeId",
                table: "SchedulerMeetings");

            migrationBuilder.DropIndex(
                name: "IX_Punches_VisitorEntryId",
                table: "Punches");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_RoomId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_MeetingAttendees_MeetingId",
                table: "MeetingAttendees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visitor",
                table: "Visitor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeBlock",
                table: "TimeBlock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pass",
                table: "Pass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Offices",
                table: "Offices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntryLog",
                table: "EntryLog");

            migrationBuilder.DropIndex(
                name: "IX_EntryLog_PassId",
                table: "EntryLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointment",
                table: "Appointment");

            migrationBuilder.RenameTable(
                name: "Visitor",
                newName: "Visitors");

            migrationBuilder.RenameTable(
                name: "TimeBlock",
                newName: "TimeBlocks");

            migrationBuilder.RenameTable(
                name: "Pass",
                newName: "Passes");

            migrationBuilder.RenameTable(
                name: "Offices",
                newName: "Office");

            migrationBuilder.RenameTable(
                name: "EntryLog",
                newName: "EntryLogs");

            migrationBuilder.RenameTable(
                name: "Appointment",
                newName: "Appointments");

            migrationBuilder.RenameIndex(
                name: "IX_TimeBlock_SiteId",
                table: "TimeBlocks",
                newName: "IX_TimeBlocks_SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_TimeBlock_RoomId",
                table: "TimeBlocks",
                newName: "IX_TimeBlocks_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Pass_AppointmentId",
                table: "Passes",
                newName: "IX_Passes_AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryLog_VisitorId",
                table: "EntryLogs",
                newName: "IX_EntryLogs_VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_EntryLog_AppointmentId",
                table: "EntryLogs",
                newName: "IX_EntryLogs_AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_VisitorId",
                table: "Appointments",
                newName: "IX_Appointments_VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_EmployeeId",
                table: "Appointments",
                newName: "IX_Appointments_EmployeeId");

            migrationBuilder.AlterColumn<string>(
                name: "VisitorName",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VisitorMobileNo",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitDateTime",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VehicleType",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VehicleNo",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PurposeOfVisit",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "VisitorsEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Sites",
                type: "INTEGER",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "VisitLocation",
                table: "LocalODs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TypeOfLocalOD",
                table: "LocalODs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PurposeOfVisit",
                table: "LocalODs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OutDateTime",
                table: "LocalODs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeNo",
                table: "LocalODs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "LocalODs",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visitors",
                table: "Visitors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeBlocks",
                table: "TimeBlocks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Passes",
                table: "Passes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Office",
                table: "Office",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntryLogs",
                table: "EntryLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorsEntries_GatePassId",
                table: "VisitorsEntries",
                column: "GatePassId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerRooms_OfficeId_Name",
                table: "SchedulerRooms",
                columns: new[] { "OfficeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerMeetings_OfficeId_RoomId_Start",
                table: "SchedulerMeetings",
                columns: new[] { "OfficeId", "RoomId", "Start" });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerMeetings_Start",
                table: "SchedulerMeetings",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_Punches_VisitorEntryId_TimestampUtc",
                table: "Punches",
                columns: new[] { "VisitorEntryId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_RoomId_StartUtc",
                table: "Meetings",
                columns: new[] { "RoomId", "StartUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_MeetingId_EmployeeId",
                table: "MeetingAttendees",
                columns: new[] { "MeetingId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GatePasses_PassNumber",
                table: "GatePasses",
                column: "PassNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Department",
                table: "Employees",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeId",
                table: "Employees",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsActive",
                table: "Employees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedUtc",
                table: "AuditLogs",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_ContactNumber",
                table: "Visitors",
                column: "ContactNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_CreatedAt",
                table: "Visitors",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_Email",
                table: "Visitors",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_IdType_IdNumber",
                table: "Visitors",
                columns: new[] { "IdType", "IdNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_IsBlacklisted",
                table: "Visitors",
                column: "IsBlacklisted");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_QrToken",
                table: "Passes",
                column: "QrToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passes_Status_ValidFrom",
                table: "Passes",
                columns: new[] { "Status", "ValidFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_Office_Name",
                table: "Office",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntryLogs_PassId_Action",
                table: "EntryLogs",
                columns: new[] { "PassId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_EntryLogs_Timestamp",
                table: "EntryLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CreatedAt",
                table: "Appointments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Status_ScheduledDate",
                table: "Appointments",
                columns: new[] { "Status", "ScheduledDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Employees_EmployeeId",
                table: "Appointments",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Visitors_VisitorId",
                table: "Appointments",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Appointments_AppointmentId",
                table: "EntryLogs",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Passes_PassId",
                table: "EntryLogs",
                column: "PassId",
                principalTable: "Passes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Visitors_VisitorId",
                table: "EntryLogs",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GatePasses_Sites_SiteId",
                table: "GatePasses",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GatePasses_VisitorsEntries_VisitorEntryId",
                table: "GatePasses",
                column: "VisitorEntryId",
                principalTable: "VisitorsEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passes_Appointments_AppointmentId",
                table: "Passes",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerMeetings_Employees_OrganizerId",
                table: "SchedulerMeetings",
                column: "OrganizerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerMeetings_Office_OfficeId",
                table: "SchedulerMeetings",
                column: "OfficeId",
                principalTable: "Office",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerMeetings_SchedulerRooms_RoomId",
                table: "SchedulerMeetings",
                column: "RoomId",
                principalTable: "SchedulerRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerRooms_Office_OfficeId",
                table: "SchedulerRooms",
                column: "OfficeId",
                principalTable: "Office",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeBlocks_Rooms_RoomId",
                table: "TimeBlocks",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeBlocks_Sites_SiteId",
                table: "TimeBlocks",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorsEntries_GatePasses_GatePassId",
                table: "VisitorsEntries",
                column: "GatePassId",
                principalTable: "GatePasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorsEntries_Sites_SiteId",
                table: "VisitorsEntries",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
