namespace BarbieDataScraper;

public class BarbieDoll
{
    public string  Name {get; set;}
    public string ReleaseDate {get; set;}
    public string Sku { get; set;}
    public string?  Edition  {get; set;}
    public string? Collection {get; set;}
    public string? Classification {get; set;}


    public BarbieDoll()
    {
        this.Name = "";
        this.ReleaseDate = "";
        this.Sku = "0";
    }
    public BarbieDoll(string name, string releaseDate, string sku, string edition, string collection, string classification)
    {
        Name = name;
        ReleaseDate = releaseDate;
        Sku = sku; 
        Edition = edition;
        Collection = collection;
        Classification = classification;
    }
}