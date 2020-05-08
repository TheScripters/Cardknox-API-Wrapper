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
    /// Primary object class for interacting with the Cardknox transaction API.
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
