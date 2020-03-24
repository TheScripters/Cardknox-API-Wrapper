using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// The Sale command is used to make a purchase on an EBTW cardholder's cash benefit account.
    /// </summary>
    public class EBTWSale : Sale
    {
        internal string Operation => "ebtw:sale";

        /// <summary>
        /// Items included in the sale transaction operation
        /// </summary>
        public EBTWItems Items { get; set; }
    }
    /// <summary>
    /// The Cash Balance enables a cash withdrawal from on an EBTW cardholder's cash benefit account.
    /// </summary>
    public class EBTWBalance : OperationBase
    {
        internal string Operation => "ebtw:balance";
    }
    /// <summary>
    /// Void a transaction
    /// </summary>
    public class EBTWVoid : OperationBase
    {
        internal string Operation => "ebtw:void";

        /// <summary>
        /// Used to reference a previous transaction when doing a follow-up transaction, typically a refund, void, or capture. (Note: xRefnum can be a 64-bit number and should be stored as BIGINT, Long, Int64 or String)
        /// </summary>
        public string RefNum { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class EBTWItems : IEnumerable<EBTWItem>
    {
        private List<EBTWItem> _items = new List<EBTWItem>();

        /// <summary>
        /// 
        /// </summary>
        public int Count => _items.Count;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(EBTWItem item) => _items.Add(item);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<EBTWItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class EBTWItem
    {
        /// <summary>
        /// Unit price for item specified in xUPC
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Quantity of item specified in xUPC.
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// Universal Product Code.
        /// </summary>
        public string Upc { get; set; }
    }
}
