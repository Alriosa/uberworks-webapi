using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uberworks_webapi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBL_USERS",
                columns: table => new
                {
                    PK_USER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CL_FIRST_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CL_LAST_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CL_EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CL_PHONE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CL_PASSWORD = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CL_ROLE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CL_STATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    CL_REGISTRATION_DATE = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_USERS", x => x.PK_USER_ID);
                    table.CheckConstraint("CK_USERS_ROLE", "CL_ROLE IN ('CLIENT','PROFESSIONAL','ADMINISTRATOR','SUPPORT')");
                });

            migrationBuilder.CreateTable(
                name: "TBL_WORKTYPES",
                columns: table => new
                {
                    PK_WORK_TYPE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CL_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CL_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_INCLUDES = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_NOT_INCLUDES = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_WORKTYPES", x => x.PK_WORK_TYPE_ID);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PENALTIES",
                columns: table => new
                {
                    PK_PENALTY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_USER_ID = table.Column<int>(type: "int", nullable: false),
                    CL_TYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CL_REASON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_START_DATE = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    CL_END_DATE = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PENALTIES", x => x.PK_PENALTY_ID);
                    table.ForeignKey(
                        name: "FK_TBL_PENALTIES_TBL_USERS_PK_USER_ID",
                        column: x => x.PK_USER_ID,
                        principalTable: "TBL_USERS",
                        principalColumn: "PK_USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PROFESSIONALS",
                columns: table => new
                {
                    PK_PROFESSIONAL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_USER_ID = table.Column<int>(type: "int", nullable: false),
                    CL_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_EXPERIENCE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_AVAILABILITY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CL_LOCATION = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CL_AVERAGE_RATING = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PROFESSIONALS", x => x.PK_PROFESSIONAL_ID);
                    table.ForeignKey(
                        name: "FK_TBL_PROFESSIONALS_TBL_USERS_PK_USER_ID",
                        column: x => x.PK_USER_ID,
                        principalTable: "TBL_USERS",
                        principalColumn: "PK_USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_REWARDS",
                columns: table => new
                {
                    PK_REWARD_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_USER_ID = table.Column<int>(type: "int", nullable: false),
                    CL_POINTS = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CL_LAST_UPDATE_DATE = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_REWARDS", x => x.PK_REWARD_ID);
                    table.ForeignKey(
                        name: "FK_TBL_REWARDS_TBL_USERS_PK_USER_ID",
                        column: x => x.PK_USER_ID,
                        principalTable: "TBL_USERS",
                        principalColumn: "PK_USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_SERVICES",
                columns: table => new
                {
                    PK_SERVICE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_WORK_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    CL_CLIENT_ID = table.Column<int>(type: "int", nullable: false),
                    CL_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_IMAGE_URL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CL_LOCATION = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CL_PROPOSED_PRICE = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CL_STATUS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    CL_REQUEST_DATE = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SERVICES", x => x.PK_SERVICE_ID);
                    table.ForeignKey(
                        name: "FK_TBL_SERVICES_TBL_USERS_CL_CLIENT_ID",
                        column: x => x.CL_CLIENT_ID,
                        principalTable: "TBL_USERS",
                        principalColumn: "PK_USER_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_SERVICES_TBL_WORKTYPES_PK_WORK_TYPE_ID",
                        column: x => x.PK_WORK_TYPE_ID,
                        principalTable: "TBL_WORKTYPES",
                        principalColumn: "PK_WORK_TYPE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CHATS",
                columns: table => new
                {
                    PK_CHAT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_PROFESSIONAL_ID = table.Column<int>(type: "int", nullable: false),
                    CL_CLIENT_ID = table.Column<int>(type: "int", nullable: false),
                    CL_MESSAGE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_MESSAGE_DATE = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CHATS", x => x.PK_CHAT_ID);
                    table.ForeignKey(
                        name: "FK_TBL_CHATS_TBL_PROFESSIONALS_PK_PROFESSIONAL_ID",
                        column: x => x.PK_PROFESSIONAL_ID,
                        principalTable: "TBL_PROFESSIONALS",
                        principalColumn: "PK_PROFESSIONAL_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_CHATS_TBL_USERS_CL_CLIENT_ID",
                        column: x => x.CL_CLIENT_ID,
                        principalTable: "TBL_USERS",
                        principalColumn: "PK_USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_PAYMENTS",
                columns: table => new
                {
                    PK_PAYMENT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_SERVICE_ID = table.Column<int>(type: "int", nullable: false),
                    CL_METHOD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CL_AMOUNT = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CL_STATUS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    CL_PAYMENT_DATE = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_PAYMENTS", x => x.PK_PAYMENT_ID);
                    table.CheckConstraint("CK_PAYMENTS_METHOD", "CL_METHOD IS NULL OR CL_METHOD IN ('CREDITCARD','PAYPAL','ZELLE')");
                    table.ForeignKey(
                        name: "FK_TBL_PAYMENTS_TBL_SERVICES_PK_SERVICE_ID",
                        column: x => x.PK_SERVICE_ID,
                        principalTable: "TBL_SERVICES",
                        principalColumn: "PK_SERVICE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_REVIEWS",
                columns: table => new
                {
                    PK_REVIEW_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_PROFESSIONAL_ID = table.Column<int>(type: "int", nullable: false),
                    PK_SERVICE_ID = table.Column<int>(type: "int", nullable: false),
                    CL_CLIENT_ID = table.Column<int>(type: "int", nullable: false),
                    CL_CLIENT_RATING = table.Column<byte>(type: "tinyint", nullable: true),
                    CL_PROFESSIONAL_RATING = table.Column<byte>(type: "tinyint", nullable: true),
                    CL_COMMENT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CL_REVIEW_DATE = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_REVIEWS", x => x.PK_REVIEW_ID);
                    table.CheckConstraint("CK_REVIEWS_RATINGS", "(CL_CLIENT_RATING IS NULL OR CL_CLIENT_RATING BETWEEN 1 AND 5) AND (CL_PROFESSIONAL_RATING IS NULL OR CL_PROFESSIONAL_RATING BETWEEN 1 AND 5)");
                    table.ForeignKey(
                        name: "FK_TBL_REVIEWS_TBL_PROFESSIONALS_PK_PROFESSIONAL_ID",
                        column: x => x.PK_PROFESSIONAL_ID,
                        principalTable: "TBL_PROFESSIONALS",
                        principalColumn: "PK_PROFESSIONAL_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_REVIEWS_TBL_SERVICES_PK_SERVICE_ID",
                        column: x => x.PK_SERVICE_ID,
                        principalTable: "TBL_SERVICES",
                        principalColumn: "PK_SERVICE_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_REVIEWS_TBL_USERS_CL_CLIENT_ID",
                        column: x => x.CL_CLIENT_ID,
                        principalTable: "TBL_USERS",
                        principalColumn: "PK_USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBL_SERVICE_PROFESSIONALS",
                columns: table => new
                {
                    PK_SERVICE_PROFESSIONAL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PK_PROFESSIONAL_ID = table.Column<int>(type: "int", nullable: false),
                    PK_SERVICE_ID = table.Column<int>(type: "int", nullable: false),
                    CL_NEGOTIATED_PRICE = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CL_STATUS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "UNDER NEGOTIATION")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SERVICE_PROFESSIONALS", x => x.PK_SERVICE_PROFESSIONAL_ID);
                    table.ForeignKey(
                        name: "FK_TBL_SERVICE_PROFESSIONALS_TBL_PROFESSIONALS_PK_PROFESSIONAL_ID",
                        column: x => x.PK_PROFESSIONAL_ID,
                        principalTable: "TBL_PROFESSIONALS",
                        principalColumn: "PK_PROFESSIONAL_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBL_SERVICE_PROFESSIONALS_TBL_SERVICES_PK_SERVICE_ID",
                        column: x => x.PK_SERVICE_ID,
                        principalTable: "TBL_SERVICES",
                        principalColumn: "PK_SERVICE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CHATS_CL_CLIENT_ID",
                table: "TBL_CHATS",
                column: "CL_CLIENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CHATS_PK_PROFESSIONAL_ID",
                table: "TBL_CHATS",
                column: "PK_PROFESSIONAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PAYMENTS_PK_SERVICE_ID",
                table: "TBL_PAYMENTS",
                column: "PK_SERVICE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PENALTIES_PK_USER_ID",
                table: "TBL_PENALTIES",
                column: "PK_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_PROFESSIONALS_PK_USER_ID",
                table: "TBL_PROFESSIONALS",
                column: "PK_USER_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_REVIEWS_CL_CLIENT_ID",
                table: "TBL_REVIEWS",
                column: "CL_CLIENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_REVIEWS_PK_PROFESSIONAL_ID",
                table: "TBL_REVIEWS",
                column: "PK_PROFESSIONAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_REVIEWS_PK_SERVICE_ID",
                table: "TBL_REVIEWS",
                column: "PK_SERVICE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_REWARDS_PK_USER_ID",
                table: "TBL_REWARDS",
                column: "PK_USER_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SERVICE_PROFESSIONALS_PK_PROFESSIONAL_ID",
                table: "TBL_SERVICE_PROFESSIONALS",
                column: "PK_PROFESSIONAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SERVICE_PROFESSIONALS_PK_SERVICE_ID",
                table: "TBL_SERVICE_PROFESSIONALS",
                column: "PK_SERVICE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SERVICES_CL_CLIENT_ID",
                table: "TBL_SERVICES",
                column: "CL_CLIENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_SERVICES_PK_WORK_TYPE_ID",
                table: "TBL_SERVICES",
                column: "PK_WORK_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_USERS_CL_EMAIL",
                table: "TBL_USERS",
                column: "CL_EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_CHATS");

            migrationBuilder.DropTable(
                name: "TBL_PAYMENTS");

            migrationBuilder.DropTable(
                name: "TBL_PENALTIES");

            migrationBuilder.DropTable(
                name: "TBL_REVIEWS");

            migrationBuilder.DropTable(
                name: "TBL_REWARDS");

            migrationBuilder.DropTable(
                name: "TBL_SERVICE_PROFESSIONALS");

            migrationBuilder.DropTable(
                name: "TBL_PROFESSIONALS");

            migrationBuilder.DropTable(
                name: "TBL_SERVICES");

            migrationBuilder.DropTable(
                name: "TBL_USERS");

            migrationBuilder.DropTable(
                name: "TBL_WORKTYPES");
        }
    }
}
