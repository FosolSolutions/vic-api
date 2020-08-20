using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vic.Data.Entities;

namespace Vic.Data.Configurations
{
    /// <summary>
    /// PageConfiguration class, provides a way to configure pages in the database.
    ///</summary>
    public class PageConfiguration : BaseEntityConfiguration<Page>
    {
        #region Methods
        protected void LookupConfigure(EntityTypeBuilder<Page> builder)
        {
            builder.ToTable("Pages");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();


            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Path).IsRequired().HasMaxLength(500);
            builder.Property(m => m.Body).IsRequired();

            base.Configure(builder);
        }

        public override void Configure(EntityTypeBuilder<Page> builder)
        {
            LookupConfigure(builder);
        }
        #endregion
    }
}
