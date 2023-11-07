using System.Text.Json.Serialization;

namespace KudaGo.Application.Common.ApiClient.Dto
{
    public class EventCategory
    {
        [JsonPropertyName("slug")]
        public string Slug { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }
    }
}
