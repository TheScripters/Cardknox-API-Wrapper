using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi
{
    /// <summary>
    /// 
    /// </summary>
    public class CardknoxRequest
    {
        internal string _key { get; }
        internal string _software { get; }
        internal string _softwareVersion { get; }
        internal string _cardknoxVersion { get; } = "4.5.5";
        internal const string _url = "https://x1.cardknox.com/gateway";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="software"></param>
        /// <param name="softwareVersion"></param>
        /// <param name="cardknoxVer"></param>
        public CardknoxRequest(string key, string software, string softwareVersion, string cardknoxVer = null)
        {
            _key = key;
            _software = software;
            _softwareVersion = softwareVersion;
            if (cardknoxVer != null)
                _cardknoxVersion = cardknoxVer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="software"></param>
        /// <param name="softwareVersion"></param>
        /// <param name="cardknoxVer"></param>
        /// <returns></returns>
        public static CardknoxRequest BeginRequest(string key, string software, string softwareVersion, string cardknoxVer = null)
        {
            CardknoxRequest r = new CardknoxRequest(key, software, softwareVersion, cardknoxVer);

            return r;
        }
    }
}
