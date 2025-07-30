using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(50);
            entity.HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Drama" },
                new Genre { Id = 3, Name = "Comedy" }
            );
        });

        modelBuilder.Entity<Actor>().Property(p => p.Name).HasMaxLength(50);
        modelBuilder.Entity<Actor>().Property(p => p.Photo).IsUnicode();

        modelBuilder.Entity<Movie>().Property(p => p.Title).HasMaxLength(50);
        modelBuilder.Entity<Movie>().Property(p => p.Poster).IsUnicode();
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Comment> Comments { get; set; }
}