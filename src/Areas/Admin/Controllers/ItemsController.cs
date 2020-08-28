using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Options;
using Synology.FileStation;
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
        private readonly IFileStationApi _fileStation;
        private readonly JsonSerializerOptions _serializeOptions;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ItemsController object, initializes with specified arguments.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileStation"></param>
        /// <param name="serializeOptions"></param>
        public ItemsController(VicContext context, IFileStationApi fileStation, IOptions<JsonSerializerOptions> serializeOptions)
        {
            _context = context;
            _fileStation = fileStation;
            _serializeOptions = serializeOptions.Value;
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
        /// Add the new item to the datasource and upload the file to the NAS.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> AddAsync([FromForm] ItemUploadModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model.File == null) throw new ArgumentNullException(nameof(model), $"Property 'file' is required.");
            
            var itemModel = JsonSerializer.Deserialize<ItemModel>(model.Item, _serializeOptions);
            if (String.IsNullOrWhiteSpace(itemModel.Path)) throw new ArgumentException($"Property 'path' is required.", nameof(model));
            if (String.IsNullOrWhiteSpace(itemModel.Name)) throw new ArgumentException($"Property 'name' is required.", nameof(model));

            // TODO: Query FileStation if the folder exists.
            // Do not allow uploading a new file if the file path already exists.
            //var pathExists = _context.Items.Any(i => i.Path == itemModel.Path);
            //if (!pathExists) throw new ArgumentException($"Path '{itemModel.Path}' does not exist.");

            var fileExists = _context.Items.Any(i => i.Path == $"{itemModel.Path}/{model.File.FileName}");
            if (fileExists) throw new ArgumentException($"File '{model.File.FileName}' already exists.");

            await _fileStation.UploadAsync(itemModel.Path, model.File);

            // Need to update the path to include the file.
            itemModel.Path = $"{itemModel.Path}/{model.File.FileName}";
            return Add(itemModel);
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
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RemoveAsync(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException("Item does not exist");

            await _fileStation.DeleteAsync(item.Path, false);

            _context.Items.Remove(item);
            _context.SaveChanges();

            return new JsonResult(new ItemModel(item));
        }
        #endregion
    }
}
