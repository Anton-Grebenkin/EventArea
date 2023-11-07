using Newtonsoft.Json;

namespace KudaGo.Application.Common.Messages
{
    public class CallbackData
    {
        [JsonProperty("t")]
        public CallbackType CallbackType  { get; set; }
        [JsonProperty("d")]
        public string Data { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static CallbackData FromJsonString(string json)
        {
            return JsonConvert.DeserializeObject<CallbackData>(json);
        }
    }

    public enum CallbackType
    {
        CitySelection = 0,
        ChangeCity = 1,
        SelectCategories = 2
    } 
}
