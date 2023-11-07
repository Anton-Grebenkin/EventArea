
namespace KudaGo.Application.Common.Data.Entites
{
    public class Event
    {
        public long Id { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime AddDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<ImageInfo> Images { get; set; } = new List<ImageInfo>();
        public ICollection<string> Categories { get; set; } = new List<string>();
        public string SiteUrl { get; set; }
        public bool Recommended { get; set; } = false;
    }

    public class ImageInfo
    {
        public string Image { get; set; }
    }
}
