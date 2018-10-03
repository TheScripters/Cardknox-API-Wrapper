using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CCAuthOnly : CCSale
    {
        /// <summary>
        /// 
        /// </summary>
        internal new string Operation => "cc:authonly";
    }
}
