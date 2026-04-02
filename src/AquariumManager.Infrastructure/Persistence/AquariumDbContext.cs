using AquariumManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquariumManager.Infrastructure.Persistence;

public class AquariumDbContext : DbContext
{
    public AquariumDbContext(DbContextOptions<AquariumDbContext> options)
        : base(options)
    {
    }

    public DbSet<Species> Species => Set<Species>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryLot> InventoryLots => Set<InventoryLot>();
    public DbSet<MortalityRecord> MortalityRecords => Set<MortalityRecord>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Species>(builder =>
        {
            builder.ToTable("Species");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.CommonName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ScientificName).HasMaxLength(200);
            builder.Property(s => s.Type).HasMaxLength(100);
            builder.Property(s => s.Variety).HasMaxLength(100);
            builder.Property(s => s.Category).HasMaxLength(100);
              builder.Property(s => s.MinPH)
              .HasPrecision(3, 2);

        builder.Property(s => s.MaxPH)
              .HasPrecision(3, 2);

        builder.Property(s => s.MinTemperature)
              .HasPrecision(4, 1); // ej. 0.0 a 99.9

        builder.Property(s => s.MaxTemperature)
              .HasPrecision(4, 1);
        });

        modelBuilder.Entity<InventoryItem>(builder =>
        {
            builder.ToTable("InventoryItems");
            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.Species)
                   .WithMany(s => s.InventoryItems)
                   .HasForeignKey(i => i.SpeciesId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.CostPrice).HasColumnType("decimal(18,2)");
            builder.Property(i => i.SalePrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<InventoryLot>(builder =>
        {
            builder.ToTable("InventoryLots");
            builder.HasKey(l => l.Id);

            builder.HasOne(l => l.Species)
                   .WithMany(s => s.InventoryLots)
                   .HasForeignKey(l => l.SpeciesId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Supplier)
                   .WithMany(s => s.InventoryLots)
                   .HasForeignKey(l => l.SupplierId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Property(l => l.UnitCost).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<MortalityRecord>(builder =>
        {
            builder.ToTable("MortalityRecords");
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.InventoryLot)
                   .WithMany(l => l.MortalityRecords)
                   .HasForeignKey(m => m.InventoryLotId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Supplier>(builder =>
        {
            builder.ToTable("Suppliers");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        });
    }
}
