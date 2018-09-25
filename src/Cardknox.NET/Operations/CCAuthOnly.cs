using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    public class CCAuthOnly : CCSale
    {
        public new string Operation => CardknoxOperations.CCAuthOnly;
    }
}
