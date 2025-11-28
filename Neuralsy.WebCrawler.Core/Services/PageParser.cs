using Neuralsy.WebCrawler.Core.Entities;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Neuralsy.WebCrawler.Core.Interfaces;

namespace Neuralsy.WebCrawler.Core.Services;

/// <inheritdoc />
public class PageParser : IPageParser
{
    private readonly int _emailStartIndex = "mailto:".Length;
    private readonly XmlReaderSettings _xmlReaderSettings = new() { Async = true };

    /// <inheritdoc />
    public async Task<Links> GetLinksAsync(string content)
    {
        var result = new Links();

        // html content is assumed here to be a valid xml content
        using var textReader = new StringReader(content);
        using var xmlReader = XmlReader.Create(textReader, _xmlReaderSettings);

        while (await xmlReader.ReadAsync())
        {
            if (xmlReader is not { NodeType: XmlNodeType.Element, Name : "a" })
            {
                continue;
            }

            var href = xmlReader.GetAttribute("href");
            if (href is null)
            {
                continue;
            }

            if (href.StartsWith("mailto:"))
            {
                var email = href.Substring(_emailStartIndex);
                if (email.Length > 0)
                {
                    result.Emails.Add(email);
                }
            }
            else
            {
                result.Urls.Add(href);
            }
        }

        return result;
    }
}