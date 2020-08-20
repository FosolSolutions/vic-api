using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vic.Data.Entities;

namespace Vic.Data.Configurations
{
    /// <summary>
    /// ItemConfiguration class, provides a way to configure items in the database.
    ///</summary>
    public class ItemConfiguration : BaseEntityConfiguration<Item>
    {
        #region Methods
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();

            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Path).IsRequired().HasMaxLength(500);
            builder.Property(m => m.Description).HasMaxLength(500);
            builder.Property(m => m.Author).HasMaxLength(250);
            builder.Property(m => m.ContentType).HasMaxLength(250);

            builder.HasOne(m => m.Parent).WithMany(m => m.Items).HasForeignKey(m => m.ParentId).OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasIndex(m => m.Path).IsUnique(true);
            builder.HasIndex(m => new { m.Author, m.PublishedOn, m.Name });

            base.Configure(builder);
        }
        #endregion
    }
}
