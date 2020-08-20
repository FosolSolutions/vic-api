using Fosol.Core.Http.Json;

namespace Synology.FileStation.Models
{
    public enum SortBy
    {
        [EnumValue("name")]
        Name,
        [EnumValue("user")]
        User,
        [EnumValue("group")]
        Group,
        [EnumValue("mtime")]
        Modified,
        [EnumValue("atime")]
        Accessed,
        [EnumValue("ctime")]
        Change,
        [EnumValue("crtime")]
        Created,
        [EnumValue("posix")]
        POSIX

    }
}