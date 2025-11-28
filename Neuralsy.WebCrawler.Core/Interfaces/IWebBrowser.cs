namespace Neuralsy.WebCrawler.Core.Interfaces;

/// <summary>
/// Represents a web browser
/// </summary>
public interface IWebBrowser
{
    /// <summary>
    /// Gets html content corresponding to given <paramref name="url" />
    /// </summary>
    /// <returns>Returns page content, otherwise null if url couldn't be retrieved</returns>
    string? GetHtml(string url);
}