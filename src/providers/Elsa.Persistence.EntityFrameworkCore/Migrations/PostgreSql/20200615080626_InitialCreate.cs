using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Elsa.Persistence.EntityFrameworkCore.Migrations.PostgreSql
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(nullable: true),
                    InstanceId = table.Column<string>(nullable: true),
                    DefinitionId = table.Column<string>(nullable: true),
                    CorrelationId = table.Column<string>(nullable: true),
                    Version = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    FinishedAt = table.Column<DateTime>(nullable: true),
                    FaultedAt = table.Column<DateTime>(nullable: true),
                    AbortedAt = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Fault = table.Column<string>(nullable: true),
                    ExecutionLog = table.Column<string>(nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitionVersions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(nullable: true),
                    VersionId = table.Column<string>(nullable: false),
                    DefinitionId = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Variables = table.Column<string>(nullable: false),
                    IsSingleton = table.Column<bool>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsLatest = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitionVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDefinitionVersions_WorkflowDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstanceTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(nullable: true),
                    ActivityId = table.Column<string>(nullable: false),
                    Tag = table.Column<string>(nullable: true),
                    Input = table.Column<string>(nullable: true),
                    WorkflowInstanceId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    ScheduleDate = table.Column<DateTime>(nullable: true),
                    ExecutionDate = table.Column<DateTime>(nullable: true),
                    IterationCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceTasks_WorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitionActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(nullable: true),
                    ActivityId = table.Column<string>(nullable: false),
                    WorkflowDefinitionVersionId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Left = table.Column<int>(nullable: false),
                    Top = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: false),
                    Output = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitionActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDefinitionActivities_WorkflowDefinitionVersions_Wor~",
                        column: x => x.WorkflowDefinitionVersionId,
                        principalTable: "WorkflowDefinitionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitionConnections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(nullable: true),
                    WorkflowDefinitionVersionId = table.Column<int>(nullable: false),
                    SourceActivityId = table.Column<string>(nullable: false),
                    DestinationActivityId = table.Column<string>(nullable: true),
                    Outcome = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitionConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDefinitionConnections_WorkflowDefinitionVersions_Wo~",
                        column: x => x.WorkflowDefinitionVersionId,
                        principalTable: "WorkflowDefinitionVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionActivities_WorkflowDefinitionVersionId",
                table: "WorkflowDefinitionActivities",
                column: "WorkflowDefinitionVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionConnections_WorkflowDefinitionVersionId",
                table: "WorkflowDefinitionConnections",
                column: "WorkflowDefinitionVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitionVersions_DefinitionId",
                table: "WorkflowDefinitionVersions",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceTasks_WorkflowInstanceId",
                table: "WorkflowInstanceTasks",
                column: "WorkflowInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowDefinitionActivities");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitionConnections");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceTasks");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitionVersions");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");
        }
    }
}
