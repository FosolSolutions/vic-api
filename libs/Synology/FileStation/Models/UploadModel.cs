namespace Synology.FileStation.Models
{
    public class UploadModel
    {
        #region Properties
        public bool BlSkip { get; set; }
        public string File { get; set; }
        public int Pid { get; set; }
        public int Progress { get; set; }
        #endregion
    }
}
