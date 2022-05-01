using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HFM.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ConnectionString = table.Column<string>(type: "TEXT", nullable: false),
                    Guid = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientVersion = table.Column<string>(type: "TEXT", nullable: false),
                    OperatingSystem = table.Column<string>(type: "TEXT", nullable: false),
                    Implementation = table.Column<string>(type: "TEXT", nullable: false),
                    Processor = table.Column<string>(type: "TEXT", nullable: false),
                    Threads = table.Column<int>(type: "INTEGER", nullable: true),
                    DriverVersion = table.Column<string>(type: "TEXT", nullable: true),
                    ComputeVersion = table.Column<string>(type: "TEXT", nullable: true),
                    CUDAVersion = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Proteins",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectID = table.Column<int>(type: "INTEGER", nullable: false),
                    Credit = table.Column<double>(type: "REAL", nullable: false),
                    KFactor = table.Column<double>(type: "REAL", nullable: false),
                    Frames = table.Column<int>(type: "INTEGER", nullable: false),
                    Core = table.Column<string>(type: "TEXT", nullable: true),
                    Atoms = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeoutDays = table.Column<double>(type: "REAL", nullable: false),
                    ExpirationDays = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proteins", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WorkUnits",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DonorName = table.Column<string>(type: "TEXT", nullable: true),
                    DonorTeam = table.Column<int>(type: "INTEGER", nullable: false),
                    CoreVersion = table.Column<string>(type: "TEXT", nullable: true),
                    Result = table.Column<string>(type: "TEXT", nullable: true),
                    Assigned = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Finished = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProjectRun = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectClone = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectGen = table.Column<int>(type: "INTEGER", nullable: false),
                    HexID = table.Column<string>(type: "TEXT", nullable: true),
                    FramesCompleted = table.Column<int>(type: "INTEGER", nullable: true),
                    FrameTimeInSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    ProteinID = table.Column<long>(type: "INTEGER", nullable: false),
                    ClientID = table.Column<long>(type: "INTEGER", nullable: false),
                    PlatformID = table.Column<long>(type: "INTEGER", nullable: true),
                    ClientSlot = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkUnits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkUnits_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkUnits_Platforms_PlatformID",
                        column: x => x.PlatformID,
                        principalTable: "Platforms",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WorkUnits_Proteins_ProteinID",
                        column: x => x.ProteinID,
                        principalTable: "Proteins",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkUnitFrames",
                columns: table => new
                {
                    WorkUnitID = table.Column<long>(type: "INTEGER", nullable: false),
                    FrameID = table.Column<int>(type: "INTEGER", nullable: false),
                    RawFramesComplete = table.Column<int>(type: "INTEGER", nullable: false),
                    RawFramesTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeStamp = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkUnitFrames", x => new { x.WorkUnitID, x.FrameID });
                    table.ForeignKey(
                        name: "FK_WorkUnitFrames_WorkUnits_WorkUnitID",
                        column: x => x.WorkUnitID,
                        principalTable: "WorkUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkUnits_ClientID",
                table: "WorkUnits",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkUnits_PlatformID",
                table: "WorkUnits",
                column: "PlatformID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkUnits_ProteinID",
                table: "WorkUnits",
                column: "ProteinID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "WorkUnitFrames");

            migrationBuilder.DropTable(
                name: "WorkUnits");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Proteins");
        }
    }
}
