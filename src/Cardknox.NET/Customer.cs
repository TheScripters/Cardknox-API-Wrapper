using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// The customer's email address.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The customer's fax number.
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BillFirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BillMiddleName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillLastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillCompany { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillStreet { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillStreet2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillZip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillCountry { get; set; } = "USA";
        /// <summary>
        /// 
        /// </summary>
        public string BillPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillMobile { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string ShipFirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipMiddleName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipLastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipCompany { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipStreet { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipStreet2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipZip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipCountry { get; set; } = "USA";
        /// <summary>
        /// 
        /// </summary>
        public string ShipPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipMobile { get; set; }
    }
}