using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    public class CCRefund : OperationBase
    {
        public string Operation => CardknoxOperations.CCRefund;

        public string RefNum { get; set; }
        public bool AllowDuplicate { get; set; } = false;
    }
}
