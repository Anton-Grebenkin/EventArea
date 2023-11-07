
namespace KudaGo.Application.Common.Data.Entites
{
    public class User
    {
        public long Id { get; set; }
        public string City { get; set; }
        public string UserName { get; set; }
        public bool RecommendEvents { get; set; }
        public ICollection<string> PreferredEventCategories { get; set; }

        public User() 
        {
            PreferredEventCategories = new List<string>();
        }
    }
}
