using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Z.Infrastructure.EFCore;
using Z.Listening.Domain.Entities;

namespace Z.Listening.Infrastructure.Configs
{
    class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("T_Categories");
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.OwnsOneMultilingualString(e => e.Name);
            builder.Property(e => e.CoverUrl).IsRequired(false).HasMaxLength(500).IsUnicode();
        }
    }
}
