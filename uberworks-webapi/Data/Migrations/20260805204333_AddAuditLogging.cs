using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uberworks_webapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_ADMIN_ACTION_LOGS",
                columns: table => new
                {
                    PK_ADMIN_ACTION_LOG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CL_OCCURRED_AT = table.Column<DateTime>(type: "datetime", nullable: false),
                    CL_SOURCE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CL_ACTOR_USER_ID = table.Column<int>(type: "int", nullable: false),
                    CL_ACTOR_USERNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CL_ACTOR_ROLE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CL_ACTION = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CL_TARGET_ENTITY_TYPE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CL_TARGET_ENTITY_ID = table.Column<int>(type: "int", nullable: true),
                    CL_DETAILS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_IP_ADDRESS = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ADMIN_ACTION_LOGS", x => x.PK_ADMIN_ACTION_LOG_ID);
                });

            migrationBuilder.CreateTable(
                name: "TBL_ERROR_LOGS",
                columns: table => new
                {
                    PK_ERROR_LOG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CL_OCCURRED_AT = table.Column<DateTime>(type: "datetime", nullable: false),
                    CL_SOURCE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CL_USER_ID = table.Column<int>(type: "int", nullable: true),
                    CL_USERNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CL_REQUEST_METHOD = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CL_REQUEST_PATH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CL_STATUS_CODE = table.Column<int>(type: "int", nullable: false),
                    CL_EXCEPTION_TYPE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CL_MESSAGE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CL_STACK_TRACE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_IP_ADDRESS = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_ERROR_LOGS", x => x.PK_ERROR_LOG_ID);
                });

            migrationBuilder.CreateTable(
                name: "TBL_USER_ACTION_LOGS",
                columns: table => new
                {
                    PK_USER_ACTION_LOG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CL_OCCURRED_AT = table.Column<DateTime>(type: "datetime", nullable: false),
                    CL_SOURCE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CL_ACTOR_USER_ID = table.Column<int>(type: "int", nullable: true),
                    CL_ACTOR_USERNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CL_ACTION = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CL_TARGET_ENTITY_TYPE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CL_TARGET_ENTITY_ID = table.Column<int>(type: "int", nullable: true),
                    CL_DETAILS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_IP_ADDRESS = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_USER_ACTION_LOGS", x => x.PK_USER_ACTION_LOG_ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ADMIN_ACTION_LOGS_CL_ACTOR_USER_ID",
                table: "TBL_ADMIN_ACTION_LOGS",
                column: "CL_ACTOR_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ADMIN_ACTION_LOGS_CL_OCCURRED_AT",
                table: "TBL_ADMIN_ACTION_LOGS",
                column: "CL_OCCURRED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_ERROR_LOGS_CL_OCCURRED_AT",
                table: "TBL_ERROR_LOGS",
                column: "CL_OCCURRED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_ACTION_LOGS_CL_ACTOR_USER_ID",
                table: "TBL_USER_ACTION_LOGS",
                column: "CL_ACTOR_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USER_ACTION_LOGS_CL_OCCURRED_AT",
                table: "TBL_USER_ACTION_LOGS",
                column: "CL_OCCURRED_AT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_ADMIN_ACTION_LOGS");

            migrationBuilder.DropTable(
                name: "TBL_ERROR_LOGS");

            migrationBuilder.DropTable(
                name: "TBL_USER_ACTION_LOGS");
        }
    }
}
