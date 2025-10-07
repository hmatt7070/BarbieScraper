using AngleSharp;
using AngleSharp.Dom;

namespace BarbieDataScraper;

public class BarbieFinder
{
    private readonly HttpClient _client;
    private readonly IBrowsingContext _context;
    private record BarbieHtmlChunks(IElement? NameElement, IElement? TableElement);

    public BarbieFinder()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://en.barbiepedia.com/");
        _context = BrowsingContext.New(Configuration.Default);
    }

    public async Task<BarbieDoll?> FindBarbieDoll(string barbieSKU)
    {
        return await ParseBarbieFromHtml(barbieSKU);
    }

    private async Task<BarbieDoll?>ParseBarbieFromHtml(string barbieSKU)
    {
        var barbieHtmlChunks = await GetBarbieDollInformation(barbieSKU);
        
        if (barbieHtmlChunks.NameElement.OuterHtml == null || barbieHtmlChunks.TableElement.OuterHtml == null)
        {
            return null;
        }
        
        BarbieDoll? barbieDoll = new BarbieDoll();
        
        barbieDoll.Sku = barbieSKU;
        barbieDoll.Name = barbieHtmlChunks.NameElement.InnerHtml;
        //TODO PARSE DOLL INFORMATION
        
        barbieDoll.ReleaseDate = "";

        return barbieDoll;
    }
    
    private async Task<string?> GetCatalogPage(string barbieSKU)
    {
        var payload = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "t", "" },
                { "sBuscar", $"{barbieSKU}" }
            }
        );

        //Sends a post request containing the search term, and then returns the first product link on the page containing the search term
        try
        {
            //post request
            HttpResponseMessage productSearchPostResponse = await _client.PostAsync("barbie/buscar/", payload);
            productSearchPostResponse.EnsureSuccessStatusCode();

            //
            var _context = BrowsingContext.New(Configuration.Default);
            await using var searchResponseString = await productSearchPostResponse.Content.ReadAsStreamAsync();
            IDocument searchResultsDocument = await _context.OpenAsync(req => req.Content(searchResponseString));

            //finds first <a href> tag in the product-item class
            var productAnchorTag = searchResultsDocument.QuerySelector(".product-item a:nth-of-type(1)");

            //assigns either null for no result, or the link for the item 
            return productAnchorTag?.GetAttribute("href");
        }
        catch (HttpRequestException exception)
        {
            return null;
        }
    }

    
    
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
                
                //TODO FIX RETURNS EMPTY 
                var productTableInformation =  document.QuerySelector("table:nth-of-type(1)");
                
                return new BarbieHtmlChunks(dollName, productTableInformation);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}