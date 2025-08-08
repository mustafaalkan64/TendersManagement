using Microsoft.AspNetCore.Authorization;

namespace Offers.Permissions
{
    public static class AuthorizationPolicies
    {
        public static void RegisterPolicies(AuthorizationOptions options)
        {
            // Equipment policies
            options.AddPolicy("CanEditEquipment", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanDuzenle")));
            options.AddPolicy("CanAddEquipment", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanEkle")));
            options.AddPolicy("CanDeleteEquipment", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanSil")));
            options.AddPolicy("CanListEquipment", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanListele")));

            // EquipmentModel policies
            options.AddPolicy("CanEditEquipmentModel", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanModelDuzenle")));
            options.AddPolicy("CanDeleteEquipmentModel", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanModelSil")));
            options.AddPolicy("CanAddEquipmentModel", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanModelEkle")));
            options.AddPolicy("CanListEquipmentModel", policy =>
                policy.Requirements.Add(new PermissionRequirement("EkipmanModelListele")));

            // Company policies
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

            // ConsultantCompany policies
            options.AddPolicy("CanEditConsultantCompany", policy =>
                policy.Requirements.Add(new PermissionRequirement("DanismanSirketDuzenle")));
            options.AddPolicy("CanDeleteConsultantCompany", policy =>
                policy.Requirements.Add(new PermissionRequirement("DanismanSirketSil")));
            options.AddPolicy("CanAddConsultantCompany", policy =>
                policy.Requirements.Add(new PermissionRequirement("DanismanSirketEkle")));
            options.AddPolicy("CanSeeDetailsConsultantCompany", policy =>
                policy.Requirements.Add(new PermissionRequirement("DanismanSirketDetay")));
            options.AddPolicy("CanListConsultantCompany", policy =>
                policy.Requirements.Add(new PermissionRequirement("DanismanSirketListele")));

            // Owner policies
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

            // TeknikSartname policies
            options.AddPolicy("CanEditTeknikSartname", policy =>
                policy.Requirements.Add(new PermissionRequirement("TeknikSartnameDuzenle")));
            options.AddPolicy("CanDeleteTeknikSartname", policy =>
                policy.Requirements.Add(new PermissionRequirement("TeknikSartnameSil")));
            options.AddPolicy("CanAddTeknikSartname", policy =>
                policy.Requirements.Add(new PermissionRequirement("TeknikSartnameEkle")));
            options.AddPolicy("CanListTeknikSartname", policy =>
                policy.Requirements.Add(new PermissionRequirement("TeknikSartnameListele")));

            // Offer policies
            options.AddPolicy("CanListOffer", policy =>
                policy.Requirements.Add(new PermissionRequirement("OfferListele")));
            options.AddPolicy("CanAddOffer", policy =>
                policy.Requirements.Add(new PermissionRequirement("OfferEkle")));
            options.AddPolicy("CanEditOffer", policy =>
                policy.Requirements.Add(new PermissionRequirement("OfferDuzenle")));
            options.AddPolicy("CanDeleteOffer", policy =>
                policy.Requirements.Add(new PermissionRequirement("OfferSil")));
        }
    }
}
