using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    public class CCCredit : CCSale
    {
        public new string Operation => CardknoxOperations.CCCredit;
    }
}
