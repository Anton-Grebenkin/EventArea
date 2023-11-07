using System.Text.Json.Serialization;

namespace KudaGo.Application.Common.ApiClient.Dto
{
    public class Event
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("publication_date")]
        public long PublicationDate { get; set; }

        [JsonPropertyName("dates")]
        public ICollection<Date> Dates { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("place")]
        public Place Place { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("images")]
        public List<ImageInfo> Images { get; set; }

        [JsonPropertyName("site_url")]
        public string SiteUrl { get; set; }

        [JsonPropertyName("categories")]
        public ICollection<string> Categories { get; set; }
    }

    public class Date
    {
        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("end")]
        public long End { get; set; }
    }

    public class ImageInfo
    {
        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("source")]
        public Source Source { get; set; }
    }

    public class Place
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public class Source
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
