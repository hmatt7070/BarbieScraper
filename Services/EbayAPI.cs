using BarbieDataScraper.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BarbieDataScraper.Services
{
    public class EbayAPI
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string? _apiKey = Environment.GetEnvironmentVariable("API_KEY");
        public int maxRetries { get; set; } = 2;
        public string MaxSearchResults { get; set; } = "60";
        public string CategoryId { get; set; } = "262346";
        public string RemoveOutliers { get; set; } = "true";
        public string SiteId { get; set; } = "0";
        public string MaxPages { get; set; } = "1";

        /// <summary>
        /// Searches ebay sold listings and determines the average price and the median price the barbie was sold for 
        /// </summary>
        /// <param name="barbie"></param>
        /// <param name="useAspects"></param>
        /// <returns></returns>
        public async Task<BarbieDoll> GetPriceOfBarbie(BarbieDoll barbie, bool useAspects)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("API Key not set.");
                return barbie;
            }

            //Console.WriteLine("Searching Prices...");//logging
            //configuring data to be included in the body
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
                    aspects.Add(new Aspect { Name = "MPN", Value = barbie.Mpn});
                }
                if (!string.IsNullOrEmpty(barbie.ReleaseDate))
                {
                    aspects.Add(new Aspect { Name = "Year Manufactured", Value = barbie.ReleaseDate });
                }
                if (aspects.Count > 0)
                {
                    requestData.Aspects = aspects;
                }
            }

            int attempt = 0;

            while (attempt < maxRetries)
            {
                //the body of the post request 
                using var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://ebay-average-selling-price.p.rapidapi.com/findCompletedItems"),
                    Headers =
                    {
                        { "x-rapidapi-key", _apiKey },
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

                try
                {
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var body = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("Searching Complete");//logging
                        if (!String.IsNullOrEmpty(barbie.Name))
                        {
                            var jResult = JsonNode.Parse(body);
                            if (jResult != null)
                            {
                                // 1. ?[1] safely accesses the second element
                                // 2. ?.GetValueKind() gets its type (e.g., Object, Null)
                                // 3. ?? JsonValueKind.Null handles cases where "warning" or [1] is missing
                                // 4. != JsonValueKind.Null checks if it's anything *other* than null
                                bool hasErrorWarning = (jResult["warning"]?[1]?.GetValueKind() ?? JsonValueKind.Null) !=
                                                       JsonValueKind.Null;

                                if (hasErrorWarning)
                                {
                                    // It was ["warning", {...}], so an error occurred.
                                    barbie.MedianPrice = "Error";
                                    barbie.AveragePrice = "Error";
                                }
                                else if (jResult["success"].GetValue<bool>() == true)
                                {
                                    barbie.MedianPrice = jResult["median_price"].GetValue<float>().ToString();
                                    barbie.AveragePrice = jResult["average_price"].GetValue<float>().ToString();
                                }
                            }
                        }

                        return barbie;
                    }
                }
                catch (HttpRequestException ex)
                {
                    attempt++;
                    Console.WriteLine(ex.ToString());
                    if (attempt < maxRetries)
                    {
                        await Task.Delay(1500 * attempt);
                    }
                }
            }
            barbie.MedianPrice = "Error";
            barbie.AveragePrice = "Error";
            return barbie;
        }
    }
}
