using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi
{
    /// <summary>
    /// 
    /// </summary>
    public class RecurringRequest
    {
        internal string Key { get; }
        internal string Software { get; }
        internal string SoftwareVersion { get; }
        internal string CardknoxVersion { get; } = "1.0.0";
        internal const string _url = "https://api.cardknox.com/recurring";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Your Cardknox API Key.</param>
        /// <param name="software">Name of your software.</param>
        /// <param name="softwareVersion">Version number of your software.</param>
        /// <param name="cardknoxVer">Gateway API Version.</param>
        public RecurringRequest(string key, string software, string softwareVersion, string cardknoxVer = null)
        {
            Key = key;
            Software = software;
            SoftwareVersion = softwareVersion;
            if (cardknoxVer != null)
                CardknoxVersion = cardknoxVer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Your Cardknox API Key.</param>
        /// <param name="software">Name of your software.</param>
        /// <param name="softwareVersion">Version number of your software.</param>
        /// <param name="cardknoxVer">Gateway API Version.</param>
        /// <returns></returns>
        public static RecurringRequest BeginRequest(string key, string software, string softwareVersion, string cardknoxVer = null)
        {
            return new RecurringRequest(key, software, softwareVersion, cardknoxVer);
        }
    }
}
