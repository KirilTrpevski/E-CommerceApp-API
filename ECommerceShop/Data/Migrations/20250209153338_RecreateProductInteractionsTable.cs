using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceShop.Migrations
{
    /// <inheritdoc />
    public partial class RecreateProductInteractionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the ProductInteractions table
            migrationBuilder.CreateTable(
                name: "ProductInteractions",
                columns: table => new
                {
                    InteractionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InteractionType = table.Column<string>(type: "TEXT", nullable: false),
                    InteractionDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInteractions", x => x.InteractionId);
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
            migrationBuilder.CreateIndex(
            name: "IX_ProductInteractions_ProductId",
            table: "ProductInteractions",
            column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInteractions_UserId",
                table: "ProductInteractions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the ProductInteractions table if the migration is rolled back
            migrationBuilder.DropTable(
                name: "ProductInteractions");
        }
    }
}
