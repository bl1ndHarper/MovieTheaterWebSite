using Microsoft.EntityFrameworkCore;
using MovieTheater.Infrastructure.Entities;

namespace MovieTheater.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<HallSector> HallSectors => Set<HallSector>();
    public DbSet<HallSeat> HallSeats => Set<HallSeat>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<MovieStats> MovieStats => Set<MovieStats>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<SessionSeat> SessionSeats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite keys
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        modelBuilder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });

        // Enums
        modelBuilder.Entity<Movie>()
            .Property(m => m.ActivityStatus)
            .HasConversion<string>();

        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();

        modelBuilder.Entity<HallSeat>()
            .Property(b => b.Status)
            .HasConversion<string>();

        // Generated autoincrement IDs
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Movie>()
            .Property(m => m.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Actor>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Genre>()
            .Property(g => g.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Rating>()
            .Property(r => r.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Hall>()
            .Property(h => h.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<HallSector>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<HallSeat>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Session>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Booking>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Recommendation>()
            .Property(r => r.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<MovieStats>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();

        // SessionSeat config
        modelBuilder.Entity<SessionSeat>()
            .HasOne(x => x.Session)
            .WithMany(s => s.SessionSeats)
            .HasForeignKey(x => x.SessionId);

        modelBuilder.Entity<SessionSeat>()
            .HasOne(x => x.HallSeat)
            .WithMany()
            .HasForeignKey(x => x.HallSeatId);

        modelBuilder.Entity<SessionSeat>()
            .Property(x => x.Status)
            .HasConversion<string>();
    }
}
