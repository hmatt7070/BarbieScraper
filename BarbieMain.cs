
using BarbieDataScraper.Models;
using BarbieDataScraper.Services;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        BarbieDoll barbie = new BarbieDoll();
        Services.BarbieDataScraper finder = new Services.BarbieDataScraper();
        barbie = await finder.FindBarbieDoll("17313");

        /*EbayAPI ebayAPI = new EbayAPI();
        var result = await ebayAPI.GetPriceOfBarbie(barbie, true);
        JsonNode jObject = JsonNode.Parse(result);

        if (jObject != null && jObject["success"]!.GetValue<bool>() == true)
        {
            string averagePrice = jObject["average_price"]!.GetValue<float>().ToString();
            string medianPrice = jObject["median_price"]!.GetValue<float>().ToString();
            string minPrice = jObject["min_price"]!.GetValue<float>().ToString();
            string maxPrice = jObject["max_price"]!.GetValue<float>().ToString();

            Console.WriteLine($"Barbie Name: {barbie.Name}");

            Console.WriteLine($"Average Price: {averagePrice}");
            Console.WriteLine($"Median Price: {medianPrice}");
            Console.WriteLine($"Minimum Price: {minPrice}\nMaximum Price: {maxPrice}");
        }*/
    }
}
