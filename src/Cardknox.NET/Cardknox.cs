using CardknoxApi.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using static System.String;

namespace CardknoxApi
{
    /// <summary>
    /// Primary object class for interacting with the Cardknox API.
    /// </summary>
    public class Cardknox
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RequestStartedEventHandler(object sender, CardknoxEventArgs e);
        /// <summary>
        /// 
        /// </summary>
        public event RequestStartedEventHandler RequestStarted;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RequestCompletedEventHandler(object sender, CardknoxEventArgs e);
        /// <summary>
        /// 
        /// </summary>
        public event RequestCompletedEventHandler RequestCompleted;

        /// <summary>
        /// 
        /// </summary>
        private CardknoxRequest _request { get; }
        private NameValueCollection _values { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">The <see cref="CardknoxRequest"/> object that is used to make the request.</param>
        public Cardknox(CardknoxRequest request)
        {
            _values = new NameValueCollection
            {
                { "xKey", request._key },
                { "xVersion", request._cardknoxVersion },
                { "xSoftwareName", request._software },
                { "xSoftwareVersion", request._softwareVersion }
            };
            _request = request;
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _sale.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _sale.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_sale.CardNum))
            {
                _values.Add("xCardNum", _sale.CardNum);
                if (!IsNullOrWhiteSpace(_sale.CVV))
                    _values.Add("xCVV", _sale.CVV);
                if (!IsNullOrWhiteSpace(_sale.Exp))
                    _values.Add("xExp", _sale.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_sale.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                _values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MagStripe))
            {
                _values.Add("xMagStripe", _sale.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_sale.Street))
                _values.Add("xStreet", _sale.Street);

            if (!IsNullOrWhiteSpace(_sale.Zip))
                _values.Add("xZip", _sale.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_sale.IP))
                _values.Add("xIP", _sale.IP);

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _save.Operation);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_save.CardNum))
            {
                _values.Add("xCardNum", _save.CardNum);
                if (!IsNullOrWhiteSpace(_save.CVV))
                    _values.Add("xCVV", _save.CVV);
                if (!IsNullOrWhiteSpace(_save.Exp))
                    _values.Add("xExp", _save.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_save.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_save.Token))
            {
                _values.Add("xToken", _save.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_save.MagStripe))
            {
                _values.Add("xMagStripe", _save.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_save.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_save.Name))
                _values.Add("xName", _save.Name);

            if (!IsNullOrWhiteSpace(_save.Street))
                _values.Add("xStreet", _save.Street);

            if (!IsNullOrWhiteSpace(_save.Zip))
                _values.Add("xZip", _save.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_save.IP))
                _values.Add("xIP", _save.IP);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _refund.Operation);

            _values.Add("xAmount", String.Format("{0:N2}", _refund.Amount));
            _values.Add("xRefNum", _refund.RefNum);
            // END required information

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _auth.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _auth.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_auth.CardNum))
            {
                _values.Add("xCardNum", _auth.CardNum);
                if (!IsNullOrWhiteSpace(_auth.CVV))
                    _values.Add("xCVV", _auth.CVV);
                if (!IsNullOrWhiteSpace(_auth.Exp))
                    _values.Add("xExp", _auth.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_auth.Exp))
                    requiredAdded = false; 
            }
            else if (!IsNullOrWhiteSpace(_auth.Token))
            {
                _values.Add("xToken", _auth.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_auth.MagStripe))
            {
                _values.Add("xMagStripe", _auth.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_auth.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_auth.Street))
                _values.Add("xStreet", _auth.Street);

            if (!IsNullOrWhiteSpace(_auth.Zip))
                _values.Add("xZip", _auth.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_auth.IP))
                _values.Add("xIP", _auth.IP);

            AddCommonFields(_auth);

            AddSpecialFields(_auth);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _capture.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _capture.Amount));
            _values.Add("xRefNum", _capture.RefNum);
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_capture.Street))
                _values.Add("xStreet", _capture.Street);

            if (!IsNullOrWhiteSpace(_capture.Zip))
                _values.Add("xZip", _capture.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_capture.IP))
                _values.Add("xIP", _capture.IP);

            AddCommonFields(_capture);

            AddSpecialFields(_capture);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _credit.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _credit.Amount));
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_credit.CardNum))
            {
                _values.Add("xCardNum", _credit.CardNum);
                if (!IsNullOrWhiteSpace(_credit.CVV))
                    _values.Add("xCVV", _credit.CVV);
                if (!IsNullOrWhiteSpace(_credit.Exp))
                    _values.Add("xExp", _credit.Exp);
                requiredAdded = true;

                if (IsNullOrWhiteSpace(_credit.Exp))
                    requiredAdded = false;
            }
            else if (!IsNullOrWhiteSpace(_credit.Token))
            {
                _values.Add("xToken", _credit.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.MagStripe))
            {
                _values.Add("xMagStripe", _credit.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_credit.Operation} operation.");
            // END required information

            // The next many fields are optional and so there will be a lot of if statements here
            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_credit.Street))
                _values.Add("xStreet", _credit.Street);

            if (!IsNullOrWhiteSpace(_credit.Zip))
                _values.Add("xZip", _credit.Zip);

            // IP is optional, but is highly recommended for fraud detection
            if (!IsNullOrWhiteSpace(_credit.IP))
                _values.Add("xIP", _credit.IP);

            AddCommonFields(_credit);

            AddSpecialFields(_credit);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _void.Operation);
            _values.Add("xRefNum", _void.RefNum);
            // END required information

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _adjust.Operation);
            _values.Add("xRefNum", _adjust.RefNum);
            _values.Add("xAmount", Format("{0:N2}", _adjust.Amount));
            // END required information

            if (!IsNullOrWhiteSpace(_adjust.Street))
                _values.Add("xStreet", _adjust.Street);

            if (!IsNullOrWhiteSpace(_adjust.Zip))
                _values.Add("xZip", _adjust.Zip);

            if (!IsNullOrWhiteSpace(_adjust.Name))
                _values.Add("xName", _adjust.Name);

            if (!IsNullOrWhiteSpace(_adjust.IP))
                _values.Add("xIP", _adjust.IP);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _auth.Operation);
            _values.Add("xAuthCode", _auth.AuthCode);
            // END required information

            if (!IsNullOrWhiteSpace(_auth.Street))
                _values.Add("xStreet", _auth.Street);

            if (!IsNullOrWhiteSpace(_auth.Zip))
                _values.Add("xZip", _auth.Zip);

            if (!IsNullOrWhiteSpace(_auth.Name))
                _values.Add("xName", _auth.Name);

            if (!IsNullOrWhiteSpace(_auth.IP))
                _values.Add("xIP", _auth.IP);

            AddCommonFields(_auth);

            AddSpecialFields(_auth);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _refund.Operation);
            _values.Add("xRefNum", _refund.RefNum);
            // END required information

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _release.Operation);
            _values.Add("xRefNum", _release.RefNum);
            // END required information

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _sale.Operation);
            if (!IsNullOrWhiteSpace(_sale.Name))
                throw new InvalidOperationException("Name is required.");
            bool requiredAdded = false;
            if (!IsNullOrWhiteSpace(_sale.Routing) && !IsNullOrWhiteSpace(_sale.Account))
            {
                _values.Add("xRouting", _sale.Routing);
                _values.Add("xAccount", _sale.Account);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                _values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MICR))
            {
                _values.Add("xMICR", _sale.MICR);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            _values.Add("xAmount", Format("{0:N2}", _sale.Amount));

            _values.Add("xName", _sale.Name);
            // END required information

            if (!IsNullOrWhiteSpace(_sale.IP))
                _values.Add("xIP", _sale.IP);

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _credit.Operation);
            if (!IsNullOrWhiteSpace(_credit.Name))
                throw new InvalidOperationException("Name is required.");
            bool requiredAdded = false;
            if (!IsNullOrWhiteSpace(_credit.Routing) && !IsNullOrWhiteSpace(_credit.Account))
            {
                _values.Add("xRouting", _credit.Routing);
                _values.Add("xAccount", _credit.Account);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.Token))
            {
                _values.Add("xToken", _credit.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.MICR))
            {
                _values.Add("xMICR", _credit.MICR);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_credit.Operation} operation.");
            _values.Add("xAmount", Format("{0:N2}", _credit.Amount));
            _values.Add("xName", _credit.Name);
            // END required information

            if (!IsNullOrWhiteSpace(_credit.IP))
                _values.Add("xIP", _credit.IP);

            AddCommonFields(_credit);

            AddSpecialFields(_credit);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _save.Operation);
            if (!IsNullOrWhiteSpace(_save.Name))
                throw new InvalidOperationException("Name is required.");
            bool requiredAdded = false;
            if (!IsNullOrWhiteSpace(_save.Routing) && !IsNullOrWhiteSpace(_save.Account))
            {
                _values.Add("xRouting", _save.Routing);
                _values.Add("xAccount", _save.Account);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_save.MICR))
            {
                _values.Add("xMICR", _save.MICR);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_save.Operation} operation.");
            _values.Add("xName", _save.Name);
            // END required information

            if (!IsNullOrWhiteSpace(_save.IP))
                _values.Add("xIP", _save.IP);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _void.Operation);
            _values.Add("xAmount", Format("{0:N2}", _void.Amount));
            _values.Add("xRefNum", _void.RefNum);
            // END required information

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _refund.Operation);
            _values.Add("xAmount", Format("{0:N2}", _refund.Amount));
            _values.Add("xRefNum", _refund.RefNum);
            // END required information

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _sale.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _sale.Amount));
            _values.Add("xDUKPT", _sale.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_sale.CardNum))
            {
                _values.Add("xCardNum", _sale.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.Token))
            {
                _values.Add("xToken", _sale.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_sale.MagStripe))
            {
                _values.Add("xMagStripe", _sale.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_sale.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_sale.Street))
                _values.Add("xStreet", _sale.Street);

            if (!IsNullOrWhiteSpace(_sale.Zip))
                _values.Add("xZip", _sale.Zip);

            if (!IsNullOrWhiteSpace(_sale.IP))
                _values.Add("xIP", _sale.IP);

            AddCommonFields(_sale);

            AddSpecialFields(_sale);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _credit.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _credit.Amount));
            _values.Add("xDUKPT", _credit.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_credit.CardNum))
            {
                _values.Add("xCardNum", _credit.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.Token))
            {
                _values.Add("xToken", _credit.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_credit.MagStripe))
            {
                _values.Add("xMagStripe", _credit.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_credit.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_credit.Street))
                _values.Add("xStreet", _credit.Street);

            if (!IsNullOrWhiteSpace(_credit.Zip))
                _values.Add("xZip", _credit.Zip);

            if (!IsNullOrWhiteSpace(_credit.IP))
                _values.Add("xIP", _credit.IP);

            AddCommonFields(_credit);

            AddSpecialFields(_credit);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _bal.Operation);
            _values.Add("xDUKPT", _bal.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_bal.CardNum))
            {
                _values.Add("xCardNum", _bal.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_bal.Token))
            {
                _values.Add("xToken", _bal.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_bal.MagStripe))
            {
                _values.Add("xMagStripe", _bal.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_bal.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_bal.IP))
                _values.Add("xIP", _bal.IP);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
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
            if (_values.AllKeys.Length > 4 && !force)
                throw new InvalidOperationException("A new instance of Cardknox is required to perform this operation unless 'force' is set to 'true'.");
            else if (force)
            {
                string[] toRemove = _values.AllKeys;
                foreach (var v in toRemove)
                    _values.Remove(v);
                _values.Add("xKey", _request._key);
                _values.Add("xVersion", _request._cardknoxVersion);
                _values.Add("xSoftwareName", _request._software);
                _values.Add("xSoftwareVersion", _request._softwareVersion);
            }

            // BEGIN required information
            _values.Add("xCommand", _voucher.Operation);
            _values.Add("xAmount", String.Format("{0:N2}", _voucher.Amount));
            _values.Add("xDUKPT", _voucher.DUKPT);
            bool requiredAdded = false;
            // These groups are mutually exclusive
            if (!IsNullOrWhiteSpace(_voucher.CardNum))
            {
                _values.Add("xCardNum", _voucher.CardNum);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_voucher.Token))
            {
                _values.Add("xToken", _voucher.Token);
                requiredAdded = true;
            }
            else if (!IsNullOrWhiteSpace(_voucher.MagStripe))
            {
                _values.Add("xMagStripe", _voucher.MagStripe);
                requiredAdded = true;
            }
            if (!requiredAdded)
                throw new Exception($"Missing required values. Please refer to the API documentation for the {_voucher.Operation} operation.");
            // END required information

            // Optional, but recommended
            if (!IsNullOrWhiteSpace(_voucher.Street))
                _values.Add("xStreet", _voucher.Street);

            if (!IsNullOrWhiteSpace(_voucher.Zip))
                _values.Add("xZip", _voucher.Zip);

            if (!IsNullOrWhiteSpace(_voucher.IP))
                _values.Add("xIP", _voucher.IP);

            AddCommonFields(_voucher);

            AddSpecialFields(_voucher);

            if (RequestStarted != null)
                Log.LogRequest(_values);
            else RequestStarted.Invoke(this, new CardknoxEventArgs(_values));

            var resp = MakeRequest();
            if (RequestCompleted != null)
                Log.LogResponse(resp);
            else RequestCompleted.Invoke(this, new CardknoxEventArgs(resp));

            return new CardknoxResponse(resp);
        }
        #endregion

        #region private methods
        private NameValueCollection MakeRequest()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient c = new WebClient();
            string req = System.Text.Encoding.ASCII.GetString(c.UploadValues(CardknoxRequest._url, _values));
            NameValueCollection resp = HttpUtility.ParseQueryString(req);

            return resp;
        }

        private void AddCommonFields(OperationBase _base)
        {
            if (!IsNullOrWhiteSpace(_base.Invoice))
                _values.Add("xInvoice", _base.Invoice);

            if (_base.Tip != null)
                _values.Add("xTip", Format("{0:N2}", _base.Tip));

            if (_base.Tax != null)
                _values.Add("xTax", Format("{0:N2}", _base.Tax));

            if (!IsNullOrWhiteSpace(_base.BillFirstName))
                _values.Add("xBillFirstName", _base.BillFirstName);

            if (!IsNullOrWhiteSpace(_base.BillMiddleName))
                _values.Add("xBillMiddleName", _base.BillMiddleName);

            if (!IsNullOrWhiteSpace(_base.BillLastName))
                _values.Add("xBillLastName", _base.BillLastName);

            if (!IsNullOrWhiteSpace(_base.BillCompany))
                _values.Add("xBillCompany", _base.BillCompany);

            if (!IsNullOrWhiteSpace(_base.BillStreet))
                _values.Add("xBillStreet", _base.BillStreet);

            if (!IsNullOrWhiteSpace(_base.BillStreet2))
                _values.Add("xBillStreet2", _base.BillStreet2);

            if (!IsNullOrWhiteSpace(_base.BillCity))
                _values.Add("xBillCity", _base.BillCity);

            if (!IsNullOrWhiteSpace(_base.BillState))
                _values.Add("xBillState", _base.BillState);

            if (!IsNullOrWhiteSpace(_base.BillZip))
                _values.Add("xBillZip", _base.BillZip);

            if (!IsNullOrWhiteSpace(_base.BillCountry))
                _values.Add("xBillCountry", _base.BillCountry);

            if (!IsNullOrWhiteSpace(_base.BillPhone))
                _values.Add("xBillPhone", _base.BillPhone);

            if (!IsNullOrWhiteSpace(_base.BillMobile))
                _values.Add("xBillMobile", _base.BillMobile);

            if (!IsNullOrWhiteSpace(_base.ShipFirstName))
                _values.Add("xShipFirstName", _base.ShipFirstName);

            if (!IsNullOrWhiteSpace(_base.ShipMiddleName))
                _values.Add("xShipMiddleName", _base.ShipMiddleName);

            if (!IsNullOrWhiteSpace(_base.ShipLastName))
                _values.Add("xShipLastName", _base.ShipLastName);

            if (!IsNullOrWhiteSpace(_base.ShipCompany))
                _values.Add("xShipCompany", _base.ShipCompany);

            if (!IsNullOrWhiteSpace(_base.ShipStreet))
                _values.Add("xShipStreet", _base.ShipStreet);

            if (!IsNullOrWhiteSpace(_base.ShipStreet2))
                _values.Add("xShipStreet2", _base.ShipStreet2);

            if (!IsNullOrWhiteSpace(_base.ShipCity))
                _values.Add("xShipCity", _base.ShipCity);

            if (!IsNullOrWhiteSpace(_base.ShipState))
                _values.Add("xShipState", _base.ShipState);

            if (!IsNullOrWhiteSpace(_base.ShipZip))
                _values.Add("xShipZip", _base.ShipZip);

            if (!IsNullOrWhiteSpace(_base.ShipCountry))
                _values.Add("xShipCountry", _base.ShipCountry);

            if (!IsNullOrWhiteSpace(_base.ShipPhone))
                _values.Add("xShipPhone", _base.ShipPhone);

            if (!IsNullOrWhiteSpace(_base.ShipMobile))
                _values.Add("xShipMobile", _base.ShipMobile);
        }

        private void AddSpecialFields(Sale _sale)
        {
            if (_sale.RxAmount > 0)
                _values.Add("xRxAmount", Format("{0:N2}", _sale.RxAmount));

            if (_sale.DentalAmount > 0)
                _values.Add("xDentalAmount", Format("{0:N2}", _sale.DentalAmount));

            if (_sale.VisionAmount > 0)
                _values.Add("xVisionAmount", Format("{0:N2}", _sale.VisionAmount));

            if (_sale.TransitAmount > 0)
                _values.Add("xTransitAmount", Format("{0:N2}", _sale.TransitAmount));

            if (_sale.CopayAmount > 0)
                _values.Add("xCopayAmount", Format("{0:N2}", _sale.CopayAmount));

            if (_sale.ClinicalAmount > 0)
                _values.Add("xClinicalAmount", Format("{0:N2}", _sale.ClinicalAmount));

            if (!IsNullOrWhiteSpace(_sale.OrderID))
                _values.Add("xOrderID", _sale.OrderID);

            if (_sale.AllowDuplicate)
                _values.Add("xAllowDuplicate", _sale.AllowDuplicate.ToString());
        }
        #endregion
    }
}
