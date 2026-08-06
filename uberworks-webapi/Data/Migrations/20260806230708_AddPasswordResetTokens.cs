using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uberworks_webapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_PASSWORD_RESET_TOKENS",
                columns: table => new
                {
                    PK_TOKEN_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_USER_ID = table.Column<int>(type: "int", nullable: false),
                    CL_TOKEN_HASH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CL_EXPIRES_AT = table.Column<DateTime>(type: "datetime", nullable: false),
                    CL_USED = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CL_CREATED_AT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PASSWORD_RESET_TOKENS", x => x.PK_TOKEN_ID);
                    table.ForeignKey(
                        name: "FK_TBL_PASSWORD_RESET_TOKENS_TBL_USERS_PK_USER_ID",
                        column: x => x.PK_USER_ID,
                        principalTable: "TBL_USERS",
                        principalColumn: "PK_USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PASSWORD_RESET_TOKENS_CL_TOKEN_HASH",
                table: "TBL_PASSWORD_RESET_TOKENS",
                column: "CL_TOKEN_HASH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PASSWORD_RESET_TOKENS_PK_USER_ID",
                table: "TBL_PASSWORD_RESET_TOKENS",
                column: "PK_USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_PASSWORD_RESET_TOKENS");
        }
    }
}
