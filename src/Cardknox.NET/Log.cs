using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace CardknoxApi
{
    static class Log
    {
        readonly static string[] NoInclude = { "xCardNum", "xMagStripe", "xExp", "xCVV", "xToken", "xDUKPT", "xRouting", "xAccount", "xMICR" };
        static bool EnableLogging
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["Cardknox.Logging"] == "enabled";
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
                    return ConfigurationManager.AppSettings["Cardknox.LogLocation"];
                }
                catch { return ""; }
            }
        }
        public static void LogRequest(NameValueCollection _values)
        {
            if (!EnableLogging || LogLocation == "")
                return;

            string loc = String.Format(LogLocation, DateTime.Now);
            if (LogLocation.StartsWith("~"))
                loc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, loc.Replace("~/", ""));

            string body = "----" + DateTime.UtcNow.ToString("s") + "----" + Environment.NewLine;
            body += "Cardknox Request" + Environment.NewLine;
            body += "---------------------------" + Environment.NewLine;
            foreach (var k in _values.AllKeys)
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
        public static void LogResponse(NameValueCollection _values)
        {
            if (!EnableLogging || LogLocation == "")
                return;

            string loc = String.Format(LogLocation, DateTime.Now);
            if (LogLocation.StartsWith("~"))
                loc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, loc.Replace("~/", ""));

            string body = "----" + DateTime.UtcNow.ToString("s") + "----" + Environment.NewLine;
            body += "Cardknox Response" + Environment.NewLine;
            body += "---------------------------" + Environment.NewLine;
            foreach (var k in _values.AllKeys)
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
