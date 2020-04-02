using CardknoxApi.Operations;
using CardknoxApi.RecurringOperations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

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
        public CardknoxResponse CustomerAdd(CustomerAdd _cust, bool force = false)
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

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
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
