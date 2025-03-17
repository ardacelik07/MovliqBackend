namespace RunningApplicationNew.DataLayer
{
    using Microsoft.EntityFrameworkCore;
    using RunningApplicationNew.Entity;
   

   
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }

            // Kullanıcı tablosu
            public DbSet<User> Users { get; set; }
            public DbSet<RaceRoom> RaceRooms { get; set; }

            public DbSet<UserRaceRoom> UserRaceRooms { get; set; }

            public DbSet<UserResults> UserResults { get; set; }
            public DbSet<LeaderBoard> LeaderBoards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // User tablosu için ek yapılandırmalar
                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email) // Email alanı için benzersiz index
                    .IsUnique();
                 modelBuilder.Entity<UserRaceRoom>()
                    .HasKey(ur => new { ur.UserId, ur.RaceRoomId });

            // Varsayılan değerler veya diğer kurallar burada eklenebilir

            // User ve LeaderBoard arasında birebir ilişki tanımlama
            modelBuilder.Entity<User>()
                .HasOne(u => u.LeaderBoard)
                .WithOne(lb => lb.User)
                .HasForeignKey<LeaderBoard>(lb => lb.UserId);
        }
        }
}
