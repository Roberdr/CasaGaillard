using Microsoft.EntityFrameworkCore;
using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<IntervencionViewModel> Intervenciones { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map minimal model for initial compilation. Detailed mapping will be added later.
            modelBuilder.Entity<IntervencionViewModel>(eb =>
            {
                eb.HasKey(e => e.Id);
                eb.Property(e => e.Titulo).HasMaxLength(200);
            });
        }
    }
}
