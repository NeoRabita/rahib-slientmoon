using Microsoft.EntityFrameworkCore;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<UserTopic> UserTopics { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Narrator> Narrators { get; set; }
        public DbSet<CourseNarrator> CourseNarrators { get; set; }
        public DbSet<DailyThought> DailyThoughts { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Track> Tracks { get; set; }

        public DbSet<Translation> Translations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
