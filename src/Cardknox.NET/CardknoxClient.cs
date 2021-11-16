using CardknoxApi.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using static System.String;

namespace CardknoxApi
{
    /// <summary>
    /// Primary object class for interacting with the Cardknox transaction API.
    /// </summary>
    public class CardknoxClient : IDisposable
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

        private CardknoxRequest Request { get; }
        private Dictionary<string, string> Values { get; }
        private WebClient WebClient { get; }

        /// <summary>
        /// Initiate new Cardknox request
        /// </summary>
        /// <param name="request">The <see cref="CardknoxRequest"/> object that is used to make the request.</param>
        public CardknoxClient(CardknoxRequest request)
        {
            Values = new Dictionary<string, string>
            {
                { "xKey", request.Key },
                { "xVersion", request.CardknoxVersion },
                { "xSoftwareName", request.Software },
                { "xSoftwareVersion", request.SoftwareVersion }
            };
            Request = request;
            WebClient = new WebClient();
        }

        #region credit card

        /// <summary>
        /// The Sale command is a combination of an authorization and capture and intended when fulfilling an order right away. For transactions that are not fulfilled right away, use the authonly command initially and use the capture command to complete the sale.
        /// </summary>
        /// <param name="_sale"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCSale(CCSale _sale, bool force = false)
        {
            if (_sale.Amount == null || _sale.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Sale Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _sale.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _sale.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_sale.CardNum))
            {
                Values.Add("xCardNum", _sale.CardNum);
                if (!IsNullOrWhiteSpace(_sale.CVV))
                    Values.Add("xCVV", _sale.CVV);
                if (!IsNullOrWhiteSpace(_sale.Exp))
                    Values.Add("xExp", _sale.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_sale.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                Values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MagStripe))
            {
                Values.Add("xMagStripe", _sale.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_sale.Street))
                Values.Add("xStreet", _sale.Street);

            if (!IsNullOrWhiteSpace(_sale.Zip))
                Values.Add("xZip", _sale.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_sale.IP))
                Values.Add("xIP", _sale.IP);

            if (_sale.CustReceipt)
                Values.Add("xCustReceipt", _sale.CustReceipt.ToString());

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            int i = 1;
            foreach (string v in _sale.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Save command is used to send account information and request a token from Cardknox, but does not submit the transaction for processing. The response returns a token which references that account information. A token at a minimum references the credit card number, but if other data is sent, such as billing address, that will be associated with the token as well.
        /// </summary>
        /// <param name="_save"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCSave(CCSave _save, bool force = false)
        {
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _save.Operation);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_save.CardNum))
            {
                Values.Add("xCardNum", _save.CardNum);
                if (!IsNullOrWhiteSpace(_save.CVV))
                    Values.Add("xCVV", _save.CVV);
                if (!IsNullOrWhiteSpace(_save.Exp))
                    Values.Add("xExp", _save.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_save.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_save.Token))
            {
                Values.Add("xToken", _save.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_save.MagStripe))
            {
                Values.Add("xMagStripe", _save.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_save.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_save.Name))
                Values.Add("xName", _save.Name);

            if (!IsNullOrWhiteSpace(_save.Street))
                Values.Add("xStreet", _save.Street);

            if (!IsNullOrWhiteSpace(_save.Zip))
                Values.Add("xZip", _save.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_save.IP))
                Values.Add("xIP", _save.IP);

            int i = 1;
            foreach (string v in _save.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Refund command is used to refund a full or partial refund of a previous settled transaction, using RefNum.
        /// </summary>
        /// <param name="_refund"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCRefund(CCRefund _refund, bool force = false)
        {
            if (IsNullOrWhiteSpace(_refund.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (_refund.Amount == null || _refund.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Must specify a positive amount to refund.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _refund.Operation);

            Values.Add("xAmount", String.Format("{0:N2}", _refund.Amount));
            Values.Add("xRefNum", _refund.RefNum);
            // END required information

            if (_refund.CustReceipt)
                Values.Add("xCustReceipt", _refund.CustReceipt.ToString());

            int i = 1;
            foreach (string v in _refund.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The AuthOnly command authorizes an amount on a cardholder's account and places a hold on the available credit for that amount, but does not submit the charge for settlement. AuthOnly is used to reserve funds from a cardholder's credit limit for a sale that is not ready to be processed.
        /// </summary>
        /// <param name="_auth"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCAuthOnly(CCAuthOnly _auth, bool force = false)
        {
            if (_auth.Amount == null || _auth.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Auth Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _auth.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _auth.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_auth.CardNum))
            {
                Values.Add("xCardNum", _auth.CardNum);
                if (!IsNullOrWhiteSpace(_auth.CVV))
                    Values.Add("xCVV", _auth.CVV);
                if (!IsNullOrWhiteSpace(_auth.Exp))
                    Values.Add("xExp", _auth.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_auth.Exp))
                    requiredAdded = false; 
            }
            else if (!IsNullOrWhiteSpace(_auth.Token))
            {
                Values.Add("xToken", _auth.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_auth.MagStripe))
            {
                Values.Add("xMagStripe", _auth.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_auth.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_auth.Street))
                Values.Add("xStreet", _auth.Street);

            if (!IsNullOrWhiteSpace(_auth.Zip))
                Values.Add("xZip", _auth.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_auth.IP))
                Values.Add("xIP", _auth.IP);

            if (_auth.CustReceipt)
                Values.Add("xCustReceipt", _auth.CustReceipt.ToString());

            int i = 1;
            foreach (string v in _auth.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_auth);

            AddSpecialFields(_auth);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Capture command is used to settle funds from a previous authorization and withdraw the funds from the cardholder's account. The refNumber from related authorization is required when submitting a Capture request. To perform an authorization and capture in the same command, use the CCSale command.
        /// </summary>
        /// <param name="_capture"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCCapture(CCCapture _capture, bool force = false)
        {
            if (IsNullOrWhiteSpace(_capture.RefNum))
                throw new InvalidOperationException("The capture command must reference a previous authorization in the RefNum parameter.");
            if (_capture.Amount == null || _capture.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Capture Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _capture.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _capture.Amount));
            Values.Add("xRefNum", _capture.RefNum);
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_capture.Street))
                Values.Add("xStreet", _capture.Street);

            if (!IsNullOrWhiteSpace(_capture.Zip))
                Values.Add("xZip", _capture.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_capture.IP))
                Values.Add("xIP", _capture.IP);

            if (_capture.CustReceipt)
                Values.Add("xCustReceipt", _capture.CustReceipt.ToString());

            AddCommonFields(_capture);

            AddSpecialFields(_capture);

            int i = 1;
            foreach (string v in _capture.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Credit command refunds money from a merchant to a cardholder's card that is not linked to any previous transaction.
        /// </summary>
        /// <param name="_credit"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCCredit(CCCredit _credit, bool force = false)
        {
            if (_credit.Amount == null || _credit.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Credit Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _credit.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _credit.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_credit.CardNum))
            {
                Values.Add("xCardNum", _credit.CardNum);
                if (!IsNullOrWhiteSpace(_credit.CVV))
                    Values.Add("xCVV", _credit.CVV);
                if (!IsNullOrWhiteSpace(_credit.Exp))
                    Values.Add("xExp", _credit.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_credit.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_credit.Token))
            {
                Values.Add("xToken", _credit.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.MagStripe))
            {
                Values.Add("xMagStripe", _credit.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_credit.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_credit.Street))
                Values.Add("xStreet", _credit.Street);

            if (!IsNullOrWhiteSpace(_credit.Zip))
                Values.Add("xZip", _credit.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_credit.IP))
                Values.Add("xIP", _credit.IP);

            if (_credit.CustReceipt)
                Values.Add("xCustReceipt", _credit.CustReceipt.ToString());

            int i = 1;
            foreach (string v in _credit.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_credit);

            AddSpecialFields(_credit);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Void command voids a captured transaction that is pending batch, prior to the batch being settled.
        /// </summary>
        /// <param name="_void"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCVoid(CCVoid _void, bool force = false)
        {
            if (IsNullOrWhiteSpace(_void.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _void.Operation);
            Values.Add("xRefNum", _void.RefNum);
            // END required information

            int i = 1;
            foreach (string v in _void.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Adjust comand is used to change a previous authorization to a higher or lower amount. The refNumber from related authorization is required when submitting an Adjust.
        /// </summary>
        /// <param name="_adjust"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCAdjust(CCAdjust _adjust, bool force = false)
        {
            if (IsNullOrWhiteSpace(_adjust.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (_adjust.Amount == null || _adjust.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _adjust.Operation);
            Values.Add("xRefNum", _adjust.RefNum);
            Values.Add("xAmount", Format("{0:N2}", _adjust.Amount));
            // END required information

            if (!IsNullOrWhiteSpace(_adjust.Street))
                Values.Add("xStreet", _adjust.Street);

            if (!IsNullOrWhiteSpace(_adjust.Zip))
                Values.Add("xZip", _adjust.Zip);

            if (!IsNullOrWhiteSpace(_adjust.Name))
                Values.Add("xName", _adjust.Name);

            if (!IsNullOrWhiteSpace(_adjust.IP))
                Values.Add("xIP", _adjust.IP);

            int i = 1;
            foreach (string v in _adjust.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// Some authorizations will require a voice authorization before they can be approved. When a verbal authorization is issued by the bank, that number can be sent with the PostAuth command to verify the authorization.
        /// </summary>
        /// <param name="_auth"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCPostAuth(CCPostAuth _auth, bool force = false)
        {
            if (IsNullOrWhiteSpace(_auth.AuthCode))
                throw new InvalidOperationException("Invalid AuthCode specified. AuthCode must be a verification number provided by the issuing bank.");
            if (_auth.Amount == null || _auth.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _auth.Operation);
            Values.Add("xAuthCode", _auth.AuthCode);
            // END required information

            if (!IsNullOrWhiteSpace(_auth.Street))
                Values.Add("xStreet", _auth.Street);

            if (!IsNullOrWhiteSpace(_auth.Zip))
                Values.Add("xZip", _auth.Zip);

            if (!IsNullOrWhiteSpace(_auth.Name))
                Values.Add("xName", _auth.Name);

            if (!IsNullOrWhiteSpace(_auth.IP))
                Values.Add("xIP", _auth.IP);

            int i = 1;
            foreach (string v in _auth.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_auth);

            AddSpecialFields(_auth);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Void Refund command voids a pending transaction that has not yet settled or will refund the transaction if it already has settled using RefNum.
        /// </summary>
        /// <param name="_refund"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCVoidRefund(CCVoidRefund _refund, bool force = false)
        {
            if (IsNullOrWhiteSpace(_refund.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _refund.Operation);
            Values.Add("xRefNum", _refund.RefNum);
            // END required information

            int i = 1;
            foreach (string v in _refund.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Void Release command releases a pending authorization amount back to the cardholder's credit limit without waiting for the standard authorization timeframe to expire.
        /// </summary>
        /// <param name="_release"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CCVoidRelease(CCVoidRelease _release, bool force = false)
        {
            if (IsNullOrWhiteSpace(_release.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _release.Operation);
            Values.Add("xRefNum", _release.RefNum);
            // END required information

            int i = 1;
            foreach (string v in _release.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        #endregion

        #region check
        /// <summary>
        /// The Check Sale command debits funds from a customer's checking or savings account using the account and routing number. The merchant must have a supported Check/ACH processing account
        /// </summary>
        /// <param name="_sale"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CheckSale(CheckSale _sale, bool force = false)
        {
            if (_sale.Amount == null || _sale.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _sale.Operation);
            if (!IsNullOrWhiteSpace(_sale.Name))
                throw new InvalidOperationException("Name is required.");
            bool requiredAdded = false;
            if (!IsNullOrWhiteSpace(_sale.Routing) && !IsNullOrWhiteSpace(_sale.Account))
            {
                Values.Add("xRouting", _sale.Routing);
                Values.Add("xAccount", _sale.Account);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                Values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MICR))
            {
                Values.Add("xMICR", _sale.MICR);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            Values.Add("xAmount", Format("{0:N2}", _sale.Amount));

            Values.Add("xName", _sale.Name);
            // END required information

            if (!IsNullOrWhiteSpace(_sale.IP))
                Values.Add("xIP", _sale.IP);

            if (_sale.CustReceipt)
                Values.Add("xCustReceipt", _sale.CustReceipt.ToString());

            int i = 1;
            foreach (string v in _sale.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Credit command sends money from a merchant to a customer's bank account that is not linked to any previous transaction. With check transactions, this is commonly used for paying 3rd-parties such as paying vendors. To refund a previous check sale, use Check:Refund instead.
        /// </summary>
        /// <param name="_credit"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CheckCredit(CheckCredit _credit, bool force = false)
        {
            if (_credit.Amount == null || _credit.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _credit.Operation);
            if (!IsNullOrWhiteSpace(_credit.Name))
                throw new InvalidOperationException("Name is required.");
            bool requiredAdded = false;
            if (!IsNullOrWhiteSpace(_credit.Routing) && !IsNullOrWhiteSpace(_credit.Account))
            {
                Values.Add("xRouting", _credit.Routing);
                Values.Add("xAccount", _credit.Account);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.Token))
            {
                Values.Add("xToken", _credit.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.MICR))
            {
                Values.Add("xMICR", _credit.MICR);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_credit.Operation} operation.");
            Values.Add("xAmount", Format("{0:N2}", _credit.Amount));
            Values.Add("xName", _credit.Name);
            // END required information

            if (!IsNullOrWhiteSpace(_credit.IP))
                Values.Add("xIP", _credit.IP);

            if (_credit.CustReceipt)
                Values.Add("xCustReceipt", _credit.CustReceipt.ToString());

            int i = 1;
            foreach (string v in _credit.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_credit);

            AddSpecialFields(_credit);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Save command is used to send account information and request a token from Cardknox, but does not submit the transaction for processing. The response returns a token which references that account information. A token at a minimum references the account and routing number, but if other data is sent, such as billing address, that will be associated with the token as well.
        /// </summary>
        /// <param name="_save"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CheckSave(CheckSave _save, bool force = false)
        {
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _save.Operation);
            if (!IsNullOrWhiteSpace(_save.Name))
                throw new InvalidOperationException("Name is required.");
            bool requiredAdded = false;
            if (!IsNullOrWhiteSpace(_save.Routing) && !IsNullOrWhiteSpace(_save.Account))
            {
                Values.Add("xRouting", _save.Routing);
                Values.Add("xAccount", _save.Account);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_save.MICR))
            {
                Values.Add("xMICR", _save.MICR);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_save.Operation} operation.");
            Values.Add("xName", _save.Name);
            // END required information

            if (!IsNullOrWhiteSpace(_save.IP))
                Values.Add("xIP", _save.IP);

            int i = 1;
            foreach (string v in _save.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Void command voids a check transaction that is pending being sent to the bank, typically at the end of each day.
        /// </summary>
        /// <param name="_void"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CheckVoid(CheckVoid _void, bool force = false)
        {
            if (_void.Amount == null || _void.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (IsNullOrWhiteSpace(_void.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _void.Operation);
            Values.Add("xAmount", Format("{0:N2}", _void.Amount));
            Values.Add("xRefNum", _void.RefNum);
            // END required information

            int i = 1;
            foreach (string v in _void.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Refund command is used to refund a full or partial refund of a previous settled check transaction, using xRefNum
        /// </summary>
        /// <param name="_refund"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse CheckRefund(CheckRefund _refund, bool force = false)
        {
            if (_refund.Amount == null || _refund.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (IsNullOrWhiteSpace(_refund.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _refund.Operation);
            Values.Add("xAmount", Format("{0:N2}", _refund.Amount));
            Values.Add("xRefNum", _refund.RefNum);
            // END required information

            if (_refund.CustReceipt)
                Values.Add("xCustReceipt", _refund.CustReceipt.ToString());

            int i = 1;
            foreach (string v in _refund.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        #endregion

        #region ebt food stamp
        /// <summary>
        /// The Sale command is used to make a purchase on an EBT cardholder's food stamp account.
        /// </summary>
        /// <param name="_sale"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTFSSale(EBTFSSale _sale, bool force = false)
        {
            if (_sale.Amount == null || _sale.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _sale.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _sale.Amount));
            Values.Add("xDUKPT", _sale.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_sale.CardNum))
            {
                Values.Add("xCardNum", _sale.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                Values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MagStripe))
            {
                Values.Add("xMagStripe", _sale.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_sale.Street))
                Values.Add("xStreet", _sale.Street);

            if (!IsNullOrWhiteSpace(_sale.Zip))
                Values.Add("xZip", _sale.Zip);

            if (!IsNullOrWhiteSpace(_sale.IP))
                Values.Add("xIP", _sale.IP);

            int i = 1;
            foreach (string v in _sale.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Credit command is used to credit to an EBT cardholder's food stamp account.
        /// </summary>
        /// <param name="_credit"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTFSCredit(EBTFSCredit _credit, bool force = false)
        {
            if (_credit.Amount == null || _credit.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _credit.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _credit.Amount));
            Values.Add("xDUKPT", _credit.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_credit.CardNum))
            {
                Values.Add("xCardNum", _credit.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.Token))
            {
                Values.Add("xToken", _credit.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.MagStripe))
            {
                Values.Add("xMagStripe", _credit.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_credit.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_credit.Street))
                Values.Add("xStreet", _credit.Street);

            if (!IsNullOrWhiteSpace(_credit.Zip))
                Values.Add("xZip", _credit.Zip);

            if (!IsNullOrWhiteSpace(_credit.IP))
                Values.Add("xIP", _credit.IP);

            int i = 1;
            foreach (string v in _credit.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_credit);

            AddSpecialFields(_credit);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Balance command is used to check the balance on an EBT food stamp card.
        /// </summary>
        /// <param name="_bal"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTFSBalance(EBTFSBalance _bal, bool force = false)
        {
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _bal.Operation);
            Values.Add("xDUKPT", _bal.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_bal.CardNum))
            {
                Values.Add("xCardNum", _bal.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_bal.Token))
            {
                Values.Add("xToken", _bal.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_bal.MagStripe))
            {
                Values.Add("xMagStripe", _bal.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_bal.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_bal.IP))
                Values.Add("xIP", _bal.IP);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Voucher command is used to process manual EBT food stamp voucher.
        /// </summary>
        /// <param name="_voucher"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTFSVoucher(EBTFSVoucher _voucher, bool force = false)
        {
            if (_voucher.Amount == null || _voucher.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _voucher.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _voucher.Amount));
            Values.Add("xDUKPT", _voucher.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_voucher.CardNum))
            {
                Values.Add("xCardNum", _voucher.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_voucher.Token))
            {
                Values.Add("xToken", _voucher.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_voucher.MagStripe))
            {
                Values.Add("xMagStripe", _voucher.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_voucher.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_voucher.Street))
                Values.Add("xStreet", _voucher.Street);

            if (!IsNullOrWhiteSpace(_voucher.Zip))
                Values.Add("xZip", _voucher.Zip);

            if (!IsNullOrWhiteSpace(_voucher.IP))
                Values.Add("xIP", _voucher.IP);

            int i = 1;
            foreach (string v in _voucher.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_voucher);

            AddSpecialFields(_voucher);

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

        #region ebt cash benefits
        /// <summary>
        /// The Sale command is used to make a purchase on an EBT cardholder's cash benefit account.
        /// </summary>
        /// <param name="_sale"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTCBSale(EBTCBSale _sale, bool force = false)
        {
            if (_sale.Amount == null || _sale.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _sale.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _sale.Amount));
            Values.Add("xDUKPT", _sale.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_sale.CardNum))
            {
                Values.Add("xCardNum", _sale.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                Values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MagStripe))
            {
                Values.Add("xMagStripe", _sale.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_sale.Street))
                Values.Add("xStreet", _sale.Street);

            if (!IsNullOrWhiteSpace(_sale.Zip))
                Values.Add("xZip", _sale.Zip);

            if (!IsNullOrWhiteSpace(_sale.IP))
                Values.Add("xIP", _sale.IP);

            int i = 1;
            foreach (string v in _sale.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Cash command enables a cash withdrawal from on an EBT cardholder's cash benefit account.
        /// </summary>
        /// <param name="_cash"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTCBCash(EBTCBCash _cash, bool force = false)
        {
            if (_cash.Amount == null || _cash.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _cash.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _cash.Amount));
            Values.Add("xDUKPT", _cash.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_cash.CardNum))
            {
                Values.Add("xCardNum", _cash.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_cash.Token))
            {
                Values.Add("xToken", _cash.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_cash.MagStripe))
            {
                Values.Add("xMagStripe", _cash.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_cash.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_cash.IP))
                Values.Add("xIP", _cash.IP);

            if (!IsNullOrWhiteSpace(_cash.Invoice))
                Values.Add("xInvoice", _cash.Invoice);

            if (_cash.AllowDuplicate)
                Values.Add("xAllowDuplicate", _cash.AllowDuplicate.ToString());

            int i = 1;
            foreach (string v in _cash.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Balance command is used to check the balance on an EBT cash benefit account.
        /// </summary>
        /// <param name="_bal"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTCBBalance(EBTCBBalance _bal, bool force = false)
        {
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _bal.Operation);
            Values.Add("xDUKPT", _bal.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_bal.CardNum))
            {
                Values.Add("xCardNum", _bal.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_bal.MagStripe))
            {
                Values.Add("xMagStripe", _bal.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_bal.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_bal.IP))
                Values.Add("xIP", _bal.IP);

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

        #region ebt wic
        /// <summary>
        /// The Sale command is used to make a purchase on an EBTW cardholder's cash benefit account.
        /// </summary>
        /// <param name="_sale"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTWSale(EBTWSale _sale, bool force = false)
        {
            if (_sale.Items.Count == 0)
                throw new InvalidOperationException("Must specify items included in this sale.");
            if (_sale.Amount == null || _sale.Amount <= 0)
                throw new InvalidOperationException("Invalid Amount specified. Amount must be greater than zero.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _sale.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _sale.Amount));
            Values.Add("xDUKPT", _sale.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_sale.CardNum))
            {
                Values.Add("xCardNum", _sale.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                Values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MagStripe))
            {
                Values.Add("xMagStripe", _sale.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_sale.Street))
                Values.Add("xStreet", _sale.Street);

            if (!IsNullOrWhiteSpace(_sale.Zip))
                Values.Add("xZip", _sale.Zip);

            if (!IsNullOrWhiteSpace(_sale.IP))
                Values.Add("xIP", _sale.IP);

            int i = 1;
            foreach (string v in _sale.CustomFields)
            {
                Values.Add($"xCustom{i:D2}", v);
                i++;
            }

            if (_sale.Items.Count > 0)
            {
                i = 1;
                foreach (EBTWItem item in _sale.Items)
                {
                    Values.Add($"x{i}UnitPrice", String.Format("{0:N2}", item.UnitPrice));
                    Values.Add($"x{i}Qty", item.Qty.ToString());
                    Values.Add($"x{i}Upc", item.Upc);
                    i++;
                }
            }

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Balance command is used to check the balance on an EBTW cash benefit account.
        /// </summary>
        /// <param name="_bal"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTWBalance(EBTWBalance _bal, bool force = false)
        {
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _bal.Operation);
            Values.Add("xDUKPT", _bal.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_bal.CardNum))
            {
                Values.Add("xCardNum", _bal.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_bal.MagStripe))
            {
                Values.Add("xMagStripe", _bal.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_bal.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_bal.IP))
                Values.Add("xIP", _bal.IP);

            if (RequestStarted == null)
                Log.LogRequest(Values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(Values));

            var resp = MakeRequest();
            if (RequestCompleted == null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// Void EBT Wic (eWic) transaction
        /// </summary>
        /// <param name="_void"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public CardknoxResponse EBTWVoid(EBTWVoid _void, bool force = false)
        {
            if (IsNullOrWhiteSpace(_void.RefNum))
                throw new InvalidOperationException("Invalid RefNum specified. RefNum must reference a previous transaction.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _void.Operation);
            Values.Add("xDUKPT", _void.DUKPT);
            Values.Add("xRefNum", _void.RefNum);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_void.CardNum))
            {
                Values.Add("xCardNum", _void.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_void.MagStripe))
            {
                Values.Add("xMagStripe", _void.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_void.Operation} operation.");
            // END required information

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

        #region gift card
        /// <summary>
        /// The Issue command is used to issue funds to a Cardknox gift card.
        /// </summary>
        /// <param name="_issue"></param>
        /// <param name="force"></param>
        /// <returns>If <see langword="true"/> allows new command to be sent by clearing previous command entries</returns>
        public CardknoxResponse GCIssue(GCIssue _issue, bool force = false)
        {
            if (_issue.Amount == null || _issue.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Sale Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _issue.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _issue.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_issue.CardNum))
            {
                Values.Add("xCardNum", _issue.CardNum);
                if (!IsNullOrWhiteSpace(_issue.CVV))
                    Values.Add("xCVV", _issue.CVV);
                if (!IsNullOrWhiteSpace(_issue.Exp))
                    Values.Add("xExp", _issue.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_issue.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_issue.Token))
            {
                Values.Add("xToken", _issue.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_issue.MagStripe))
            {
                Values.Add("xMagStripe", _issue.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_issue.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_issue.Street))
                Values.Add("xStreet", _issue.Street);

            if (!IsNullOrWhiteSpace(_issue.Zip))
                Values.Add("xZip", _issue.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_issue.IP))
                Values.Add("xIP", _issue.IP);

            if (_issue.CustReceipt)
                Values.Add("xCustReceipt", _issue.CustReceipt.ToString());

            AddCommonFields(_issue);

            AddSpecialFields(_issue);

            int i = 1;
            foreach (string v in _issue.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Redeem command is used to debit funds from a Cardknox gift card.
        /// </summary>
        /// <param name="_redeem"></param>
        /// <param name="force">If <see langword="true"/> allows new command to be sent by clearing previous command entries</param>
        /// <returns></returns>
        public CardknoxResponse GCRedeem(GCRedeem _redeem, bool force = false)
        {
            if (_redeem.Amount == null || _redeem.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Sale Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _redeem.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _redeem.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_redeem.CardNum))
            {
                Values.Add("xCardNum", _redeem.CardNum);
                if (!IsNullOrWhiteSpace(_redeem.CVV))
                    Values.Add("xCVV", _redeem.CVV);
                if (!IsNullOrWhiteSpace(_redeem.Exp))
                    Values.Add("xExp", _redeem.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_redeem.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_redeem.Token))
            {
                Values.Add("xToken", _redeem.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_redeem.MagStripe))
            {
                Values.Add("xMagStripe", _redeem.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_redeem.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_redeem.Street))
                Values.Add("xStreet", _redeem.Street);

            if (!IsNullOrWhiteSpace(_redeem.Zip))
                Values.Add("xZip", _redeem.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_redeem.IP))
                Values.Add("xIP", _redeem.IP);

            if (_redeem.CustReceipt)
                Values.Add("xCustReceipt", _redeem.CustReceipt.ToString());

            AddCommonFields(_redeem);

            AddSpecialFields(_redeem);

            int i = 1;
            foreach (string v in _redeem.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Balance command is used to check the available balance on a Cardknox gift card.
        /// </summary>
        /// <param name="_bal"></param>
        /// <param name="force">If <see langword="true"/> allows new command to be sent by clearing previous command entries</param>
        /// <returns></returns>
        public CardknoxResponse GCBalance(GCBalance _bal, bool force = false)
        {
            if (_bal.Amount == null || _bal.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Sale Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _bal.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _bal.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_bal.CardNum))
            {
                Values.Add("xCardNum", _bal.CardNum);
                if (!IsNullOrWhiteSpace(_bal.CVV))
                    Values.Add("xCVV", _bal.CVV);
                if (!IsNullOrWhiteSpace(_bal.Exp))
                    Values.Add("xExp", _bal.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_bal.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_bal.Token))
            {
                Values.Add("xToken", _bal.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_bal.MagStripe))
            {
                Values.Add("xMagStripe", _bal.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_bal.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_bal.Street))
                Values.Add("xStreet", _bal.Street);

            if (!IsNullOrWhiteSpace(_bal.Zip))
                Values.Add("xZip", _bal.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_bal.IP))
                Values.Add("xIP", _bal.IP);

            if (_bal.CustReceipt)
                Values.Add("xCustReceipt", _bal.CustReceipt.ToString());

            AddCommonFields(_bal);

            AddSpecialFields(_bal);

            int i = 1;
            foreach (string v in _bal.CustomFields)
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

            return new CardknoxResponse(resp);
        }

        /// <summary>
        /// The Activate command is used to activate a Cardknox gift card.
        /// </summary>
        /// <param name="_activate"></param>
        /// <param name="force">If <see langword="true"/> allows new command to be sent by clearing previous command entries</param>
        /// <returns></returns>
        public CardknoxResponse GCActivate(GCActivate _activate, bool force = false)
        {
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _activate.Operation);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_activate.CardNum))
            {
                Values.Add("xCardNum", _activate.CardNum);
                if (!IsNullOrWhiteSpace(_activate.CVV))
                    Values.Add("xCVV", _activate.CVV);
                if (!IsNullOrWhiteSpace(_activate.Exp))
                    Values.Add("xExp", _activate.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_activate.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_activate.Token))
            {
                Values.Add("xToken", _activate.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_activate.MagStripe))
            {
                Values.Add("xMagStripe", _activate.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_activate.Operation} operation.");
            // END required information


            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_activate.IP))
                Values.Add("xIP", _activate.IP);

            AddCommonFields(_activate);

            int i = 1;
            foreach (string v in _activate.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        /// <summary>
        /// The Activate command is used to activate a Cardknox gift card.
        /// </summary>
        /// <param name="_deactivate"></param>
        /// <param name="force">If <see langword="true"/> allows new command to be sent by clearing previous command entries</param>
        /// <returns></returns>
        public CardknoxResponse GCDeactivate(GCDeactivate _deactivate, bool force = false)
        {
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _deactivate.Operation);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_deactivate.CardNum))
            {
                Values.Add("xCardNum", _deactivate.CardNum);
                if (!IsNullOrWhiteSpace(_deactivate.CVV))
                    Values.Add("xCVV", _deactivate.CVV);
                if (!IsNullOrWhiteSpace(_deactivate.Exp))
                    Values.Add("xExp", _deactivate.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_deactivate.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_deactivate.Token))
            {
                Values.Add("xToken", _deactivate.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_deactivate.MagStripe))
            {
                Values.Add("xMagStripe", _deactivate.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_deactivate.Operation} operation.");
            // END required information


            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_deactivate.IP))
                Values.Add("xIP", _deactivate.IP);

            AddCommonFields(_deactivate);

            int i = 1;
            foreach (string v in _deactivate.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        #endregion

        #region fraud
        /// <summary>
        /// <para>The Submit command is used in conjunction with a valid FraudWatch account to submit ecommerce transactions for a fraud verification check.</para>
        /// <para>Most fields are required. Refer to API documentation for more information.</para>
        /// <para>https://kb.cardknox.com/api/#FRAUD_Fraud_Submit</para>
        /// </summary>
        /// <param name="_submit"><see cref="Operations.FraudSubmit"/> object containing information describing a transaction to be submitted for fraud verification</param>
        /// <param name="force">If <see langword="true"/> allows new command to be sent by clearing previous command entries</param>
        /// <returns></returns>
        public CardknoxResponse FraudSubmit(FraudSubmit _submit, bool force = false)
        {
            if (_submit.Amount == null || _submit.Amount <= 0)
                throw new InvalidOperationException("Invalid amount. Sale Amount must be greater than 0.");
            if (Values.Keys.Count > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = Values.Keys.ToArray();
                foreach (var v in toRemove)
                    Values.Remove(v);
                Values.Add("xKey", Request.Key);
                Values.Add("xVersion", Request.CardknoxVersion);
                Values.Add("xSoftwareName", Request.Software);
                Values.Add("xSoftwareVersion", Request.SoftwareVersion);
            }

            // BEGIN required information
            Values.Add("xCommand", _submit.Operation);
            Values.Add("xAmount", String.Format("{0:N2}", _submit.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_submit.CardNum))
            {
                Values.Add("xCardNum", _submit.CardNum);
                if (!IsNullOrWhiteSpace(_submit.CVV))
                    Values.Add("xCVV", _submit.CVV);
                if (!IsNullOrWhiteSpace(_submit.Exp))
                    Values.Add("xExp", _submit.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_submit.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_submit.Token))
            {
                Values.Add("xToken", _submit.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_submit.MagStripe))
            {
                Values.Add("xMagStripe", _submit.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_submit.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_submit.Street))
                Values.Add("xStreet", _submit.Street);

            if (!IsNullOrWhiteSpace(_submit.Zip))
                Values.Add("xZip", _submit.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_submit.IP))
                Values.Add("xIP", _submit.IP);

            if (_submit.CustReceipt)
                Values.Add("xCustReceipt", _submit.CustReceipt.ToString());

            AddCommonFields(_submit);

            AddSpecialFields(_submit);

            int i = 1;
            foreach (string v in _submit.CustomFields)
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

            return new CardknoxResponse(resp);
        }
        #endregion

        #region private methods
        private Dictionary<string, string> MakeRequest()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //string req = System.Text.Encoding.ASCII.GetString(WebClient.UploadValues(CardknoxRequest._url, Values));
            //NameValueCollection resp = HttpUtility.ParseQueryString(req);
            Dictionary<string, string> resp = new Dictionary<string, string>();
            using (HttpClient client = new HttpClient())
            {
                using (var postContent = new FormUrlEncodedContent(Values))
                using (HttpResponseMessage response = client.PostAsync(CardknoxRequest._url, postContent).Result)
                {
                    response.EnsureSuccessStatusCode();
                    using (HttpContent content = response.Content)
                    {
                        string result = content.ReadAsStringAsync().Result;
                        var values = HttpUtility.ParseQueryString(result);
                        foreach (var key in values.AllKeys)
                            resp.Add(key, values[key]);
                    }
                }
            }

            return resp;
        }

        private void AddCommonFields(OperationBase _base)
        {
            if (!IsNullOrWhiteSpace(_base.Invoice))
                Values.Add("xInvoice", _base.Invoice);

            if (_base.Tip != null)
                Values.Add("xTip", Format("{0:N2}", _base.Tip));

            if (_base.Tax != null)
                Values.Add("xTax", Format("{0:N2}", _base.Tax));

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

            if (!IsNullOrWhiteSpace(_base.ShipFirstName))
                Values.Add("xShipFirstName", _base.ShipFirstName);

            if (!IsNullOrWhiteSpace(_base.ShipMiddleName))
                Values.Add("xShipMiddleName", _base.ShipMiddleName);

            if (!IsNullOrWhiteSpace(_base.ShipLastName))
                Values.Add("xShipLastName", _base.ShipLastName);

            if (!IsNullOrWhiteSpace(_base.ShipCompany))
                Values.Add("xShipCompany", _base.ShipCompany);

            if (!IsNullOrWhiteSpace(_base.ShipStreet))
                Values.Add("xShipStreet", _base.ShipStreet);

            if (!IsNullOrWhiteSpace(_base.ShipStreet2))
                Values.Add("xShipStreet2", _base.ShipStreet2);

            if (!IsNullOrWhiteSpace(_base.ShipCity))
                Values.Add("xShipCity", _base.ShipCity);

            if (!IsNullOrWhiteSpace(_base.ShipState))
                Values.Add("xShipState", _base.ShipState);

            if (!IsNullOrWhiteSpace(_base.ShipZip))
                Values.Add("xShipZip", _base.ShipZip);

            if (!IsNullOrWhiteSpace(_base.ShipCountry))
                Values.Add("xShipCountry", _base.ShipCountry);

            if (!IsNullOrWhiteSpace(_base.ShipPhone))
                Values.Add("xShipPhone", _base.ShipPhone);

            if (!IsNullOrWhiteSpace(_base.ShipMobile))
                Values.Add("xShipMobile", _base.ShipMobile);
        }

        private void AddSpecialFields(Sale _sale)
        {
            if (_sale.RxAmount > 0)
                Values.Add("xRxAmount", Format("{0:N2}", _sale.RxAmount));

            if (_sale.DentalAmount > 0)
                Values.Add("xDentalAmount", Format("{0:N2}", _sale.DentalAmount));

            if (_sale.VisionAmount > 0)
                Values.Add("xVisionAmount", Format("{0:N2}", _sale.VisionAmount));

            if (_sale.TransitAmount > 0)
                Values.Add("xTransitAmount", Format("{0:N2}", _sale.TransitAmount));

            if (_sale.CopayAmount > 0)
                Values.Add("xCopayAmount", Format("{0:N2}", _sale.CopayAmount));

            if (_sale.ClinicalAmount > 0)
                Values.Add("xClinicalAmount", Format("{0:N2}", _sale.ClinicalAmount));

            if (!IsNullOrWhiteSpace(_sale.OrderID))
                Values.Add("xOrderID", _sale.OrderID);

            if (_sale.AllowDuplicate)
                Values.Add("xAllowDuplicate", _sale.AllowDuplicate.ToString());

            if (_sale.Currency != null)
                Values.Add("xCurrency", _sale.Currency.Value.ToString());
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
