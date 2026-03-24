using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MedLabAInsights.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BloodGroup = table.Column<int>(type: "integer", nullable: false),
                    Contact = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "Panel",
                columns: table => new
                {
                    PanelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PanelName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PanelCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panel", x => x.PanelId);
                });

            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TestCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MinValue = table.Column<double>(type: "double precision", nullable: true),
                    MaxValue = table.Column<double>(type: "double precision", nullable: true),
                    CustomValues = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.TestId);
                });

            migrationBuilder.CreateTable(
                name: "PanelRuleSummary",
                columns: table => new
                {
                    PanelRuleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PanelId = table.Column<int>(type: "integer", nullable: false),
                    PanelRuleName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PanelRuleCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MinSeverity = table.Column<int>(type: "integer", nullable: false),
                    MaxSeverity = table.Column<int>(type: "integer", nullable: false),
                    StandardSummary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelRuleSummary", x => x.PanelRuleId);
                    table.ForeignKey(
                        name: "FK_PanelRuleSummary_Panel_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panel",
                        principalColumn: "PanelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visit",
                columns: table => new
                {
                    VisitId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    VisitDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PanelId = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<int>(type: "integer", nullable: true),
                    Systolic = table.Column<int>(type: "integer", nullable: true),
                    Diastolic = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visit", x => x.VisitId);
                    table.ForeignKey(
                        name: "FK_Visit_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visit_Panel_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panel",
                        principalColumn: "PanelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BandRuleReport",
                columns: table => new
                {
                    BandId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestId = table.Column<int>(type: "integer", nullable: false),
                    BandName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BandCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RangeMin = table.Column<double>(type: "double precision", nullable: false),
                    RangeMax = table.Column<double>(type: "double precision", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    CustomValue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StandardReport = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BandRuleReport", x => x.BandId);
                    table.ForeignKey(
                        name: "FK_BandRuleReport_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PanelTestMapping",
                columns: table => new
                {
                    PanelTestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PanelId = table.Column<int>(type: "integer", nullable: false),
                    TestId = table.Column<int>(type: "integer", nullable: false),
                    ImportanceLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelTestMapping", x => x.PanelTestId);
                    table.ForeignKey(
                        name: "FK_PanelTestMapping_Panel_PanelId",
                        column: x => x.PanelId,
                        principalTable: "Panel",
                        principalColumn: "PanelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PanelTestMapping_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitPanelSummary",
                columns: table => new
                {
                    VisitPanelSummaryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VisitId = table.Column<int>(type: "integer", nullable: false),
                    PanelRuleId = table.Column<int>(type: "integer", nullable: false),
                    StandardSummarySnapshot = table.Column<string>(type: "text", nullable: false),
                    RevisedSummary = table.Column<string>(type: "text", nullable: true),
                    IsRevised = table.Column<bool>(type: "boolean", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitPanelSummary", x => x.VisitPanelSummaryId);
                    table.ForeignKey(
                        name: "FK_VisitPanelSummary_PanelRuleSummary_PanelRuleId",
                        column: x => x.PanelRuleId,
                        principalTable: "PanelRuleSummary",
                        principalColumn: "PanelRuleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitPanelSummary_Visit_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visit",
                        principalColumn: "VisitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitTestResult",
                columns: table => new
                {
                    VisitTestResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VisitId = table.Column<int>(type: "integer", nullable: false),
                    TestId = table.Column<int>(type: "integer", nullable: false),
                    RawValue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EnteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitTestResult", x => x.VisitTestResultId);
                    table.ForeignKey(
                        name: "FK_VisitTestResult_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitTestResult_Visit_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visit",
                        principalColumn: "VisitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitTestInterpretation",
                columns: table => new
                {
                    VisitTestInterpretationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VisitTestResultId = table.Column<int>(type: "integer", nullable: false),
                    BandId = table.Column<int>(type: "integer", nullable: false),
                    StandardReportSnapshot = table.Column<string>(type: "text", nullable: false),
                    RevisedReport = table.Column<string>(type: "text", nullable: true),
                    IsRevised = table.Column<bool>(type: "boolean", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitTestInterpretation", x => x.VisitTestInterpretationId);
                    table.ForeignKey(
                        name: "FK_VisitTestInterpretation_BandRuleReport_BandId",
                        column: x => x.BandId,
                        principalTable: "BandRuleReport",
                        principalColumn: "BandId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitTestInterpretation_VisitTestResult_VisitTestResultId",
                        column: x => x.VisitTestResultId,
                        principalTable: "VisitTestResult",
                        principalColumn: "VisitTestResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BandRuleReport_TestId",
                table: "BandRuleReport",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_BandRuleReport_TestId_BandCode",
                table: "BandRuleReport",
                columns: new[] { "TestId", "BandCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_Contact",
                table: "Member",
                column: "Contact",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PanelRuleSummary_PanelId",
                table: "PanelRuleSummary",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_PanelRuleSummary_PanelId_PanelRuleCode",
                table: "PanelRuleSummary",
                columns: new[] { "PanelId", "PanelRuleCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PanelTestMapping_PanelId_TestId",
                table: "PanelTestMapping",
                columns: new[] { "PanelId", "TestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PanelTestMapping_TestId",
                table: "PanelTestMapping",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Test_TestName",
                table: "Test",
                column: "TestName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visit_MemberId_VisitDateTime",
                table: "Visit",
                columns: new[] { "MemberId", "VisitDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Visit_PanelId",
                table: "Visit",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPanelSummary_EvaluatedAt",
                table: "VisitPanelSummary",
                column: "EvaluatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPanelSummary_PanelRuleId",
                table: "VisitPanelSummary",
                column: "PanelRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPanelSummary_VisitId",
                table: "VisitPanelSummary",
                column: "VisitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestInterpretation_BandId",
                table: "VisitTestInterpretation",
                column: "BandId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestInterpretation_EvaluatedAt",
                table: "VisitTestInterpretation",
                column: "EvaluatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestInterpretation_VisitTestResultId",
                table: "VisitTestInterpretation",
                column: "VisitTestResultId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestResult_TestId",
                table: "VisitTestResult",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTestResult_VisitId_TestId",
                table: "VisitTestResult",
                columns: new[] { "VisitId", "TestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PanelTestMapping");

            migrationBuilder.DropTable(
                name: "VisitPanelSummary");

            migrationBuilder.DropTable(
                name: "VisitTestInterpretation");

            migrationBuilder.DropTable(
                name: "PanelRuleSummary");

            migrationBuilder.DropTable(
                name: "BandRuleReport");

            migrationBuilder.DropTable(
                name: "VisitTestResult");

            migrationBuilder.DropTable(
                name: "Test");

            migrationBuilder.DropTable(
                name: "Visit");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Panel");
        }
    }
}
