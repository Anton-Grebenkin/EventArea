using System.Text.Json.Serialization;

namespace KudaGo.Application.Common.ApiClient.Dto
{
    public record City
    {
        [JsonPropertyName("slug")]
        public string Slug { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }
    }
}
