using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vic.Data.Entities;

namespace Vic.Data.Configurations
{
    /// <summary>
    /// TagConfiguration class, provides a way to configure tags in the database.
    ///</summary>
    public class TagConfiguration : BaseEntityConfiguration<Tag>
    {
        #region Methods
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever().HasMaxLength(25);

            base.Configure(builder);
        }
        #endregion
    }
}
