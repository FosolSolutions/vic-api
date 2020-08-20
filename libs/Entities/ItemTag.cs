using System;

namespace Vic.Data.Entities
{
    /// <summary>
    /// ItemTag class, provides the many-to-many relationship between items and tags.
    /// </summary>
    public class ItemTag : BaseEntity
    {
        #region Properties
        /// <summary>
        /// get/set - Foreign key to the item.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// get/set - The item.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// get/set - Foreign key to the tag.
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        /// get/set - The tag.
        /// </summary>
        public Tag Tag { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of an ItemTag object.
        /// </summary>
        public ItemTag() { }

        /// <summary>
        /// Creates a new instance of an ItemTag object, initializes with specified arguments.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tag"></param>
        public ItemTag(Item item, Tag tag)
        {
            this.ItemId = item?.Id ?? throw new ArgumentNullException(nameof(item));
            this.Item = item;
            this.TagId = tag?.Id ?? throw new ArgumentNullException(nameof(tag));
            this.Tag = tag;
        }
        #endregion
    }
}
