using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Synology.FileStation;
using Synology.FileStation.Models;
using Vic.Api.Helpers.Services;
using Vic.Api.Models;
using Vic.Data;

namespace Vic.Api.Controllers
{
    /// <summary>
    /// FileStationController class, provides a controller for FileStation endpoints.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FileStationController : ControllerBase
    {
        #region Variables
        private readonly IFileStationApi _fileStation;
        private readonly IDataService _service;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a FileStationController object, initializes with specified arguments.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileStation"></param>
        public FileStationController(VicContext context, IFileStationApi fileStation)
        {
            _fileStation = fileStation;
            _service = new DataService(fileStation, context);
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Fetch a page list of shares in the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpGet("shares")]
        public async Task<IActionResult> SharesAsync(string path, int page = 1, int quantity = 0)
        {
            var result = await _fileStation.ListSharesAsync(path ?? "/", page - 1, quantity, SortBy.Name, SortDirection.Ascending, "all", false, new[] { "size", "time" });

            return new JsonResult(new PageModel<ItemModel>(page, result.Data.Total, result.Data.Shares.Select(s => new ItemModel(s))));
        }

        /// <summary>
        /// Fetch a page list of files and folders in the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpGet("files")]
        public async Task<IActionResult> FilesAsync(string path, int page = 1, int quantity = 0)
        {
            var result = await _service.ListAsync(path, page, quantity, SortBy.Modified, SortDirection.Descending, "all", false, new[] { "size", "time" });

            return new JsonResult(result);
        }

        /// <summary>
        /// Download the file specified by the 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("files/download")]
        public async Task<IActionResult> DownloadAsync(string path)
        {
            var response = await _fileStation.DownloadAsync(path);

            var fileName = response.Content.Headers.ContentDisposition?.FileName ?? Path.GetFileName(path);
            var data = await response.Content.ReadAsStreamAsync();

            return File(data, response.Content.Headers.ContentType.MediaType, fileName);
        }

        /// <summary>
        /// Download a thumbnail of the file for the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        [HttpGet("files/thumb")]
        public async Task<IActionResult> ThumbnailAsync(string path, ThumbSize size = ThumbSize.Small, int rotate = 0)
        {
            var response = await _fileStation.ThumbAsync(path, size, rotate);

            var fileName = response.Content.Headers.ContentDisposition?.FileName ?? Path.GetFileName(path);
            var data = await response.Content.ReadAsStreamAsync();

            return File(data, response.Content.Headers.ContentType.MediaType, fileName);
        }
        #endregion
    }
}
