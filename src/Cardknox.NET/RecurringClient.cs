using CardknoxApi.Operations;
using CardknoxApi.RecurringOperations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using static System.String;

namespace CardknoxApi
{
    /// <summary>
    /// Primary object class for interacting with the Cardknox Recurring v1 API.
    /// </summary>
    public class RecurringClient : IDisposable
    {
        #region events
        /// <summary>
        /// Event Handler for when a request has started. The results will contain the non-sensitive field values being sent to the API endpoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RequestStartedEventHandler(object sender, CardknoxEventArgs e);
        /// <summary>
        /// Fired when a request has been started, returns non-sensitive field values sent to API endpoint
        /// </summary>
        public event RequestStartedEventHandler RequestStarted;
        /// <summary>
        /// Event Handler for when a request has completed. The results will contain the field values being received from the API endpoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RequestCompletedEventHandler(object sender, CardknoxEventArgs e);
        /// <summary>
        /// Fired when a request has completed, returns field values returned from API endpoint
        /// </summary>
        public event RequestCompletedEventHandler RequestCompleted;
        #endregion

        private RecurringRequest Request { get; }
        private NameValueCollection Values { get; }
        private WebClient WebClient { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public RecurringClient(RecurringRequest request)
        {
            Values = new NameValueCollection
            {
                { "xKey", request.Key },
                { "xVersion", request.CardknoxVersion },
                { "xSoftwareName", request.Software },
                { "xSoftwareVersion", request.SoftwareVersion }
            };
            Request = request;
            WebClient = new WebClient();
        }

        #region Customer
        /// <summary>
        /// Use this command to add a new customer record, which can then be linked to future payment methods and schedules. Creating a new schedule without linking it to an existing customer automatically generates a new customer for you.
        /// </summary>
        /// <param name="_cust"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse CustomerAdd(CustomerAdd _cust, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _cust.Operation);

            if (!IsNullOrWhiteSpace(_cust.CustomerNumber))
                Values.Add("xCustomerNumber", _cust.CustomerNumber);
            if (!IsNullOrWhiteSpace(_cust.CustomerNotes))
                Values.Add("xCustomerNotes", _cust.CustomerNotes);

            AddCommonFields(_cust);

            int i = 1;
            foreach (string v in _cust.CustomFields)
            {
                Values.Add($"xCustomerCustom{i:D2}", v);
                i++;
            }

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// Use this command to update existing customer information.
        /// </summary>
        /// <param name="_cust"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse CustomerUpdate(CustomerUpdate _cust, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _cust.Operation);
            Values.Add("xCustomerID", _cust.CustomerID);

            if (!IsNullOrWhiteSpace(_cust.CustomerNumber))
                Values.Add("xCustomerNumber", _cust.CustomerNumber);
            if (!IsNullOrWhiteSpace(_cust.CustomerNotes))
                Values.Add("xCustomerNotes", _cust.CustomerNotes);

            AddCommonFields(_cust);

            int i = 1;
            foreach (string v in _cust.CustomFields)
            {
                Values.Add($"xCustomerCustom{i:D2}", v);
                i++;
            }

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// <para>Use this command to remove a customer record.</para>
        /// <para>Note: The customer record is not completely deleted from the database; instead, the removed property is set to true. Customers with active schedules cannot be removed.</para>
        /// </summary>
        /// <param name="_cust"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse CustomerRemove(CustomerRemove _cust, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _cust.Operation);
            Values.Add("xCustomerID", _cust.CustomerID);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// Use this command to retrieve a customer’s details.
        /// </summary>
        /// <param name="_cust"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse CustomerGet(CustomerGet _cust, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _cust.Operation);
            Values.Add("xCustomerID", _cust.CustomerID);
            Values.Add("xRemoved", _cust.Removed.ToString());

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// Use this command to find customers using specific search parameters.
        /// </summary>
        /// <param name="_cust"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse CustomerFind(CustomerFind _cust, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _cust.Operation);

            if (!IsNullOrWhiteSpace(_cust.CustomerID))
                Values.Add("xCustomerID", _cust.CustomerID);
            Values.Add("xRemoved", _cust.Removed.ToString());

            AddCommonFields(_cust);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cust"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse CustomerTransaction(CustomerTransaction _cust, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _cust.Operation);

            if (IsNullOrWhiteSpace(_cust.CustomerID) && IsNullOrWhiteSpace(_cust.PaymentMethodID))
                throw new InvalidOperationException("xPaymentMethodID is required when xCustomerID is not specified.");

            if (!IsNullOrWhiteSpace(_cust.CustomerID))
                Values.Add("xCustomerID", _cust.PaymentMethodID);
            if (!IsNullOrWhiteSpace(_cust.CustomerID))
                Values.Add("xPaymentMethodID", _cust.PaymentMethodID);
            Values.Add("xAmount", String.Format("{0:N2}", _cust.Amount));
            Values.Add("xUseBackupPaymentMethods", _cust.UseBackupPaymentMethods.ToString());
            if (!IsNullOrWhiteSpace(_cust.Token) && IsNullOrWhiteSpace(_cust.PaymentMethodID))
                Values.Add("xToken", _cust.Token);
            if (!IsNullOrWhiteSpace(_cust.TokenType))
                Values.Add("xTokenType", _cust.TokenType);
            if (!IsNullOrWhiteSpace(_cust.TokenType))
                Values.Add("xTokenType", _cust.TokenType);
            if (!IsNullOrWhiteSpace(_cust.Name))
                Values.Add("xName", _cust.Name);
            if (!IsNullOrWhiteSpace(_cust.Street))
                Values.Add("xStreet", _cust.Street);
            if (!IsNullOrWhiteSpace(_cust.Zip))
                Values.Add("xZip", _cust.Zip);
            if (!IsNullOrWhiteSpace(_cust.Description))
                Values.Add("xDescription", _cust.Description);
            if (!IsNullOrWhiteSpace(_cust.Invoice))
                Values.Add("xInvoice", _cust.Invoice);
            if (!IsNullOrWhiteSpace(_cust.PONum))
                Values.Add("xPONum", _cust.PONum);

            if (!IsNullOrWhiteSpace(_cust.ShipFirstName))
                Values.Add("xShipFirstName", _cust.ShipFirstName);
            if (!IsNullOrWhiteSpace(_cust.ShipMiddleName))
                Values.Add("xShipMiddleName", _cust.ShipMiddleName);
            if (!IsNullOrWhiteSpace(_cust.ShipLastName))
                Values.Add("xShipLastName", _cust.ShipLastName);
            if (!IsNullOrWhiteSpace(_cust.ShipCompany))
                Values.Add("xShipCompany", _cust.ShipCompany);
            if (!IsNullOrWhiteSpace(_cust.ShipStreet))
                Values.Add("xShipStreet", _cust.ShipStreet);
            if (!IsNullOrWhiteSpace(_cust.ShipStreet2))
                Values.Add("xShipStreet2", _cust.ShipStreet2);
            if (!IsNullOrWhiteSpace(_cust.ShipCity))
                Values.Add("xShipCity", _cust.ShipCity);
            if (!IsNullOrWhiteSpace(_cust.ShipState))
                Values.Add("xShipState", _cust.ShipState);
            if (!IsNullOrWhiteSpace(_cust.ShipZip))
                Values.Add("xShipZip", _cust.ShipZip);
            if (!IsNullOrWhiteSpace(_cust.ShipCountry))
                Values.Add("xShipCountry", _cust.ShipCountry);
            if (!IsNullOrWhiteSpace(_cust.ShipPhone))
                Values.Add("xShipPhone", _cust.ShipPhone);
            if (!IsNullOrWhiteSpace(_cust.ShipMobile))
                Values.Add("xShipMobile", _cust.ShipMobile);
            if (!IsNullOrWhiteSpace(_cust.ShipEmail))
                Values.Add("xShipEmail", _cust.ShipEmail);

            int i = 1;
            foreach (string v in _cust.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }
        #endregion

        #region PaymentMethod

        /// <summary>
        /// Use this command to add a new payment method to a customer's account profile.
        /// </summary>
        /// <param name="_pmt"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse PaymentMethodAdd(PaymentMethodAdd _pmt, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _pmt.Operation);
            Values.Add("xCustomerID", _pmt.CustomerID);
            Values.Add("xToken", _pmt.Token);
            Values.Add("xTokenType", _pmt.TokenType);
            // END required information

            if (!String.IsNullOrWhiteSpace(_pmt.TokenAlias))
                Values.Add("xTokenAlias", _pmt.TokenAlias);

            // CONDITIONALLY REQUIRED
            if (!String.IsNullOrWhiteSpace(_pmt.Exp))
                Values.Add("xExp", _pmt.Exp);
            if (!String.IsNullOrWhiteSpace(_pmt.Routing))
                Values.Add("xRouting", _pmt.Routing);
            if (!String.IsNullOrWhiteSpace(_pmt.Name))
                Values.Add("xName", _pmt.Name);

            if (!String.IsNullOrWhiteSpace(_pmt.Street))
                Values.Add("xStreet", _pmt.Street);
            if (!String.IsNullOrWhiteSpace(_pmt.Zip))
                Values.Add("xZip", _pmt.Zip);
            if (_pmt.SetToDefault)
                Values.Add("xSetToDefault", _pmt.SetToDefault.ToString());

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// <para>Use this command to update an existing payment method.</para>
        /// <para>Note: All fields with values must be passed in (even the fields that are not being updated). Any fields not passed in are treated as being set to blank.</para>
        /// </summary>
        /// <param name="_pmt"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse PaymentMethodUpdate(PaymentMethodUpdate _pmt, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _pmt.Operation);
            Values.Add("xCustomerID", _pmt.CustomerID);
            Values.Add("xToken", _pmt.Token);
            Values.Add("xTokenType", _pmt.TokenType);
            // END required information

            if (!String.IsNullOrWhiteSpace(_pmt.TokenAlias))
                Values.Add("xTokenAlias", _pmt.TokenAlias);

            // CONDITIONALLY REQUIRED
            if (!String.IsNullOrWhiteSpace(_pmt.Exp))
                Values.Add("xExp", _pmt.Exp);
            if (!String.IsNullOrWhiteSpace(_pmt.Routing))
                Values.Add("xRouting", _pmt.Routing);
            if (!String.IsNullOrWhiteSpace(_pmt.Name))
                Values.Add("xName", _pmt.Name);

            if (!String.IsNullOrWhiteSpace(_pmt.Street))
                Values.Add("xStreet", _pmt.Street);
            if (!String.IsNullOrWhiteSpace(_pmt.Zip))
                Values.Add("xZip", _pmt.Zip);
            if (_pmt.SetToDefault)
                Values.Add("xSetToDefault", _pmt.SetToDefault.ToString());

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// <para>Use this command to remove a payment method.</para>
        /// <para>Note: The payment method is not completely deleted from the database; instead, the removed property is set to true. Customers with active schedules cannot be removed.</para>
        /// </summary>
        /// <param name="_pmt"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse PaymentMethodRemove(PaymentMethodRemove _pmt, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _pmt.Operation);
            Values.Add("xPaymentMethodID", _pmt.PaymentMethodID);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// Use this command to retrieve details of a payment method.
        /// </summary>
        /// <param name="_pmt"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse PaymentMethodGet(PaymentMethodGet _pmt, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _pmt.Operation);
            Values.Add("xPaymentMethodID", _pmt.PaymentMethodID);
            Values.Add("xRemoved", _pmt.Removed.ToString());

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        /// <summary>
        /// Use this command to find payment methods using specific search parameters.
        /// </summary>
        /// <param name="_pmt"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public RecurringResponse PaymentMethodFind(PaymentMethodFind _pmt, bool force = false)
        {
            if (Values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Recurring is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.AllKeys;
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _pmt.Operation);

            if (!String.IsNullOrWhiteSpace(_pmt.CustomerID))
                Values.Add("xCustomerID", _pmt.CustomerID);
            if (!String.IsNullOrWhiteSpace(_pmt.Name))
                Values.Add("xName", _pmt.Name);
            Values.Add("xRemoved", _pmt.Removed.ToString());

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new RecurringResponse(resp);
        }

        #endregion

        #region private methods
        private NameValueCollection MakeRequest()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string req = System.Text.Encoding.ASCII.GetString(WebClient.UploadValues(RecurringRequest._url, Values));
            NameValueCollection resp = HttpUtility.ParseQueryString(req);

            return resp;
        }

        private void AddCommonFields(Customer _base)
        {
            if (!IsNullOrWhiteSpace(_base.Email))
                Values.Add("xEmail", _base.Email);

            if (!IsNullOrWhiteSpace(_base.Fax))
                Values.Add("xFax", _base.Fax);

            if (!IsNullOrWhiteSpace(_base.BillFirstName))
                Values.Add("xBillFirstName", _base.BillFirstName);

            if (!IsNullOrWhiteSpace(_base.BillMiddleName))
                Values.Add("xBillMiddleName", _base.BillMiddleName);

            if (!IsNullOrWhiteSpace(_base.BillLastName))
                Values.Add("xBillLastName", _base.BillLastName);

            if (!IsNullOrWhiteSpace(_base.BillCompany))
                Values.Add("xBillCompany", _base.BillCompany);

            if (!IsNullOrWhiteSpace(_base.BillStreet))
                Values.Add("xBillStreet", _base.BillStreet);

            if (!IsNullOrWhiteSpace(_base.BillStreet2))
                Values.Add("xBillStreet2", _base.BillStreet2);

            if (!IsNullOrWhiteSpace(_base.BillCity))
                Values.Add("xBillCity", _base.BillCity);

            if (!IsNullOrWhiteSpace(_base.BillState))
                Values.Add("xBillState", _base.BillState);

            if (!IsNullOrWhiteSpace(_base.BillZip))
                Values.Add("xBillZip", _base.BillZip);

            if (!IsNullOrWhiteSpace(_base.BillCountry))
                Values.Add("xBillCountry", _base.BillCountry);

            if (!IsNullOrWhiteSpace(_base.BillPhone))
                Values.Add("xBillPhone", _base.BillPhone);

            if (!IsNullOrWhiteSpace(_base.BillMobile))
                Values.Add("xBillMobile", _base.BillMobile);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            WebClient.Dispose();
        }
    }
}
