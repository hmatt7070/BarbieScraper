using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace BarbieDataScraper;
public class BarbieMain
{
    static async Task Main(string[] args)
    {
        var searchTerms = new List<string>();
        BarbieFinder barbieFinder = new BarbieFinder();

         string filepath = "Barbie Dolls.csv";

         var config = new CsvConfiguration(CultureInfo.InvariantCulture)
         {
             HasHeaderRecord = false
         };

         //read SKUs from csv file 
         using (var reader = new StreamReader(filepath))
         using (var csv = new CsvReader(reader, config))
         {
             while (csv.Read())
             {
                 string searchTerm = csv.GetField<string>(0);
                 searchTerms.Add(searchTerm);
             }
         }
         var barbieDolls = new List<BarbieDoll>();
         foreach (var searchTerm in searchTerms)
         {
             var barbieDoll = await barbieFinder.FindBarbieDoll(searchTerm);
             barbieDolls.Add(barbieDoll);
             //small delay to help server load
             await Task.Delay(4500);
         }

         //write barbies to csv file 
         using (var writer = new StreamWriter("/home/matt/Documents/SortedBarbieDolls.csv"))
         using (var csv = new CsvWriter(writer, config))
         {
             csv.WriteRecords(barbieDolls);
         }

         Console.WriteLine("Done");
    }
}
