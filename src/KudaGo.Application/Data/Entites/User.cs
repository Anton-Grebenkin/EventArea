using KudaGo.Application.ApiClient.Dto;


namespace KudaGo.Application.Data.Entites
{
    public class User
    {
        public long Id { get; set; }
        public string City { get; set; }
        public string UserName { get; set; }
        public ICollection<string> PreferredEventCategories { get; set; }

        public User() 
        {
            PreferredEventCategories = new List<string>();
        }
    }
}
