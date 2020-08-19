using Synology.FileStation.Models;

namespace Vic.Api.Models
{
    public class FolderModel
    {
        #region Properties
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDir { get; set; }
        #endregion

        #region Constructors
        public FolderModel() { }

        public FolderModel(FileModel file)
        {
            this.Name = file.Name;
            this.Path = file.Path;
            this.IsDir = file.IsDir;
        }
        #endregion
    }
}
