using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodShop.Persistence.Migrations
{
    public partial class updateOrderEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderItemId",
                table: "OrderDetails",
                newName: "OrderDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "OrderDetails",
                newName: "OrderItemId");
        }
    }
}
