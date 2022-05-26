using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NikGame.Dart.Service
{
    public class DartShootMap : IEntityTypeConfiguration<DartShoot>
    {
        public void Configure(EntityTypeBuilder<DartShoot> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("DartShoots");

            builder.HasOne(x => x.User)
                .WithMany(x => x.DartShoots)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.DartMatch)
                .WithMany(x => x.DartShoots)
                .HasForeignKey(x => x.MatchId);

            builder.HasOne(x => x.MatchUser)
                .WithMany(x => x.DartShoots)
                .HasForeignKey(x => x.MatchUserId);

        }
    }
}