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
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<InvoiceImage> InvoiceImages { get; set; }
        public DbSet<WorkshopProfile> WorkshopProfiles { get; set; }

        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceComponent> ServiceComponents { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // -------------------------
            // Users
            // -------------------------
            builder.Entity<User>()
                .HasKey(x => x.Id);

            builder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            // -------------------------
            // Password Reset Tokens
            // -------------------------
            builder.Entity<PasswordResetToken>()
                .HasKey(x => x.Id);

            builder.Entity<PasswordResetToken>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PasswordResetToken>()
                .HasIndex(t => new { t.UserId, t.ExpiresAt });

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
                .HasOne(sr => sr.WorkshopProfile)
                .WithMany()
                .HasForeignKey(sr => sr.WorkshopProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // NEW: service link
            builder.Entity<ServiceRecord>()
                .HasOne(sr => sr.Service)
                .WithMany(s => s.ServiceRecords)
                .HasForeignKey(sr => sr.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

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
            // Services
            // -------------------------
            builder.Entity<Service>()
                .HasKey(s => s.Id);

            builder.Entity<Service>()
                .HasOne(s => s.WorkshopProfile)
                .WithMany(wp => wp.Services)
                .HasForeignKey(s => s.WorkshopProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServiceComponent>()
                .HasKey(sc => sc.Id);

            builder.Entity<ServiceComponent>()
                .HasOne(sc => sc.Service)
                .WithMany(s => s.Components)
                .HasForeignKey(sc => sc.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // Invoices
            // -------------------------
            builder.Entity<Invoices>()
                .HasKey(x => x.Id);

            builder.Entity<Invoices>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoices>()
                .HasOne(i => i.Workshop)
                .WithMany()
                .HasForeignKey(i => i.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // Invoice Images
            // -------------------------
            builder.Entity<InvoiceImage>()
                .HasKey(x => x.Id);

            builder.Entity<InvoiceImage>()
                .HasOne(ii => ii.ServiceRecord)
                .WithOne()
                .HasForeignKey<InvoiceImage>(ii => ii.ServiceRecordId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .Property(wp => wp.OperatingHours)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                    v => System.Text.Json.JsonSerializer.Deserialize<WeeklyOperatingHours>(v, jsonOptions)
                            ?? new WeeklyOperatingHours()
                )
                .HasColumnType("jsonb");

            builder.Entity<WorkshopProfile>()
                .Property(wp => wp.Address)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, jsonOptions),
                    v => System.Text.Json.JsonSerializer.Deserialize<AddressObject>(v, jsonOptions)
                            ?? new AddressObject()
                )
                .HasColumnType("jsonb");

            // new: store enum as string so the database shows "Pending", "Approved", "Rejected"
            builder.Entity<WorkshopProfile>()
                .Property(wp => wp.ApprovalStatus)
                .HasConversion<string>();

            // new: index for faster queries like "only approved workshops"
            builder.Entity<WorkshopProfile>()
                .HasIndex(wp => wp.ApprovalStatus);
        }
    }
}
