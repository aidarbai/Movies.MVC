using Cinema.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.DbContext
{
    public class CinemaDbContext : IdentityDbContext<ApplicationUser>
    {
        public CinemaDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genres> Genres { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(a => a.HasKey(user => user.Id));
            
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Movie>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
