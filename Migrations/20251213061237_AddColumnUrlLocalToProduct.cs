using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnUrlLocalToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "img_url",
                table: "Product",
                type: "varchar(400)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(400)");

            migrationBuilder.AddColumn<string>(
                name: "img_url_local",
                table: "Product",
                type: "varchar(400)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img_url_local",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "img_url",
                table: "Product",
                type: "varchar(400)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(400)",
                oldNullable: true);
        }
    }
}
