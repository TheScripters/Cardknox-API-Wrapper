using CardknoxApi.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.RecurringOperations
{
    /// <summary>
    /// 
    /// </summary>
    public class CardknoxCustomer : Customer
    {
        /// <summary>
        /// Customer's Unique ID number. This field cannot be updated.
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// Indeicates the default payment method used to process the transaction
        /// </summary>
        public string DefaultPaymentMethodID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomerNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomerNotes { get; set; }

        /// <summary>
        /// Indicates whether to return deleted rows
        /// </summary>
        public bool? Removed { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public CardknoxCustomFields CustomFields { get; set; } = new CardknoxCustomFields();
    }
}
