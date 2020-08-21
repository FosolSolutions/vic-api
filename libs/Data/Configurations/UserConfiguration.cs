using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vic.Data.Entities;

namespace Vic.Data.Configurations
{
    /// <summary>
    /// UserConfiguration class, provides a way to configure users in the database.
    ///</summary>
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        #region Methods
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();

            builder.Property(m => m.Username).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Password).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Email).IsRequired().HasMaxLength(50);
            builder.Property(m => m.DisplayName).HasMaxLength(50);
            builder.Property(m => m.FirstName).HasMaxLength(50);
            builder.Property(m => m.LastName).HasMaxLength(50);

            base.Configure(builder);
        }
        #endregion
    }
}
