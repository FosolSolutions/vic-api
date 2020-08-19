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
    public class FileStationApi : IDisposable, IFileStationApi
    {
        #region Variables
        private readonly Options.SynologyOptions _options;
        private readonly IHttpRequestClient _client;
        private readonly ILogger _logger;
        private string _sid;
        #endregion

        #region Constructors
        public FileStationApi(IOptionsMonitor<Options.SynologyOptions> options, IHttpRequestClient httpClient, ILogger<FileStationApi> logger)
        {
            _options = options.CurrentValue;
            _client = httpClient;
            _logger = logger;
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<DataModel<AuthenticationModel>> LoginAsync(string username, string password, string session = "FileStation", string format = "sid")
        {
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

        public async Task<DataModel<string>> LogoutAsync(string session = "FileStation")
        {
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

        public async Task<DataModel<SharesModel>> ListSharesAsync(string path = "/", int offset = 0, int limit = 0, string sortBy = "name", string sortDirection = "asc", string fileType = "all", bool onlyWritable = false, string[] additional = null)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var values = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.List" },
                { "version", "2" },
                { "method", "list_share" },
                { "_sid", _sid },
                { "offset", $"{offset}" },
                { "limit", $"{limit}" },
                { "sort_by", sortBy },
                { "sort_direction", sortDirection },
                { "onlywritable", $"{onlyWritable}" },
                { "additional", $"[{String.Join(",", additional ?? new string [0])}]" },
                { "action", "list" },
                { "check_dir", "true" },
                { "filetype", fileType },
                { "folder_path", path },
            };

            var form = new FormUrlEncodedContent(values);
            var response = await _client.PostAsync(Path("entry"), form);
            return await HandleResponseAsync<SharesModel>(response);
        }

        public async Task<DataModel<FilesModel>> ListAsync(string path, int offset = 0, int limit = 0, string sortBy = "name", string sortDirection = "asc", string fileType = "all", bool onlyWritable = false, string[] additional = null)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var values = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.List" },
                { "version", "2" },
                { "method", "list" },
                { "_sid", _sid },
                { "offset", $"{offset}" },
                { "limit", $"{limit}" },
                { "sort_by", sortBy },
                { "sort_direction", sortDirection },
                { "onlywritable", $"{onlyWritable}" },
                { "additional", $"[{String.Join(",", additional ?? new string [0])}]" },
                { "action", "list" },
                { "check_dir", "true" },
                { "filetype", fileType },
                { "folder_path", path },
            };

            var form = new FormUrlEncodedContent(values);
            var response = await _client.PostAsync(Path("entry"), form);
            return await HandleResponseAsync<FilesModel>(response);
        }

        public async Task<HttpResponseMessage> ThumbAsync(string path, string size = "small", int rotate = 0)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

            if (String.IsNullOrWhiteSpace(_sid)) await AuthenticateAsync();
            var query = new Dictionary<string, string>()
            {
                { "api", "SYNO.FileStation.Thumb" },
                { "version", "2" },
                { "method", "get" },
                { "_sid", _sid },
                { "size", size },
                { "rotate", $"{rotate}" },
                { "path", path },
            };

            var form = new FormUrlEncodedContent(query);
            return await _client.PostAsync(Path("entry"), form);
        }
        public async Task<DataModel<UploadModel>> UploadAsync(string pathAndFile, byte[] file, bool overwrite = false)
        {
            if (String.IsNullOrWhiteSpace(pathAndFile)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(pathAndFile));
            if (file == null || file.Length == 0) throw new ArgumentException($"Argument cannot be null or empty.", nameof(file));

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

        public async Task<HttpResponseMessage> DownloadAsync(string path, string mode = "download")
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

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

        public async Task<DataModel<string>> DeleteAsync(string path, bool recursive = false)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException($"Argument cannot be null, empty or whitespace.", nameof(path));

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
        private string Path(string name)
        {
            return $"{_options.HostUrl}/webapi/{name}.cgi";
        }

        private async Task AuthenticateAsync()
        {
            var result = await LoginAsync(_options.Username, _options.Password);
            _sid = result.Data.Sid;
        }

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
