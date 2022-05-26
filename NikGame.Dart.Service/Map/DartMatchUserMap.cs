using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NikGame.Dart.Service
{
    public class DartMatchUserMap : IEntityTypeConfiguration<DartMatchUser>
    {
        public void Configure(EntityTypeBuilder<DartMatchUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("DartMatchUsers");

            builder.HasOne(x => x.User)
                .WithMany(x => x.DartMatchUsers)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.DartMatch)
                .WithMany(x => x.DartMatchUsers)
                .HasForeignKey(x => x.MatchId);

        }
    }
}