using Microsoft.AspNetCore.Http;

namespace Vic.Api.Models
{
    /// <summary>
    /// ItemUploadModel class, provides a model to represent an item.
    /// </summary>
    public class ItemUploadModel
    {
        #region Properties
        /// <summary>
        /// get/set - The file to upload with the item.
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// get/set - The serialized item.  This is required because by default it won't deserialize.
        /// </summary>
        public string Item { get; set; }
        #endregion
    }
}
