using Fosol.Core.Http.Converters;
using System;
using System.Text.Json.Serialization;

namespace Synology.FileStation.Models
{
    public class TimeModel
    {
        #region Properties
        [JsonConverter(typeof(MicrosecondEpochJsonConverter))]
        [JsonPropertyName("atime")]
        public DateTime AccessedOn { get; set; }

        [JsonConverter(typeof(MicrosecondEpochJsonConverter))]
        [JsonPropertyName("crtime")]
        public DateTime CreatedOn { get; set; }

        [JsonConverter(typeof(MicrosecondEpochJsonConverter))]
        [JsonPropertyName("ctime")]
        public DateTime ChangedOn { get; set; }

        [JsonConverter(typeof(MicrosecondEpochJsonConverter))]
        [JsonPropertyName("mtime")]
        public DateTime UpdatedOn { get; set; }
        #endregion
    }
}
