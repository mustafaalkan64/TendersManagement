using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext() { } // This one
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<EquipmentModel> EquipmentModels { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<OfferItem> OfferItems { get; set; }
    public DbSet<EquipmentModelFeature> EquipmentModelFeatures { get; set; }
    public DbSet<EquipmentFeature> EquipmentFeatures { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<ProjectOwner> ProjectOwners { get; set; }
    public DbSet<CompanyEquipmentModel> CompanyEquipmentModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OfferItem>()
            .HasOne(oi => oi.Offer)
            .WithMany(o => o.OfferItems)
            .HasForeignKey(oi => oi.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EquipmentModel>()
            .HasOne(em => em.Equipment)
            .WithMany(e => e.Models)
            .HasForeignKey(em => em.EquipmentId);

        var units = new[]
        {
            new Unit { Id = 1, Name = "Kg", ShortCode = "Kg" },
            new Unit { Id = 2, Name = "Ton", ShortCode = "Ton" },
            new Unit { Id = 3, Name = "Lt", ShortCode = "Lt" },
            new Unit { Id = 4, Name = "Cm", ShortCode = "Cm" },
            new Unit { Id = 5, Name = "Mm", ShortCode = "Mm" },
            new Unit { Id = 6, Name = "M", ShortCode = "M" },
            new Unit { Id = 7, Name = "m³", ShortCode = "m³" },
            new Unit { Id = 8, Name = "m²", ShortCode = "m²" },
            new Unit { Id = 9, Name = "°", ShortCode = "°" },
            new Unit { Id = 10, Name = "Adet", ShortCode = "Adet" },
            new Unit { Id = 11, Name = "Hp", ShortCode = "Hp" },
            new Unit { Id = 12, Name = "-", ShortCode = "-" },
            new Unit { Id = 13, Name = "Evet", ShortCode = "Evet" },
            new Unit { Id = 14, Name = "Hayýr", ShortCode = "Hayýr" }
        };

        modelBuilder.Entity<Unit>().HasData(units);
    }
} 