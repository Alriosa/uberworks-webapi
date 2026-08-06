using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uberworks_webapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerCompanyRolesAndWorkerLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS");

            migrationBuilder.AddColumn<int>(
                name: "CL_COMPANY_USER_ID",
                table: "TBL_PROFESSIONALS",
                type: "int",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS",
                sql: "CL_ROLE IN ('MASTER_ADMIN','ADMIN','CLIENT','PROFESSIONAL','MANAGER','COMPANY')");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PROFESSIONALS_CL_COMPANY_USER_ID",
                table: "TBL_PROFESSIONALS",
                column: "CL_COMPANY_USER_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TBL_PROFESSIONALS_TBL_USERS_CL_COMPANY_USER_ID",
                table: "TBL_PROFESSIONALS",
                column: "CL_COMPANY_USER_ID",
                principalTable: "TBL_USERS",
                principalColumn: "PK_USER_ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBL_PROFESSIONALS_TBL_USERS_CL_COMPANY_USER_ID",
                table: "TBL_PROFESSIONALS");

            migrationBuilder.DropCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS");

            migrationBuilder.DropIndex(
                name: "IX_TBL_PROFESSIONALS_CL_COMPANY_USER_ID",
                table: "TBL_PROFESSIONALS");

            migrationBuilder.DropColumn(
                name: "CL_COMPANY_USER_ID",
                table: "TBL_PROFESSIONALS");

            migrationBuilder.AddCheckConstraint(
                name: "CK_USERS_ROLE",
                table: "TBL_USERS",
                sql: "CL_ROLE IN ('MASTER_ADMIN','ADMIN','CLIENT','PROFESSIONAL')");
        }
    }
}
