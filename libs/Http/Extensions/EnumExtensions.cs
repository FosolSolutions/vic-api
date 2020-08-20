using Fosol.Core.Http.Json;
using System.Reflection;

namespace Fosol.Core.Http.Extensions
{
    /// <summary>
    /// EnumExtensions enum, provides extensions for enum values.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the string name for the specified 'enumItem'.
        /// This will check if the EnumValueAttribute exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItem"></param>
        /// <returns></returns>
        public static string GetValue<T>(this T enumItem)
        {
            var type = typeof(T);
            var attr = type.GetCustomAttribute<EnumValueAttribute>();
            return attr?.Value ?? enumItem.ToString();
        }
    }
}
