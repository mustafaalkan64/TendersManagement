using Models;
using System;

namespace Offers.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Units.Any())
            {
                context.Units.AddRange(
                    new Unit { Name = "Kg", ShortCode = "Kg" },
                    new Unit { Name = "Ton", ShortCode = "Ton" },
                    new Unit { Name = "Lt", ShortCode = "Lt" },
                    new Unit { Name = "Cm", ShortCode = "Cm" },
                    new Unit { Name = "Mm", ShortCode = "Mm" },
                    new Unit { Name = "M", ShortCode = "M" },
                    new Unit { Name = "m³", ShortCode = "m³" },
                    new Unit { Name = "m²", ShortCode = "m²" },
                    new Unit { Name = "°", ShortCode = "°" },
                    new Unit { Name = "Adet", ShortCode = "Adet" },
                    new Unit { Name = "Hp", ShortCode = "Hp" },
                    new Unit { Name = "-", ShortCode = "-" },
                    new Unit { Name = "Evet", ShortCode = "Evet" },
                    new Unit { Name = "Hayır", ShortCode = "Hayır" }
                );
                context.SaveChanges();
            }

            if (!context.Permissions.Any())
            {
                context.Permissions.AddRange(
                    new Permission { Name = "EkipmanDuzenle", DisplayName = "Ekipman Duzenle" },
                    new Permission { Name = "EkipmanEkle", DisplayName = "Ekipman Ekle" },
                    new Permission { Name = "EkipmanSil", DisplayName = "Ekipman Sil" },
                    new Permission { Name = "EkipmanListele", DisplayName = "Ekipman Listele" },
                    new Permission { Name = "EkipmanModelDuzenle", DisplayName = "Ekipman Model Duzenle" },
                    new Permission { Name = "EkipmanModelSil", DisplayName = "Ekipman Model Sil" },
                    new Permission { Name = "EkipmanModelEkle", DisplayName = "Ekipman Model Ekle" },
                    new Permission { Name = "EkipmanModelListele", DisplayName = "Ekipman Model Listele" },
                    new Permission { Name = "SirketDuzenle", DisplayName = "Şirket Duzenle" },
                    new Permission { Name = "SirketSil", DisplayName = "Şirket Sil" },
                    new Permission { Name = "SirketEkle", DisplayName = "Şirket Ekle" },
                    new Permission { Name = "SirketListele", DisplayName = "Şirket Listele" },
                    new Permission { Name = "SirketDetay", DisplayName = "Şirket Detay" },
                    new Permission { Name = "YatirimciDuzenle", DisplayName = "Yatırımcı Duzenle" },
                    new Permission { Name = "YatirimciSil", DisplayName = "Yatırımcı Sil" },
                    new Permission { Name = "YatirimciEkle", DisplayName = "Yatırımcı Ekle" },
                    new Permission { Name = "YatirimciListele", DisplayName = "Yatırımcı Listele" },
                    new Permission { Name = "YatirimciDetay", DisplayName = "Yatırımcı Detay" },
                    new Permission { Name = "OfferDuzenle", DisplayName = "Teklif Duzenle" },
                    new Permission { Name = "OfferSil", DisplayName = "Teklif Sil" },
                    new Permission { Name = "OfferEkle", DisplayName = "Teklif Ekle" },
                    new Permission { Name = "OfferListele", DisplayName = "Teklif Listele" },
                    new Permission { Name = "TeknikSartnameDuzenle", DisplayName = "Teknik Şartname Duzenle" },
                    new Permission { Name = "TeknikSartnameSil", DisplayName = "Teknik Şartname Sil" },
                    new Permission { Name = "TeknikSartnameEkle", DisplayName = "Teknik Şartname Ekle" },
                    new Permission { Name = "TeknikSartnameListele", DisplayName = "Teknik Şartname Listele" }
                );
                context.SaveChanges();
            }
        }
    }
}
