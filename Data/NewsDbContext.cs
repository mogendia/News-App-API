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
                new Section { Id = 1, Name = "zayed1" },
                new Section { Id = 2, Name = "zayed2" },
                new Section { Id = 3, Name = "water company" },
                new Section { Id = 4, Name = "electricity company" },
                new Section { Id = 5, Name = "post" },
                new Section { Id = 6, Name = "city authority" }
            );
        }


    }
}
