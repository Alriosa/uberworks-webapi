using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uberworks_webapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS");

            migrationBuilder.AddCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS",
                sql: "CL_ROLE IN ('MASTER_ADMIN','ADMIN','CLIENT','PROFESSIONAL')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS");

            migrationBuilder.AddCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS",
                sql: "CL_ROLE IN ('CLIENT','PROFESSIONAL','ADMINISTRATOR','SUPPORT')");
        }
    }
}
