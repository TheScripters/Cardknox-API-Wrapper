using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Sale command is used to make a purchase on an EBT cardholder's cash benefit account.
    /// </summary>
    public class EBTCBSale : Sale
    {
        internal string Operation => "ebtcb:sale";
    }
    /// <summary>
    /// The Cash command enables a cash withdrawal from on an EBT cardholder's cash benefit account.
    /// </summary>
    public class EBTCBCash : OperationBase
    {
        internal string Operation => "ebtcb:cash";
    }
    /// <summary>
    /// The Balance command is used to check the balance on an EBT cash benefit account.
    /// </summary>
    public class EBTCBBalance : OperationBase
    {
        internal string Operation => "ebtcb:balance";
    }
}
