using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi
{
    /// <summary>
    /// Single character indicating result
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Indicates the transaction was Approved.
        /// </summary>
        A,
        /// <summary>
        /// Indicates the transaction was Declined.
        /// </summary>
        D,
        /// <summary>
        /// Indicates the transaction had an Error. Refer to the <see cref="CardknoxResponse.Error"/> property for details.
        /// </summary>
        E
    }
    /// <summary>
    /// Indicates the status of the transaction
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// Indicates the transaction was Approved.
        /// </summary>
        Approved,
        /// <summary>
        /// Indicates the transaction was Declined.
        /// </summary>
        Declined,
        /// <summary>
        /// Indicates the transaction had an Error. Refer to the <see cref="CardknoxResponse.Error"/> property for details.
        /// </summary>
        Error
    }
    /// <summary>
    /// Card type.
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 
        /// </summary>
        Unknown,
        /// <summary>
        /// 
        /// </summary>
        EBT,
        /// <summary>
        /// 
        /// </summary>
        GiftCard,
        /// <summary>
        /// 
        /// </summary>
        Amex,
        /// <summary>
        /// 
        /// </summary>
        Visa,
        /// <summary>
        /// 
        /// </summary>
        MasterCard,
        /// <summary>
        /// 
        /// </summary>
        Discover,
        /// <summary>
        /// 
        /// </summary>
        Diners,
        /// <summary>
        /// 
        /// </summary>
        JCB
    }
    /// <summary>
    /// The Address Verification Service (AVS) response code
    /// </summary>
    public enum AvsResponseType
    {
        /// <summary>
        /// Address: Match &amp; 5 Digit Zip: Match
        /// </summary>
        YYY,
        /// <summary>
        /// Address: No Match &amp; 5 Digit Zip: Match
        /// </summary>
        NYZ,
        /// <summary>
        /// Address: Match &amp; 5 Digit Zip: No Match
        /// </summary>
        YNA,
        /// <summary>
        /// Address: No Match &amp; 5 Digit Zip: No Match
        /// </summary>
        NNN,
        /// <summary>
        /// Address Information not verified for domestic transaction
        /// </summary>
        XXU,
        /// <summary>
        /// Address: Match &amp; 9 Digit Zip: Match
        /// </summary>
        YYX,
        /// <summary>
        /// Address: No Match &amp; 9 Digit Zip: Match
        /// </summary>
        NYW,
        /// <summary>
        /// Retry / System Unavailable
        /// </summary>
        XXR,
        /// <summary>
        /// Service Not Supported
        /// </summary>
        XXS
    }
    /// <summary>
    /// Specifies if the order origin is Internet OR Phone for FraudWatch verification.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Order originated via Internet
        /// </summary>
        Internet,
        /// <summary>
        /// Order originated via Phone
        /// </summary>
        Phone
    }
    /// <summary>
    /// Specifies the currency type for this transaction on applicable accounts
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>
        /// 
        /// </summary>
        USD,
        /// <summary>
        /// 
        /// </summary>
        GBP,
        /// <summary>
        /// 
        /// </summary>
        ILS,
        /// <summary>
        /// 
        /// </summary>
        EUR,
        /// <summary>
        /// 
        /// </summary>
        MXN,
        /// <summary>
        /// 
        /// </summary>
        CAD
    }
}
