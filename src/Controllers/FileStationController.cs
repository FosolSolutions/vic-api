using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Synology.FileStation;
using Vic.Api.Models;

namespace Vic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileStationController : ControllerBase
    {
        #region Variables
        private readonly IFileStationApi _api;
        private readonly ILogger _logger;
        #endregion

        #region Constructors

        public FileStationController(IFileStationApi api, ILogger<FileStationController> logger)
        {
            _api = api;
            _logger = logger;
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
            var result = await _api.ListSharesAsync(path ?? "/", page - 1, quantity);

            return new JsonResult(new PageModel<FolderModel>(page, result.Data.Total, result.Data.Shares.Select(s => new FolderModel(s))));
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
            var result = await _api.ListAsync(path ?? "/", page - 1, quantity);

            return new JsonResult(new PageModel<FolderModel>(page, result.Data.Total, result.Data.Files.Select(s => new FolderModel(s))));
        }

        /// <summary>
        /// Fetch a page list of files and folders in the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("files/download")]
        public async Task<IActionResult> DownloadAsync(string path)
        {
            var response = await _api.DownloadAsync(path);

            var fileName = response.Content.Headers.ContentDisposition?.FileName ?? Path.GetFileName(path);
            var data = await response.Content.ReadAsStreamAsync();

            return File(data, response.Content.Headers.ContentType.MediaType, fileName);
        }

        /// <summary>
        /// Fetch a page list of files and folders in the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        [HttpGet("files/thumb")]
        public async Task<IActionResult> ThumbnailAsync(string path, string size = "small", int rotate = 0)
        {
            var response = await _api.ThumbAsync(path, size, rotate);

            var fileName = response.Content.Headers.ContentDisposition?.FileName ?? Path.GetFileName(path);
            var data = await response.Content.ReadAsStreamAsync();

            return File(data, response.Content.Headers.ContentType.MediaType, fileName);
        }
        #endregion
    }
}
