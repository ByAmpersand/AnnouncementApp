using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AnnouncementBackend.Entities
{
    public class Announcement
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
