using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CCCapture : Sale
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Operation => "cc:capture";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
}
