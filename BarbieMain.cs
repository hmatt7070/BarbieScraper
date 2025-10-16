
using BarbieDataScraper.Models;
using BarbieDataScraper.Services;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        Services.BarbieDataScraper finder = new Services.BarbieDataScraper();
        FileManipulation fm = new FileManipulation();
        await fm.ParseBarbiesFromSkuCsv("Barbie Dolls.csv", "ParseBarbies.csv", true, 1700);

    }
}
