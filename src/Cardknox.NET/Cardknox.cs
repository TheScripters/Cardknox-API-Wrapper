using Cardknox.Operations;
using System;
using System.Collections.Specialized;

namespace Cardknox
{
    public class Cardknox
    {
        /// <summary>
        /// 
        /// </summary>
        private CardknoxRequest _request { get; }
        private NameValueCollection _values { get; }

        //public Sale Sale;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">The <see cref="CardknoxRequest"/> object that is used to make the request.</param>
        public Cardknox(CardknoxRequest request)
        {
            _values = new NameValueCollection();
            _request = request;
        }

        public string CCSale(Sale _sale)
        {


            return "";
        }
    }
}
