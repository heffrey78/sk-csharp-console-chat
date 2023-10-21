using System.Web;
using Microsoft.SemanticKernel.SkillDefinition;
using HtmlAgilityPack;

public class GoogleSkill
{
    [SKFunction]
    public async Task<List<(string, string)>> SearchAsync(string prompt)
    {
        var results = new List<(string, string)>();
        try
        {
            // Build the search query URL
            var query = HttpUtility.UrlEncode(prompt);
            var url = $"https://www.google.com/search?q={query}";

            // Send the request to Google and parse the HTML response
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Extract the search results from the HTML
            var searchResults = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("g")).ToList();

            // Extract the site descriptions and URLs from the search results
            foreach (var result in searchResults)
            {
                var titleNode = result.Descendants("h3").FirstOrDefault();
                var title = titleNode?.InnerText.Trim();
                var linkNode = result.Descendants("a").FirstOrDefault();
                var link = linkNode?.GetAttributeValue("href", "").Trim();
                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(link))
                {
                    results.Add((title, link));
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any errors
            Console.WriteLine($"Error searching Google: {ex.Message}");
        }

        return results;
    }
}