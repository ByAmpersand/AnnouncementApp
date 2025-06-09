using AnnouncementBackend.DTOs;

namespace AnnouncementBackend.Services
{
    public interface IAnnouncementService
    {
        Task<AnnouncementDetailsDTO> GetAnnouncementWithSimilarAsync(int id);
    }
}