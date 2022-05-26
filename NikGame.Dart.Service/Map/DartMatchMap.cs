using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NikGame.Dart.Service
{
    public class DartMatchMap : IEntityTypeConfiguration<DartMatch>
    {
        public void Configure(EntityTypeBuilder<DartMatch> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("DartMatches");

            builder.HasOne(x => x.Winer)
                .WithMany(x => x.WinDartMatches)
                .HasForeignKey(x => x.WinerId).IsRequired(false);

        }
    }
}