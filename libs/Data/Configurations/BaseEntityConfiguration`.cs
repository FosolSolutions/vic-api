using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vic.Data.Entities;

namespace Vic.Data.Configurations
{
    /// <summary>
    /// BaseEntityConfiguration class, provides a way to configure base entity in the database.
    ///</summary>
    public abstract class BaseEntityConfiguration<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseEntity
    {
        #region Methods
        protected void BaseConfigure(EntityTypeBuilder<TBase> builder)
        {
            builder.Property(m => m.CreatedOn).HasColumnType("DATETIME ");
            builder.Property(m => m.CreatedOn).HasDefaultValueSql("UTC_TIMESTAMP()");
            builder.Property(m => m.UpdatedOn).HasColumnType("DATETIME ");
        }

        public virtual void Configure(EntityTypeBuilder<TBase> builder)
        {
            BaseConfigure(builder);
        }
        #endregion
    }
}
