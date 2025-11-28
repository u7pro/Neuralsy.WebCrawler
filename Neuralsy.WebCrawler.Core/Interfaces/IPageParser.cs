using System.Threading.Tasks;
using Neuralsy.WebCrawler.Core.Entities;

namespace Neuralsy.WebCrawler.Core.Interfaces;

/// <summary>
/// Represents an html page parser
/// </summary>
public interface IPageParser
{
    /// <summary>
    /// Parses given <paramref name="content" /> and retrieves anchors links (urls and mails)
    /// </summary>
    /// <param name="content">html content</param>
    Task<Links> GetLinksAsync(string content);
}