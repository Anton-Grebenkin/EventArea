using Newtonsoft.Json;

namespace KudaGo.Application.ApiClient.Dto
{
    public record City
    {
        [JsonProperty("slug")]
        public string Slug { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }
    }
}
