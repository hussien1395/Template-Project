using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using Template_Project.Models;

namespace Template_Project.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog = Template_Project;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieSubImage>().HasKey(p => new { p.MovieId, p.ImagePath });
            modelBuilder.Entity<MovieActor>().HasKey(p => new { p.MovieId, p.ActorId });

            //modelBuilder.Entity<Category>().Property(b => b.Image).HasDefaultValue("defaultImg.png");
            //modelBuilder.Entity<Cinema>().Property(b => b.Image).HasDefaultValue("defaultImg.png");
            //modelBuilder.Entity<Actor>().Property(b => b.Image).HasDefaultValue("defaultImg.png");
        }
    }
}
