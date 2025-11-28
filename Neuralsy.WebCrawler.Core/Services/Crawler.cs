using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuralsy.WebCrawler.Core.Entities;
using Neuralsy.WebCrawler.Core.Interfaces;

namespace Neuralsy.WebCrawler.Core.Services;

/// <inheritdoc />
public class Crawler(IWebBrowser webBrowser, IPageParser pageParser) : ICrawler
{
    /// <inheritdoc />
    public async Task<IReadOnlySet<string>> GetEmailsAsync(string url, int maximumDepth)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumDepth, -1);

        var links = new Links();
        await GetEmailsAsync(url, maximumDepth, links);

        return links.Emails;
    }

    private async Task GetEmailsAsync(string url, int maximumDepth, Links links)
    {
        if (!links.Urls.Add(url))
        {
            // we've already visited this url, so we skip crawling it again
            return;
        }

        var content = webBrowser.GetHtml(url);
        if (content is null)
        {
            return;
        }

        var parseResult = await pageParser.GetLinksAsync(content);

        links.Emails.UnionWith(parseResult.Emails);

        // crawl referenced pages if any and maximum depth didn't reach 0
        var canVisitChildPages = maximumDepth != 0;
        if (canVisitChildPages)
        {
            if (maximumDepth > 0)
            {
                maximumDepth--;
            }

            // start crawling on references pages, then await for tasks completion
            var tasks = new List<Task>();
            foreach (var resultVisitedUrl in parseResult.Urls)
            {
                var task = GetEmailsAsync(resultVisitedUrl, maximumDepth, links);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}