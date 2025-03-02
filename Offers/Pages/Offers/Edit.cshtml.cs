using Microsoft.AspNetCore.Mvc;
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
using System.IO;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using DocumentFormat.OpenXml;
using System.Globalization;

namespace Pages.Offers
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Offer Offer { get; set; }

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

        private async Task LoadRelatedData(CancellationToken cancellationToken = default)
        {
            var equipmentModels = await _context.EquipmentModels
            .Include(em => em.Equipment)
            .OrderBy(em => em.Equipment.Name)
            .ThenBy(em => em.Brand)
            .ThenBy(em => em.Model)
            .ToListAsync(cancellationToken);  

            CompanySummaries = await _context.OfferItems
                .Where(oi => oi.OfferId == Offer.Id)
                .GroupBy(oi => oi.Company.Name)
                .Select(g => new CompanySummaryViewModel
                {
                    CompanyName = g.Key,
                    TotalPrice = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderBy(s => s.TotalPrice)
                .ToListAsync(cancellationToken);

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

            var offer = await GetOfferById(Offer.Id);
            OfferItems = offer.OfferItems.OrderBy(x => x.Company.Name).ThenBy(x => x.Price).ToList();

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

        private async Task<List<EquipmentModel>> GetEquipmentModelsByCompanyId(int companyId)
        {
            var companyEquipmentModels = await _context.CompanyEquipmentModels
                .Where(cem => cem.CompanyId == companyId)
                .Select(cem => cem.EquipmentModelId)
                .ToListAsync();

            if (companyEquipmentModels.Any())
            {
                return await _context.EquipmentModels
                    .Include(em => em.Equipment)
                    .Where(em => companyEquipmentModels.Contains(em.Id))
                    .OrderBy(em => em.Equipment.Name)
                    .ThenBy(em => em.Brand)
                    .ThenBy(em => em.Model)
                    .ToListAsync();
            }

            // If no company equipment models exist, return all equipment models
            return await _context.EquipmentModels
                .Include(em => em.Equipment)
                .OrderBy(em => em.Equipment.Name)
                .ThenBy(em => em.Brand)
                .ThenBy(em => em.Model)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Offer =  await GetOfferById(id);

            if (Offer == null)
            {
                return NotFound();
            }

            await LoadRelatedData();
            NewItem.EquipmentModelId = 0;
            NewItem.CompanyId = 0;
            return Page();
        }

        public async Task<IActionResult> OnGetUpdateEquipmentModelsAsync(int companyId)
        {
            var equipmentModels = await GetEquipmentModelsByCompanyId(companyId);
            return new JsonResult(equipmentModels.Select(em => new
            {
                id = em.Id,
                text = $"{em.Equipment.Name} - {em.Brand} {em.Model}"
            }));
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if(string.IsNullOrEmpty(Offer.OfferName))
            {
                await LoadRelatedData();
                return Page();
            }

            if (!(Offer.SonTeklifBildirme > Offer.TeklifGonderimTarihi))
            {
                StatusMessage = "Son teklif bildirme tarihi teklif gönderim tarihinden sonra olmalıdır";
                await LoadRelatedData();
                return Page();
            }

            if (!(Offer.TeklifSunumTarihi > Offer.TeklifGonderimTarihi && Offer.TeklifSunumTarihi <= Offer.SonTeklifBildirme)) {

                StatusMessage = "Teklif sunum tarihi teklif gonderim tarihinden sonra ve son teklif bildirme tarihinden önce olmalıdır";
                await LoadRelatedData();
                return Page();
            }

            _context.Attach(Offer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            var offer = await GetOfferById(Offer.Id);

            var offerItems = offer.OfferItems.ToList();

            var company = offerItems.FirstOrDefault(x => x.CompanyId == companyId)?.Company;

            var projectOwner = offer.ProjectOwner;
            CultureInfo trCulture = new CultureInfo("tr-TR");

            // Create a copy of the template to modify
            byte[] modifiedDocument;
            if (company.Name == "Çetinkaya")
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
                        ReplaceText(wordDoc, "Yatirimci", projectOwner?.Name ?? "");
                        ReplaceText(wordDoc, "aaaa", projectOwner?.Address ?? "");
                        ReplaceText(wordDoc, "yzyzyz", Offer.OfferName ?? "");
                        ReplaceText(wordDoc, "ddmmyyyy", DateTime.Now.ToString("dd.MM.yyyy") ?? "");
                        ReplaceText(wordDoc, "M12", Offer.TeklifGonderimTarihi?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "N13", Offer.SonTeklifBildirme?.ToString("dd.MM.yyyy"));

                        decimal totalPrices = 0;
                        foreach (var offerItem in offerItems.Where(x => x.CompanyId == companyId).ToList())
                        {
                            var result = new StringBuilder();
                            foreach (var feature in offerItem.EquipmentModel?.Features?.ToList())
                            {
                                result.AppendLine($"{feature.FeatureKey} {feature.FeatureValue} {feature.Unit?.Name ?? ""}");
                            }
                            var equipment = offerItem.EquipmentModel.Equipment.Name;
                            var features = result.ToString();
                            var equipmentModel = offerItem.EquipmentModel.Brand + " " + offerItem.EquipmentModel.Model;
                            var sayi = offerItem.Quantity;
                            var price = offerItem.Price;
                            var totalPrice = sayi * price;
                            totalPrices += totalPrice;

                            string[] rowValues = { equipment, features, equipmentModel, "Adet", sayi.ToString(), price.ToString("#,##0.00", trCulture) + " TL", totalPrice.ToString("#,##0.00", trCulture) + " TL" }; // Example row values

                            AddRowToTable(wordDoc, rowValues, true);

                        }
                        string[] totalPriceRow = { "", "", "", "", "", "Toplam Fiyat (TL) ", totalPrices.ToString("#,##0.00", trCulture) + " TL" }; // Example row values
                        AddRowToTable(wordDoc, totalPriceRow, true);
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
                        ReplaceText(wordDoc, "N13", Offer.SonTeklifBildirme?.ToString("dd.MM.yyyy"));
                        ReplaceText(wordDoc, "ddmmyyyy", DateTime.Now.ToString("dd.MM.yyyy") ?? "");

                        decimal totalPrices = 0;
                        var no = 1;

                        
                        foreach (var offerItem in offerItems.Where(x => x.CompanyId == companyId).ToList())
                        {
                            var result = new StringBuilder();
                            foreach (var feature in offerItem.EquipmentModel?.Features?.ToList())
                            {
                                result.AppendLine($"{feature.FeatureKey} {feature.FeatureValue} {feature.Unit?.Name ?? ""}");
                            }
                            var equipment = offerItem.EquipmentModel.Equipment.Name;
                            var features = result.ToString();
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
                    }



                    modifiedDocument = memoryStream.ToArray();
                }
            }

                return File(modifiedDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{company.Name}-Teklif.docx");
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

            // Create cell and add the styled paragraph
            TableCell cell = new TableCell(paragraph);
            return cell;
        }

        private TableCell CreateStyledCellForAnotherOrders(string text)
        {
            // Create paragraph
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new Justification() { Val = JustificationValues.Center }); // Center align
            paragraph.PrependChild(paragraphProperties);

            // Create run properties for styling
            RunProperties runProperties = new RunProperties();
            //runProperties.Append(new Bold());   // Make text bold
            //runProperties.Append(new Italic()); // Make text italic
            runProperties.Append(new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }); // Set font to Calibri
            runProperties.Append(new FontSize() { Val = "28" }); // Set font size to 9.5pt (19 in OpenXml)

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

            // Create cell and add the styled paragraph
            TableCell cell = new TableCell(paragraph);
            return cell;
        }

        private void ReplaceText(WordprocessingDocument wordDoc, string placeholder, string newText)
        {
            //var body = wordDoc.MainDocumentPart.Document.Body;
            //foreach (var text in body.Descendants<Text>())
            //{
            //    if (text.Text.Contains(placeholder))
            //    {
            //        text.Text = text.Text.Replace(placeholder, newText);
            //    }
            //}

            var body = wordDoc.MainDocumentPart.Document.Body;

            // Replace text in the main body of the document
            foreach (var text in body.Descendants<Text>())
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, newText);
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

        private void AddRowToTable(WordprocessingDocument wordDoc, string[] cellValues, bool isCetinkaya = true)
        {

                var mainPart = wordDoc.MainDocumentPart;
                var table = mainPart.Document.Body.Elements<Table>().Skip(1).Take(1).FirstOrDefault(); // Select the first table

                if (table == null)
                {
                    Console.WriteLine("No table found in the document.");
                    return;
                }

                // Create a new row
                TableRow newRow = new TableRow();

                // Add cells with values
                foreach (var value in cellValues)
                {
                    if(isCetinkaya)
                    {
                        TableCell cell = CreateStyledCell(value);
                        newRow.Append(cell);
                    }
                    else
                    {
                        TableCell cell = CreateStyledCellForAnotherOrders(value);
                        newRow.Append(cell);
                    }
                }

                // Append the row to the table
                table.Append(newRow);

                // Save changes
                mainPart.Document.Save();
        }

        public async Task<IActionResult> OnPostAddItemAsync()
        {
            var offer = await GetOfferById(Offer.Id);
            
            OfferItems = offer.OfferItems.ToList();

            if (NewItem == null || NewItem.EquipmentModelId == 0 || NewItem.CompanyId == 0 || NewItem.Price <= 0 || NewItem.Quantity <= 0)
            {
                await LoadRelatedData();
                await UpdateOfferTotalPrice(Offer.Id);
                return Page();
            }

            NewItem.OfferId = Offer.Id;

            if(OfferItems.Any(x => x.CompanyId == NewItem.CompanyId && x.EquipmentModelId == NewItem.EquipmentModelId))
            {
                await LoadRelatedData();

                StatusMessage = "Zaten kurum bu ekipmana teklif vermiş";
                await UpdateOfferTotalPrice(Offer.Id);
                return Page();
            }


            if (OfferItems.Any())
            {
                var filteredItems = OfferItems.Where(x => x.EquipmentModelId == NewItem.EquipmentModelId);
                var minOffer = filteredItems.Any() ? filteredItems.Min(x => x.Price) : (decimal?)null;

                if (minOffer != null)
                {
                    var twentyPercentMore = (double)minOffer * (1.2);

                    if (NewItem.Price < minOffer)
                    {
                        await LoadRelatedData();

                        StatusMessage = "Teklif tutarı en düşük teklif tutarından düşük olamaz";
                        await UpdateOfferTotalPrice(Offer.Id);
                        return Page();
                    }

                    if ((double)NewItem.Price > twentyPercentMore)
                    {
                        await LoadRelatedData();

                        StatusMessage = "Teklif tutarı en düşük teklif tutarının %20sinden fazla olamaz";
                        await UpdateOfferTotalPrice(Offer.Id);
                        return Page();
                    }
                }
            }

            _context.OfferItems.Add(NewItem);
            await _context.SaveChangesAsync();

            // Update total price
            await UpdateOfferTotalPrice(Offer.Id);

            return RedirectToPage("./Edit", new { id = Offer.Id });
        }

        public async Task<IActionResult> OnPostDeleteItemAsync(int itemId)
        {
            var offerItem = await _context.OfferItems.FindAsync(itemId);
            if (offerItem == null)
            {
                return NotFound();
            }

            var offerId = offerItem.OfferId;
            _context.OfferItems.Remove(offerItem);
            await _context.SaveChangesAsync();

            // Update total price
            await UpdateOfferTotalPrice(offerId);

            return RedirectToPage("./Edit", new { id = offerId });
        }

        private async Task UpdateOfferTotalPrice(int offerId)
        {
            var offer = await _context.Offers.AsNoTracking()
                .Include(o => o.OfferItems)
                .FirstOrDefaultAsync(o => o.Id == offerId).ConfigureAwait(false);

            if (offer != null)
            {
                _context.Entry(offer).State = EntityState.Modified;
                offer.TotalPrice = offer.OfferItems.Sum(item => item.Price * item.Quantity);
                Offer.TotalPrice = offer.TotalPrice;
            }
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.Id == id);
        }

        private async Task<Offer> GetOfferById(int? id)
        {

            return await _context.Offers
            .Include(o => o.ProjectOwner)
            .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.EquipmentModel)
                    .ThenInclude(em => em.Equipment)
            .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.Company)
            .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.EquipmentModel)
                  .ThenInclude(em => em.Features)
                   .ThenInclude(o => o.Unit)
            .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IActionResult> OnPostUpdatePriceAsync(int itemId, decimal newPrice)
        {
            var offerItem = await _context.OfferItems.FindAsync(itemId);

            if (offerItem != null)
            {
                offerItem.Price = newPrice;
                _context.Entry(offerItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Edit", new { id = offerItem?.OfferId });
        }
    }
} 