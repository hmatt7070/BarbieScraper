using BarbieDataScraper.Models;
using BarbieDataScraper.Services;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        var dolls = await FileManipulation.ImportBarbiesFromCSV("Data/ParsedBarbies.csv");
        List<BarbieDoll> newDolls = new();
        EbayAPI api = new EbayAPI();
        foreach (var item in dolls)
        {
            newDolls.Add(await api.GetPriceOfBarbie(item, true));
        }
        await FileManipulation.WriteFromList(newDolls, "NewDolls.csv");
    }
}
