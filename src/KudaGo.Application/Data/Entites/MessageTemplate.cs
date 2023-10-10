

using MongoDB.Bson;

namespace KudaGo.Application.Data.Entites
{
    public class MessageTemplate
    {
        public ObjectId Id { get; set; }
        public string Text { get; set; }
        public MessageTemplateType MessageTemplateType { get; set; }
    }

    public enum MessageTemplateType
    {
        WelcomeMessage = 0,
        CitySelection = 1
    }
}
