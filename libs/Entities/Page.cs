namespace Vic.Data.Entities
{
    public class Page : BaseEntity
    {
        #region Properties
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Body { get; set; }

        public int SortOrder { get; set; }

        public bool IsEnabled { get; set; }
        #endregion
    }
}
