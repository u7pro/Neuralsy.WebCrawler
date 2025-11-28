using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Neuralsy.WebCrawler.Core.Entities;
using Neuralsy.WebCrawler.Core.Interfaces;
using CrawlingOptions = (string Url, int MaximumDepth);

namespace Neuralsy.WebCrawler.Core.Services;

/// <inheritdoc />
public class Crawler(IWebBrowser webBrowser, IPageParser pageParser) : ICrawler
{
    /// <inheritdoc />
    public async Task<IReadOnlySet<string>> GetEmailsAsync(string url, int maximumDepth)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumDepth, -1);

        // we use a reader / writer channel so we do job asynchronously without any recursion
        var crawlingChannel = Channel.CreateUnbounded<CrawlingOptions>();

        var initialContext = (url, maximumDepth);
        await crawlingChannel.Writer.WriteAsync(initialContext);
        
        var links = new Links();

        await foreach (var context in crawlingChannel.Reader.ReadAllAsync())
        {
            await GetEmailsAsync(context.Url, context.MaximumDepth, links, crawlingChannel);

            if (crawlingChannel.Reader.Count == 0)
            {
                crawlingChannel.Writer.Complete();
            }
        }

        return links.Emails;
    }

    private async Task GetEmailsAsync(string url, int maximumDepth, Links links, Channel<CrawlingOptions> crawlingChannel)
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

            // push info of crawling on references pages 
            foreach (var resultVisitedUrl in parseResult.Urls)
            {
                var childContext = (resultVisitedUrl, maximumDepth);
                await crawlingChannel.Writer.WriteAsync(childContext);
            }
        }
    }
}