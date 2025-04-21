using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceShop.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductInteractionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This will remove the ProductInteractions table from the database
            migrationBuilder.DropTable(
                name: "ProductInteractions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This will re-create the ProductInteractions table in case you want to undo the migration
            migrationBuilder.CreateTable(
                name: "ProductInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    InteractionType = table.Column<string>(nullable: true),
                    InteractionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductInteractions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductInteractions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
