using Microsoft.EntityFrameworkCore;

class DeviceDb : DbContext
{
    public DeviceDb(DbContextOptions<DeviceDb> options)
        : base(options) { }

    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .OwnsOne(d => d.Location, locationBuilder =>
            {
                locationBuilder.Property(l => l.Lat);
                locationBuilder.Property(l => l.Lng);
                
                locationBuilder.OwnsMany(l => l.Path);
                locationBuilder.OwnsMany(l => l.Area);
            });
    }
}   
