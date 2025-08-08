using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pages.Offers.Helpers
{
    /// <summary>
    /// Helper class for generating and manipulating offer-related Word documents.
    /// </summary>
    public static class OfferDocumentHelper
    {
        public static void ReplaceText(WordprocessingDocument wordDoc, string placeholder, string newText)
        {
            var body = wordDoc.MainDocumentPart.Document.Body;
            foreach (var text in body.Descendants<Text>())
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, newText);
                    if (newText != null)
                    {
                        string[] lines = newText.Split('\n');
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

        public static void AddRowToTable(WordprocessingDocument wordDoc, string[] cellValues, bool isCetinkaya = true, bool isTeknikSartname = false)
        {
            Table? table = null;
            var mainPart = wordDoc.MainDocumentPart;
            if (!isTeknikSartname)
                table = mainPart.Document.Body.Elements<Table>().Skip(1).Take(1).FirstOrDefault();
            else
                table = mainPart.Document.Body.Elements<Table>().FirstOrDefault();
            if (table == null)
                return;
            TableRow newRow = new TableRow();
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
            table.Append(newRow);
            mainPart.Document.Save();
        }

        public static TableCell CreateStyledCell(string text)
        {
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new Justification() { Val = JustificationValues.Center });
            paragraph.PrependChild(paragraphProperties);
            RunProperties runProperties = new RunProperties();
            runProperties.Append(new Bold());
            runProperties.Append(new Italic());
            runProperties.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            runProperties.Append(new FontSize() { Val = "19" });
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                Run run = new Run(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                run.PrependChild(runProperties.CloneNode(true));
                paragraph.Append(run);
                if (line != lines.Last())
                {
                    paragraph.Append(new Run(new Break()));
                }
            }
            TableCellProperties tcp = new TableCellProperties(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            TableCell cell = new TableCell(paragraph);
            cell.AppendChild(tcp);
            return cell;
        }

        public static TableCell CreateStyledCellForAnotherOrders(string text, bool isLeft = false)
        {
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            if (!isLeft)
                paragraphProperties.Append(new Justification() { Val = JustificationValues.Center });
            else
                paragraphProperties.Append(new Justification() { Val = JustificationValues.Left });
            paragraph.PrependChild(paragraphProperties);
            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            runProperties.Append(new FontSize() { Val = "24" });
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                Run run = new Run(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                run.PrependChild(runProperties.CloneNode(true));
                paragraph.Append(run);
                if (line != lines.Last())
                {
                    paragraph.Append(new Run(new Break()));
                }
            }
            TableCellProperties tcp = new TableCellProperties(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            TableCell cell = new TableCell(paragraph);
            cell.AppendChild(tcp);
            return cell;
        }
    }
}
