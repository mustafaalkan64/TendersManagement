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
    public DbSet<ConsultantCompany> ConsultantCompanies { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<OfferItem> OfferItems { get; set; }
    public DbSet<EquipmentModelFeature> EquipmentModelFeatures { get; set; }
    public DbSet<EquipmentFeature> EquipmentFeatures { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<ProjectOwner> ProjectOwners { get; set; }
    public DbSet<CompanyEquipmentModel> CompanyEquipmentModels { get; set; }
    public DbSet<OfferTeknikSartname> OfferTeknikSartnames { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OfferItem>()
            .HasOne(oi => oi.Offer)
            .WithMany(o => o.OfferItems)
            .HasForeignKey(oi => oi.OfferId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<CompanyEquipmentModel>()
            .HasOne(oi => oi.Company)
            .WithMany(o => o.CompanyEquipmentModels)
            .HasForeignKey(oi => oi.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<CompanyEquipmentModel>()
            .HasOne(oi => oi.EquipmentModel)
            .WithMany(o => o.CompanyEquipmentModels)
            .HasForeignKey(oi => oi.EquipmentModelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EquipmentModel>()
            .HasOne(em => em.Equipment)
            .WithMany(e => e.Models)
            .HasForeignKey(em => em.EquipmentId);

    }
} 