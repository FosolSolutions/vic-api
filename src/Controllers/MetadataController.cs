using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vic.Api.Models;
using Vic.Data;
using Vic.Data.Entities;

namespace Vic.Api.Controllers
{
    /// <summary>
    /// MetadataController class, provides a controller for metadata endpoints.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {
        #region Variables
        private readonly VicContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a MetadataController object, initializes with specified arguments.
        /// </summary>
        /// <param name="context"></param>
        public MetadataController(VicContext context)
        {
            _context = context;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Fetch the item from the datasource for the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("items/{id}")]
        public IActionResult Get(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException("Item does not exist");

            return new JsonResult(new ItemModel(item));
        }

        /// <summary>
        /// Fetch the item from the datasource for the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("items/one")]
        public IActionResult Get(string path)
        {
            var item = _context.Items.FirstOrDefault(i => i.Path == path) ?? throw new ArgumentOutOfRangeException("Item does not exist");

            return new JsonResult(new ItemModel(item));
        }

        /// <summary>
        /// Add the new item to the datasource.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("items")]
        public IActionResult Add(ItemModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!model.PublishedOn.HasValue) throw new ArgumentException($"Item must have a 'publishedOn' date.", nameof(model));

            // Determine if the parent already exists.
            var path = System.IO.Path.GetDirectoryName(model.Path).Replace("\\", "/");
            var parent = _context.Items.FirstOrDefault(i => i.Path == path) ?? new Item()
            {
                Name = new System.IO.DirectoryInfo(path).Name,
                Path = path,
                PublishedOn = model.PublishedOn.Value,
                CreatedOn = model.CreatedOn ?? DateTime.UtcNow,
                UpdatedOn = model.UpdatedOn ?? DateTime.UtcNow
            };

            var item = new Item()
            {
                Name = model.Name,
                Path = model.Path,
                Description = model.Description,
                Author = model.Author,
                PublishedOn = model.PublishedOn.Value,
                CreatedOn = model.CreatedOn ?? DateTime.UtcNow,
                UpdatedOn = model.UpdatedOn ?? DateTime.UtcNow,
                Parent = parent
            };

            _context.Items.Add(item);
            _context.SaveChanges();

            return new JsonResult(new ItemModel(item));
        }

        /// <summary>
        /// Update the item for the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("items/{id}")]
        public IActionResult Update(int id, ItemModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!model.PublishedOn.HasValue) throw new ArgumentException($"Item must have a 'publishedOn' date.", nameof(model));

            // Determine if the parent already exists.
            var path = System.IO.Path.GetDirectoryName(model.Path).Replace("\\", "/");
            var parent = _context.Items.FirstOrDefault(i => i.Path == path) ?? new Item()
            {
                Name = new System.IO.DirectoryInfo(path).Name,
                Path = path,
                PublishedOn = model.PublishedOn.Value,
                CreatedOn = model.CreatedOn ?? DateTime.UtcNow,
                UpdatedOn = model.UpdatedOn ?? DateTime.UtcNow
            };

            var item = _context.Items.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), "Item does not exist.");
            item.Name = model.Name;
            item.Description = model.Description;
            item.Path = model.Path;
            item.Author = model.Author;
            item.PublishedOn = model.PublishedOn.Value;
            item.CreatedOn = model.CreatedOn ?? DateTime.UtcNow;
            item.UpdatedOn = model.UpdatedOn ?? DateTime.UtcNow;
            item.Parent = parent;

            if (parent.Id == 0) _context.Add(parent);
            _context.Update(item);
            _context.SaveChanges();

            return new JsonResult(new ItemModel(item));
        }

        /// <summary>
        /// Fetch the item from the datasource for the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("items/{id}")]
        public IActionResult Remove(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException("Item does not exist");
            _context.Items.Remove(item);
            _context.SaveChanges();

            return new JsonResult(new ItemModel(item));
        }
        #endregion
    }
}
