using System;
using AnnouncementBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnnouncementBackend.Data
{
    public class AnnouncementContext: DbContext
    {
        public AnnouncementContext(DbContextOptions<AnnouncementContext> options) : base(options) { }
        public DbSet<Announcement> Announcements { get; set; }
    }
}
