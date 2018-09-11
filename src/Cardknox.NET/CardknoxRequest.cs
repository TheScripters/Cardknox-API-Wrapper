using System;
using System.Collections.Generic;
using System.Text;

namespace Cardknox.NET
{
    public class CardknoxRequest
    {
        private string _key { get; }
        private string _software { get; }
        private string _softwareVersion { get; }
        private string _cardknoxVersion { get; } = "4.5.5";
        private const string Url = "https://x1.cardknox.com/gateway";

        public CardknoxRequest(string key, string software, string softwareVersion, string cardknoxVer = null)
        {
            _key = key;
            _software = software;
            _softwareVersion = softwareVersion;
            if (cardknoxVer != null)
                _cardknoxVersion = cardknoxVer;
        }

        public static CardknoxRequest BeginRequest(string key, string software, string softwareVersion, string cardknoxVer = null)
        {
            CardknoxRequest r = new CardknoxRequest(key, software, softwareVersion, cardknoxVer);

            return r;
        }

        public CardknoxRequest BeginRequest()
        {


            return this;
        }
    }
}
