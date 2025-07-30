using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPIPeliculas.Migrations
{
    /// <inheritdoc />
    public partial class relationactormovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActorsMovies",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "INTEGER", nullable: false),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Character = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorsMovies", x => new { x.ActorId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_ActorsMovies_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorsMovies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorsMovies_MovieId",
                table: "ActorsMovies",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorsMovies");
        }
    }
}
