using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ShellProgressBar;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        var searchTerms = new List<string>(); 
        BarbieFinder barbieFinder = new BarbieFinder();
        
        string inputFilePath = "Barbie Dolls.csv";

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
         HasHeaderRecord = false
        };

        //read SKUs from csv file 
        using (var reader = new StreamReader(inputFilePath))
        using (var csv = new CsvReader(reader, config))
        {
         Console.WriteLine("Reading barbies from CSV file.");
         while (csv.Read())
         {
             string searchTerm = csv.GetField<string>(0);
             searchTerms.Add(searchTerm);
         }
        }

        //implement a progress bar and search for barbie dolls
        var barbieDolls = new List<BarbieDoll>();
        var options = new ProgressBarOptions
        {
         ProgressCharacter = '-',
         ProgressBarOnBottom = true
        };

        using (var pbar = new ProgressBar(searchTerms.Count, "Searching for dolls...", options))
        {
        foreach (var searchTerm in searchTerms)
            {
                var barbieDoll = await barbieFinder.FindBarbieDoll(searchTerm);
                barbieDolls.Add(barbieDoll);
                pbar.Tick($"Processed barbie {searchTerm}.");
                //small delay to help server load
                await Task.Delay(2500);
            }
        }

        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string filepath = Path.Combine(projectDirectory, "ParsedBarbieDolls.csv");
        //write barbies to csv file 
        using (var writer = new StreamWriter(filepath))
        using (var csv = new CsvWriter(writer, config))
        {
         Console.WriteLine("Writing barbies to CSV file.");
         csv.WriteRecords(barbieDolls);
        }

        Console.WriteLine("Done");
    }
}
