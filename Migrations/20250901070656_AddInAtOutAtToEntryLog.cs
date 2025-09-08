using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gate_Pass_management.Migrations
{
    /// <inheritdoc />
    public partial class AddInAtOutAtToEntryLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Appointments_AppointmentId",
                table: "EntryLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "PassId",
                table: "EntryLogs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppointmentId",
                table: "EntryLogs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "InAt",
                table: "EntryLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OutAt",
                table: "EntryLogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Appointments_AppointmentId",
                table: "EntryLogs",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryLogs_Appointments_AppointmentId",
                table: "EntryLogs");

            migrationBuilder.DropColumn(
                name: "InAt",
                table: "EntryLogs");

            migrationBuilder.DropColumn(
                name: "OutAt",
                table: "EntryLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "PassId",
                table: "EntryLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AppointmentId",
                table: "EntryLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryLogs_Appointments_AppointmentId",
                table: "EntryLogs",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
