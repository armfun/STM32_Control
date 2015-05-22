using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Control.CrashReport.Core
{
    public class ExceptionReport
    {
        private readonly StringBuilder _reportString;

        public ExceptionReport(StringBuilder stringBuilder)
        {
            _reportString = stringBuilder;
        }

        public override string ToString()
        {
            return _reportString.ToString();
        }

        private bool Equals(ExceptionReport obj)
        {
            return Equals(obj._reportString.ToString(), _reportString.ToString());
        }

        public override bool Equals(object obj)
        {
            return Equals((ExceptionReport)obj);
        }

        public override int GetHashCode()
        {
            return _reportString.GetHashCode();
        }

    }
}
