using Newtonsoft.Json;

namespace KudaGo.Application.Messages
{
    public class CallbackData
    {
        public CallbackType CallbackType  { get; set; }
        public string? NextCommand { get; set; }
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
    } 
}
