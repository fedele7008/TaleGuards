using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthManager.Migrations
{
    /// <inheritdoc />
    public partial class CreateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Uid = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "CHAR(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME(6)", nullable: false, defaultValueSql: "(UTC_TIMESTAMP)"),
                    Username = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Verified = table.Column<bool>(type: "TINYINT(1)", nullable: false, defaultValue: false),
                    Admin = table.Column<bool>(type: "TINYINT(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Uid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecretKey = table.Column<string>(type: "CHAR(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConnectionUrl = table.Column<string>(type: "VARCHAR(512)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Sid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accesses",
                columns: table => new
                {
                    Uid = table.Column<int>(type: "INT", nullable: false),
                    Sid = table.Column<int>(type: "INT", nullable: false),
                    Banned = table.Column<bool>(type: "TINYINT(1)", nullable: false, defaultValue: false),
                    SuspensionEndAt = table.Column<DateTime>(type: "DATETIME(6)", nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesses", x => new { x.Sid, x.Uid });
                    table.ForeignKey(
                        name: "FK_Accesses_Accounts_Uid",
                        column: x => x.Uid,
                        principalTable: "Accounts",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accesses_Services_Sid",
                        column: x => x.Sid,
                        principalTable: "Services",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SuspensionLogs",
                columns: table => new
                {
                    Lid = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "VARCHAR(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoggedAt = table.Column<DateTime>(type: "DATETIME(6)", nullable: false, defaultValueSql: "(UTC_TIMESTAMP)"),
                    SuspensionEndAt = table.Column<DateTime>(type: "DATETIME(6)", nullable: true, defaultValueSql: "NULL"),
                    Reason = table.Column<string>(type: "VARCHAR(5000)", nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comment = table.Column<string>(type: "VARCHAR(1000)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssigneeUid = table.Column<int>(type: "INT", nullable: false),
                    AssignerUid = table.Column<int>(type: "INT", nullable: true),
                    Sid = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuspensionLogs", x => x.Lid);
                    table.ForeignKey(
                        name: "FK_SuspensionLogs_Accounts_AssigneeUid",
                        column: x => x.AssigneeUid,
                        principalTable: "Accounts",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuspensionLogs_Accounts_AssignerUid",
                        column: x => x.AssignerUid,
                        principalTable: "Accounts",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_SuspensionLogs_Services_Sid",
                        column: x => x.Sid,
                        principalTable: "Services",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Accesses_Uid",
                table: "Accesses",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_Name",
                table: "Services",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuspensionLogs_AssigneeUid",
                table: "SuspensionLogs",
                column: "AssigneeUid");

            migrationBuilder.CreateIndex(
                name: "IX_SuspensionLogs_AssignerUid",
                table: "SuspensionLogs",
                column: "AssignerUid");

            migrationBuilder.CreateIndex(
                name: "IX_SuspensionLogs_Sid",
                table: "SuspensionLogs",
                column: "Sid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accesses");

            migrationBuilder.DropTable(
                name: "SuspensionLogs");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
