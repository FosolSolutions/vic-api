using System;
using System.Collections.Generic;

namespace Fosol.Core.Http.Extensions
{
    /// <summary>
    /// UriExtensions static class, provides extensions methods for Uri objects.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Add the specified 'key' and 'value' query string parameter to the specified 'uri'.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>A new Uri object including the new query parameter.</returns>
        public static Uri AddQueryString(this Uri uri, string key, string value)
        {
            return new Uri(uri.OriginalString.AddQueryString(key, value));
        }

        /// <summary>
        /// Add the array of 'key' and 'value' query string parameters to the specified 'uri'.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="queryParams"></param>
        /// <returns>A new Uri object including the new query parameter.</returns>
        public static Uri AddQueryString(this Uri uri, params KeyValuePair<string, string>[] queryParams)
        {
            return new Uri(uri.OriginalString.AddQueryString(queryParams));
        }
    }
}
