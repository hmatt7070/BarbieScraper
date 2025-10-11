
using BarbieDataScraper.Services;
using System.Text.Json;

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

        Console.WriteLine(JsonSerializer.Serialize(result));

    }
}
