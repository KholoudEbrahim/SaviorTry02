using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixAvailabilityIdentityFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "TempAvailabilityEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<string>(nullable: false),
                    FromTime = table.Column<string>(nullable: false),
                    ToTime = table.Column<string>(nullable: false),
                    MedicalStaffMemberId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempAvailabilityEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TempAvailabilityEntries_MedicalStaffMembers_MedicalStaffMemberId",
                        column: x => x.MedicalStaffMemberId,
                        principalTable: "MedicalStaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.Sql(@"
            INSERT INTO TempAvailabilityEntries (Day, FromTime, ToTime, MedicalStaffMemberId)
            SELECT Day, FromTime, ToTime, MedicalStaffMemberId FROM AvailabilityEntries
        ");


            migrationBuilder.DropTable("AvailabilityEntries");


            migrationBuilder.RenameTable(
                name: "TempAvailabilityEntries",
                newName: "AvailabilityEntries");


            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityEntries_MedicalStaffMemberId",
                table: "AvailabilityEntries",
                column: "MedicalStaffMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable("AvailabilityEntries");

            migrationBuilder.CreateTable(
                name: "AvailabilityEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Day = table.Column<string>(nullable: false),
                    FromTime = table.Column<string>(nullable: false),
                    ToTime = table.Column<string>(nullable: false),
                    MedicalStaffMemberId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailabilityEntries_MedicalStaffMembers_MedicalStaffMemberId",
                        column: x => x.MedicalStaffMemberId,
                        principalTable: "MedicalStaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}

