using Neuralsy.WebCrawler.Core.Services;
using Neuralsy.WebCrawler.Tests.Helpers;
using NUnit.Framework;

namespace Neuralsy.WebCrawler.Tests;

public class CrawlingTests
{
    private readonly Dictionary<string, string> _pages = new()
    {
        ["./index.html"] =
            // lang=html
            """
            <html>
            <h1>INDEX</h1>
            <a href="./child1.html">child1</a>
            <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a> 
            </html>
            """,

        ["./child1.html"] =
            // lang=html
            """
            <html> 
            <h1>CHILD1</h1> 
            <a href="./index.html">index</a> 
            <a href="./child2.html">child2</a> 
            <a href="mailto:ailleurs@mozilla.org">Envoyer l'email ailleurs</a> 
            <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a> 
            </html> 
            """,

        ["./child2.html"] =
            // lang=html
            """
            <html> 
            <h1>CHILD2</h1> 
            <a href="./index.html">index</a> 
            <a href="mailto:loin@mozilla.org">Envoyer l'email loin</a> 
            <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a> 
            </html>
            """,
    };

    [TestCase("", 0, TestName = "EmptyUrl")]
    [TestCase("404", 0, TestName = "UnknownUrl")]
    [TestCase("./index.html", 0, "nullepart@mozilla.org", TestName = "OnlyTargetedUrl")]
    [TestCase("./index.html", 1, "nullepart@mozilla.org", "ailleurs@mozilla.org", TestName = "DepthLimitedTo1")]
    [TestCase("./index.html", 2, "nullepart@mozilla.org", "ailleurs@mozilla.org", "loin@mozilla.org", TestName = "DepthLimitedTo2")]
    [TestCase("./index.html", -1, "nullepart@mozilla.org", "ailleurs@mozilla.org", "loin@mozilla.org", TestName = "InfiniteDepth")]
    public async Task Check(string url, int maxDepth, params string[] expectedEmails)
    {
        var browser = new InMemoryWebBrowser(_pages);
        var pageParser = new PageParser();
        var crawler = new Crawler(browser, pageParser);

        var emails = await crawler.GetEmailsAsync(url, maxDepth);

        Assert.That(emails, Is.EquivalentTo(expectedEmails));
    }
}