using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CCCredit : Sale
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Operation => "cc:credit";
    }
}
