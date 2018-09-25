using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    public class CCCapture : CCSale
    {
        public new string Operation => CardknoxOperations.CCCapture;
    }
}
