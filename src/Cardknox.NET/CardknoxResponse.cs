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
    /// <remarks>See <see href="https://docs.cardknox.com/#response-parameters"/> for a list and details of all parameters</remarks>
    public class CardknoxResponse
    {
        /// <summary>
        /// Single character code indicating if the transaction was Approved or not. A = Approved E = Error D = Declined
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
        /// <remarks>See <see href="https://docs.cardknox.com/api/error-codes"/> for a list of all error codes</remarks>
        public string ErrorCode { get; }
        /// <summary>
        /// <para>Cardknox transaction Reference Number</para>
        /// <para>Note: RefNum is always returned regardless of the outcome of the transaction.</para>
        /// </summary>
        public string RefNum { get; }
        /// <summary>
        /// Invoice number
        /// </summary>
        public string Invoice { get; }
        /// <summary>
        /// The card expiration number (if applicable)
        /// </summary>
        public string Exp { get; }
        /// <summary>
        /// Authorization code, for approved transactions only
        /// </summary>
        public string AuthCode { get; }
        /// <summary>
        /// Batch into which the transaction will settle
        /// </summary>
        public string Batch { get; }
        /// <summary>
        /// The Address Verification Service (AVS) response code
        /// </summary>
        public AvsResponseType AvsResultCode { get; }
        /// <summary>
        /// Raw contents of <see cref="AvsResultCode"/>
        /// </summary>
        public string AvsResultCodeString { get; }
        /// <summary>
        /// AVS response verbiage
        /// </summary>
        public string AvsResult { get; }
        /// <summary>
        /// Card code verification (CCV) response code
        /// </summary>
        public CvvResultCodeType CvvResultCode { get; }
        /// <summary>
        /// Raw contents of <see cref="CvvResultCode"/>
        /// </summary>
        public string CvvResultCodeString { get; }
        /// <summary>
        /// CVV result verbiage
        /// </summary>
        public string CvvResult { get; }
        /// <summary>
        /// The total amount authorized, inclusive of tax and tip (if applicable)
        /// </summary>
        public decimal? AuthAmount { get; }
        /// <summary>
        /// Raw contents of <see cref="AuthAmount"/>
        /// </summary>
        public string AuthAmountString { get; }
        /// <summary>
        /// A masked version of the credit card used for the transaction
        /// </summary>
        public string MaskedCardNumber { get; }
        /// <summary>
        /// Type of credit card used for the transaction
        /// </summary>
        public CardType CardType { get; }
        /// <summary>
        /// Raw contents of <see cref="CardType"/>
        /// </summary>
        public string CardTypeString { get; }
        /// <summary>
        /// Name of cardholder
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Token returned for use with future transaction
        /// </summary>
        public string Token { get; }
        /// <summary>
        /// Currency that the transaction was processed in
        /// </summary>
        public CurrencyType Currency { get; }
        /// <summary>
        /// Currency that the transaction was processed in
        /// (Raw value of <see cref="Currency"/>)
        /// </summary>
        public string CurrencyString { get; }
        /// <summary>
        /// Transaction method
        /// </summary>
        public string EntryMethod { get; }
        /// <summary>
        /// Date and time the transaction was processed
        /// </summary>
        public DateTime Date { get; }
        /// <summary>
        /// Raw value of <see cref="Date"/>
        /// </summary>
        public string DateString { get; }
        /// <summary>
        /// Current reference number
        /// </summary>
        /// <remarks>
        /// <b>Used for troubleshooting purposes only.</b> Returned when using a command that modifies an existing transaction, such as cc:void, cc:capture, or cc:adjust.
        /// </remarks>
        public string RefnumCurrent { get; }
        /// <summary>
        /// Indicates if the response contains an error
        /// </summary>
        /// <returns><c>true</c> if the response contained any error values</returns>
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
            if (_values.AllKeys.Contains("xInvoice"))
                Invoice = _values["xInvoice"];
            if (_values.AllKeys.Contains("xExp"))
                Exp = _values["xExp"];
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
            if (_values.AllKeys.Contains("xCvvResultCode"))
            {
                CvvResultCodeString = _values["xCvvResultCode"];
                try
                {
                    CvvResultCode = (CvvResultCodeType)Enum.Parse(typeof(CvvResultCodeType), _values["xCvvResultCode"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xCvvResult"))
                CvvResult = _values["xCvvResult"];
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
            if (_values.AllKeys.Contains("xName"))
                Name = _values["xName"];
            if (_values.AllKeys.Contains("xToken"))
                Token = _values["xToken"];
            if (_values.AllKeys.Contains("xCurrency"))
            {
                CurrencyString = _values["xCurrency"];
                try
                {
                    Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), _values["xCurrency"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xEntryMethod"))
                EntryMethod = _values["xEntryMethod"];
            if (_values.AllKeys.Contains("xDate"))
            {
                DateString = _values["xDate"];
                try
                {
                    Date = Convert.ToDateTime(_values["xDate"]);
                }
                catch { }
            }
        }
    }
}
