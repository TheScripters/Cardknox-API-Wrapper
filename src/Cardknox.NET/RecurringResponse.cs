using CardknoxApi.RecurringOperations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CardknoxApi
{
    /// <summary>
    /// Object representing response from the Cardknox Recurring API endpoint.
    /// </summary>
    public class RecurringResponse : Customer
    {
        /// <summary>
        /// Request status
        /// </summary>
        public RecurringStatusType Status { get; set; }
        /// <summary>
        /// Raw contents of <see cref="Status"/>
        /// </summary>
        public string StatusString { get; }
        /// <summary>
        /// Result verbiage
        /// </summary>
        public RecurringResultType Result { get; set; }
        /// <summary>
        /// Raw contents of <see cref="Result"/>
        /// </summary>
        public string ResultString { get; }
        /// <summary>
        /// Data returned in reports
        /// </summary>
        public List<Dictionary<string, object>> ReportData { get; set; } = new List<Dictionary<string, object>>();
        /// <summary>
        /// Amount of records retrieved when running a report
        /// </summary>
        public int RecordsReturned { get; set; }
        /// <summary>
        /// Error message if applicable
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Customer Number
        /// </summary>
        public string CustomerNumber { get; set; }
        /// <summary>
        /// Customer ID assigned by Cardknox
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BillName { get; set; }
        /// <summary>
        /// Request Reference Number
        /// </summary>
        public string RecurringRefNum { get; set; }
        /// <summary>
        /// Date customer entered. Given in ET.
        /// </summary>
        public DateTime? EnteredDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_values"></param>
        public RecurringResponse(NameValueCollection _values)
        {
            if (_values.AllKeys.Contains("xResult"))
            {
                ResultString = _values["xResult"];
                try
                {
                    Result = (RecurringResultType)Enum.Parse(typeof(RecurringResultType), _values["xResult"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xError"))
                Error = _values["xError"];
            if (_values.AllKeys.Contains("xErrorCode"))
                ErrorCode = _values["xErrorCode"];
            if (_values.AllKeys.Contains("xStatus"))
            {
                StatusString = _values["xStatus"];
                try
                {
                    Status = (RecurringStatusType)Enum.Parse(typeof(RecurringStatusType), _values["xStatus"]);
                }
                catch { }
            }
            if (_values.AllKeys.Contains("xRecurringRefNum"))
                RecurringRefNum = _values["xRecurringRefNum"];
            if (_values.AllKeys.Contains("xEnteredDate"))
                EnteredDate = DateTime.Parse(_values["xEnteredDate"]);
            if (_values.AllKeys.Contains("xBillFirstName"))
                BillFirstName = _values["xBillFirstName"];
            if (_values.AllKeys.Contains("xBillMiddleName"))
                BillMiddleName = _values["xBillMiddleName"];
            if (_values.AllKeys.Contains("xBillLastName"))
                BillLastName = _values["xBillLastName"];
            if (_values.AllKeys.Contains("xBillName"))
                BillName = _values["xBillName"];
            if (_values.AllKeys.Contains("xBillStreet"))
                BillStreet = _values["xBillStreet"];
            if (_values.AllKeys.Contains("xBillStreet2"))
                BillStreet2 = _values["xBillStreet2"];
            if (_values.AllKeys.Contains("xBillCity"))
                BillCity = _values["xBillCity"];
            if (_values.AllKeys.Contains("xBillState"))
                BillState = _values["xBillState"];
            if (_values.AllKeys.Contains("xBillZip"))
                BillZip = _values["xBillZip"];
            if (_values.AllKeys.Contains("xBillCountry"))
                BillCountry = _values["xBillCountry"];
            if (_values.AllKeys.Contains("xBillCompany"))
                BillCompany = _values["xBillCompany"];
            if (_values.AllKeys.Contains("xBillPhone"))
                BillPhone = _values["xBillPhone"];
            if (_values.AllKeys.Contains("xBillMobile"))
                BillMobile = _values["xBillMobile"];
            if (_values.AllKeys.Contains("xCustomerID"))
                CustomerID = _values["xCustomerID"];
            if (_values.AllKeys.Contains("xCustomerNumber"))
                CustomerNumber = _values["xCustomerNumber"];
            if (_values.AllKeys.Contains("xReportData"))
            {
                ReportData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>($"[{_values["xReportData"]}]");
            }
        }
    }
}
