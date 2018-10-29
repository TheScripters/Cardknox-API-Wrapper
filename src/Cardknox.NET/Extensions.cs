using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CardknoxApi
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(this AppDomain domain, string path)
        {
            if (!path.StartsWith("~/"))
                return path;

            return Path.Combine(domain.BaseDirectory, path.Replace("~/", ""));
        }
    }
}
