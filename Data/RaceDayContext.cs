

using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Result> Results { get; set; } = null!;
        public DbSet<RaceDayAPI.Models.Route> Routes { get; set; } = null!;
        public DbSet<Weather> Weather { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.UserID);
                entity.Property(u => u.UserID).ValueGeneratedNever();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");
                entity.HasKey(e => e.EventID);
                entity.Property(e => e.EventID).ValueGeneratedNever();

                entity.HasOne(e => e.Organiser)
                    .WithMany(u => u.OrganisedEvents)
                    .HasForeignKey(e => e.OrganiserID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.CategoryID);
                entity.Property(c => c.CategoryID).ValueGeneratedNever();

                entity.HasIndex(c => new { c.EventID, c.CategoryName })
                    .IsUnique();

                entity.HasOne(c => c.Event)
                    .WithMany(e => e.Categories)
                    .HasForeignKey(c => c.EventID);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrolments");
                entity.HasKey(e => e.EnrollmentID);

                entity.Property(e => e.EnrollmentID)
                    .HasColumnName("EnrolmentID")
                    .ValueGeneratedNever();

                entity.Property(e => e.EnrollmentDate)
                    .HasColumnName("EnrolmentDate")
                    .HasDefaultValueSql("SYSDATETIME()");

                entity.HasIndex(e => new { e.ParticipantID, e.EventID })
                    .IsUnique();

                entity.HasOne(e => e.Event)
                    .WithMany(e => e.Enrollments)
                    .HasForeignKey(e => e.EventID);

                entity.HasOne(e => e.Participant)
                    .WithMany(u => u.Enrollments)
                    .HasForeignKey(e => e.ParticipantID);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CategoryID);
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable("Results");
                entity.HasKey(r => r.ResultID);
                entity.Property(r => r.ResultID).ValueGeneratedNever();

                entity.Property(r => r.EnrollmentID)
                    .HasColumnName("EnrolmentID");

                entity.HasIndex(r => r.EnrollmentID).IsUnique();

                entity.HasOne(r => r.Enrollment)
                    .WithOne(e => e.Result)
                    .HasForeignKey<Result>(r => r.EnrollmentID);
            });

            modelBuilder.Entity<Weather>(entity =>
            {
                entity.ToTable("Weather");
                entity.HasKey(w => w.WeatherID);
                entity.Property(w => w.WeatherID).ValueGeneratedNever();

                entity.HasOne(w => w.Event)
                    .WithOne(e => e.Weather)
                    .HasForeignKey<Weather>(w => w.EventID);
            });

            modelBuilder.Entity<RaceDayAPI.Models.Route>(entity =>
            {
                entity.ToTable("Routes");
                entity.HasKey(r => r.RouteID);
                entity.Property(r => r.RouteID).ValueGeneratedNever();

                entity.HasOne(r => r.Event)
                    .WithOne(e => e.Route)
                    .HasForeignKey<RaceDayAPI.Models.Route>(r => r.EventID);
            });
        }
    }
}

