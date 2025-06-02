using Microsoft.EntityFrameworkCore;
class DeviceDb : DbContext
{
    public DeviceDb(DbContextOptions<DeviceDb> options)
        : base(options) { }

    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .OwnsOne(d => d.location, locationBuilder =>
            {
                locationBuilder.Property(l => l.lat);
                locationBuilder.Property(l => l.lng);

                locationBuilder.OwnsMany(l => l.path);
                locationBuilder.OwnsMany(l => l.area);
            });
        modelBuilder.Entity<Device>().OwnsOne(d => d.weatherData);
    }
}
