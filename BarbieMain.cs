
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

        EbayAPI ebayAPI = new EbayAPI();
        var result = await ebayAPI.GetPriceOfBarbie(barbie, true);
        JsonNode jObject = JsonNode.Parse(result);

        Console.WriteLine(jObject);

    }
}
