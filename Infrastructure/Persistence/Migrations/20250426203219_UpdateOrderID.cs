using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            string tempOrderItemsTable = "TempOrderItems_" + Guid.NewGuid().ToString("N");
            string tempAvailabilityTable = "TempAvailability_" + Guid.NewGuid().ToString("N");


            migrationBuilder.CreateTable(
                name: tempOrderItemsTable,
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(nullable: false),
                    MedicineID = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{tempOrderItemsTable}", x => x.Id);
                    table.ForeignKey(
                        name: $"FK_{tempOrderItemsTable}_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: $"FK_{tempOrderItemsTable}_Medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql($@"
            INSERT INTO {tempOrderItemsTable} (
                OrderID, MedicineID, Quantity, Price,
                CreatedAt, UpdatedAt, IsDeleted
            )
            SELECT 
                OrderID, MedicineID, Quantity, Price,
                CreatedAt, UpdatedAt, IsDeleted
            FROM OrderItems
        ");

            migrationBuilder.DropTable("OrderItems");
            migrationBuilder.RenameTable(
                name: tempOrderItemsTable,
                newName: "OrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MedicineID",
                table: "OrderItems",
                column: "MedicineID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderID",
                table: "OrderItems",
                column: "OrderID");


            migrationBuilder.CreateTable(
                name: tempAvailabilityTable,
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
                    table.PrimaryKey($"PK_{tempAvailabilityTable}", x => x.Id);
                    table.ForeignKey(
                        name: $"FK_{tempAvailabilityTable}_MedicalStaff_MedicalStaffMemberId",
                        column: x => x.MedicalStaffMemberId,
                        principalTable: "MedicalStaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql($@"
            INSERT INTO {tempAvailabilityTable} (
                Day, FromTime, ToTime, MedicalStaffMemberId
            )
            SELECT 
                Day, FromTime, ToTime, MedicalStaffMemberId
            FROM AvailabilityEntries
        ");

            migrationBuilder.DropTable("AvailabilityEntries");
            migrationBuilder.RenameTable(
                name: tempAvailabilityTable,
                newName: "AvailabilityEntries");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityEntries_MedicalStaffMemberId",
                table: "AvailabilityEntries",
                column: "MedicalStaffMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable("OrderItems");
            migrationBuilder.DropTable("AvailabilityEntries");
        }
    }
}