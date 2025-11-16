using Microsoft.EntityFrameworkCore;
using autocare_api.Models;

namespace autocare_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceRecord> ServiceRecords { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceImage> InvoiceImages { get; set; }
        public DbSet<WorkshopProfile> WorkshopProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // -------------------------
            // Users
            // -------------------------
            builder.Entity<User>()
                .HasKey(x => x.Id);

            builder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            builder.Entity<User>()
                .Property(x => x.Role)
                .HasMaxLength(20);

            // -------------------------
            // Vehicles
            // -------------------------
            builder.Entity<Vehicle>()
                .HasKey(x => x.Id);

            builder.Entity<Vehicle>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Vehicle>()
                .HasIndex(v => v.PlateNumber);

            // -------------------------
            // Service Records
            // -------------------------
            builder.Entity<ServiceRecord>()
                .HasKey(x => x.Id);

            builder.Entity<ServiceRecord>()
                .HasOne(sr => sr.Vehicle)
                .WithMany(v => v.ServiceRecords)
                .HasForeignKey(sr => sr.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServiceRecord>()
                .HasOne(sr => sr.Workshop)
                .WithMany()    // Workshop doesn't need list of all records
                .HasForeignKey(sr => sr.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceRecord>()
                .HasOne(sr => sr.Invoice)
                .WithOne(i => i.ServiceRecord)
                .HasForeignKey<Invoice>(i => i.ServiceRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // Service Items
            // -------------------------
            builder.Entity<ServiceItem>()
                .HasKey(x => x.Id);

            builder.Entity<ServiceItem>()
                .HasOne(si => si.ServiceRecord)
                .WithMany(sr => sr.ServiceItems)
                .HasForeignKey(si => si.ServiceRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // Invoice
            // -------------------------
            builder.Entity<Invoice>()
                .HasKey(x => x.Id);

            builder.Entity<Invoice>()
                .Property(i => i.InvoiceNumber)
                .HasMaxLength(50);

            builder.Entity<Invoice>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoice>()
                .HasOne(i => i.Workshop)
                .WithMany()
                .HasForeignKey(i => i.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // Invoice Image (OCR)
            // -------------------------
            builder.Entity<InvoiceImage>()
                .HasKey(x => x.Id);

            builder.Entity<InvoiceImage>()
                .HasOne(ii => ii.ServiceRecord)
                .WithOne()
                .HasForeignKey<InvoiceImage>(ii => ii.ServiceRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // Map JSON field to PostgreSQL jsonb
            builder.Entity<InvoiceImage>()
                .Property(ii => ii.ExtractedJson)
                .HasColumnType("jsonb");

            // -------------------------
            // Workshop Profile
            // -------------------------
            builder.Entity<WorkshopProfile>()
                .HasKey(x => x.Id);

            builder.Entity<WorkshopProfile>()
                .HasOne(wp => wp.User)
                .WithOne()
                .HasForeignKey<WorkshopProfile>(wp => wp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkshopProfile>()
                .Property(wp => wp.WorkshopName)
                .HasMaxLength(100);
        }
    }
}
