using CsvHelper;
using CsvHelper.Configuration;
using ShellProgressBar;
using System.Globalization;

namespace BarbieDataScraper.Services
{
    internal class FileManipulation
    {
        public FileManipulation() { }

        public async Task ParseBarbiesFromSkuFile(string readPath, string writePath, bool displayProgressBar)
        {
            var searchTerms = new List<string>();
            BarbieDataScraper barbieDataScraper = new BarbieDataScraper();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            //read SKUs from csv file 
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
            async Task writeFromSearchTerms(List<string> searchTerms, Action<string> reportProgress)
            {
                foreach (var searchTerm in searchTerms)
                {
                    var barbieDoll = await barbieDataScraper.FindBarbieDoll(searchTerm);
                    barbieDolls.Add(barbieDoll);
                    reportProgress($"Processed barbie {searchTerm}");
                    //small delay to help server load
                    await Task.Delay(2500);
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
                    await writeFromSearchTerms(searchTerms, pbar.Tick);
                }
            } else { 
                await writeFromSearchTerms(searchTerms, _ => { }); 
            }
        }
    }
}
