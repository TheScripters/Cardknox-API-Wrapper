using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// Some authorizations will require a voice authorization before they can be approved. When a verbal authorization is issued by the bank, that number can be sent with the PostAuth command to verify the authorization.
    /// </summary>
    public class CCPostAuth : Sale
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operation => "cc:postauth";

        /// <summary>
        /// Verificaton number provided by the issuing bank to be used with the cc:postauth command.
        /// </summary>
        public string AuthCode { get; set; }
    }
}
