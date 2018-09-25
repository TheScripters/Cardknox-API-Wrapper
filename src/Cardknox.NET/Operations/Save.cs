using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    public class Save : OperationBase
    {
        public string Operation => CardknoxOperations.CCSave;
    }
}
