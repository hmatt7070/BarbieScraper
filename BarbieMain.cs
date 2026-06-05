using BarbieDataScraper.Models;
using BarbieDataScraper.Services;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        await FileManipulation.FindBarbiesUsingMpn("Data/Barbie Dolls.csv",
            "Data/ParsedBarbies.csv", true,
            0);
        var dolls = await FileManipulation.ImportBarbiesFromCSV("Data/ParsedBarbies.csv");
        EbayAPI api = new EbayAPI();
        foreach (var item in dolls)
        {
            BarbieDoll barbieDoll = await api.GetPriceOfBarbie(item, true);
            item.AveragePrice = barbieDoll.AveragePrice;
            item.MedianPrice = barbieDoll.MedianPrice;
        }
        await FileManipulation.WriteFromList(dolls, "DollsWithPrice.csv");
    }
}
