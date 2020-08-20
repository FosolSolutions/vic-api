using System;

namespace Vic.Data.Entities
{
    public abstract class BaseEntity
    {
        #region Properties
        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
        #endregion
    }
}
