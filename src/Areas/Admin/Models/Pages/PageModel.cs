using Vic.Data.Entities;

namespace Vic.Api.Areas.Admin.Models.Pages
{
    public class PageModel
    {
        #region Properties
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Body { get; set; }

        public int SortOrder { get; set; }

        public bool IsEnabled { get; set; }
        #endregion

        #region Constructors
        public PageModel() { }

        public PageModel(Page page)
        {
            this.Id = page.Id;
            this.Name = page.Name;
            this.Path = page.Path;
            this.Body = page.Body;
            this.SortOrder = page.SortOrder;
            this.IsEnabled = page.IsEnabled;
        }
        #endregion
    }
}
