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
                    new Unit { Id = 14, Name = "Hayır", ShortCode = "Hayır" }
                );
                context.SaveChanges();
            }
        }
    }
}
