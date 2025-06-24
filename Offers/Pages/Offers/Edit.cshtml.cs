﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Packaging;
using System.Reflection;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DocumentFormat.OpenXml;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Offers.Services.Offer;
using System.Text.RegularExpressions;

namespace Pages.Offers
{
    [Authorize(Policy = "CanEditOffer")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IOfferService _offerService;
        private readonly IMemoryCache _cache;
        private const string Cetinkaya = "Çetinkaya";

        public EditModel(ApplicationDbContext context, IMemoryCache cache, IOfferService offerService)
        {
            _context = context;
            _cache = cache;
            _offerService = offerService;
        }

        [BindProperty]
        public Offer Offer { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public SelectList ProjectOwnerList { get; set; }

        public List<CompanySummaryViewModel> CompanySummaries { get; set; }

        public Decimal MinOfferAmount { get; set; }
        public string MinOfferCompany { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public OfferItem NewItem { get; set; } = new OfferItem();

        public SelectList EquipmentModelList { get; set; }
        public List<OfferItem> OfferItems { get; set; }
        public SelectList EquipmentList { get; set; }
        public SelectList CompanyList { get; set; }

        [BindProperty]
        public int SelectedCompanyId { get; set; }

        private async Task LoadRelatedData(int? id = 0, CancellationToken cancellationToken = default)
        {
            Offer = await GetOfferByIdAsync(id, cancellationToken);
            if (Offer == null)
            {
                throw new ArgumentNullException("Teklif Bulunamadi");
            }

            var offerItemsQuery = Offer.OfferItems.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                offerItemsQuery = offerItemsQuery.Where(x => x.EquipmentModel.Model.ToLower().Contains(SearchString.ToLower())
                                                    || x.Company.Name.ToLower().Contains(SearchString.ToLower())
                                                    || x.EquipmentModel.Equipment.Name.ToLower().Contains(SearchString.ToLower())
                                                    || x.EquipmentModel.Brand.ToLower().Contains(SearchString.ToLower()));
            }
            OfferItems = offerItemsQuery.OrderBy(x => x.Company.Name).ThenBy(x => x.Price).ToList();

            //var equipmentModels = await _context.EquipmentModels
            //.Include(em => em.Equipment)
            //.OrderBy(em => em.Equipment.Name)
            //.ThenBy(em => em.Brand)
            //.ThenBy(em => em.Model)
            //.ToListAsync(cancellationToken);

            var equipmentModels = new List<EquipmentModel?>();

            if(NewItem.CompanyId > 0)
            {
                equipmentModels = await GetEquipmentModelsByCompanyId(NewItem.CompanyId, cancellationToken);
            }

            CompanySummaries = OfferItems
                .GroupBy(oi => oi.Company.Name)
                .Select(g => new CompanySummaryViewModel
                {
                    CompanyName = g.Key,
                    TotalPrice = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderBy(s => s.TotalPrice)
                .ToList();

            MinOfferAmount = CompanySummaries.Any() ? CompanySummaries.First().TotalPrice : 0;
            MinOfferCompany = CompanySummaries.Any() ? CompanySummaries.First().CompanyName : "";

            EquipmentModelList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "Ekipman Modeli Seciniz" }
                }.Concat(equipmentModels.Select(em => new SelectListItem
                {
                    Value = em.Id.ToString(),
                    Text = $"{em.Equipment.Name} - {em.Brand} {em.Model}"
                })), "Value", "Text");

            CompanyList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "Sirket Seciniz" }
                }.Concat(await _context.Companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync(cancellationToken)), "Value", "Text");

            ProjectOwnerList = new SelectList(
               await _context.ProjectOwners.OrderBy(p => p.Name).ToListAsync(cancellationToken),
               "Id",
               "Name"
           );
        }

        public async Task<IActionResult> OnGetEquipmentModelsAsync(int companyId)
        {
            var equipmentModels = await GetEquipmentModelsByCompanyId(companyId);
            return new JsonResult(equipmentModels.Select(em => new
            {
                id = em.Id,
                text = $"{em.Equipment.Name} - {em.Brand} {em.Model}"
            }));
        }

        private async Task<List<EquipmentModel>> GetEquipmentModelsByCompanyId(int companyId, CancellationToken cancellationToken = default)
        {
            var companyEquipmentModels = await _context.CompanyEquipmentModels
                .Where(cem => cem.CompanyId == companyId)
                .Select(cem => cem.EquipmentModelId)
                .ToListAsync(cancellationToken);

            if (companyEquipmentModels.Any())
            {
                var equipmentModels = await _context.EquipmentModels
                    .Include(em => em.Equipment)
                    //.Where(em => companyEquipmentModels.Contains(em.Id))
                    .OrderBy(em => em.Equipment.Name)
                    .ThenBy(em => em.Brand)
                    .ThenBy(em => em.Model)
                    .ToListAsync(cancellationToken);

                return equipmentModels.Where(em => companyEquipmentModels.Contains(em.Id)).ToList();
            }

            // If no company equipment models exist, return all equipment models
            return await _context.EquipmentModels
                .Include(em => em.Equipment)
                .OrderBy(em => em.Equipment.Name)
                .ThenBy(em => em.Brand)
                .ThenBy(em => em.Model)
                .ToListAsync(cancellationToken);
        }

        public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken = default)
        {
            await LoadRelatedData(id, cancellationToken);
            NewItem.EquipmentModelId = 0;
            NewItem.CompanyId = 0;
            return Page();
        }

        public async Task<IActionResult> OnGetUpdateEquipmentModelsAsync(int companyId, CancellationToken cancellationToken = default)
        {
            var equipmentModels = await GetEquipmentModelsByCompanyId(companyId, cancellationToken);
            return new JsonResult(equipmentModels.Select(em => new
            {
                id = em.Id,
                text = $"{em.Equipment.Name} - {em.Brand} {em.Model}"
            }));
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            Offer.IsApproved = true;
            _context.Attach(Offer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                ClearCache(Offer.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(Offer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Offer.OfferName))
            {
                await LoadRelatedData(Offer.Id, cancellationToken);
                return Page();
            }

            if (!(Offer.SonTeklifBildirme > Offer.TeklifGonderimTarihi))
            {
                StatusMessage = "Son teklif bildirme tarihi teklif gönderim tarihinden sonra olmalıdır";
                await LoadRelatedData(Offer.Id, cancellationToken);
                return Page();
            }

            //if (!(Offer.TeklifSunumTarihi > Offer.TeklifGonderimTarihi && Offer.TeklifSunumTarihi <= Offer.SonTeklifBildirme))
            //{

            //    StatusMessage = "Teklif sunum tarihi teklif gonderim tarihinden sonra ve son teklif bildirme tarihinden önce olmalıdır";
            //    await LoadRelatedData(Offer.Id);
            //    return Page();
            //}

            _context.Attach(Offer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                ClearCache(Offer.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(Offer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDownloadAsync(int companyId)
        {
            string templatePath = "";

            var offer = await GetOfferByIdAsync(Offer.Id);

            var offerItems = offer.OfferItems.ToList();

            var company = offerItems.FirstOrDefault(x => x.CompanyId == companyId)?.Company;

            var teklifGirisTarihi = offerItems.FirstOrDefault(x => x.CompanyId == companyId && x.OfferId == Offer.Id && x.TeklifGirisTarihi != DateTime.MinValue)?.TeklifGirisTarihi;

            var projectOwner = offer.ProjectOwner;
            CultureInfo trCulture = new CultureInfo("tr-TR");

            // Create a copy of the template to modify
            byte[] modifiedDocument;
            if (company?.Name == Cetinkaya)
            {
                templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", "CetinkayaTeklif.docx");
                if (!System.IO.File.Exists(templatePath))
                {
                    return NotFound("Template not found.");
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                    {
                        await fileStream.CopyToAsync(memoryStream);
                    }

                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true))
                    {
                        ReplaceText(wordDoc, "frmadx", company.TicariUnvan);
                        ReplaceText(wordDoc, "{Unvan}", company.TicariUnvan.ToUpper());
                        ReplaceText(wordDoc, "vrg", company.VergiNo);
                        ReplaceText(wordDoc, "vergdaire", company.VergiDairesiAdi);
                        ReplaceText(wordDoc, "Ticarisicil", company.TicariSicilNo);
                        ReplaceText(wordDoc, "fffaaaxxx", company.Address);
                        ReplaceText(wordDoc, "xxxaaayyyy", company.Telefon);
                        ReplaceText(wordDoc, "ffkkss", company.Faks);
                        ReplaceText(wordDoc, "Firmaeposta", company.Eposta);
                        ReplaceText(wordDoc, "Yatirimci", projectOwner?.Name.ToUpper() ?? "");
                        ReplaceText(wordDoc, "aaaa", projectOwner?.Address.ToUpper() ?? "");
                        ReplaceText(wordDoc, "yzyzyz", Offer.OfferName.ToUpper() ?? "");
                        ReplaceText(wordDoc, "ddmmyyyy", teklifGirisTarihi.Value.ToString("dd.MM.yyyy") ?? "");
                        ReplaceText(wordDoc, "M12", Offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "N13", Offer.TeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "BCDAY", Offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "BFDAY", Offer.TeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));

                        decimal totalPrices = 0;
                        var equipmentNames = new StringBuilder();
                        var equipmentList = new List<string>();

                        foreach (var offerItem in offerItems.Where(x => x.CompanyId == companyId).ToList())
                        {
                            var result = new StringBuilder();
                            foreach (var feature in offerItem.EquipmentModel?.Features?.ToList())
                            {
                                result.AppendLine($"{feature.FeatureKey.ToUpper()} {feature.FeatureValue.ToUpper()} {feature.Unit?.Name?.ToUpper().Replace("-", "") ?? ""}");
                            }
                            var equipment = offerItem.EquipmentModel.Equipment.Name.ToUpper();
                            equipmentList.Add(equipment);
                            var features = Regex.Replace(result.ToString(), @"[\r\n]$", "");
                            var equipmentModel = offerItem.EquipmentModel.Brand.ToUpper() + "\n" + offerItem.EquipmentModel.Model.ToUpper();
                            var sayi = offerItem.Quantity;
                            var price = offerItem.Price;
                            var totalPrice = sayi * price;
                            totalPrices += totalPrice;

                            string[] rowValues = { equipment, features, equipmentModel, "Adet", sayi.ToString(), price.ToString("#,##0.00", trCulture) + " TL", totalPrice.ToString("#,##0.00", trCulture) + " TL" }; // Example row values

                            AddRowToTable(wordDoc, rowValues, true);

                        }
                        string[] totalPriceRow = { "", "", "", "", "", "Toplam Fiyat (TL) ", totalPrices.ToString("#,##0.00", trCulture) + " TL" }; // Example row values
                        AddRowToTable(wordDoc, totalPriceRow, true);

                        foreach (var equipment in equipmentList.Distinct().ToList())
                        {
                            equipmentNames.AppendLine(equipment);
                        }

                        ReplaceText(wordDoc, "AXCCD", Regex.Replace(equipmentNames.ToString(), @"[\r\n]$", ""));
                    }

                    modifiedDocument = memoryStream.ToArray();
                }
            }
            else
            {
                templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", "TeklifTemplate.docx");
                if (!System.IO.File.Exists(templatePath))
                {
                    return NotFound("Template not found.");
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                    {
                        await fileStream.CopyToAsync(memoryStream);
                    }

                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true))
                    {
                        ReplaceText(wordDoc, "A1", company.TicariUnvan.ToUpper());
                        ReplaceText(wordDoc, "{Unvan}", company.TicariUnvan);
                        ReplaceText(wordDoc, "B2", company.VergiNo);
                        ReplaceText(wordDoc, "C3", company.VergiDairesiAdi);
                        ReplaceText(wordDoc, "D4", company.TicariSicilNo);
                        ReplaceText(wordDoc, "E5", company.Address);
                        ReplaceText(wordDoc, "F6", company.Telefon);
                        ReplaceText(wordDoc, "G7", company.Faks);
                        ReplaceText(wordDoc, "Firmaeposta", company.Eposta);
                        ReplaceText(wordDoc, "I9", projectOwner?.Name ?? "");
                        ReplaceText(wordDoc, "K10", projectOwner?.Address ?? "");
                        ReplaceText(wordDoc, "L11", Offer.OfferName ?? "");
                        ReplaceText(wordDoc, "M12", Offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "N13", Offer.TeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "ddmmyyyy", teklifGirisTarihi.Value.ToString("dd.MM.yyyy") ?? "");
                        ReplaceText(wordDoc, "BCDAY", Offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "BFDAY", Offer.TeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));

                        decimal totalPrices = 0;
                        var no = 1;
                        var equipmentList = new List<string>();

                        var equipmentNames = new StringBuilder();
                        foreach (var offerItem in offerItems.Where(x => x.CompanyId == companyId).ToList())
                        {
                            var result = new StringBuilder();
                            foreach (var feature in offerItem.EquipmentModel?.Features?.ToList())
                            {
                                result.AppendLine($"{feature.FeatureKey} {feature.FeatureValue} {feature.Unit?.Name?.ToString().Replace("-", "") ?? ""}");
                            }
                            var equipment = offerItem.EquipmentModel.Equipment.Name;
                            equipmentList.Add(equipment);
                            var features = Regex.Replace(result.ToString(), @"[\r\n]$", "");
                            var equipmentModel = offerItem.EquipmentModel.Brand + " " + offerItem.EquipmentModel.Model;
                            var sayi = offerItem.Quantity;
                            var price = offerItem.Price;
                            var totalPrice = sayi * price;
                            totalPrices += totalPrice;

                            string[] rowValues = { no.ToString(), equipment, features, equipmentModel, "Adet", sayi.ToString(), price.ToString("#,##0.00", trCulture) + " TL", totalPrice.ToString("#,##0.00", trCulture) + " TL" }; // Example row values

                            AddRowToTable(wordDoc, rowValues, false);
                            no += 1;

                        }
                        string[] totalPriceRow = { "", "", "", "", "", "", "GENEL TOPLAM ", totalPrices.ToString("#,##0.00", trCulture) + " TL" }; // Example row values
                        AddRowToTable(wordDoc, totalPriceRow, false);

                        var equipments = ConvertListToString(equipmentList.Distinct().ToList());
                        ReplaceText(wordDoc, "O14", equipments);
                    }
                    modifiedDocument = memoryStream.ToArray();
                }
            }

            return File(modifiedDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{company.Name}-Teklif.docx");
        }

        public async Task<IActionResult> OnPostTeknikSartnameAsync(CancellationToken cancellationToken)
        {
            string templatePath = "";

            var offer = await GetOfferByIdAsync(Offer.Id);

            var _equipmentModelIds = new List<int>();

            var offerItems = offer.OfferItems.ToList();

            OfferItems = offerItems;

            var projectOwner = offer.ProjectOwner;
            CultureInfo trCulture = new CultureInfo("tr-TR");

            // Create a copy of the template to modify
            byte[] modifiedDocument;

            templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", "TeknikSartname.docx");
            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("Template not found.");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var equipmentNames = string.Join(", ", offerItems
                        .Select(x => x.EquipmentModel.Equipment.Name)
                        .Distinct());
                    ReplaceText(wordDoc, "AAAA", offer.OfferName);
                    ReplaceText(wordDoc, "BBBB", offer.ProjectAddress);
                    ReplaceText(wordDoc, "AXBY", offer.ProjectOwner.Name);
                    ReplaceText(wordDoc, "XXXX", equipmentNames);
                    ReplaceText(wordDoc, "DDMMYYYY", offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));

                    var offerTeknikSartname = await _context.OfferTeknikSartnames.Where(x => x.OfferId == offer.Id).ToListAsync(cancellationToken);
                    if(offerTeknikSartname.Any())
                    {

                        foreach (var teknikSartname in offerTeknikSartname)
                        {
                            teknikSartname.Features = Regex.Replace(teknikSartname.Features, @"[\r\n]$", "");
                            string[] rowValues = { 
                                teknikSartname.No.ToString(), 
                                teknikSartname.EquipmentName.ToString(),
                                teknikSartname.Features,
                                teknikSartname.Birim,
                                teknikSartname.Miktar.ToString()
                            }; // Example row values
                            AddRowToTable(wordDoc, rowValues, false, true);
                        }
                    }
                    else
                    {
                        var teknikSartnameList = await _offerService.GetOfferTeknikSartnameByOfferId(offer.Id);
                        foreach (var teknikSartname in teknikSartnameList)
                        {
                            teknikSartname.Features = Regex.Replace(teknikSartname.Features, @"[\r\n]$", "");
                            string[] rowValues = {
                                teknikSartname.No.ToString(),
                                teknikSartname.EquipmentName.ToString(),
                                teknikSartname.Features,
                                teknikSartname.Birim,
                                teknikSartname.Miktar.ToString()
                            }; // Example row values
                            AddRowToTable(wordDoc, rowValues, false, true);
                        }
                    }                   
                }
                modifiedDocument = memoryStream.ToArray();
            }

            return File(modifiedDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Teknik Şartname.docx");
        }

        public async Task<IActionResult> OnPostDavetAsync(int companyId, CancellationToken cancellationToken = default)
        {
            string templatePath = "";

            var offer = await GetOfferByIdAsync(Offer.Id, cancellationToken);

            var offerItems = offer?.OfferItems.ToList();

            var company = offerItems?.FirstOrDefault(x => x.CompanyId == companyId)?.Company;

            var projectOwner = offer?.ProjectOwner;
            CultureInfo trCulture = new CultureInfo("tr-TR");

            // Create a copy of the template to modify
            byte[] modifiedDocument;

            templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", "Davet.docx");
            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("Template not found.");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    ReplaceText(wordDoc, "A1", offer?.ProjectOwner?.Name?.ToUpper());
                    ReplaceText(wordDoc, "C3", offer.ProjectOwner.Name);
                    ReplaceText(wordDoc, "H10", offer.ProjectOwner.Name);
                    ReplaceText(wordDoc, "B2", company.TicariUnvan);
                    ReplaceText(wordDoc, "D4", company.Address);
                    ReplaceText(wordDoc, "E5", offer.ProjectAddress);
                    ReplaceText(wordDoc, "F6", offer.OfferName);
                    ReplaceText(wordDoc, "G7", offer.ProjectOwner.Address);
                    ReplaceText(wordDoc, "H8", offer.ProjectOwner.Telephone);
                    ReplaceText(wordDoc, "AXBY", Offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "DDMMYYY", Offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "XXYY", Offer.TeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "BCDAY", Offer.SonTeklifBildirme?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "HHMM", Offer.SonTeklifBildirme?.ToString("HH.mm"));
                }
                modifiedDocument = memoryStream.ToArray();
            }

            return File(modifiedDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{company.Name}-Davet.docx");
        }

        public async Task<IActionResult> OnPostGarantiDavetAsync(int companyId, CancellationToken cancellationToken = default)
        {
            string templatePath = "";

            var offer = await GetOfferByIdAsync(Offer.Id, cancellationToken);

            // Create a copy of the template to modify
            byte[] modifiedDocument;

            templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", "GarantiDavet.docx");
            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("Template not found.");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    ReplaceText(wordDoc, "AXDC", offer.ProjectOwner.Name.ToUpper());
                    ReplaceText(wordDoc, "C3", offer.ProjectOwner.Name);
                    ReplaceText(wordDoc, "A0", offer.ProjectAddress);
                    ReplaceText(wordDoc, "B1", offer.OfferName);
                    ReplaceText(wordDoc, "D4", offer.ProjectOwner.Address);
                    ReplaceText(wordDoc, "E5", offer.ProjectOwner.Telephone);
                    ReplaceText(wordDoc, "XTDA", offer.DanismanlikTeklifGonderim?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "BCDY", Offer.DanismanlikTeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "XYZT", Offer.DanismanlikSonTeklifSunum?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "AA", Offer.DanismanlikSonTeklifSunum?.ToString("HH.mm"));
                }
                modifiedDocument = memoryStream.ToArray();
            }

            return File(modifiedDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Garanti-Davet.docx");
        }

        public async Task<IActionResult> OnPostGarantiTeklifAsync(int companyId, CancellationToken cancellationToken = default)
        {
            string templatePath = "";

            var offer = await GetOfferByIdAsync(Offer.Id, cancellationToken);
            CultureInfo trCulture = new CultureInfo("tr-TR");

            // Create a copy of the template to modify
            byte[] modifiedDocument;

            templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", "GarantiTeklif.docx");
            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("Template not found.");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }

                var companySummaries = offer?.OfferItems
                    .GroupBy(oi => oi.Company.Name)
                    .Select(g => new CompanySummaryViewModel
                    {
                        CompanyName = g.Key,
                        TotalPrice = g.Sum(oi => oi.Price * oi.Quantity)
                    })
                    .OrderBy(s => s.TotalPrice)
                    .ToList();

                var minOfferAmount = companySummaries.Any() ? companySummaries.First().TotalPrice : 0;

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var isPlaniHazirligi = (minOfferAmount * (decimal)Offer.IsPlaniHazirligiYuzde) / 100;
                    var otp = (minOfferAmount * (decimal)Offer.OTPYuzde) / 100;
                    ReplaceText(wordDoc, "AXBY", GetRandomDate(Offer.DanismanlikTeklifGonderim, Offer.DanismanlikSonTeklifSunum).ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "A1", offer.ProjectOwner.Name);
                    ReplaceText(wordDoc, "B2", offer.ProjectOwner.Address);
                    ReplaceText(wordDoc, "Z1", offer.OfferName);
                    ReplaceText(wordDoc, "C3", offer.DanismanlikTeklifGonderim?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "D4", Offer.DanismanlikTeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "E5", Offer.HazirlanmaSuresi.ToString());
                    ReplaceText(wordDoc, "F6", Offer.PersonelSayisi.ToString());
                    ReplaceText(wordDoc, "LX", Offer.OtpHazirlanmaSuresi.ToString());
                    ReplaceText(wordDoc, "MY", Offer.OtpPersonelSayisi.ToString());
                    ReplaceText(wordDoc, "H7", isPlaniHazirligi.ToString("#,##0.00", trCulture));
                    ReplaceText(wordDoc, "I8", otp.ToString("#,##0.00", trCulture));
                    ReplaceText(wordDoc, "K9", (isPlaniHazirligi + otp).ToString("#,##0.00", trCulture));
                    ReplaceText(wordDoc, "ASDX", offer.DanismanlikTeklifGonderim?.ToString("dd.MM.yyyy"));
                    ReplaceText(wordDoc, "BDFX", offer.DanismanlikTeklifGecerlilikSuresi?.ToString("dd.MM.yyyy"));

                }
                modifiedDocument = memoryStream.ToArray();
            }

            return File(modifiedDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Garanti-Teklif.docx");
        }

        public async Task<IActionResult> OnPostGarantiTeknikSartnameAsync(CancellationToken cancellationToken = default)
        {
            string templatePath = "";

            var offer = await GetOfferByIdAsync(Offer.Id, cancellationToken);

            CultureInfo trCulture = new CultureInfo("tr-TR");

            // Create a copy of the template to modify
            byte[] modifiedDocument;

            templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", "GarantiTeknikSartname.docx");
            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("Template not found.");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, true))
                {
                    ReplaceText(wordDoc, "AZDC", offer.ProjectOwner.Name);
                    ReplaceText(wordDoc, "AXDY", offer.OfferName);
                    ReplaceText(wordDoc, "BCDY", offer.ProjectAddress);
                    ReplaceText(wordDoc, "XDAY", offer.DanismanlikTeklifGonderim?.ToString("dd.MM.yyyy"));
                }
                modifiedDocument = memoryStream.ToArray();
            }

            return File(modifiedDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Garanti-TeknikŞartname.docx");
        }

        private TableCell CreateStyledCell(string text)
        {
            // Create paragraph
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new Justification() { Val = JustificationValues.Center }); // Center align
            paragraph.PrependChild(paragraphProperties);

            // Create run properties for styling
            RunProperties runProperties = new RunProperties();
            runProperties.Append(new Bold());   // Make text bold
            runProperties.Append(new Italic()); // Make text italic
            runProperties.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" }); // Set font to Calibri
            runProperties.Append(new FontSize() { Val = "19" }); // Set font size to 9.5pt (19 in OpenXml)

            // Split text by new line character and add runs
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                Run run = new Run(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                run.PrependChild(runProperties.CloneNode(true)); // Clone run properties for each run
                paragraph.Append(run);

                // Add a line break if it's not the last line
                if (line != lines.Last())
                {
                    paragraph.Append(new Run(new Break()));
                }
            }

            TableCellProperties tcp = new TableCellProperties(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            // Create cell and add the styled paragraph
            TableCell cell = new TableCell(paragraph);
            cell.AppendChild(tcp);
            return cell;
        }

        private TableCell CreateStyledCellForAnotherOrders(string text, bool isLeft = false)
        {
            // Create paragraph
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            if (!isLeft)
                paragraphProperties.Append(new Justification() { Val = JustificationValues.Center }); // Center align
            else
                paragraphProperties.Append(new Justification() { Val = JustificationValues.Left }); // Center align

            paragraph.PrependChild(paragraphProperties);

            // Create run properties for styling
            RunProperties runProperties = new RunProperties();
            //runProperties.Append(new Bold());   // Make text bold
            //runProperties.Append(new Italic()); // Make text italic
            runProperties.Append(new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }); // Set font to Calibri
            runProperties.Append(new FontSize() { Val = "24" }); // Set font size to 9.5pt (19 in OpenXml)

            // Split text by new line character and add runs
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                Run run = new Run(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                run.PrependChild(runProperties.CloneNode(true)); // Clone run properties for each run
                paragraph.Append(run);

                // Add a line break if it's not the last line
                if (line != lines.Last())
                {
                    paragraph.Append(new Run(new Break()));
                }
            }

            TableCellProperties tcp = new TableCellProperties(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            // Create cell and add the styled paragraph
            TableCell cell = new TableCell(paragraph);
            cell.AppendChild(tcp);

            return cell;
        }

        private void ReplaceText(WordprocessingDocument wordDoc, string placeholder, string newText)
        {

            var body = wordDoc.MainDocumentPart.Document.Body;

            // Replace text in the main body of the document
            foreach (var text in body.Descendants<Text>())
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, newText);
                    if(newText != null)
                    {
                        string[] lines = newText.Split('\n');


                        // Create a paragraph

                        if (lines.Length > 1)
                        {
                            Paragraph paragraph = new Paragraph();

                            foreach (string line in lines)
                            {
                                if (!string.IsNullOrEmpty(line))
                                {
                                    Run run = new Run();
                                    run.Append(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                                    run.Append(new Break());
                                    paragraph.Append(run);
                                }

                            }

                            text.Parent.ReplaceChild(paragraph, text);
                        }
                    }
                    
                }

            }

            // Replace text inside content controls (structured document tags)
            foreach (var sdt in body.Descendants<SdtElement>())
            {
                foreach (var text in sdt.Descendants<Text>())
                {
                    if (text.Text.Contains(placeholder))
                    {
                        text.Text = text.Text.Replace(placeholder, newText);
                    }
                }
            }

            // Replace text inside fields (e.g., merge fields)
            foreach (var fieldCode in body.Descendants<FieldCode>())
            {
                if (fieldCode.Text.Contains(placeholder))
                {
                    var fieldText = fieldCode.Parent.Descendants<Text>().FirstOrDefault();
                    if (fieldText != null)
                    {
                        fieldText.Text = fieldText.Text.Replace(placeholder, newText);
                    }
                }
            }
        }

        private void AddRowToTable(WordprocessingDocument wordDoc, string[] cellValues, bool isCetinkaya = true, bool isTeknikSartname = false)
        {

            Table? table = null;
            var mainPart = wordDoc.MainDocumentPart;
            if(!isTeknikSartname)
                table = mainPart.Document.Body.Elements<Table>().Skip(1).Take(1).FirstOrDefault(); // Select the first table
            else
                table = mainPart.Document.Body.Elements<Table>().FirstOrDefault(); // Select the first table

            if (table == null)
            {
                Console.WriteLine("No table found in the document.");
                return;
            }

            // Create a new row
            TableRow newRow = new TableRow();

            // Add cells with values
            int order = 0;
            foreach (var value in cellValues)
            {
                if (isCetinkaya)
                {
                    TableCell cell = CreateStyledCell(value);
                    newRow.Append(cell);
                }
                else
                {
                    bool isLeft = order == 2 ? true : false;
                    TableCell cell = CreateStyledCellForAnotherOrders(value, isLeft);
                    newRow.Append(cell);
                    order++;
                }
            }

            // Append the row to the table
            table.Append(newRow);

            // Save changes
            mainPart.Document.Save();
        }

        private DateTime GetRandomDate(DateTime? start, DateTime? end)
        {
            Random rand = new Random();
            int range = (end.Value - start.Value).Days;
            return start.Value.AddDays(rand.Next(range + 1));
        }

        public async Task<IActionResult> OnPostAddItemAsync(CancellationToken cancellationToken = default)
        {
            var offer = await GetOfferByIdAsync(Offer.Id, cancellationToken);

            var projectOwner = await _context.Offers
                    .Include(o => o.ProjectOwner)
                    .FirstOrDefaultAsync(o => o.Id == Offer.Id, cancellationToken);
            var projectOwnerTraktorHp = projectOwner.ProjectOwner.Hp;

            OfferItems = offer.OfferItems.ToList();

            if (NewItem == null || NewItem.EquipmentModelId == 0 || NewItem.CompanyId == 0 || NewItem.Price <= 0 || NewItem.Quantity <= 0)
            {
                await LoadRelatedData(Offer.Id, cancellationToken);
                return Page();
            }

            var equipmentModelUnitFeature = await _context.EquipmentModelFeatures.Include(x => x.Unit).FirstOrDefaultAsync(x => x.EquipmentModelId == NewItem.EquipmentModelId && x.Unit.Name == "Hp", cancellationToken);
            if(equipmentModelUnitFeature != null)
            {
                string[] valueRange = equipmentModelUnitFeature.FeatureValue.Split('-');
                if(valueRange.Length > 1)
                {
                    var firstValue = valueRange.First();
                    var lastOne = valueRange.Last();
                    if (projectOwnerTraktorHp < Int32.Parse(lastOne))
                        //if (!((projectOwnerTraktorHp > Int32.Parse(firstValue) && projectOwnerTraktorHp <= Int32.Parse(lastOne)) || projectOwnerTraktorHp > Int32.Parse(lastOne)))
                    {
                        await LoadRelatedData(Offer.Id, cancellationToken);

                        StatusMessage = "Traktor Hp degeri, makine ekipman hp degeri araliginda veya bu degerden buyuk olmalidir";
                        return Page();
                    }
                }
                else
                {
                    if(projectOwnerTraktorHp < Int32.Parse(equipmentModelUnitFeature.FeatureValue))
                    {
                        await LoadRelatedData(Offer.Id, cancellationToken);
                        StatusMessage = "Traktor Hp degeri, makine ekipman hp degerinden küçük olmamalidir";
                        return Page();
                    }
                }
            }


            NewItem.OfferId = Offer.Id;
            var offerItem = OfferItems.Where(x => x.OfferId == Offer.Id && x.CompanyId == NewItem.CompanyId && x.TeklifGirisTarihi != DateTime.MinValue).FirstOrDefault();
            if (offerItem == null)
                NewItem.TeklifGirisTarihi = GetRandomDate(Offer.TeklifGonderimTarihi, Offer.SonTeklifBildirme);
            else
                NewItem.TeklifGirisTarihi = offerItem.TeklifGirisTarihi;

            if (OfferItems.Any(x => x.CompanyId == NewItem.CompanyId && x.EquipmentModelId == NewItem.EquipmentModelId))
            {
                await LoadRelatedData(Offer.Id, cancellationToken);
                StatusMessage = "Zaten bu kurum, aynı ekipman modele daha önce teklif vermiş";
                return Page();
            }

            var equipmentModel = await _context.EquipmentModels.FindAsync(NewItem.EquipmentModelId);
            var filteredItems = OfferItems.Where(x => x.EquipmentModel.EquipmentId == equipmentModel?.EquipmentId);
            if (OfferItems.Any(x => x.CompanyId == NewItem.CompanyId && x.EquipmentModel.EquipmentId == equipmentModel?.EquipmentId))
            {
                await LoadRelatedData(Offer.Id, cancellationToken);
                StatusMessage = "Zaten bu firma, aynı ekipmana daha önce teklif vermiş";
                return Page();
            }


            if (OfferItems.Any())
            {
                //var filteredItems = OfferItems.Where(x => x.EquipmentModelId == NewItem.EquipmentModelId);
                var minOffer = filteredItems.Any() ? filteredItems.Min(x => x.Price) : (decimal?)null;

                if (minOffer != null)
                {
                    var twentyPercentMore = (double)minOffer * (1.2);

                    if (NewItem.Price < minOffer)
                    {
                        await LoadRelatedData(Offer.Id, cancellationToken);

                        StatusMessage = "Teklif tutarı en düşük teklif tutarından düşük olamaz";
                        return Page();
                    }

                    if ((double)NewItem.Price > twentyPercentMore)
                    {
                        await LoadRelatedData(Offer.Id, cancellationToken);

                        StatusMessage = "Teklif tutarı en düşük teklif tutarının %20sinden fazla olamaz";
                        return Page();
                    }
                }
            }

            _context.OfferItems.Add(NewItem);
            await _context.SaveChangesAsync(cancellationToken);

            ClearCache(Offer.Id);
            await _offerService.ReCreateOfferTeknikSartnameByOfferId(Offer.Id);


            var offerItems = await _context.OfferItems.Where(oi => oi.OfferId == Offer.Id && oi.CompanyId == NewItem.CompanyId && oi.TeklifGirisTarihi == DateTime.MinValue).ToListAsync(cancellationToken);
            if (offerItems.Any())
            {
                var teklifGirisTarihi = GetRandomDate(Offer.TeklifGonderimTarihi, Offer.SonTeklifBildirme);
                foreach (var item in offerItems)
                {
                    item.TeklifGirisTarihi = teklifGirisTarihi;
                    _context.Entry(item).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            return RedirectToPage("./Edit", new { id = Offer.Id });
        }

        public async Task<IActionResult> OnPostDeleteItemAsync(int itemId, CancellationToken cancellationToken = default)
        {
            var offerItem = await _context.OfferItems.FindAsync(itemId, cancellationToken);
            if (offerItem == null)
            {
                return NotFound();
            }

            var offerId = offerItem.OfferId;
            _context.OfferItems.Remove(offerItem);
            await _context.SaveChangesAsync(cancellationToken);
            ClearCache(Offer.Id);

            await _offerService.ReCreateOfferTeknikSartnameByOfferId(Offer.Id);

            return RedirectToPage("./Edit", new { id = offerId });
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.Id == id);
        }

        private async Task<Offer?> GetOfferByIdAsync(int? id, CancellationToken cancellationToken = default)
        {
            return await _offerService.GetOfferByIdAsync(id, cancellationToken);
        }

        public void ClearCache(int id)
        {
            string cacheKey = $"offer_{id}";
            _cache.Remove(cacheKey); // Remove the cache item when needed
        }

        public async Task<IActionResult> OnPostUpdatePriceAsync(int itemId, decimal newPrice, CancellationToken cancellationToken = default)
        {
            var offerItem = await _context.OfferItems.Include(x => x.EquipmentModel).ThenInclude(x => x.Equipment).FirstOrDefaultAsync(x => x.Id == itemId, cancellationToken);

            if (offerItem != null)
            {
                var offerId = offerItem.OfferId;
                var offerDetail = await GetOfferByIdAsync(offerId, cancellationToken);

                decimal? min = offerDetail?.OfferItems.Where(x => x.EquipmentModel.EquipmentId == offerItem.EquipmentModel.Equipment.Id).Min(x => x.Price);

                var twentyPercentMore = min * (decimal)1.2;

                if (newPrice <= min)
                {
                    StatusMessage = "Teklif tutarı en dusuk teklif miktarindan az olamaz";
                    await LoadRelatedData(offerId, cancellationToken);
                    return Page();
                }

                if(newPrice > twentyPercentMore)
                {
                    StatusMessage = "Teklif tutarı en dusuk teklif miktarindan yuzde 20si araliginda olmalidir";
                    await LoadRelatedData(offerId, cancellationToken);
                    return Page();
                }

                offerItem.Price = newPrice;
                _context.Entry(offerItem).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                ClearCache(offerItem.OfferId);
            }

            return RedirectToPage("./Edit", new { id = offerItem?.OfferId });
        }

        public async Task<IActionResult> OnGetEquipmentModelPriceAsync(int companyId, int equipmentModelId, CancellationToken cancellationToken = default)
        {
            var companyEquipmentModel = await _context.CompanyEquipmentModels
                .Where(cem => cem.CompanyId == companyId && cem.EquipmentModelId == equipmentModelId)
                .Select(cem => new { cem.Price })
                .FirstOrDefaultAsync(cancellationToken);

            return new JsonResult(companyEquipmentModel?.Price);
        }

        private string ConvertListToString(IEnumerable<string> list)
        {
            return string.Join(", ", list);
        }
    }
}