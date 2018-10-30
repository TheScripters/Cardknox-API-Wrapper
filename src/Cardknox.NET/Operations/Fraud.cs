using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Submit command is used in conjunction with a valid FraudWatch account to submit ecommerce transactions for a fraud verification check.
    /// </summary>
    public class FraudSubmit : Sale
    {
        internal string Operation => "fraud:submit";

        /// <summary>
        /// Masked Card number with BIN and last 4 digits exposed
        /// </summary>
        public new string CardNum { get; set; }

        /// <summary>
        /// Transaction RefNum received from Gateway for FraudWatch verification.
        /// </summary>
        public string GatewayRefNum { get; set; }
        /// <summary>
        /// Transaction status received from gateway for FraudWatch verification.
        /// </summary>
        public StatusType GatewayResult { get; set; }
        /// <summary>
        /// CVV for for FraudWatch verification. (M or N)
        /// </summary>
        public string GatewayCVV { get; set; }
        /// <summary>
        /// Street Address for FraudWatch verification.
        /// </summary>
        public AvsResponseType GatewayAVS { get; set; }
        /// <summary>
        /// Transaction RefNum received from Gateway for FraudWatch verification.
        /// </summary>
        public string GatewayError { get; set; }
        /// <summary>
        /// Specifies if the order origin is Internet OR Phone for FraudWatch verification.
        /// </summary>
        public OrderType OrderType { get; set; }
    }
}
