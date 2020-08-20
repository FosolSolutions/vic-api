using System.Collections.Generic;

namespace Vic.Data.Entities
{
    /// <summary>
    /// Tag class, provides a model that represents a tag in the database.
    /// </summary>
    public class Tag : BaseEntity
    {
        #region Properties
        /// <summary>
        /// get/set - Primary key and name of the tag.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// get - A collection of items with this tag.
        /// </summary>
        public ICollection<ItemTag> Items { get; } = new List<ItemTag>();
        #endregion
    }
}
