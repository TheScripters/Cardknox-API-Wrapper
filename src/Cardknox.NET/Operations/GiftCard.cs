using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Issue command is used to issue funds to a Cardknox gift card.
    /// </summary>
    public class GCIssue : Sale
    {
        internal string Operation => "gift:issue";
    }
    /// <summary>
    /// The Redeem command is used to debit funds from a Cardknox gift card.
    /// </summary>
    public class GCRedeem : Sale
    {
        internal string Operation => "gift:redeem";
    }
    /// <summary>
    /// The Balance command is used to check the available balance on a Cardknox gift card.
    /// </summary>
    public class GCBalance : Sale
    {
        internal string Operation => "gift:balance";
    }
}
