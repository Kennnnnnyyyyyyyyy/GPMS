using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gate_Pass_management.Migrations
{
    /// <inheritdoc />
    public partial class IntakeFormExtensions_VehicleFoodLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ArrivingWithVehicle",
                table: "VisitorAppointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ComplimentaryDrink",
                table: "VisitorAppointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FoodPreference",
                table: "VisitorAppointments",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FoodRequired",
                table: "VisitorAppointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VehicleNumber",
                table: "VisitorAppointments",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "VisitorAppointments",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisitLocation",
                table: "VisitorAppointments",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivingWithVehicle",
                table: "VisitorAppointments");

            migrationBuilder.DropColumn(
                name: "ComplimentaryDrink",
                table: "VisitorAppointments");

            migrationBuilder.DropColumn(
                name: "FoodPreference",
                table: "VisitorAppointments");

            migrationBuilder.DropColumn(
                name: "FoodRequired",
                table: "VisitorAppointments");

            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                table: "VisitorAppointments");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "VisitorAppointments");

            migrationBuilder.DropColumn(
                name: "VisitLocation",
                table: "VisitorAppointments");
        }
    }
}
