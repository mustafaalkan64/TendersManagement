using Offers.Permissions;
using Offers.Services.Company;
using Offers.Services.Currency;
using Offers.Services.Equipment;
using Offers.Services.EquipmentModel;
using Offers.Services.Offer;
using Offers.Services.ProjectOwner;

namespace Offers.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IOfferService, OfferService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IEquipmentService, EquipmentService>();
            services.AddTransient<IEquipmentModelService, EquipmentModelService>();
            services.AddTransient<IProjectOwnerService, ProjectOwnerService>();
            services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
            {
                client.BaseAddress = new Uri("https://www.tcmb.gov.tr/");
                client.Timeout = TimeSpan.FromSeconds(30);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
            });
            return services;
        }
    }
}
