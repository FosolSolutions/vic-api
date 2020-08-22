using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Synology.FileStation;
using Synology.FileStation.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vic.Api.Models;
using Vic.Data;

namespace Vic.Api.Helpers.Services
{
    /// <summary>
    /// DataService class, provides a way to interact with both the synology FileStation API and the Victoria Web database.
    /// </summary>
    public class DataService : IDataService
    {
        #region Variables
        private readonly IFileStationApi _fileStation;
        private readonly VicContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a DataService object, initializes with specified arguments.
        /// </summary>
        /// <param name="fileStation"></param>
        /// <param name="context"></param>
        public DataService(IFileStationApi fileStation, VicContext context)
        {
            _fileStation = fileStation;
            _context = context;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fetch a list of items from synology for the specified page.
        /// Fetch related metadata for those items from the database.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="fileType"></param>
        /// <param name="onlyWritable"></param>
        /// <param name="additional"></param>
        /// <returns></returns>
        public async Task<PageModel<ItemModel>> ListAsync(string path, int page = 1, int quantity = 0, SortBy sortBy = SortBy.Name, SortDirection sortDirection = SortDirection.Ascending, string fileType = "all", bool onlyWritable = false, string[] additional = null)
        {
            if (page < 1) throw new ArgumentException("Argument must be greater than 0", nameof(page));
            if (quantity < 0) throw new ArgumentException("Argument must be greater than or equal to 0.", nameof(quantity));
            if (quantity == 0) page = 1;

            // Fetch items from synology.
            var sitems = await _fileStation.ListAsync(path ?? "/", page - 1, quantity, sortBy, sortDirection, fileType, onlyWritable, additional);

            // Fetch item metadata from database.
            var query = (from p in _context.Items.AsNoTracking()
                         join i in _context.Items on p.Id equals i.ParentId
                         where p.Path == path
                         select i);

            query = sortBy switch
            {
                SortBy.Name => sortDirection == SortDirection.Ascending ? query.OrderBy(i => i.Name) : query.OrderByDescending(i => i.Name),
                SortBy.Created => sortDirection == SortDirection.Ascending ? query.OrderBy(i => i.CreatedOn) : query.OrderByDescending(i => i.CreatedOn),
                SortBy.Modified => sortDirection == SortDirection.Ascending ? query.OrderBy(i => i.UpdatedOn) : query.OrderByDescending(i => i.UpdatedOn),
                _ => query.OrderByDescending(i => i.CreatedOn)
            };

            if (page > 1 && quantity > 0)
            {
                query = query.Skip((page - 1) & quantity).Take(quantity);
            }
            var items = query.ToArray();

            // Convert to models based on whether we have metadata or not.
            var result = sitems.Data.Files.Select(si =>
            {
                var item = items.FirstOrDefault(i => i.Path == si.Path);
                return item == null ? new ItemModel(si) : new ItemModel(item);
            }).OrderByDescending(i => i.PublishedOn).ThenByDescending(i => i.CreatedOn).ThenBy(i => i.Name);

            return new PageModel<ItemModel>(page, sitems.Data.Total, result);
        }
        #endregion
    }
}
