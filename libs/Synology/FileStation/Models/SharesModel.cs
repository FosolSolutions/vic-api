using System;
using System.Collections.Generic;

namespace Synology.FileStation.Models
{
    public class SharesModel
    {
        #region Variables
        #endregion

        #region Properties
        public int Offset { get; set; }
        public int Total { get; set; }
        public List<ShareModel> Shares { get; set; }
        #endregion

        #region Constructors
        #endregion
    }
}
