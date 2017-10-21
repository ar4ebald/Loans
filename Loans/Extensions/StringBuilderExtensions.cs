using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Loans.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendQuery<T>(this StringBuilder sb, string key, T value, bool first = false)
        {
            sb.Append(first ? '?' : '&')
                .Append(WebUtility.UrlEncode(key))
                .Append('=')
                .Append(WebUtility.UrlEncode(value?.ToString() ?? ""));

            return sb;
        }
    }
}
