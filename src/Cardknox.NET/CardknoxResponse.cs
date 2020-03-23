using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace CardknoxApi
{
    /// <summary>
    /// Object representing response from the Cardknox API endpoint.
    /// </summary>
    public class CardknoxResponse
    {
        // xExp=1249

        /// <summary>
        /// Single character representing status
        /// </summary>
        public ResultType Result { get; }
        /// <summary>
        /// Raw contents of <see cref="Result"/>
        /// </summary>
        public string ResultString { get; }
        /// <summary>
        /// Contains error message if the transaction failed. <see cref="Status"/> will have the value of <see cref="StatusType.Error"/> and <see cref="Result"/> will be <see cref="ResultType.E"/>
        /// </summary>
        public string Error { get; }
        /// <summary>
        /// Status of transction
        /// </summary>
        public StatusType Status { get; }
        /// <summary>
        /// Raw contents of <see cref="Status"/>
        /// </summary>
        public string StatusString { get; }
        /// <summary>
        /// Contains error code if the transaction failed. <see cref="Status"/> will have the value of <see cref="StatusType.Error"/> and <see cref="Result"/> will be <see cref="ResultType.E"/>
        /// </summary>
        public string ErrorCode { get; }
        /// <summary>
        /// <para>Cardknox transaction Reference Number</para>
        /// <para>Note: RefNum is always returned regardless of the outcome of the transaction.</para>
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
        public AvsResponseType AvsResultCode { get; }
        /// <summary>
        /// Raw contents of <see cref="AvsResultCode"/>
        /// </summary>
        public string AvsResultCodeString { get; }
        /// <summary>
        /// 
        /// </summary>
        public string AvsResult { get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? AuthAmount { get; }
        /// <summary>
        /// Raw contents of <see cref="AuthAmount"/>
        /// </summary>
        public string AuthAmountString { get; }
        /// <summary>
        /// 
        /// </summary>
        public string MaskedCardNumber { get; }
        /// <summary>
        /// 
        /// </summary>
        public CardType CardType { get; }
        /// <summary>
        /// Raw contents of <see cref="CardType"/>
        /// </summary>
        public string CardTypeString { get; }
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
            {
                ResultString = _values["xResult"];
                try
                {
                    Result = (ResultType)Enum.Parse(typeof(ResultType), _values["xResult"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xError"))
                Error = _values["xError"];
            if (_values.AllKeys.Contains("xErrorCode"))
                ErrorCode = _values["xErrorCode"];
            if (_values.AllKeys.Contains("xStatus"))
            {
                StatusString = _values["xStatus"];
                try
                {
                    Status = (StatusType)Enum.Parse(typeof(StatusType), _values["xStatus"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xRefNum"))
                RefNum = _values["xRefNum"];
            if (_values.AllKeys.Contains("xAuthCode"))
                AuthCode = _values["xAuthCode"];
            if (_values.AllKeys.Contains("xBatch"))
                Batch = _values["xBatch"];
            if (_values.AllKeys.Contains("xAvsResultCode"))
            {
                AvsResultCodeString = _values["xAvsResultCode"];
                try
                {
                    AvsResultCode = (AvsResponseType)Enum.Parse(typeof(AvsResponseType), _values["xAvsResultCode"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xAvsResult"))
                AvsResult = HttpUtility.UrlDecode(_values["xAvsResult"]);
            if (_values.AllKeys.Contains("xAuthAmount"))
            {
                AuthAmountString = _values["xAuthAmount"];
                try
                {
                    AuthAmount = Decimal.Parse(_values["xAuthAmount"]);
                }
                catch { }
            }
            else AuthAmount = null;
            if (_values.AllKeys.Contains("xMaskedCardNumber"))
                MaskedCardNumber = _values["xMaskedCardNumber"];
            if (_values.AllKeys.Contains("xCardType"))
            {
                CardTypeString = _values["xCardType"];
                try
                {
                    CardType = (CardType)Enum.Parse(typeof(CardType), _values["xCardType"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xToken"))
                Token = _values["xToken"];
            if (_values.AllKeys.Contains("xEntryMethod"))
                EntryMethod = _values["xEntryMethod"];
        }
    }
}
