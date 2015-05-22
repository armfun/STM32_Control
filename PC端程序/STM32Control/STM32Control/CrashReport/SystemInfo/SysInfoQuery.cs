using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Control.CrashReport.SystemInfo
{
    internal class SysInfoQuery
    {
        private readonly string _name;
        private readonly bool _useNameAsDisplayField;
        private readonly string _queryText;

        public SysInfoQuery(string name, string query, bool useNameAsDisplayField)
        {
            _name = name;
            _useNameAsDisplayField = useNameAsDisplayField;
            _queryText = query;
        }

        public string QueryText
        {
            get { return _queryText; }
        }

        public string DisplayField
        {
            get { return _useNameAsDisplayField ? "Name" : "Caption"; }
        }

        public string Name
        {
            get { return _name; }
        }
    }

}
