using Cardknox.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;

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
            _values.Add("xKey", request._key);
            _values.Add("xVersion", request._cardknoxVersion);
            _values.Add("xSoftwareName", request._software);
            _values.Add("xSoftwareVersion", request._softwareVersion);
            _request = request;
        }

        #region credit card

        public CardknoxResponse CCSale(Sale _sale, bool force = false)
        {
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // Required information
            _values.Add("xCommand", _sale.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _sale.Amount));
            // These groups are mutually exclusive
            if (!String.IsNullOrWhiteSpace(_sale.CardNum))
            {
                _values.Add("xCardNum", _sale.CardNum);
                _values.Add("xCVV", _sale.CVV);
                _values.Add("xExp", _sale.Exp);
            }
            else if (!String.IsNullOrWhiteSpace(_sale.Token))
            {
                _values.Add("xToken", _sale.Token);
            }
            else if (!String.IsNullOrWhiteSpace(_sale.MagStripe))
            {
                _values.Add("xMagStripe", _sale.MagStripe);
            }
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!String.IsNullOrWhiteSpace(_sale.Street))
                _values.Add("xStreet", _sale.Street);

            if (!String.IsNullOrWhiteSpace(_sale.Zip))
                _values.Add("xZip", _sale.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!String.IsNullOrWhiteSpace(_sale.IP))
                _values.Add("xIP", _sale.IP);

            if (!String.IsNullOrWhiteSpace(_sale.Invoice))
                _values.Add("xInvoice", _sale.Invoice);



            var resp = MakeRequest();
            return new CardknoxResponse(resp);
        }

        #endregion

        private NameValueCollection MakeRequest()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient c = new WebClient();
            string req = System.Text.Encoding.ASCII.GetString(c.UploadValues(CardknoxRequest._url, _values));
            NameValueCollection resp = HttpUtility.ParseQueryString(req);

            return resp;
        }
    }
}
