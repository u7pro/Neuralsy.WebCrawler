using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neuralsy.WebCrawler.Core.Interfaces;

/// <summary>
/// Represents a web crawler for emails
/// </summary>
public interface ICrawler
{
    /// <summary>
    /// Gets emails found on given <paramref name="url" /> and its referenced pages according to <paramref name="maximumDepth" /> value
    /// </summary>
    /// <param name="url">content url</param>
    /// <param name="maximumDepth">maximum depth of crawling, or -1 for infinite depth</param>
    /// <exception cref="maximumDepth">negative values other than -1 are not allowed</exception>
    Task<IReadOnlySet<string>> GetEmailsAsync(string url, int maximumDepth);
}