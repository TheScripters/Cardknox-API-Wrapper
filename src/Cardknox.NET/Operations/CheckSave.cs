using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Save command is used to send account information and request a token from Cardknox, but does not submit the transaction for processing. The response returns a token which references that account information. A token at a minimum references the account and routing number, but if other data is sent, such as billing address, that will be associated with the token as well.
    /// </summary>
    public class CheckSave : OperationBase
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Operation => "check:save";
    }
}
