using System.Text.Json.Serialization;

namespace KudaGo.Application.Common.ApiClient.Dto
{
    public class GetEventsResult
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }

        [JsonPropertyName("previous")]
        public string Previous { get; set; }

        [JsonPropertyName("results")]
        public IEnumerable<Event> Results { get; set; }
    }
}
