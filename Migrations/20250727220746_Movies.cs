﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPIPeliculas.Migrations
{
    /// <inheritdoc />
    public partial class Movies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    InCinemas = table.Column<bool>(type: "INTEGER", nullable: false),
                    LaunchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Poster = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
