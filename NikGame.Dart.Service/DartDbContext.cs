using Microsoft.EntityFrameworkCore;
using NikGame.Service;

namespace NikGame.Dart.Service
{
    public class DartDbContext : BaseDbContext, IDartUnitOfWork
    {
        public DartDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }


        public DbSet<AppUser> Users { get; set; }
        public DbSet<DartMatch> DartMatches { get; set; }
        public DbSet<DartShoot> DartMatchSets { get; set; }
        public DbSet<DartMatchUser> DartMatchUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AppUserMap());
            builder.ApplyConfiguration(new DartMatchMap());
            builder.ApplyConfiguration(new DartShootMap());
            builder.ApplyConfiguration(new DartMatchUserMap());
        }
    }
}
