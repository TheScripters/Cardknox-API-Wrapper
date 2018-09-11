using System;
using System.Collections.Generic;
using System.Text;

namespace Cardknox.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Sale : OperationBase
    {
        public string Operation => CardknoxOperations.CCSale;
    }
}
