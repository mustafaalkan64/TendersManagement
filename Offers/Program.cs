using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Offers.Data;
using Offers.Permissions;
using Offers.Services.Offer;
using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Ensure JSON encoding supports Turkish characters properly
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    //options.SerializerOptions.ReferenceHandler = Newtonsoft.Json.ReferenceLoopHandling.Ignore
});

// Add Turkish culture support
var cultureInfo = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});


// Add services to the container.
builder.Services.AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Add services to the container.
builder.Services.AddMemoryCache(); // Add MemoryCache service

// Configure request localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = new List<CultureInfo> { cultureInfo };
    options.SupportedUICultures = new List<CultureInfo> { cultureInfo };
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditEquipment", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanDuzenle")));

    options.AddPolicy("CanAddEquipment", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanEkle")));

    options.AddPolicy("CanDeleteEquipment", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanSil")));

    options.AddPolicy("CanListEquipment", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanListele")));

    options.AddPolicy("CanEditEquipmentModel", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanModelDuzenle")));

    options.AddPolicy("CanDeleteEquipmentModel", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanModelSil")));

    options.AddPolicy("CanAddEquipmentModel", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanModelEkle")));

    options.AddPolicy("CanListEquipmentModel", policy =>
        policy.Requirements.Add(new PermissionRequirement("EkipmanModelListele")));

    options.AddPolicy("CanEditCompany", policy =>
        policy.Requirements.Add(new PermissionRequirement("SirketDuzenle")));

    options.AddPolicy("CanDeleteCompany", policy =>
        policy.Requirements.Add(new PermissionRequirement("SirketSil")));

    options.AddPolicy("CanAddCompany", policy =>
        policy.Requirements.Add(new PermissionRequirement("SirketEkle")));

    options.AddPolicy("CanSeeDetailsCompany", policy =>
        policy.Requirements.Add(new PermissionRequirement("SirketDetay")));

    options.AddPolicy("CanListCompany", policy =>
        policy.Requirements.Add(new PermissionRequirement("SirketListele")));

    options.AddPolicy("CanEditOwner", policy =>
        policy.Requirements.Add(new PermissionRequirement("YatirimciDuzenle")));

    options.AddPolicy("CanDeleteOwner", policy =>
        policy.Requirements.Add(new PermissionRequirement("YatirimciSil")));

    options.AddPolicy("CanAddOwner", policy =>
        policy.Requirements.Add(new PermissionRequirement("YatirimciEkle")));

    options.AddPolicy("CanListOwner", policy =>
        policy.Requirements.Add(new PermissionRequirement("YatirimciListele")));

    options.AddPolicy("CanSeeDetailsOwner", policy =>
        policy.Requirements.Add(new PermissionRequirement("YatirimciDetay")));

    options.AddPolicy("CanEditTeknikSartname", policy =>
        policy.Requirements.Add(new PermissionRequirement("TeknikSartnameDuzenle")));

    options.AddPolicy("CanDeleteTeknikSartname", policy =>
        policy.Requirements.Add(new PermissionRequirement("TeknikSartnameSil")));

    options.AddPolicy("CanAddTeknikSartname", policy =>
        policy.Requirements.Add(new PermissionRequirement("TeknikSartnameEkle")));

    options.AddPolicy("CanListTeknikSartname", policy =>
        policy.Requirements.Add(new PermissionRequirement("TeknikSartnameListele")));

    options.AddPolicy("CanListOffer", policy =>
        policy.Requirements.Add(new PermissionRequirement("OfferListele")));

    options.AddPolicy("CanAddOffer", policy =>
        policy.Requirements.Add(new PermissionRequirement("OfferEkle")));

    options.AddPolicy("CanEditOffer", policy =>
        policy.Requirements.Add(new PermissionRequirement("OfferDuzenle")));

    options.AddPolicy("CanDeleteOffer", policy =>
        policy.Requirements.Add(new PermissionRequirement("OfferSil")));

});

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IOfferService, OfferService>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Seed(context);
}

// Apply localization settings
app.UseRequestLocalization();

// Force UTF-8 for responses
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
    await next();
});

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        // Seed Roles and Admin User
        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
