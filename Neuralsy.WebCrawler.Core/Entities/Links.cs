using System;
using System.Collections.Generic;

namespace Neuralsy.WebCrawler.Core.Entities;

/// <summary>
/// Represents a set of anchors links (urls and emails)
/// </summary>
public record Links
{
    /// <summary>
    /// Gets a set of urls
    /// </summary>
    /// <remarks>this set is case-sensitive</remarks>
    public HashSet<string> Urls { get; } = new();

    /// <summary>
    /// Gets a set of emails addresses
    /// </summary>
    /// <remarks>this set is case-insensitive</remarks>
    public HashSet<string> Emails { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}