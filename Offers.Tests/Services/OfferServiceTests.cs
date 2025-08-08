using Offers.Tests.Infrastructure;

namespace Offers.Tests.Services
{
    public class OfferServiceTests : TestBase
    {
        private readonly OfferService _offerService;

        public OfferServiceTests()
        {
            _offerService = new OfferService(Context, MemoryCache);
        }

        [Fact]
        public async Task GetOfferByIdAsync_WithValidId_ReturnsOffer()
        {
            // Arrange
            var offerId = 1;

            // Act
            var result = await _offerService.GetOfferByIdAsync(offerId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(offerId);
            result.OfferName.Should().Be("Test Offer");
            result.ProjectOwner.Should().NotBeNull();
            result.OfferItems.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetOfferByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var invalidOfferId = 999;

            // Act
            var result = await _offerService.GetOfferByIdAsync(invalidOfferId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOfferByIdAsync_WithNullId_ReturnsNull()
        {
            // Arrange
            int? nullId = null;

            // Act
            var result = await _offerService.GetOfferByIdAsync(nullId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOfferByIdAsync_CachesResult_OnSecondCall()
        {
            // Arrange
            var offerId = 1;

            // Act - First call
            var firstResult = await _offerService.GetOfferByIdAsync(offerId);
            
            // Modify the database to ensure we're getting cached result
            var offerInDb = await Context.Offers.FindAsync(offerId);
            offerInDb.OfferName = "Modified Name";
            await Context.SaveChangesAsync();

            // Act - Second call (should return cached result)
            var secondResult = await _offerService.GetOfferByIdAsync(offerId);

            // Assert
            firstResult.Should().NotBeNull();
            secondResult.Should().NotBeNull();
            secondResult.OfferName.Should().Be("Modified Name"); // Should be cached value, not modified
        }

        [Fact]
        public async Task ClearCache_RemovesCachedOffer()
        {
            // Arrange
            var offerId = 1;
            
            // First call to cache the offer
            await _offerService.GetOfferByIdAsync(offerId);
            
            // Modify the database
            var offerInDb = await Context.Offers.FindAsync(offerId);
            offerInDb.OfferName = "Modified Name";
            await Context.SaveChangesAsync();

            // Act
            var result = await _offerService.GetOfferByIdAsync(offerId);

            // Assert
            result.Should().NotBeNull();
            result.OfferName.Should().Be("Modified Name"); // Should be fresh from database
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task GetOfferByIdAsync_WithInvalidIds_ReturnsNull(int invalidId)
        {
            // Act
            var result = await _offerService.GetOfferByIdAsync(invalidId);

            // Assert
            result.Should().BeNull();
        }
        [Fact]
        public async Task GetOfferByIdAsync_CacheIsNotSharedBetweenDifferentIds()
        {
            // Arrange
            var offerId1 = 1;
            var offerId2 = 2;
            // Add a second offer
            var offer2 = new Offer
            {
                Id = offerId2,
                OfferName = "Second Offer",
                ProjectOwnerId = 1,
                ProjectOwner = Context.ProjectOwners.First(),
                TeklifGonderimTarihi = DateTime.Now.AddDays(-2),
                TeklifGecerlilikSuresi = DateTime.Now.AddDays(10),
                SonTeklifBildirme = DateTime.Now.AddDays(5),
                IsApproved = false,
                CreatedDate = DateTime.Now,
                ProjectAddress = "Second Project Address"
            };
            Context.Offers.Add(offer2);
            Context.SaveChanges();

            // Act
            var result1 = await _offerService.GetOfferByIdAsync(offerId1);
            var result2 = await _offerService.GetOfferByIdAsync(offerId2);

            // Assert
            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
            result1.Id.Should().Be(offerId1);
            result2.Id.Should().Be(offerId2);
        }

        [Fact]
        public async Task ClearCache_OnlyRemovesSpecifiedOffer()
        {
            // Arrange
            var offerId1 = 1;
            var offerId2 = 2;
            var offer2 = new Offer
            {
                Id = offerId2,
                OfferName = "Second Offer",
                ProjectOwnerId = 1,
                ProjectOwner = Context.ProjectOwners.First(),
                TeklifGonderimTarihi = DateTime.Now.AddDays(-2),
                TeklifGecerlilikSuresi = DateTime.Now.AddDays(10),
                SonTeklifBildirme = DateTime.Now.AddDays(5),
                IsApproved = false,
                CreatedDate = DateTime.Now,
                ProjectAddress = "Second Project Address"
            };
            Context.Offers.Add(offer2);
            Context.SaveChanges();
            // Cache both offers
            await _offerService.GetOfferByIdAsync(offerId1);
            await _offerService.GetOfferByIdAsync(offerId2);
            // Modify both in DB
            var db1 = await Context.Offers.FindAsync(offerId1);
            db1.OfferName = "Changed1";
            var db2 = await Context.Offers.FindAsync(offerId2);
            db2.OfferName = "Changed2";
            await Context.SaveChangesAsync();
            // Act
            var result1 = await _offerService.GetOfferByIdAsync(offerId1);
            var result2 = await _offerService.GetOfferByIdAsync(offerId2);
            // Assert
            result1.OfferName.Should().Be("Changed1"); // cache cleared
            result2.OfferName.Should().Be("Changed2"); // still cached
        }

        [Fact]
        public async Task GetOfferByIdAsync_OfferWithNoItems_ReturnsOfferWithEmptyItems()
        {
            // Arrange
            var offer = new Offer
            {
                Id = 99,
                OfferName = "No Items",
                ProjectOwnerId = 1,
                ProjectOwner = Context.ProjectOwners.First(),
                TeklifGonderimTarihi = DateTime.Now,
                TeklifGecerlilikSuresi = DateTime.Now.AddDays(10),
                SonTeklifBildirme = DateTime.Now.AddDays(5),
                IsApproved = false,
                CreatedDate = DateTime.Now,
                ProjectAddress = "Second Project Address"
            };
            Context.Offers.Add(offer);
            Context.SaveChanges();
            // Act
            var result = await _offerService.GetOfferByIdAsync(99);
            // Assert
            result.Should().NotBeNull();
            result.OfferItems.Should().BeEmpty();
        }

        [Fact]
        public async Task GetOfferByIdAsync_OfferWithMultipleItemsAndCompanies_ReturnsAllItems()
        {
            // Arrange
            var company2 = new Company { 
                Id = 2, 
                Name = "Test Company",
                Eposta = "company.email@hotmail.com",
                TicariSicilNo = "1122334455",
                TicariUnvan = "Company Title",
                VergiDairesiAdi = "Vergi Dairesi",
                VergiNo = "Vergi No",
                Address = "Company Address",
                Telefon = "555-1234",
                Faks = "555-5678",
                CreatedDate = DateTime.Now };

            Context.Companies.Add(company2);
            var offer = new Offer
            {
                Id = 77,
                OfferName = "Multi Item Offer",
                ProjectOwnerId = 1,
                ProjectOwner = Context.ProjectOwners.First(),
                TeklifGonderimTarihi = DateTime.Now,
                TeklifGecerlilikSuresi = DateTime.Now.AddDays(10),
                SonTeklifBildirme = DateTime.Now.AddDays(5),
                IsApproved = false,
                CreatedDate = DateTime.Now,
                OfferItems = new List<OfferItem>(),
                ProjectAddress = "Multi Item Project Address",
            };
            var item1 = new OfferItem
            {
                Id = 201,
                OfferId = 77,
                CompanyId = 1,
                EquipmentModelId = 1,
                Quantity = 1,
                Price = 1000,
                CreatedDate = DateTime.Now
            };
            var item2 = new OfferItem
            {
                Id = 202,
                OfferId = 77,
                CompanyId = 2,
                EquipmentModelId = 1,
                Quantity = 2,
                Price = 2000,
                CreatedDate = DateTime.Now
            };
            offer.OfferItems.Add(item1);
            offer.OfferItems.Add(item2);
            Context.Offers.Add(offer);
            Context.OfferItems.AddRange(item1, item2);
            Context.SaveChanges();
            // Act
            var result = await _offerService.GetOfferByIdAsync(77);
            // Assert
            result.Should().NotBeNull();
            result.OfferItems.Should().HaveCount(2);
            result.OfferItems.Select(x => x.CompanyId).Should().Contain(new[] { 1, 2 });
        }
    }
}
