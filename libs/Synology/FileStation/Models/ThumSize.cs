using Fosol.Core.Http.Json;

namespace Synology.FileStation.Models
{
    public enum ThumbSize
    {
        [EnumValue("small")]
        Small,
        [EnumValue("medium")]
        Medium,
        [EnumValue("large")]
        Large,
        [EnumValue("original")]
        Original

    }
}