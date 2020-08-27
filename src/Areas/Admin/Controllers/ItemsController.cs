using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vic.Api.Models;
using Vic.Data;
using Vic.Data.Entities;

namespace Vic.Api.Areas.Admin.Controllers
{
    /// <summary>
    /// ItemsController class, provides a controller for item metadata endpoints.
    /// </summary>
    [Authorize]
    [ApiController]
    [Area("admin")]
    [Route("[area]/[controller]")]
    public class ItemsController : ControllerBase
    {
        #region Variables
        private readonly VicContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ItemsController object, initializes with specified arguments.
        /// </summary>
        /// <param name="context"></param>
        public ItemsController(VicContext context)
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
        [HttpGet("{id}")]
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
        [HttpGet("one")]
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
        [HttpPost()]
        public IActionResult Add(ItemModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Determine if the parent already exists.
            var path = System.IO.Path.GetDirectoryName(model.Path).Replace("\\", "/");
            var parent = _context.Items.FirstOrDefault(i => i.Path == path) ?? new Item()
            {
                Name = new System.IO.DirectoryInfo(path).Name,
                Path = path,
                PublishedOn = model.PublishedOn?.ToUniversalTime(),
                CreatedOn = model.CreatedOn?.ToUniversalTime() ?? DateTime.UtcNow,
                UpdatedOn = model.UpdatedOn?.ToUniversalTime() ?? DateTime.UtcNow
            };

            var item = new Item()
            {
                Name = model.Name,
                Path = model.Path,
                Description = model.Description,
                Author = model.Author,
                PublishedOn = model.PublishedOn?.ToUniversalTime(),
                CreatedOn = model.CreatedOn?.ToUniversalTime() ?? DateTime.UtcNow,
                UpdatedOn = model.UpdatedOn?.ToUniversalTime() ?? DateTime.UtcNow,
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
        [HttpPut("{id}")]
        public IActionResult Update(int id, ItemModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Determine if the parent already exists.
            var path = System.IO.Path.GetDirectoryName(model.Path).Replace("\\", "/");
            var parent = _context.Items.FirstOrDefault(i => i.Path == path) ?? new Item()
            {
                Name = new System.IO.DirectoryInfo(path).Name,
                Path = path,
                PublishedOn = model.PublishedOn?.ToUniversalTime(),
                CreatedOn = model.CreatedOn?.ToUniversalTime() ?? DateTime.UtcNow,
                UpdatedOn = model.UpdatedOn?.ToUniversalTime() ?? DateTime.UtcNow
            };

            var item = _context.Items.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), "Item does not exist.");
            item.Name = model.Name;
            item.Description = model.Description;
            item.Path = model.Path;
            item.Author = model.Author;
            item.PublishedOn = model.PublishedOn?.ToUniversalTime();
            item.CreatedOn = model.CreatedOn?.ToUniversalTime() ?? DateTime.UtcNow;
            item.UpdatedOn = model.UpdatedOn?.ToUniversalTime() ?? DateTime.UtcNow;
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
        [HttpDelete("{id}")]
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
