using Neuralsy.WebCrawler.Core.Interfaces;

namespace Neuralsy.WebCrawler.Tests.Helpers;

/// <summary>
/// Represents an in-memory <see cref="IWebBrowser" /> based on a dictionary of well-known pages
/// </summary>
public class InMemoryWebBrowser(IReadOnlyDictionary<string, string> pages) : IWebBrowser
{
    /// <inheritdoc />
    public string? GetHtml(string url)
    {
        pages.TryGetValue(url, out var content);
        return content;
    }
}