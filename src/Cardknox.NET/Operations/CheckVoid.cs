using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Void command voids a check transaction that is pending being sent to the bank, typically at the end of each day.
    /// </summary>
    public class CheckVoid : OperationBase
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Operation => "check:void";

        /// <summary>
        /// Used to reference a previous transaction when doing a follow-up transaction, typically a refund, void, or capture. (Note: xRefnum can be a 64-bit number and should be stored as BIGINT, Long, Int64 or String)
        /// </summary>
        public string RefNum { get; set; }
    }
}
