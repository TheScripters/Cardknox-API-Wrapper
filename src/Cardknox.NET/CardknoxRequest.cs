using System;
using System.Collections.Generic;
using System.Text;

namespace Cardknox
{
    /// <summary>
    /// 
    /// </summary>
    public class CardknoxRequest
    {
        private string _key { get; }
        private string _software { get; }
        private string _softwareVersion { get; }
        private string _cardknoxVersion { get; } = "4.5.5";
        private const string Url = "https://x1.cardknox.com/gateway";

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CardknoxRequest BeginRequest()
        {


            return this;
        }
    }
}
