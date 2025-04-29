using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItemMedicineRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
            IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Medicines_MedicineID')
            BEGIN
                ALTER TABLE [OrderItems] DROP CONSTRAINT [FK_OrderItems_Medicines_MedicineID]
            END
        ");


            migrationBuilder.Sql(@"
            IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_MedicineID' AND object_id = OBJECT_ID('OrderItems'))
            BEGIN
                DROP INDEX [IX_OrderItems_MedicineID] ON [OrderItems]
            END
        ");

          
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'MedicineId' AND object_id = OBJECT_ID('OrderItems'))
                BEGIN
                    EXEC sp_rename 'OrderItems.MedicineId', 'MedicineID', 'COLUMN'
                END
            ");
            }

 
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Medicines_MedicineID",
                table: "OrderItems",
                column: "MedicineID",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);


            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MedicineID",
                table: "OrderItems",
                column: "MedicineID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Medicines_MedicineID",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_MedicineID",
                table: "OrderItems");

            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(@"
                EXEC sp_rename 'OrderItems.MedicineID', 'MedicineId', 'COLUMN'
            ");
            }

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Medicines_MedicineId",
                table: "OrderItems",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}