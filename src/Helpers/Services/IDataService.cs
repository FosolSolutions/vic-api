using Synology.FileStation.Models;
using System.Threading.Tasks;
using Vic.Api.Models;

namespace Vic.Api.Helpers.Services
{
    public interface IDataService
    {
        Task<PageModel<ItemModel>> ListAsync(string path, int page = 1, int quantity = 0, SortBy sortBy = SortBy.Name, SortDirection sortDirection = SortDirection.Ascending, string fileType = "all", bool onlyWritable = false, string[] additional = null);
    }
}