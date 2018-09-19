using System;
using System.Collections.Generic;
using System.Text;

namespace Cardknox
{
    public class Customer
    {
        public string BillFirstName { get; set; }
        public string BillMiddleName { get; set; }
        public string BillLastName { get; set; }
        public string BillCompany { get; set; }
        public string BillStreet { get; set; }
        public string BillStreet2 { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillZip { get; set; }
        public string BillCountry { get; set; } = "USA";
        public string BillPhone { get; set; }
        public string BillMobile { get; set; }

        public string ShipFirstName { get; set; }
        public string ShipMiddleName { get; set; }
        public string ShipLastName { get; set; }
        public string ShipCompany { get; set; }
        public string ShipStreet { get; set; }
        public string ShipStreet2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public string ShipCountry { get; set; } = "USA";
        public string ShipPhone { get; set; }
        public string ShipMobile { get; set; }
    }
}
