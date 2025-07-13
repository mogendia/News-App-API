namespace NewsApp.Data
{
    public class NewsDbContext : IdentityDbContext<ApplicationUser>
    {
        public NewsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Section> Sections { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Ad> Ads { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Section>().HasData(
                new Section { Id = 1, Name = "Entertainment Variety" },
                new Section { Id = 2, Name = "News" },
                new Section { Id = 3, Name = "Economics" },
                new Section { Id = 4, Name = "Accidents" },
                new Section { Id = 5, Name = "sports" },
                new Section { Id = 6, Name = "Real Estate" }
            );
        }


    }
}
