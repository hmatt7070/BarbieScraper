using System.Text.Json.Serialization;

namespace BarbieDataScraper.Models
{
    internal class Aspect
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
    internal class EbayDTO
    {
        [JsonPropertyName("keywords")]
        public string Keywords { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("excluded_keywords")]
        public string ExcludedKeywords { get; set; }

        [JsonPropertyName("max_search_results")]
        public string MaxSearchResults { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("remove_outliers")]
        public string RemoveOutliers { get; set; }

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("max_pages")]
        public string MaxPages { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("aspects")]
        public List<Aspect>? Aspects { get; set; }
    }
}
