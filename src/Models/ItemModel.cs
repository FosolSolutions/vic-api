using Synology.FileStation.Models;
using System;
using Vic.Data.Entities;

namespace Vic.Api.Models
{
    /// <summary>
    /// ItemModel class, provides a model to represent an item.
    /// </summary>
    public class ItemModel
    {
        #region Properties
        /// <summary>
        /// get/set - Primary key to the item in the database.
        /// </summary>
        /// <value></value>
        public int? Id { get; set; }

        /// <summary>
        /// get/set - Display name for the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// get/set - The unique path to the item.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// get/set - Whether this item is a folder.
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// get/set - The author of the item.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// get/set - A description of the item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// get/set - The date the item was published.
        /// </summary>
        public DateTime? PublishedOn { get; set; }

        /// <summary>
        /// get/set - The date the item was created.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// get/set - The date the item was updated.
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of an ItemModel.
        /// </summary>
        public ItemModel() { }

        /// <summary>
        /// Creates a new instance of an ItemModel, initializes with specified arguments.
        /// </summary>
        /// <param name="file"></param>
        public ItemModel(FileModel file)
        {
            this.Name = file?.Name ?? throw new ArgumentNullException(nameof(file));
            this.Path = file.Path;
            this.IsFolder = file.IsDir;
            this.CreatedOn = file.Additional?.Time?.CreatedOn;
            this.UpdatedOn = file.Additional?.Time?.UpdatedOn;
        }

        /// <summary>
        /// Creates a new instance of an ItemModel, initializes with specified arguments.
        /// </summary>
        /// <param name="item"></param>
        public ItemModel(Item item)
        {
            this.Id = item?.Id ?? throw new ArgumentNullException(nameof(item));
            this.Name = item.Name;
            this.Path = item.Path;
            this.IsFolder = item.IsFolder;
            this.Author = item.Author;
            this.Description = item.Description;
            this.PublishedOn = item.PublishedOn;
            this.CreatedOn = item.CreatedOn;
            this.UpdatedOn = item.UpdatedOn;
        }
        #endregion
    }
}
