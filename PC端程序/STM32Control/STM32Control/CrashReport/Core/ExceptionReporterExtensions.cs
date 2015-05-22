using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Control.CrashReport.Core
{
    public static class ExceptionReporterExtensions	
	{
		public static StringBuilder AppendDottedLine(this StringBuilder stringBuilder)
		{
			return stringBuilder.AppendLine("-----------------------------");
		}

		public static string GetString(this string newString, string currentString)
		{
			return string.IsNullOrEmpty(newString) ? currentString : newString.Trim();
		}

        public static bool GetBool(this string configString, bool currentValue)
        {
            if (string.IsNullOrEmpty(configString)) return currentValue;

            switch (configString.ToLower())
            {
                case "y":
                case "true":
                    return true;

                case "n":
                case "false":
                    return false;
            }

            return currentValue;
        }

		public static bool IsEmpty(this string input)
		{
			return string.IsNullOrEmpty(input);
		}
	}
}
