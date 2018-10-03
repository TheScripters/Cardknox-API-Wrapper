using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationBase : Customer
    {
        /// <summary>
        /// 
        /// </summary>
        public string CardNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CVV { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Exp { get; set; }
        /// <summary>
        /// Checking Account Routing number
        /// </summary>
        public string Routing { get; set; }
        /// <summary>
        /// Checking Account number
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// The total amount of the transaction, inclusive of tax and tip if applicable. This the total amount of the transaction.
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Zip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MagStripe { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool CardPresent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Tax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Tip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Invoice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }
    }
}
