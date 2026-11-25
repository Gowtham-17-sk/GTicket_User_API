using System.Net;
using System.Text.RegularExpressions;
using GTicket_User_API.Services.Interface;

namespace GTicket_User_API.Services.Implement;

public partial class EmailTemplateService(
    IWebHostEnvironment environment,
    ILogger<EmailTemplateService> logger) : IEmailTemplateService
{
    // Solution Explorer la irukkura folder name oda exact match aaganum
    private const string TemplateFolder = "Template";

    public async Task<string> RenderAsync(string templateFileName, IDictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        if (Path.GetFileName(templateFileName) != templateFileName)
            throw new ArgumentException("Invalid template name.", nameof(templateFileName));

        // ContentRootPath = project folder (VS la run pannumbodhu G:\...\GTicket_User_API)
        var path = Path.Combine(environment.ContentRootPath, TemplateFolder, templateFileName);

        if (!File.Exists(path))
        {
            logger.LogError("Template file not found: {Path}", path);
            throw new FileNotFoundException($"Template not found: {templateFileName}");
        }

        var html = await File.ReadAllTextAsync(path, cancellationToken);

        foreach (var (key, value) in values)
        {
            html = html.Replace("{{" + key + "}}", WebUtility.HtmlEncode(value));
        }

        var leftover = PlaceholderRegex().Matches(html);
        if (leftover.Count > 0)
        {
            logger.LogWarning("Template {Template} has unreplaced placeholders: {Placeholders}",
                templateFileName, string.Join(", ", leftover.Select(m => m.Value).Distinct()));
        }

        return html;
    }

    [GeneratedRegex(@"\{\{\w+\}\}")]
    private static partial Regex PlaceholderRegex();
}