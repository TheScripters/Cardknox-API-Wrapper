using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Adjust comand is used to change a previous authorization to a higher or lower amount. The refNumber from related authorization is required when submitting an Adjust.
    /// </summary>
    public class CCAdjust : OperationBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operation => "cc:adjust";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
}
