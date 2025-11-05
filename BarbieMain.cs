
using BarbieDataScraper.Models;
using BarbieDataScraper.Services;
using CsvHelper;
using CsvHelper.Configuration;
using ShellProgressBar;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        List<BarbieDoll> barbies =  await FileManipulation.ImportBarbiesFromCSV("Data/ParsedBarbies.csv");

        var options = new ProgressBarOptions
        {
            ProgressCharacter = '-',
            ProgressBarOnBottom = true,
            ForegroundColor = ConsoleColor.Yellow
        };

        EbayAPI ebayAPI = new EbayAPI();
        bool isUsingAspects = true;

        using var pbar = new ProgressBar(barbies.Count,"Finding prices for dolls...", options);
        foreach (var barbie in barbies)
        {
            if (!String.IsNullOrEmpty(barbie.Name))
            {                
                var jResult = JsonNode.Parse(await ebayAPI.GetPriceOfBarbie(barbie, isUsingAspects));
                if (jResult != null)
                {
                    // 1. ?[1] safely accesses the second element
                    // 2. ?.GetValueKind() gets its type (e.g., Object, Null)
                    // 3. ?? JsonValueKind.Null handles cases where "warning" or [1] is missing
                    // 4. != JsonValueKind.Null checks if it's anything *other* than null
                    bool hasErrorWarning = (jResult["warning"]?[1]?.GetValueKind() ?? JsonValueKind.Null) != JsonValueKind.Null;

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
                    pbar.Tick($"Processed {barbie.Name}. Average Price: {barbie.AveragePrice} Median Price: {barbie.MedianPrice}");
                }
                else { pbar.Tick("Error with API search."); }
            }
            else
            {
                pbar.Tick($"Barbie {barbie.Mpn} is empty.");
            }
        }

        using var writer = new StreamWriter("UpdatedBarbies.csv");
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
        {
            csv.WriteRecords(barbies);
        }
    }
}
