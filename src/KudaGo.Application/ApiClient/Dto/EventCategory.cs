using Newtonsoft.Json;

namespace KudaGo.Application.ApiClient.Dto
{
    public class EventCategory
    {
        [JsonProperty("slug")]
        public string Slug { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }
    }
}
