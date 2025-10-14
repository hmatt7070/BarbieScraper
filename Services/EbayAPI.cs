using BarbieDataScraper.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BarbieDataScraper.Services
{
    public class EbayAPI
    {
        private static readonly HttpClient client = new HttpClient();
        public string MaxSearchResults { get; set; } = "60";
        public string CategoryId { get; set; } = "262346";
        public string RemoveOutliers { get; set; } = "true";
        public string SiteId { get; set; } = "0";
        public string MaxPages { get; set; } = "1";

        public async Task<string> GetPriceOfBarbie(BarbieDoll barbie, bool useAspects)
        {
            Console.WriteLine("Searching Prices...");//logging
            //configuring data to be inlcuded in the body
            var requestData = new EbayDTO
            {
                Keywords = $"{barbie.Name} {barbie.ReleaseDate}",
                MaxSearchResults = this.MaxSearchResults,
                CategoryId = this.CategoryId,
                RemoveOutliers = this.RemoveOutliers,
                SiteId = this.SiteId,
                MaxPages = this.MaxPages
            };
            //optional narrowing of the search based on the ebay description of the item
            if (useAspects)
            {
                var aspects = new List<Aspect>();
                if (!string.IsNullOrEmpty(barbie.Mpn)) {
                    new Aspect { Name = "Mpn", Value = barbie.Mpn };
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

            //the body of the post request 
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

            //the result of the post request
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Searching Complete");//logging
                return body;
            }
        }
    }
}
