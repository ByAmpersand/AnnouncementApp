using System.ComponentModel.DataAnnotations;

namespace AnnouncementBackend.DTOs
{
    public class CreateAnnouncementDTO
    {
        [MaxLength(100)]
        public string Title { get; set; } = null!;
        [MaxLength(2000)]
        public string Description { get; set; } = null!;
    }
}
