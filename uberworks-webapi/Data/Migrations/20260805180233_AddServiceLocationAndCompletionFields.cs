using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uberworks_webapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceLocationAndCompletionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CL_LOCATION",
                table: "TBL_SERVICES");

            migrationBuilder.AddColumn<DateTime>(
                name: "CL_CLIENT_CONFIRMED_AT",
                table: "TBL_SERVICES",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CL_COMPLETION_PHOTO_URL",
                table: "TBL_SERVICES",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CL_EXACT_ADDRESS",
                table: "TBL_SERVICES",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CL_LATITUDE",
                table: "TBL_SERVICES",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CL_LONGITUDE",
                table: "TBL_SERVICES",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CL_PROFESSIONAL_CONFIRMED_AT",
                table: "TBL_SERVICES",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CL_ZONE",
                table: "TBL_SERVICES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CL_ARRIVAL_CONFIRMED_AT",
                table: "TBL_SERVICE_PROFESSIONALS",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CL_ESTIMATED_ARRIVAL_MINUTES",
                table: "TBL_SERVICE_PROFESSIONALS",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CL_CLIENT_CONFIRMED_AT",
                table: "TBL_SERVICES");

            migrationBuilder.DropColumn(
                name: "CL_COMPLETION_PHOTO_URL",
                table: "TBL_SERVICES");

            migrationBuilder.DropColumn(
                name: "CL_EXACT_ADDRESS",
                table: "TBL_SERVICES");

            migrationBuilder.DropColumn(
                name: "CL_LATITUDE",
                table: "TBL_SERVICES");

            migrationBuilder.DropColumn(
                name: "CL_LONGITUDE",
                table: "TBL_SERVICES");

            migrationBuilder.DropColumn(
                name: "CL_PROFESSIONAL_CONFIRMED_AT",
                table: "TBL_SERVICES");

            migrationBuilder.DropColumn(
                name: "CL_ZONE",
                table: "TBL_SERVICES");

            migrationBuilder.DropColumn(
                name: "CL_ARRIVAL_CONFIRMED_AT",
                table: "TBL_SERVICE_PROFESSIONALS");

            migrationBuilder.DropColumn(
                name: "CL_ESTIMATED_ARRIVAL_MINUTES",
                table: "TBL_SERVICE_PROFESSIONALS");

            migrationBuilder.AddColumn<string>(
                name: "CL_LOCATION",
                table: "TBL_SERVICES",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
