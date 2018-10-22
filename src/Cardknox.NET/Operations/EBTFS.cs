using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Sale command is used to make a purchase on an EBT cardholder's food stamp account.
    /// </summary>
    public class EBTFSSale : Sale
    {
        internal string Operation => "ebtfs:sale";
    }
    /// <summary>
    /// The Credit command is used to credit to an EBT cardholder's food stamp account.
    /// </summary>
    public class EBTFSCredit : Sale
    {
        internal string Operation => "ebtfs:credit";
    }
    /// <summary>
    /// The Balance command is used to check the balance on an EBT food stamp card.
    /// </summary>
    public class EBTFSBalance : OperationBase
    {
        internal string Operation => "ebtfs:balance";
    }
    /// <summary>
    /// The Voucher command is used to process manual EBT food stamp voucher.
    /// </summary>
    public class EBTFSVoucher : Sale
    {
        internal string Operation => "ebtf:voucher";

        /// <summary>
        /// The EBT voucher approval code.
        /// </summary>
        public string VoucherApproval { get; set; }
        /// <summary>
        /// The EBT voucher serial number.
        /// </summary>
        public string VoucherSerial { get; set; }
    }
}
