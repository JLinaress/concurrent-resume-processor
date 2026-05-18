using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
using System.Text.RegularExpressions;
namespace ProcessorLib.Services;

public static class DocumentParser
{
    public static string ParsePdf(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"No file at: {filePath}");

        using var doc = PdfDocument.Open(filePath);
        var pages = doc.GetPages().Take(3).ToList();
        if (pages.Count == 0)
            return "";

        var texts = pages.Select(p => p.Text).Where(t => !string.IsNullOrWhiteSpace(t));
        return string.Join(" ", texts);
    }

    public static string ParseWord(string filePath)
    {
        using var doc = WordprocessingDocument.Open(filePath, false);
        
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body == null) return "";
        
        return body.InnerText;
    }
    
    //TODO: Come back and refactor instead of using Regex 
    public static string CleanText(string text) => 
        Regex.Replace(text ?? "", @"[^\w\s\-\|]", " ").ToLower();
}