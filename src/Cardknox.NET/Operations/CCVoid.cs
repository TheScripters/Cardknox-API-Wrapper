using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    public class CCVoid : OperationBase
    {
        public string Operation => CardknoxOperations.CCVoid;

        public string RefNum { get; set; }
    }
}
