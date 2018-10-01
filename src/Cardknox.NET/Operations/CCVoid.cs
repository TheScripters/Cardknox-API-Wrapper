using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CCVoid : OperationBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operation => "cc:void";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
}
