using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using Vic.Data.Configurations;
using Vic.Data.Entities;

namespace Vic.Data
{
    /// <summary>
    /// VicContext sealed class, provides a database context for the victoria web database.
    /// </summary>
    public sealed class VicContext : DbContext
    {
        #region Variables
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Properties
        /// <summary>
        /// get/set - Pages provide a way to dynamically control the content of each page.
        /// </summary>
        public DbSet<Page> Pages { get; set; }

        /// <summary>
        /// get/set - Items represent files, folders or anything else.
        /// </summary>
        public DbSet<Item> Items { get; set; }

        /// <summary>
        /// get/set - Tags are a way of identifying related items.
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// get/set - The many-to-many relationship between items and tags.
        /// </summary>
        public DbSet<ItemTag> ItemTags { get; set; }

        /// <summary>
        /// get/set - Users who have accounts.
        /// </summary>
        public DbSet<User> Users { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a VicContext object, initializes with the specified arguments.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="httpContextAccessor"></param>
        public VicContext(DbContextOptions<VicContext> options, IHttpContextAccessor httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Configures the DbContext with the specified options.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Creates the datasource.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyAllConfigurations(typeof(PageConfiguration), this);

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.UseValueConverterForType(typeof(DateTime), dateTimeConverter);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Save the entities with who created them or updated them.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            // get entries that are being Added or Updated
            var modifiedEntries = ChangeTracker.Entries()
                    .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    else if (entry.State != EntityState.Deleted)
                    {
                        entity.UpdatedOn = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChanges();
        }

        /// <summary>
        /// Wrap the save changes in a transaction for rollback.
        /// </summary>
        /// <returns></returns>
        public int CommitTransaction()
        {
            var result = 0;
            using (var transaction = this.Database.BeginTransaction())
            {
                try
                {
                    result = this.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateException)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return result;
        }
        #endregion
    }
}
