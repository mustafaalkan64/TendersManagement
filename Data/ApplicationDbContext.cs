using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferItem> OfferItems { get; set; }
        public DbSet<EquipmentModel> EquipmentModels { get; set; }

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
        }
    }
} 