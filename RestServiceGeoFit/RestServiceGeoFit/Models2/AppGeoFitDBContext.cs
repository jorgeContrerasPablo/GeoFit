namespace RestServiceGeoFit.Models2
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AppGeoFitDBContext : DbContext
    {
        public AppGeoFitDBContext(string stringConnectionDB)
            : base(stringConnectionDB)
        {
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<FeedBack> FeedBacks { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Joined> Joineds { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Sport> Sports { get; set; }
        public virtual DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedBack>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.Players)
                .WithMany(e => e.Games)
                .Map(m => m.ToTable("Participate").MapLeftKey("GameID").MapRightKey("PlayerID"));

            modelBuilder.Entity<Photo>()
                .Property(e => e.PhotoName)
                .IsUnicode(false);

            modelBuilder.Entity<Photo>()
                .HasMany(e => e.Teams)
                .WithOptional(e => e.Photo)
                .HasForeignKey(e => e.EmblemID);

            modelBuilder.Entity<Place>()
                .Property(e => e.Direction)
                .IsUnicode(false);

            modelBuilder.Entity<Place>()
                .Property(e => e.PlaceName)
                .IsUnicode(false);

            modelBuilder.Entity<Place>()
                .Property(e => e.PlaceMail)
                .IsUnicode(false);

            modelBuilder.Entity<Place>()
                .Property(e => e.Link)
                .IsUnicode(false);

            modelBuilder.Entity<Place>()
                .HasMany(e => e.FeedBacks)
                .WithOptional(e => e.Place)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Place>()
                .HasMany(e => e.Teams)
                .WithMany(e => e.Places)
                .Map(m => m.ToTable("PlaceTeam").MapLeftKey("PlaceID").MapRightKey("TeamID"));

            modelBuilder.Entity<Player>()
                .Property(e => e.Password)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.PlayerNick)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.PlayerName)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.PlayerMail)
                .IsUnicode(false);

            modelBuilder.Entity<Sport>()
                .Property(e => e.SportName)
                .IsUnicode(false);

            modelBuilder.Entity<Sport>()
                .HasMany(e => e.Players)
                .WithOptional(e => e.Sport)
                .HasForeignKey(e => e.FavoriteSportID);

            modelBuilder.Entity<Team>()
                .Property(e => e.TeamName)
                .IsUnicode(false);

            modelBuilder.Entity<Team>()
                .Property(e => e.ColorTeam)
                .IsUnicode(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Games)
                .WithRequired(e => e.Team)
                .HasForeignKey(e => e.Team1ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.Games1)
                .WithRequired(e => e.Team1)
                .HasForeignKey(e => e.Team2ID)
                .WillCascadeOnDelete(false);
        }
    }
}
