namespace Synology.FileStation.Models
{
    public class FileModel
    {
        #region Properties
        public bool IsDir { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public AdditionalAttributesModel Additional { get; set; }
        #endregion

        #region Constructors
        #endregion
    }
}
