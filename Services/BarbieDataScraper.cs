using AngleSharp;
using AngleSharp.Dom;

namespace BarbieDataScraper.Services;
public class BarbieDataScraper
{
    private readonly HttpClient _client;
    private readonly IBrowsingContext _context;
    private  record BarbieHtmlChunks(IElement? NameElement, IElement? TableBodyElement, IElement? DescriptionElement);

    public BarbieDataScraper()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://en.barbiepedia.com/");
        _context = BrowsingContext.New(Configuration.Default);
    }

    /// <summary>
    /// The entry point for the user to search for a doll
    /// </summary>
    /// <param name="barbieSKU"></param>
    /// <returns>The found doll</returns>
    public async Task<BarbieDoll?> FindBarbieDoll(string barbieSKU)
    {
        barbieSKU = barbieSKU.ToUpper();
        return await ParseBarbieFromHtml(barbieSKU);
    }

    /// <summary>
    /// Parses through the HTML containing the barbie information
    /// </summary>
    /// <param name="barbieSKU"></param>
    /// <returns>The barbie doll with the appropriate information</returns>
    private async Task<BarbieDoll?>ParseBarbieFromHtml(string barbieSKU)
    {
        var barbieHtmlChunks = await GetBarbieDollInformation(barbieSKU);

        BarbieDoll? barbieDoll = new BarbieDoll();
        if (barbieHtmlChunks == null)
        {
            barbieDoll.Sku = barbieSKU;
            return barbieDoll;
        }
        
        barbieDoll.Sku = barbieSKU;
        barbieDoll.Name = barbieHtmlChunks.NameElement.TextContent;

        if (barbieHtmlChunks.DescriptionElement != null) { barbieDoll.Description = barbieHtmlChunks.DescriptionElement.TextContent; }

        //prunes off extra information from the table that isn't doll information
        var dollInformationElements = barbieHtmlChunks.TableBodyElement.QuerySelectorAll
            ("tr:has(td.datasheet-features-type:first-child):not(:has(div))");
        
        //selects the corresponding doll information and assigns it to the doll
        foreach (var dollInformationElement in dollInformationElements)
        {
            switch (dollInformationElement.FirstElementChild.TextContent)
            {
                case "Edition": barbieDoll.Edition = dollInformationElement.LastElementChild.FirstElementChild.TextContent;
                    break;
                case "Collection": barbieDoll.Collection = dollInformationElement.LastElementChild.FirstElementChild.TextContent;
                    break;
                case "Classification": barbieDoll.Classification = dollInformationElement.LastElementChild.FirstElementChild.TextContent;
                    break;
                case "Release Date": barbieDoll.ReleaseDate = dollInformationElement.LastElementChild.FirstElementChild.TextContent;
                    break;
            }
        }
        return barbieDoll;
    }
    
    /// <summary>
    /// Gets the HTML from the correct link. Then, it disects the html into two parts; the header that contains the doll
    /// name, and the table element that contains the rest of the doll information
    /// </summary>
    /// <param name="barbieSKU"></param>
    /// <returns>A record of the Barbie Html Chunks</returns>
    private async Task<BarbieHtmlChunks?> GetBarbieDollInformation(string barbieSKU)
    {
        string? productRelativeLink =  await GetCatalogPage(barbieSKU);
        if (productRelativeLink != null)
        {
            try
            {
                HttpResponseMessage productPageGetResponse = await _client.GetAsync(productRelativeLink);
                productPageGetResponse.EnsureSuccessStatusCode();
                        
                await using var productPageContent = await productPageGetResponse.Content.ReadAsStreamAsync();
                IDocument document = await _context.OpenAsync(req => req.Content(productPageContent));

                //finds first h1 element in .product-page class (which is the doll name)
                var dollName = document.QuerySelector(".product-page h1:nth-of-type(1)");
                //finds first table tag (containing the doll information)
                
                var productTableInformation =  document.QuerySelector("tbody:nth-of-type(1)");

                var description = document.QuerySelector(".description p:nth-of-type(2)");
                
                return new BarbieHtmlChunks(dollName, productTableInformation, description);
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    
    /// <summary>
    /// Searches for the Barbie Doll based on the barbieSKU. It then selects the link on the page containing the sku number. 
    /// </summary>
    /// <param name="barbieSKU"></param>
    /// <returns>Task<string></returns>
    private async Task<string?> GetCatalogPage(string barbieSKU)
    {
        var payload = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "t", "" },
                { "sBuscar", $"{barbieSKU}" }
            }
        );

        //Sends a post request containing the search term, and then returns the link on the page containing the search term
        try
        {
            //post request
            HttpResponseMessage productSearchPostResponse = await _client.PostAsync("barbie/buscar/", payload);
            productSearchPostResponse.EnsureSuccessStatusCode();

            //gets the catalog page information
            await using var searchResponseString = await productSearchPostResponse.Content.ReadAsStreamAsync();
            IDocument searchResultsDocument = await _context.OpenAsync(req => req.Content(searchResponseString));

            //finds the correct link based on if the link contains the sku
            var productAnchorTag = searchResultsDocument.QuerySelector($".product-item a[href*='{barbieSKU}']");

            //assigns either null for no result, or the link for the item 
            return productAnchorTag?.GetAttribute("href");
        }
        catch (HttpRequestException exception)
        {
            return null;
        }
    }
}