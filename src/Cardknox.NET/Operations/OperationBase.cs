using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationBase : Customer
    {
        public string CardNum { get; set; }
        public string CVV { get; set; }
        public string Exp { get; set; }
        public decimal? Amount { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string MagStripe { get; set; }
        public bool CardPresent { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Tip { get; set; }
        public string Invoice { get; set; }
        public string IP { get; set; }
    }
}
