using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;

namespace Fosol.Core.Http.Extensions
{
    /// <summary>
    /// StringExtensions static class, provides extensions methods for string objects.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Add the specified 'key' and 'value' query string parameter to the specified 'uri'.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>A new string object including the new query parameter.</returns>
        public static string AddQueryString(this string uri, string key, string value)
        {
            var result = QueryHelpers.AddQueryString(uri, key, value);
            return result;
        }

        /// <summary>
        /// Add the array of 'key' and 'value' query string parameters to the specified 'uri'.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="queryParams"></param>
        /// <returns>A new string object including the new query parameter.</returns>
        public static string AddQueryString(this string uri, params KeyValuePair<string, string>[] queryParams)
        {
            var items = QueryHelpers.ParseQuery(uri)
                .SelectMany(q => q.Value, (k, v) => new KeyValuePair<string, string>(k.Key, v));

            var qb = new QueryBuilder(items);
            foreach (var value in queryParams)
            {
                qb.Add(value.Key, value.Value);
            }

            return $"{uri.RemoveQueryString()}{qb.ToQueryString()}";
        }

        /// <summary>
        /// Add the array of 'key' and 'value' query string parameters to the specified 'uri'.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="queryParams"></param>
        /// <returns>A new string object including the new query parameter.</returns>
        public static string AddQueryString(this string uri, IDictionary<string, string> queryParams)
        {
            return QueryHelpers.AddQueryString(uri, queryParams);
        }

        /// <summary>
        /// Extract and remove the query string from the specified 'uri'.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string RemoveQueryString(this string uri)
        {
            return uri.Split("?")[0];
        }
    }
}
