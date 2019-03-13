using Microsoft.EntityFrameworkCore;
using WM.GUID.Domain;

namespace WM.GUID.Persistence
{
    public class WMDbContext : DbContext
    {
        public WMDbContext(DbContextOptions<WMDbContext> options)
            : base(options)
        {
        }

        public DbSet<GuidMetadata> GUIDs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WMDbContext).Assembly);
        }
    }
}
