using CardknoxApi.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace CardknoxApi
{
    /// <summary>
    /// <para>Static class for logging communication to a file, specified in "Cardknox.LogLocation" in AppSettings</para>
    /// <para>Available config settings: Cardknox.Logging ("enabled" to enable logging) and Cardknox.LogLocation.</para>
    /// <para>Refer to <see href="https://github.com/TheScripters/Cardknox-API-Wrapper/wiki/Logging">https://github.com/TheScripters/Cardknox-API-Wrapper/wiki/Logging</see> for setup information</para>
    /// </summary>
    static class Log
    {
        /// <summary>
        /// Keys of sensitive data entries to skip logging for security reasons
        /// </summary>
        internal readonly static string[] NoInclude = { "xCardNum", "xMagStripe", "xExp", "xCVV", "xToken", "xDUKPT", "xRouting", "xAccount", "xMICR" };
        static bool EnableLogging
        {
            get
            {
                try
                {
                    return CardknoxConfiguration.LoggingEnabled;
                }
                catch { return false; }
            }
        }
        static string LogLocation
        {
            get
            {
                try
                {
                    return CardknoxConfiguration.LogLocation;
                }
                catch { return ""; }
            }
        }
        /// <summary>
        /// Log request values to file specified in <c>Cardknox.LogLocation</c> if <c>Cardknox.Logging</c> is set to <c>enabled</c>
        /// </summary>
        /// <param name="_values"></param>
        internal static void LogRequest(Dictionary<string, string> _values)
        {
            if (!EnableLogging || LogLocation == "")
                return;

            string loc = String.Format(LogLocation, DateTime.Now);
            loc = AppDomain.CurrentDomain.MapPath(loc);

            string body = "----" + DateTime.UtcNow.ToString("s") + "----" + Environment.NewLine;
            body += "Cardknox Request" + Environment.NewLine;
            body += "---------------------------" + Environment.NewLine;
            foreach (var k in _values.Keys)
            {
                if (!NoInclude.Contains(k))
                    body += k + " = " + _values[k] + Environment.NewLine;
            }

            byte[] data = Encoding.UTF8.GetBytes(body);
            using (FileStream fs = new FileStream(loc, FileMode.Append))
            {
                fs.Write(data, 0, data.Length);
            }
        }
        /// <summary>
        /// Log response values to file specified in <c>Cardknox.LogLocation</c> if <c>Cardknox.Logging</c> is set to <c>enabled</c>
        /// </summary>
        /// <param name="_values"></param>
        internal static void LogResponse(Dictionary<string, string> _values)
        {
            if (!EnableLogging || LogLocation == "")
                return;

            string loc = String.Format(LogLocation, DateTime.Now);
            loc = AppDomain.CurrentDomain.MapPath(loc);

            string body = "----" + DateTime.UtcNow.ToString("s") + "----" + Environment.NewLine;
            body += "Cardknox Response" + Environment.NewLine;
            body += "---------------------------" + Environment.NewLine;
            foreach (var k in _values.Keys)
            {
                body += k + " = " + _values[k] + Environment.NewLine;
            }
            body += Environment.NewLine; // add second newline at end of response for better separation

            byte[] data = Encoding.UTF8.GetBytes(body);
            using (FileStream fs = new FileStream(loc, FileMode.Append))
            {
                fs.Write(data, 0, data.Length);
            }
        }
    }
}
