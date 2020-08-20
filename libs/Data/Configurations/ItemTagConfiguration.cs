using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vic.Data.Entities;

namespace Vic.Data.Configurations
{
    /// <summary>
    /// ItemTagConfiguration class, provides a way to configure item tags in the database.
    ///</summary>
    public class ItemTagConfiguration : BaseEntityConfiguration<ItemTag>
    {
        #region Methods
        public override void Configure(EntityTypeBuilder<ItemTag> builder)
        {
            builder.ToTable("ItemTags");

            builder.HasKey(m => new { m.ItemId, m.TagId });
            builder.Property(m => m.ItemId).ValueGeneratedNever();
            builder.Property(m => m.TagId).ValueGeneratedNever().HasMaxLength(25);

            builder.HasOne(m => m.Item).WithMany(m => m.Tags).HasForeignKey(m => m.ItemId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(m => m.Tag).WithMany(m => m.Items).HasForeignKey(m => m.TagId).OnDelete(DeleteBehavior.NoAction);

            base.Configure(builder);
        }
        #endregion
    }
}
