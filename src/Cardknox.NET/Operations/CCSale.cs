using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CCSale : Sale
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operation => "cc:sale";
    }
}
