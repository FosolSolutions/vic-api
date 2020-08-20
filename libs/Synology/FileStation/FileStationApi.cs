using Fosol.Core.Http;
using Fosol.Core.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Synology.FileStation.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Synology.FileStation
{
    /// <summary>
    /// FileStationApi class, provides a service for making HTTP requests to synology file station.
    /// </summary>
    public class FileStationApi : IDisposable, IFileStationApi
    {
        #region Variables
        private readonly Options.SynologyOptions _options;
        private readonly IHttpRequestClient _client;
        private readonly ILogger _logger;
        private string _sid;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instances of a FileStationApi object, initializes with specified arguments.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public FileStationApi(IOptionsMonitor<Options.SynologyOptions> options, IHttpRequestClient httpClient, ILogger<FileStationApi> logger)
        {
            _options = options.CurrentValue;
            _client = httpClient;
            _logger = logger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Dispose the http request client.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }

        /// <summary>
        /// Login to synology and retrieve a session id.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="session"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public async Task<DataModel<AuthenticationModel>> LoginAsync(string username, string password, string session = "FileStation", string format = "sid")
        {
            _logger.LogDebug($"Making HTTP request to login");
            var query = new Dictionary<string, string>()
            {
                { "api", "SYNO.API.Auth" },
                { "version", "3" },
                { "method", "login" },
                { "account", username },
                { "passwd", password },
                { "session", session },
                { "format", format },
            };
            var url = Path("auth").AddQueryString(query);
            var response = await _client.SendAsync(url);
            return await HandleResponseAsync<AuthenticationModel>(response);
        }

        /// <summary>
        /// Logout of synology for the specified session.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<DataModel<string>> LogoutAsync(string session = "FileStation")
        {
            _logger.LogDebug($"Making HTTP request to logout");
            var query = new Dictionary<string, string>()
            {
                { "api", "SYNO.API.Auth" },
                { "version", "6" },
                { "method", "logout" },
                { "_sid", _sid },
                { "session", session },
            };
            var url = Path("auth").AddQueryString(query);
            var response = await _client.SendAsync(url);
            return await HandleResponseAsync<string>(response);
        }

        /// <summary>
        /// Fetch a list of shares and folders at the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="fileType"></param>
        /// <param name="onlyWritable"></param>
        /// <param name="additional"></param>
        /// <returns></returns>
        public async Task<DataModel<SharesModel>> ListSharesAsync(string path = "/", int offset = 0, int limit = 0, SortBy sortBy = SortBy.Name, SortDirection sortDirection = SortDirection.Ascending, string fileType = "all", bool onlyWritable = false, string[] additional = null)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            _logger.LogDebug($"Making HTTP request to list shares");
            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var values = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.List" },
                { "version", "2" },
                { "method", "list_share" },
                { "_sid", _sid },
                { "offset", $"{offset}" },
                { "limit", $"{limit}" },
                { "sort_by", sortBy.GetValue() },
                { "sort_direction", sortDirection.GetValue() },
                { "onlywritable", $"{onlyWritable}" },
                { "additional", $"[\"{String.Join("\",\"", additional ?? new string [0])}\"]" },
                { "action", "list" },
                { "check_dir", "true" },
                { "filetype", fileType },
                { "folder_path", path },
            };

            var form = new FormUrlEncodedContent(values);
            var response = await _client.PostAsync(Path("entry"), form);
            return await HandleResponseAsync<SharesModel>(response);
        }

        /// <summary>
        /// Fetch a list of files and folders at the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="fileType"></param>
        /// <param name="onlyWritable"></param>
        /// <param name="additional"></param>
        /// <returns></returns>
        public async Task<DataModel<FilesModel>> ListAsync(string path, int offset = 0, int limit = 0, SortBy sortBy = SortBy.Name, SortDirection sortDirection = SortDirection.Ascending, string fileType = "all", bool onlyWritable = false, string[] additional = null)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            _logger.LogDebug($"Making HTTP request to fetch a list");
            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var values = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.List" },
                { "version", "2" },
                { "method", "list" },
                { "_sid", _sid },
                { "offset", $"{offset}" },
                { "limit", $"{limit}" },
                { "sort_by", sortBy.GetValue() },
                { "sort_direction", sortDirection.GetValue() },
                { "onlywritable", $"{onlyWritable}" },
                { "additional", $"[\"{String.Join("\",\"", additional ?? new string [0])}\"]" },
                { "action", "list" },
                { "check_dir", "true" },
                { "filetype", fileType },
                { "folder_path", path },
            };

            var form = new FormUrlEncodedContent(values);
            var response = await _client.PostAsync(Path("entry"), form);
            return await HandleResponseAsync<FilesModel>(response);
        }

        /// <summary>
        /// Fetch a thumbnail for the file at the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ThumbAsync(string path, ThumbSize size = ThumbSize.Small, int rotate = 0)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            _logger.LogDebug($"Making HTTP request to fetch a thumbnail");
            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var query = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.Thumb" },
                { "version", "2" },
                { "method", "get" },
                { "_sid", _sid },
                { "size", size.GetValue() },
                { "rotate", $"{rotate}" },
                { "path", path },
            };

            var form = new FormUrlEncodedContent(query);
            return await _client.PostAsync(Path("entry"), form);
        }

        /// <summary>
        /// Upload a new file.
        /// </summary>
        /// <param name="pathAndFile"></param>
        /// <param name="file"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public async Task<DataModel<UploadModel>> UploadAsync(string pathAndFile, byte[] file, bool overwrite = false)
        {
            if (String.IsNullOrWhiteSpace(pathAndFile)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(pathAndFile));
            if (file == null || file.Length == 0) throw new ArgumentException($"Argument cannot be null or empty.", nameof(file));

            _logger.LogDebug($"Making HTTP request to upload file");
            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var path = System.IO.Path.GetFullPath(pathAndFile);
            var fileName = System.IO.Path.GetFileName(pathAndFile);

            var query = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.Upload" },
                { "version", "2" },
                { "method", "upload" },
                { "_sid", _sid },
            };
            var url = Path("entry").AddQueryString(query);

            var values = new Dictionary<string, string>()
            {
                { "overwrite", $"{overwrite}" },
                { "path", path },
            };

            var form = new MultipartFormDataContent
            {
                new FormUrlEncodedContent(values),
                { new ByteArrayContent(file, 0, file.Length), "file", fileName }
            };
            var response = await _client.PostAsync(url, form);
            return await HandleResponseAsync<UploadModel>(response);
        }

        /// <summary>
        /// Download the file at the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DownloadAsync(string path, string mode = "download")
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            _logger.LogDebug($"Making HTTP request to download file");
            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var values = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.Download" },
                { "version", "2" },
                { "method", "download" },
                { "_sid", _sid },
                { "mode", mode },
                { "path", path },
            };

            var form = new FormUrlEncodedContent(values);
            return await _client.PostAsync(Path("entry"), form);
        }

        /// <summary>
        /// Delete the file at the specified 'path'.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public async Task<DataModel<string>> DeleteAsync(string path, bool recursive = false)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            _logger.LogDebug($"Making HTTP request to delete item");
            var query = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.Delete" },
                { "version", "2" },
                { "method", "delete" },
                { "_sid", _sid },
                { "path", path },
                { "recursive", $"{recursive}" },
            };
            var url = Path("auth").AddQueryString(query);
            var response = await _client.SendAsync(url);
            return await HandleResponseAsync<string>(response);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Create the API endpoint path.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string Path(string name)
        {
            return $"{_options.HostUrl}/webapi/{name}.cgi";
        }

        /// <summary>
        /// Make an HTTP request to fetch a session ID for authentication.
        /// </summary>
        /// <returns></returns>
        private async Task AuthenticateAsync()
        {
            var result = await LoginAsync(_options.Username, _options.Password); // TODO: Need to handle a session timeout.
            _sid = result.Data.Sid;
        }

        /// <summary>
        /// Deserialize the response and handle failed requests.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<DataModel<T>> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await _client.DeserializeAsync<DataModel<T>>(response);
                if (result.Success) return result;

                var error = await _client.DeserializeAsync<ErrorModel>(response);
                throw new HttpClientRequestException(response, ErrorDescription(error.Error.Code), HttpStatusCode.BadRequest);
            }

            throw new HttpClientRequestException(response);
        }

        /// <summary>
        /// Extract the error information from the code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ErrorDescription(int code)
        {
            return code switch
            {
                100 => "Unkown error",
                101 => "No parameter of API, method or version ",
                102 => "The requested API does not exist",
                103 => "The requested method does not exist",
                104 => "The requested version does not support the functionality",
                105 => "The logged in session does not have permission",
                106 => "Session timeout",
                107 => "Session interrupted by duplicate login",
                400 => "Invalid parameter of file operation",
                401 => "Unknown error of file operation",
                402 => "System is too busy",
                403 => "Invalid user does this file operation",
                404 => "Invalid group does this file operation",
                405 => "Invalid user and group does this file operation",
                406 => "Can’t get user/group information from the account server",
                407 => "Operation not permitted",
                408 => "No such file or directory",
                409 => "Non-supported file system",
                410 => "Failed to connect internet-based file system (ex: CIFS)",
                411 => "Read-only file system",
                412 => "Filename too long in the non-encrypted file system",
                413 => "Filename too long in the encrypted file system",
                414 => "File already exists",
                415 => "Disk quota exceeded",
                416 => "No space left on device",
                417 => "Input/output error",
                418 => "Illegal name or path",
                419 => "Illegal file name",
                420 => "Illegal file name on FAT file system",
                421 => "Device or resource busy",
                599 => "No such task of the file operation",
                _ => "Undocumented error"
            };
        }
        #endregion
    }
}
