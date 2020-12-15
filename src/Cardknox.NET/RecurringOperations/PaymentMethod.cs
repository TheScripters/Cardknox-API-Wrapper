using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.RecurringOperations
{
    /// <summary>
    /// Use this command to add a new payment method to a customer's account profile.
    /// </summary>
    public class PaymentMethodAdd
    {
        internal string Operation => "paymentmethod:add";

        /// <summary>
        /// Customer's unique ID number
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// Cardknox token that references a previously used payment method to use for the charge
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// <para>The xToken payment type</para>
        /// <para>Valid Values:</para>
        /// <list type="bullet">
        /// <item>CC (Credit Card)</item>
        /// <item>Check (Check)</item>
        /// <item>Ach</item>
        /// </list>
        /// </summary>
        public string TokenType { get; set; }
        /// <summary>
        /// Custom name for the xToken
        /// </summary>
        public string TokenAlias { get; set; }
        /// <summary>
        /// Credit card expiration date
        /// </summary>
        public string Exp { get; set; }
        /// <summary>
        /// ACH payment routing number
        /// </summary>
        public string Routing { get; set; }
        /// <summary>
        /// Name on the customer's account
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The billing street address of the cardholder
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// The billing zip code of the cardholder
        /// </summary>
        public string Zip { get; set; }
        /// <summary>
        /// <para>Sets this payment as the default payment method</para>
        /// <para>Note: If there are no other payment methods, this method is automatically set as the default</para>
        /// </summary>
        public bool SetToDefault { get; set; }
    }

    /// <summary>
    /// <para>Use this command to update an existing payment method.</para>
    /// <para>Note: All fields with values must be passed in (even the fields that are not being updated). Any fields not passed in are treated as being set to blank.</para>
    /// </summary>
    public class PaymentMethodUpdate : PaymentMethodAdd
    {
        internal new string Operation => "paymentmethod:update";

        /// <summary>
        /// Payment method's unique ID
        /// </summary>
        public string PaymentMethodID { get; set; }
    }

    /// <summary>
    /// <para>Use this command to remove a payment method.</para>
    /// <para>Note: The payment method is not completely deleted from the database; instead, the removed property is set to true. Customers with active schedules cannot be removed.</para>
    /// </summary>
    public class PaymentMethodRemove
    {
        internal string Operation => "paymentmethod:remove";

        /// <summary>
        /// Payment method's unique ID
        /// </summary>
        public string PaymentMethodID { get; set; }
    }

    /// <summary>
    /// Use this command to retrieve details of a payment method.
    /// </summary>
    public class PaymentMethodGet
    {
        internal string Operation => "report:paymentmethod";

        /// <summary>
        /// Payment method's unique ID
        /// </summary>
        public string PaymentMethodID { get; set; }

        /// <summary>
        /// Indicates whether to return deleted rows.
        /// </summary>
        public bool Removed { get; set; } = false;
    }

    /// <summary>
    /// Use this command to find payment methods using specific search parameters.
    /// </summary>
    public class PaymentMethodFind
    {
        internal string Operation => "report:paymentmethods";

        /// <summary>
        /// Customer's unique ID number
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// Name on the customer's account
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether to return deleted rows.
        /// </summary>
        public bool Removed { get; set; } = false;
    }
}
