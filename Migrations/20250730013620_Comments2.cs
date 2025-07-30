using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPIPeliculas.Migrations
{
    /// <inheritdoc />
    public partial class Comments2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "body",
                table: "Comments",
                newName: "Body");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Body",
                table: "Comments",
                newName: "body");
        }
    }
}
