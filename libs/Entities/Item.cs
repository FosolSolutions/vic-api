using System;
using System.Collections.Generic;

namespace Vic.Data.Entities
{
    /// <summary>
    /// Item class, provides a model that represents items in the database.
    /// An item can be a folder, file or possibly anything else.
    /// </summary>
    public class Item : BaseEntity
    {
        #region Properties
        /// <summary>
        /// get/set - Primary key of the item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// get/set - The display name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// get/set - The unique path to the item.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// get/set - A description of this item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// get/set - When this item was published.
        /// </summary>
        public DateTime? PublishedOn { get; set; }

        /// <summary>
        /// get/set - The author of this item.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// get/set - The content type of this item.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// get/set - Whether this item is a folder.  A folder can contain children.
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// get/set - Foreign key to the parent of this item.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// get/set - The parent of this item.
        /// </summary>
        public Item Parent { get; set; }

        /// <summary>
        /// get - Collection of children to this item.
        /// </summary>
        public ICollection<Item> Items { get; } = new List<Item>();

        /// <summary>
        /// get - Collection of tags to identify this item.
        /// </summary>
        public ICollection<ItemTag> Tags { get; } = new List<ItemTag>();
        #endregion
    }
}
