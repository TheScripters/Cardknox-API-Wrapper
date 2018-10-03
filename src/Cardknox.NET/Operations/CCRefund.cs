using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CCRefund : OperationBase
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Operation => "cc:refund";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowDuplicate { get; set; } = false;
    }
}
