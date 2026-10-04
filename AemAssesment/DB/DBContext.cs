using AemAssesment.DB;
using Microsoft.EntityFrameworkCore;

namespace AemAssesment;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options) { }

    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Well> Wells => Set<Well>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Platform>(e =>
        {
            e.ToTable("Platform");
            e.Property(p => p.Id).ValueGeneratedNever();
        });

        b.Entity<Well>(e =>
        {
            e.ToTable("Well");
            e.Property(w => w.Id).ValueGeneratedNever();
            e.HasOne(w => w.Platform)
             .WithMany(p => p.Wells)
             .HasForeignKey(w => w.PlatformId);
        });
    }
}
