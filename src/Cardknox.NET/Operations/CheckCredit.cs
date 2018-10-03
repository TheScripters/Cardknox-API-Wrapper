using System;
namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Credit command sends money from a merchant to a customer’s bank account that is not linked to any previous transaction. With check transactions, this is commonly used for paying 3rd-parties such as paying vendors. To refund a previous check sale, use Check:Refund instead.
    /// </summary>
    public class CheckCredit : Sale
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operation => "check:credit";
    }
}
