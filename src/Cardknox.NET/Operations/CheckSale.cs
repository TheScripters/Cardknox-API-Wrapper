using System;
namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Check Sale command debits funds from a customer’s checking or savings account using the account and routing number. The merchant must have a supported Check/ACH processing account
    /// </summary>
    public class CheckSale : Sale
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Operation => "check:sale";
    }
}