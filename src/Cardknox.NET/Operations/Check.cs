namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Save command is used to send account information and request a token from Cardknox, but does not submit the transaction for processing. The response returns a token which references that account information. A token at a minimum references the account and routing number, but if other data is sent, such as billing address, that will be associated with the token as well.
    /// </summary>
    public class CheckSave : OperationBase
    {
        internal string Operation => "check:save";
    }
    /// <summary>
    /// The Check Sale command debits funds from a customer’s checking or savings account using the account and routing number. The merchant must have a supported Check/ACH processing account
    /// </summary>
    public class CheckSale : Sale
    {
        internal string Operation => "check:sale";
    }
    /// <summary>
    /// The Void command voids a check transaction that is pending being sent to the bank, typically at the end of each day.
    /// </summary>
    public class CheckVoid : OperationBase
    {
        internal string Operation => "check:void";

        /// <summary>
        /// Used to reference a previous transaction when doing a follow-up transaction, typically a refund, void, or capture. (Note: xRefnum can be a 64-bit number and should be stored as BIGINT, Long, Int64 or String)
        /// </summary>
        public string RefNum { get; set; }
    }
    /// <summary>
    /// The Credit command sends money from a merchant to a customer’s bank account that is not linked to any previous transaction. With check transactions, this is commonly used for paying 3rd-parties such as paying vendors. To refund a previous check sale, use Check:Refund instead.
    /// </summary>
    public class CheckCredit : Sale
    {
        internal string Operation => "check:credit";
    }

    /// <summary>
    /// The Refund command is used to refund a full or partial refund of a previous settled check transaction, using xRefNum
    /// </summary>
    public class CheckRefund : OperationBase
    {
        internal string Operation => "check:refund";

        /// <summary>
        /// Used to reference a previous transaction when doing a follow-up transaction, typically a refund, void, or capture. (Note: xRefnum can be a 64-bit number and should be stored as BIGINT, Long, Int64 or String)
        /// </summary>
        public string RefNum { get; set; }
    }
}
