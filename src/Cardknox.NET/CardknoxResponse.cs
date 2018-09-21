using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Cardknox
{
    public class CardknoxResponse
    {
        // xExp=1249

        public string Result { get; }
        public string Error { get; }
        public string Status { get; }
        public string ErrorCode { get; }
        public string RefNum { get; }
        public string AuthCode { get; }
        public string Batch { get; }
        public string AvsResultCode { get; }
        public string AvsResult { get; }
        public decimal? AuthAmount { get; }
        public string MaskedCardNumber { get; }
        public string CardType { get; }
        public string Token { get; }
        public string EntryMethod { get; }

        public CardknoxResponse(NameValueCollection _values)
        {
            if (_values.AllKeys.Contains("xResult"))
                Result = _values["xResult"];
            if (_values.AllKeys.Contains("xError"))
                Error = _values["xError"];
            if (_values.AllKeys.Contains("xErrorCode"))
                ErrorCode = _values["xErrorCode"];
            if (_values.AllKeys.Contains("xStatus"))
                Status = _values["xStatus"];
            if (_values.AllKeys.Contains("xRefNum"))
                RefNum = _values["xRefNum"];
            if (_values.AllKeys.Contains("xAuthCode"))
                AuthCode = _values["xAuthCode"];
            if (_values.AllKeys.Contains("xBatch"))
                Batch = _values["xBatch"];
            if (_values.AllKeys.Contains("xAvsResultCode"))
                AvsResultCode = _values["xAvsResultCode"];
            if (_values.AllKeys.Contains("xAvsResult"))
                AvsResult = HttpUtility.UrlDecode(_values["xAvsResult"]);
            if (_values.AllKeys.Contains("xAuthAmount"))
                AuthAmount = Decimal.Parse(_values["xAuthAmount"]);
            else AuthAmount = null;
            if (_values.AllKeys.Contains("xMaskedCardNumber"))
                MaskedCardNumber = _values["xMaskedCardNumber"];
            if (_values.AllKeys.Contains("xCardType"))
                CardType = _values["xCardType"];
            if (_values.AllKeys.Contains("xToken"))
                Token = _values["xToken"];
            if (_values.AllKeys.Contains("xEntryMethod"))
                EntryMethod = _values["xEntryMethod"];
        }
    }
}
