using Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    /// <summary>
    /// EF Core DbContext for the gpsgateserver MySQL database.
    /// Tables registered: rfid_bins_reports, rfid_bins_37, users
    /// </summary>
    public class GpdDbContext : DbContext
    {
        public GpdDbContext(DbContextOptions<GpdDbContext> options) : base(options) { }

        public DbSet<RfidBinsReport> RfidBinsReports { get; set; }
        public DbSet<RfidBins37> RfidBins37 { get; set; }
        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // rfid_bins_reports
            modelBuilder.Entity<RfidBinsReport>(e =>
            {
                e.ToTable("rfid_bins_reports");
                e.HasKey(x => x.Id);
            });

            // rfid_bins_37
            modelBuilder.Entity<RfidBins37>(e =>
            {
                e.ToTable("rfid_bins_37");
                e.HasKey(x => x.Id);
            });

            // users
            modelBuilder.Entity<AppUser>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.UserId);
                e.HasIndex(x => x.Username).IsUnique();
            });
        }
    }
}
