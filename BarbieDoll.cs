namespace BarbieDataScraper;

public class BarbieDoll
{
    public string?  Name {get; set;}
    public string? ReleaseDate {get; set;}
    public string? Sku { get; set;}
    public string?  Edition  {get; set;}
    public string? Collection {get; set;}
    public string? Classification {get; set;}
    public string? Category {get; set;}
    
    public BarbieDoll()
    {
        
    }

    /*
    public BarbieDoll(bool isNull)
    {
        if (isNull)
        {
            Name = "";
            ReleaseDate = "";
            Sku = "";
            Edition = "";
            Collection = "";
            Classification = "";
            Category = "";
        }
    }*/
}