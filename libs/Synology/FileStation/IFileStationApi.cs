using Synology.FileStation.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Synology.FileStation
{
    public interface IFileStationApi
    {
        Task<DataModel<string>> DeleteAsync(string path, bool recursive = false);
        Task<HttpResponseMessage> DownloadAsync(string path, string mode = "download");
        Task<DataModel<FilesModel>> ListAsync(string path, int offset = 0, int limit = 0, string sortBy = "name", string sortDirection = "asc", string fileType = "all", bool onlyWritable = false, string[] additional = null);
        Task<DataModel<SharesModel>> ListSharesAsync(string path = "/", int offset = 0, int limit = 0, string sortBy = "name", string sortDirection = "asc", string fileType = "all", bool onlyWritable = false, string[] additional = null);
        Task<DataModel<AuthenticationModel>> LoginAsync(string username, string password, string session = "FileStation", string format = "sid");
        Task<DataModel<string>> LogoutAsync(string session = "FileStation");
        Task<HttpResponseMessage> ThumbAsync(string path, string size = "small", int rotate = 0);
        Task<DataModel<UploadModel>> UploadAsync(string pathAndFile, byte[] file, bool overwrite = false);
    }
}