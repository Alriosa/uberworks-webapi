using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uberworks_webapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeMoreFieldsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_REVIEWS_RATINGS",
                table: "TBL_REVIEWS");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PAYMENTS_METHOD",
                table: "TBL_PAYMENTS");

            migrationBuilder.AlterColumn<byte>(
                name: "CL_PROFESSIONAL_RATING",
                table: "TBL_REVIEWS",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_COMMENT",
                table: "TBL_REVIEWS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "CL_CLIENT_RATING",
                table: "TBL_REVIEWS",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_LOCATION",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_EXPERIENCE",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_DESCRIPTION",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_AVAILABILITY",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            // NOTE: EF Core originally scaffolded "defaultValue: new DateTime(1,1,1,...)" here
            // (i.e. DateTime.MinValue, year 0001), which SQL Server's DATETIME type can't
            // represent (its valid range starts at 1753-01-01) — that's what caused
            // "conversion of a varchar data type to a datetime data type ... out-of-range".
            // Using GETDATE() instead, same as every other datetime default in this project.
            migrationBuilder.AlterColumn<DateTime>(
                name: "CL_END_DATE",
                table: "TBL_PENALTIES",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_METHOD",
                table: "TBL_PAYMENTS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CL_AMOUNT",
                table: "TBL_PAYMENTS",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_USERNAME",
                table: "TBL_ERROR_LOGS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CL_MESSAGE",
                table: "TBL_CHATS",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_REVIEWS_RATINGS",
                table: "TBL_REVIEWS",
                sql: "CL_CLIENT_RATING BETWEEN 1 AND 5 AND CL_PROFESSIONAL_RATING BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PAYMENTS_METHOD",
                table: "TBL_PAYMENTS",
                sql: "CL_METHOD IN ('CREDITCARD','PAYPAL','ZELLE')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_REVIEWS_RATINGS",
                table: "TBL_REVIEWS");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PAYMENTS_METHOD",
                table: "TBL_PAYMENTS");

            migrationBuilder.AlterColumn<byte>(
                name: "CL_PROFESSIONAL_RATING",
                table: "TBL_REVIEWS",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "CL_COMMENT",
                table: "TBL_REVIEWS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "CL_CLIENT_RATING",
                table: "TBL_REVIEWS",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "CL_LOCATION",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CL_EXPERIENCE",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CL_DESCRIPTION",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CL_AVAILABILITY",
                table: "TBL_PROFESSIONALS",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CL_END_DATE",
                table: "TBL_PENALTIES",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "CL_METHOD",
                table: "TBL_PAYMENTS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "CL_AMOUNT",
                table: "TBL_PAYMENTS",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "CL_USERNAME",
                table: "TBL_ERROR_LOGS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CL_MESSAGE",
                table: "TBL_CHATS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_REVIEWS_RATINGS",
                table: "TBL_REVIEWS",
                sql: "(CL_CLIENT_RATING IS NULL OR CL_CLIENT_RATING BETWEEN 1 AND 5) AND (CL_PROFESSIONAL_RATING IS NULL OR CL_PROFESSIONAL_RATING BETWEEN 1 AND 5)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PAYMENTS_METHOD",
                table: "TBL_PAYMENTS",
                sql: "CL_METHOD IS NULL OR CL_METHOD IN ('CREDITCARD','PAYPAL','ZELLE')");
        }
    }
}
