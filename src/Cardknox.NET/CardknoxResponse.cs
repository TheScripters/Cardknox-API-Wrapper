using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace CardknoxApi
{
    /// <summary>
    /// 
    /// </summary>
    public class CardknoxResponse
    {
        // xExp=1249

        /// <summary>
        /// 
        /// </summary>
        public ResultType Result { get; }
        /// <summary>
        /// 
        /// </summary>
        public string Error { get; }
        /// <summary>
        /// 
        /// </summary>
        public StatusType Status { get; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorCode { get; }
        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; }
        /// <summary>
        /// 
        /// </summary>
        public string AuthCode { get; }
        /// <summary>
        /// 
        /// </summary>
        public string Batch { get; }
        /// <summary>
        /// 
        /// </summary>
        public string AvsResultCode { get; }
        /// <summary>
        /// 
        /// </summary>
        public string AvsResult { get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? AuthAmount { get; }
        /// <summary>
        /// 
        /// </summary>
        public string MaskedCardNumber { get; }
        /// <summary>
        /// 
        /// </summary>
        public CardType CardType { get; }
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; }
        /// <summary>
        /// 
        /// </summary>
        public string EntryMethod { get; }
        /// <summary>
        /// 
        /// </summary>
        public bool HasError { get { return !String.IsNullOrWhiteSpace(Error) || Status == StatusType.Error || Result == ResultType.E; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_values"></param>
        public CardknoxResponse(NameValueCollection _values)
        {
            if (_values.AllKeys.Contains("xResult"))
                Result = (ResultType)Enum.Parse(typeof(ResultType), _values["xResult"]);
            if (_values.AllKeys.Contains("xError"))
                Error = _values["xError"];
            if (_values.AllKeys.Contains("xErrorCode"))
                ErrorCode = _values["xErrorCode"];
            if (_values.AllKeys.Contains("xStatus"))
                Status = (StatusType)Enum.Parse(typeof(StatusType), _values["xStatus"]);
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
                CardType = (CardType)Enum.Parse(typeof(CardType), _values["xCardType"]);
            if (_values.AllKeys.Contains("xToken"))
                Token = _values["xToken"];
            if (_values.AllKeys.Contains("xEntryMethod"))
                EntryMethod = _values["xEntryMethod"];
        }
    }
}
