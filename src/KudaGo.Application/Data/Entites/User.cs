using KudaGo.Application.ApiClient.Dto;


namespace KudaGo.Application.Data.Entites
{
    public class User
    {
        public long Id { get; set; }
        public string City { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> RequiredCommands { get; set; }
        public IEnumerable<string> PreferredEventCategories { get; set; }
    }
}
