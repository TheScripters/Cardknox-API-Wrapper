using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CCVoidRefund : OperationBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operation => "cc:voidrefund";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CCVoidRelease : OperationBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operation => "cc:voidrelease";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
}
