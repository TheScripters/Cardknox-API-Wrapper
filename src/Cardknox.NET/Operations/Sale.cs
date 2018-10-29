using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// Serves as the base for making a sale
    /// </summary>
    public class Sale : OperationBase
    {
        /// <summary>
        /// True/False value indicating if an authorization amount is less than the initial request when full initial amount is unavailable. This variable is platform-dependent. Default is false.
        /// </summary>
        public bool AllowPartialAuth { get; set; } = false;
        /// <summary>
        /// Specifies qualifying prescription amount for FSA transactions. xAllowPartialAuth must be set to True.
        /// </summary>
        public decimal RxAmount { get; set; } = 0;
        /// <summary>
        /// Specifies qualifying dental amount for FSA transactions. xAllowPartialAuth must be set to True.
        /// </summary>
        public decimal DentalAmount { get; set; } = 0;
        /// <summary>
        /// Specifies qualifying vision amount for FSA transactions. xAllowPartialAuth must be set to True.
        /// </summary>
        public decimal VisionAmount { get; set; } = 0;
        /// <summary>
        /// Specifies qualifying transit amount for commuter card transactions. xAllowPartialAuth must be set to True.
        /// </summary>
        public decimal TransitAmount { get; set; } = 0;
        /// <summary>
        /// Specifies copay amount for FSA transactions. xAllowPartialAuth must be set to True.
        /// </summary>
        public decimal CopayAmount { get; set; } = 0;
        /// <summary>
        /// Specifies qualifying clinical amount for FSA transactions. xAllowPartialAuth must be set to True.
        /// </summary>
        public decimal ClinicalAmount { get; set; } = 0;

        /// <summary>
        /// Unique Order Number for FraudWatch verification.
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// Yes/No value indicating if customer is a repeat customer.
        /// </summary>
        public bool ExistingCustomer { get; set; } = false;
    }
}
