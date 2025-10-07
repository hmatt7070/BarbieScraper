using BarbieDataScraper;

class BarbieMain
{
    static async Task Main(string[] args)
    {
        string searchTerm = "17313"; // doll SKU
        BarbieFinder barbieFinder = new BarbieFinder();
        //string baseRequest = "t=&sBuscar=";
        
        BarbieDoll? barbieInformation = await barbieFinder.FindBarbieDoll(searchTerm);
        
    }
}
