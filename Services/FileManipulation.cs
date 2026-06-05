using BarbieDataScraper.Models;
using CsvHelper;
using CsvHelper.Configuration;
using ShellProgressBar;
using System.Globalization;

namespace BarbieDataScraper.Services
{
    public class FileManipulation
    {
        public static async Task ParseBarbiesFromSkuCsv(string readPath, string writePath, bool displayProgressBar, int delay)
        {
            var searchTerms = new List<string>();
            BarbieDataScraper barbieDataScraper = new BarbieDataScraper();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            //read MPN from csv file 
            using (var reader = new StreamReader(readPath))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    string searchTerm = csv.GetField<string>(0);
                    searchTerms.Add(searchTerm);
                }
            }

            //implement a progress bar and search for barbie dolls
            var barbieDolls = new List<BarbieDoll>();
            async Task writeFromSearchTerms(List<string> searchTerms, Action<string> reportProgress, int delay)
            {
                foreach (var searchTerm in searchTerms)
                {
                    var barbieDoll = await barbieDataScraper.FindBarbieDoll(searchTerm);
                    barbieDolls.Add(barbieDoll);
                    reportProgress($"Processed barbie {searchTerm}");
                    if (delay > 0) await Task.Delay(delay);//small delay to help server load
                    
                }
            }

            if (displayProgressBar is true)
            {
                var options = new ProgressBarOptions
                {
                    ProgressCharacter = '-',
                    ProgressBarOnBottom = true
                };

                using (var pbar = new ProgressBar(searchTerms.Count, "Searching for dolls...", options))
                {
                    await writeFromSearchTerms(searchTerms, pbar.Tick, delay);
                }
            } else { 
                await writeFromSearchTerms(searchTerms, _ => { }, delay); 
            }

            using (var writer = new StreamWriter(writePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) {HasHeaderRecord = true}))
            {
                csv.WriteRecords(barbieDolls);
            }
        }

        public static async Task<List<BarbieDoll>> ImportBarbiesFromCSV(string fileName)
        {
            var barbieList = new List<BarbieDoll>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = args => Console.WriteLine(args),
                MissingFieldFound = args => Console.WriteLine(args),
            };

            //read MPN from csv file 
            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    BarbieDoll doll = csv.GetRecord<BarbieDoll>();
                    barbieList.Add(doll);
                }
            }

            return barbieList;
        }

        public static async Task WriteFromList(List<BarbieDoll> barbieDolls, string writePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = args => Console.WriteLine(args),
                MissingFieldFound = args => Console.WriteLine(args),
            };
            using (var writer = new StreamWriter(writePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(barbieDolls);
            }
        }
    }
}
