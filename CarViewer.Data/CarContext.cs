using CarViewer.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarViewer.Data {
    public class CarContext : DbContext {
        public CarContext(DbContextOptions<CarContext> options) : base(options) { }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<BodyConfiguration> BodyConfigurations { get; set; }
        public DbSet<ServiceRecord> ServiceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Car>().ToTable("Car");
            modelBuilder.Entity<Manufacturer>().ToTable("Manufacturer");
            modelBuilder.Entity<Model>().ToTable("Model");
            modelBuilder.Entity<BodyConfiguration>().ToTable("BodyConfiguration");
            modelBuilder.Entity<ServiceRecord>().ToTable("ServiceRecord");
        }
    }
}
