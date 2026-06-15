using BarbieDataScraper.Services;
using DotNetEnv;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        Env.Load();
        
        await FileManipulation.FindBarbiesUsingMpn("Data/Barbie Dolls.csv",
            "Data/ParsedBarbies.csv", true,
            0);
        var dolls = await FileManipulation.ImportBarbiesFromCSV("Data/ParsedBarbies.csv");
        
        /*
        EbayApi api = new EbayApi();
        foreach (var item in dolls)
        {
            BarbieDoll barbieDoll = await api.GetPriceOfBarbie(item, true);
            item.AveragePrice = barbieDoll.AveragePrice;
            item.MedianPrice = barbieDoll.MedianPrice;
        }
        */
        
        await FileManipulation.WriteFromList(dolls, "FinalizedDolls.csv");
    }
}
