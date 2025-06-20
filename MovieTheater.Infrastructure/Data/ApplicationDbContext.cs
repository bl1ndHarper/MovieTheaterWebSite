using Microsoft.EntityFrameworkCore;
using MovieTheater.Infrastructure.Entities;

namespace MovieTheater.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<User> Users => Set<User>();
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        modelBuilder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });
    }
}
