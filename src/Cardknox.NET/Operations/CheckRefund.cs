using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Refund command is used to refund a full or partial refund of a previous settled check transaction, using xRefNum
    /// </summary>
    public class CheckRefund : OperationBase
    {
        internal string Operation => "check:refund";

        /// <summary>
        /// Used to reference a previous transaction when doing a follow-up transaction, typically a refund, void, or capture. (Note: xRefnum can be a 64-bit number and should be stored as BIGINT, Long, Int64 or String)
        /// </summary>
        public string RefNum { get; set; }
    }
}
