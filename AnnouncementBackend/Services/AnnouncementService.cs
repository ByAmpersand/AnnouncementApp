using AnnouncementBackend.Data;
using AnnouncementBackend.DTOs;
using AnnouncementBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace AnnouncementBackend.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly AnnouncementContext _context;

        public AnnouncementService(AnnouncementContext context)
        {
            _context = context;
        }

        public async Task<AnnouncementDetailsDTO> GetAnnouncementWithSimilarAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return null;

            var wordsInTitle = announcement.Title.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var wordsInDescription = announcement.Description.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var others = await _context.Announcements
                .Where(a => a.Id != id)
                .ToListAsync();

            var similar = others
                .Where(a =>
                {
                    var otherTitleWords = a.Title.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var otherDescriptionWords = a.Description.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    return wordsInTitle.Intersect(otherTitleWords).Any() &&
                           wordsInDescription.Intersect(otherDescriptionWords).Any();
                })
                .OrderByDescending(a => a.DateAdded)
                .Take(3)
                .ToList();

            return new AnnouncementDetailsDTO
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Description = announcement.Description,
                DateAdded = DateTime.SpecifyKind(announcement.DateAdded, DateTimeKind.Utc),
                SimilarAnnouncements = similar.Select(s => new AnnouncementDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    DateAdded = DateTime.SpecifyKind(s.DateAdded, DateTimeKind.Utc)
                }).ToList()
            };
        }

    }
}
