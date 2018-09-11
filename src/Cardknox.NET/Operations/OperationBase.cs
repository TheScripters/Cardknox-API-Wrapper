using System;
using System.Collections.Generic;
using System.Text;

namespace Cardknox.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationBase
    {
        string CardNum { get; set; }
        string CVV { get; set; }
        string Exp { get; set; }
        decimal? Amount { get; set; }
        string Name { get; set; }
        string Token { get; set; }
        string Street { get; set; }
        string Zip { get; set; }
        string MagStripe { get; set; }
        bool CardPresent { get; set; }
        decimal? Tax { get; set; }
        decimal? Tip { get; set; }
        string Invoice { get; set; }
        string IP { get; set; }
    }
}
