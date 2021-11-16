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
        public RecurringResponse(Dictionary<string, string> _values)
        {
            if (_values.Keys.Contains("xResult"))
            {
                ResultString = _values["xResult"];
                try
                {
                    Result = (RecurringResultType)Enum.Parse(typeof(RecurringResultType), _values["xResult"]);
                }
                catch { }
            }
            if (_values.Keys.Contains("xError"))
                Error = _values["xError"];
            if (_values.Keys.Contains("xErrorCode"))
                ErrorCode = _values["xErrorCode"];
            if (_values.Keys.Contains("xStatus"))
            {
                StatusString = _values["xStatus"];
                try
                {
                    Status = (RecurringStatusType)Enum.Parse(typeof(RecurringStatusType), _values["xStatus"]);
                }
                catch { }
            }
            if (_values.Keys.Contains("xRecurringRefNum"))
                RecurringRefNum = _values["xRecurringRefNum"];
            if (_values.Keys.Contains("xEnteredDate"))
                EnteredDate = DateTime.Parse(_values["xEnteredDate"]);
            if (_values.Keys.Contains("xBillFirstName"))
                BillFirstName = _values["xBillFirstName"];
            if (_values.Keys.Contains("xBillMiddleName"))
                BillMiddleName = _values["xBillMiddleName"];
            if (_values.Keys.Contains("xBillLastName"))
                BillLastName = _values["xBillLastName"];
            if (_values.Keys.Contains("xBillName"))
                BillName = _values["xBillName"];
            if (_values.Keys.Contains("xBillStreet"))
                BillStreet = _values["xBillStreet"];
            if (_values.Keys.Contains("xBillStreet2"))
                BillStreet2 = _values["xBillStreet2"];
            if (_values.Keys.Contains("xBillCity"))
                BillCity = _values["xBillCity"];
            if (_values.Keys.Contains("xBillState"))
                BillState = _values["xBillState"];
            if (_values.Keys.Contains("xBillZip"))
                BillZip = _values["xBillZip"];
            if (_values.Keys.Contains("xBillCountry"))
                BillCountry = _values["xBillCountry"];
            if (_values.Keys.Contains("xBillCompany"))
                BillCompany = _values["xBillCompany"];
            if (_values.Keys.Contains("xBillPhone"))
                BillPhone = _values["xBillPhone"];
            if (_values.Keys.Contains("xBillMobile"))
                BillMobile = _values["xBillMobile"];
            if (_values.Keys.Contains("xCustomerID"))
                CustomerID = _values["xCustomerID"];
            if (_values.Keys.Contains("xCustomerNumber"))
                CustomerNumber = _values["xCustomerNumber"];
            if (_values.Keys.Contains("xReportData"))
            {
                ReportData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>($"[{_values["xReportData"]}]");
            }
        }
    }
}
