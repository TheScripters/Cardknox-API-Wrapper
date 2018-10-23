using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CardknoxApi.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class CardknoxEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyDictionary<string, string> Results { get { return new ReadOnlyDictionary<string, string>(_results); } }
        private Dictionary<string, string> _results;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_values"></param>
        public CardknoxEventArgs(NameValueCollection _values)
        {
            _results = new Dictionary<string, string>();
            foreach (var k in _values.AllKeys)
            {
                if (!Log.NoInclude.Contains(k))
                    _results.Add(k, _values[k]);
            }
        }
    }
}
