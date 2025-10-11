
using AngleSharp.Common;
using AngleSharp.Html.Construction;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace BarbieDataScraper.Services
{
    internal class Aspect
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    internal class EbayDTO
    {
        [JsonPropertyName("keywords")]
        public string Keywords { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("excluded_keywords")]
        public string? ExcludedKeywords { get; set; }

        [JsonPropertyName("max_search_results")]
        public string MaxSearchResults { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("remove_outliers")]
        public string? RemoveOutliers { get; set; }

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("max_pages")]
        public string? MaxPages { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("aspects")]
        public List<Aspect>? Aspects { get; set; }
    }
    public class EbayAPI
    {
        private static readonly HttpClient client = new HttpClient();
        public string MaxSearchResults { get; set; } = "50";
        public string CategoryId { get; set; } = "262346";
        public string RemoveOutliers { get; set; } = "true";
        public string SiteId { get; set; } = "0";
        public string MaxPages { get; set; } = "1";

        public async Task<string> GetPriceOfBarbie(BarbieDoll barbie, bool useAspects)
        {
            var requestData = new EbayDTO
            {
                Keywords = $"{barbie.Name} {barbie.ReleaseDate}",
                MaxSearchResults = this.MaxSearchResults,
                CategoryId = this.CategoryId,
                RemoveOutliers = this.RemoveOutliers,
                SiteId = this.SiteId,
                MaxPages = this.MaxPages
            };
            if (useAspects)
            {
                var aspects = new List<Aspect>();
                if (!string.IsNullOrEmpty(barbie.Sku)) {
                    new Aspect { Name = "MPN", Value = barbie.Sku };
                }
                if (!string.IsNullOrEmpty(barbie.ReleaseDate))
                {
                    new Aspect { Name = "Year Manufactured", Value = barbie.ReleaseDate };
                }
                if (aspects.Count > 0)
                {
                    requestData.Aspects = aspects;
                }
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://ebay-average-selling-price.p.rapidapi.com/findCompletedItems"),
                Headers =
                {
                    { "x-rapidapi-key", "***REMOVED***" },
                    { "x-rapidapi-host", "ebay-average-selling-price.p.rapidapi.com" },
                },
                Content = new StringContent(JsonSerializer.Serialize(requestData))
                {
                    Headers =

                    {
                        ContentType = new MediaTypeHeaderValue("application/json")

                    }
                }
            };
            Console.WriteLine(requestData.ToDictionary());
            Console.WriteLine(JsonSerializer.Serialize(requestData));
            Console.WriteLine(request);

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
                return body;

            }
        }
    }
}
