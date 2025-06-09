using System.ComponentModel.DataAnnotations;

namespace AnnouncementBackend.DTOs
{
    public class AnnouncementDetailsDTO
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public List<AnnouncementDTO> SimilarAnnouncements { get; set; }
    }
}
