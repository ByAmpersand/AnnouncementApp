using Microsoft.AspNetCore.Mvc;
using AnnouncementBackend.Data;
using AnnouncementBackend.Entities;
using AnnouncementBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using AnnouncementBackend.Services;

namespace AnnouncementBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnnouncementController : ControllerBase
    {
        private readonly AnnouncementContext _context;
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(AnnouncementContext context, IAnnouncementService announcementService)
        {
            _context = context;
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementDTO>>> GetAllAnnouncements()
        {
            var announcements = await _context.Announcements.
                OrderByDescending(a => a.DateAdded).
                Select(a => new AnnouncementDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DateAdded = DateTime.SpecifyKind(a.DateAdded, DateTimeKind.Utc)
                }).ToListAsync();


            return Ok(announcements);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AnnouncementDTO>> GetAnnouncementById(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
                return NotFound();

            var dto = new AnnouncementDTO
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Description = announcement.Description,
                DateAdded = DateTime.SpecifyKind(announcement.DateAdded, DateTimeKind.Utc)
            };

            return Ok(dto);
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<AnnouncementDetailsDTO>> GetAnnouncementWithSimilar(int id)
        {
            var result = await _announcementService.GetAnnouncementWithSimilarAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AnnouncementDTO>> PostAnnouncement(CreateAnnouncementDTO createDto)
        {
            var announcement = new Announcement
            {
                Title = createDto.Title,
                Description = createDto.Description,
                DateAdded = DateTime.UtcNow
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            var returnDto = new AnnouncementDTO
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Description = announcement.Description,
                DateAdded = announcement.DateAdded
            };

            return CreatedAtAction(nameof(GetAnnouncementById), new { id = announcement.Id }, returnDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnouncement(int id, UpdateAnnouncementDTO updateDto)
        {
            var existingAnnouncement = await _context.Announcements.FindAsync(id);

            if (existingAnnouncement == null)
            {
                return NotFound();
            }

            existingAnnouncement.Title = updateDto.Title;
            existingAnnouncement.Description = updateDto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!AnnouncementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcements.Any(e => e.Id == id);
        }
    }
}