using System.Collections.Generic;

namespace Synology.FileStation.Models
{
    public class FilesModel
    {
        #region Variables
        #endregion

        #region Properties
        public int Offset { get; set; }
        public int Total { get; set; }
        public List<FileModel> Files { get; set; }
        #endregion

        #region Constructors
        #endregion
    }
}
