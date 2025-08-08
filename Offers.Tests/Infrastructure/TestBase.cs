using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace Offers.Tests.Infrastructure
{
    /// <summary>
    /// Base class for unit tests providing common test infrastructure
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        protected ApplicationDbContext Context { get; private set; }
        protected IMemoryCache MemoryCache { get; private set; }
        protected ILogger<T> GetLogger<T>() => Mock.Of<ILogger<T>>();

        protected TestBase()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new ApplicationDbContext(options);
            MemoryCache = new MemoryCache(new MemoryCacheOptions());

            SeedTestData();
        }

        /// <summary>
        /// Seeds the test database with common test data
        /// </summary>
        protected virtual void SeedTestData()
        {
            // Add test project owners
            var projectOwner = new ProjectOwner
            {
                Id = 1,
                Name = "Test Project Owner",
                Address = "Test Address",
                Traktor = "90",
                IdentityNo = "11223344556",
                Telephone = "555-1234",
            };
            Context.ProjectOwners.Add(projectOwner);

            // Add test companies
            var company = new Company
            {
                Id = 1,
                Name = "Test Company",
                TicariUnvan = "Test Ticari Unvan",
                VergiNo = "1234567890",
                VergiDairesiAdi = "Test Vergi Dairesi",
                TicariSicilNo = "12345",
                Address = "Test Company Address",
                Telefon = "555-1234",
                Faks = "555-5678",
                Eposta = "test@company.com",
                CreatedDate = DateTime.Now
            };
            Context.Companies.Add(company);

            // Add test equipment
            var equipment = new Equipment
            {
                Id = 1,
                Name = "Test Equipment",
            };
            Context.Equipment.Add(equipment);

            // Add test equipment model
            var equipmentModel = new EquipmentModel
            {
                Id = 1,
                EquipmentId = 1,
                Brand = "Test Brand",
                Model = "Test Model",
                Capacity = "100 HP",
                Equipment = equipment,
            };
            Context.EquipmentModels.Add(equipmentModel);

            // Add test offer
            var offer = new Offer
            {
                Id = 1,
                OfferName = "Test Offer",
                ProjectOwnerId = 1,
                ProjectOwner = projectOwner,
                TeklifGonderimTarihi = DateTime.Now.AddDays(-5),
                TeklifGecerlilikSuresi = DateTime.Now.AddDays(30),
                SonTeklifBildirme = DateTime.Now.AddDays(15),
                IsApproved = false,
                CreatedDate = DateTime.Now,
                OfferItems = new List<OfferItem>(),
                ProjectAddress = "Test Project Address",
            };
            Context.Offers.Add(offer);

            // Add test offer item
            var offerItem = new OfferItem
            {
                Id = 1,
                OfferId = 1,
                CompanyId = 1,
                EquipmentModelId = 1,
                Quantity = 2,
                Price = 50000m,
                TeklifGirisTarihi = DateTime.Now.AddDays(-3),
                Offer = offer,
                Company = company,
                EquipmentModel = equipmentModel,
                CreatedDate = DateTime.Now
            };
            Context.OfferItems.Add(offerItem);
            offer.OfferItems.Add(offerItem);

            Context.SaveChanges();
        }

        /// <summary>
        /// Creates a mock HTTP context for testing page models
        /// </summary>
        protected DefaultHttpContext CreateMockHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["User-Agent"] = "Test Agent";
            return httpContext;
        }

        /// <summary>
        /// Creates a page context for testing Razor Page models
        /// </summary>
        protected PageContext CreatePageContext()
        {
            var httpContext = CreateMockHttpContext();
            return new PageContext
            {
                HttpContext = httpContext
            };
        }

        public virtual void Dispose()
        {
            Context?.Dispose();
            MemoryCache?.Dispose();
        }
    }
}
