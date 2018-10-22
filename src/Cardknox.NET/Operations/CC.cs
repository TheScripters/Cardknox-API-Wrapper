namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Adjust comand is used to change a previous authorization to a higher or lower amount. The refNumber from related authorization is required when submitting an Adjust.
    /// </summary>
    public class CCAdjust : OperationBase
    {
        internal string Operation => "cc:adjust";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCAuthOnly : Sale
    {
        internal string Operation => "cc:authonly";
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCCapture : Sale
    {
        internal string Operation => "cc:capture";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCCredit : Sale
    {
        internal string Operation => "cc:credit";
    }
    /// <summary>
    /// Some authorizations will require a voice authorization before they can be approved. When a verbal authorization is issued by the bank, that number can be sent with the PostAuth command to verify the authorization.
    /// </summary>
    public class CCPostAuth : Sale
    {
        internal string Operation => "cc:postauth";

        /// <summary>
        /// Verificaton number provided by the issuing bank to be used with the cc:postauth command.
        /// </summary>
        public string AuthCode { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCRefund : OperationBase
    {
        internal string Operation => "cc:refund";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowDuplicate { get; set; } = false;
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCSale : Sale
    {
        internal string Operation => "cc:sale";
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCSave : OperationBase
    {
        internal string Operation => "cc:save";
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCVoid : OperationBase
    {
        internal string Operation => "cc:void";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCVoidRefund : OperationBase
    {
        internal string Operation => "cc:voidrefund";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CCVoidRelease : OperationBase
    {
        internal string Operation => "cc:voidrelease";

        /// <summary>
        /// 
        /// </summary>
        public string RefNum { get; set; }
    }
}
