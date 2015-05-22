using STM32Control.CrashReport.SystemInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Control.CrashReport.Core
{
    public class EmailBuilder
    {
        private readonly ExceptionReportInfo _reportInfo;
        private StringBuilder _stringBuilder;
        private readonly IEnumerable<SysInfoResult> _sysInfoResults;
        public EmailBuilder(ExceptionReportInfo reportInfo)
        {
            _reportInfo = reportInfo;

        }
        public EmailBuilder(ExceptionReportInfo reportInfo, IEnumerable<SysInfoResult> sysInfoResults)
            : this(reportInfo)
        {
            _sysInfoResults = sysInfoResults;
        }

        public string CreateIntro(bool takeScreenshot)
        {
            var s = new StringBuilder()
                 .AppendLine("This is a bug report email.");
            if (takeScreenshot)
            {
                s.AppendLine("A screenshot, taken at the time of the exception, is attached.")
                 .AppendLine();
            }

            return s.ToString();
        }
        #region email password encryption and  decode


        byte[] Enkey = { 3, 7, 5, 2, 0, 4, 1, 6 };
        public string EncryptCrypto(string sendline)
        {
            int c;
            string message = null;
            for (int i = 0; i < sendline.Length; i++)
            {
                c = Asc(sendline[i].ToString());
                c = (c & 0xF8 | Enkey[(c & 0x07)]);
                message = message + Chr(c);
            }
            return message;
        }
        byte[] Dekey = { 4, 6, 3, 0, 5, 2, 7, 1 };
        public string DecryptCrypto(string sendline)
        {
            int c;
            string message = null;
            for (int i = 0; i < sendline.Length; i++)
            {
                c = Asc(sendline[i].ToString());
                c = (c & 0xF8 | Dekey[(c & 0x07)]);
                message = message + Chr(c);
            }
            return message;
        }

        static int Asc(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception(App.Current.TryFindResource("ERR").ToString());
            }
        }

        static string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception(App.Current.TryFindResource("ERR").ToString());
            }

        }
        #endregion
        #region Email Text build

        public ExceptionReport Build()
        {
            _stringBuilder = new StringBuilder().AppendDottedLine();
            BuildGeneralInfo();
            BuildExceptionInfo();
            if (_reportInfo.IsSendPersonalInfo == true)
            {
                BuildSysInfo();
            }
            return new ExceptionReport(_stringBuilder);
        }

        private void BuildGeneralInfo()
        {
            _stringBuilder.AppendLine("[General Info]")
                .AppendLine()
                .AppendLine("Application: " + _reportInfo.AppName)
                .AppendLine("Version:     " + _reportInfo.AppVersion)
                .AppendLine("Region:      " + _reportInfo.RegionInfo)
                .AppendLine("Machine:     " + _reportInfo.MachineName)
                .AppendLine("User:        " + _reportInfo.UserName)
                .AppendLine("Date:        " + _reportInfo.ExceptionDate.ToShortDateString())
                .AppendLine("Time:        " + _reportInfo.ExceptionDate.ToShortTimeString())
                .AppendLine("IP:          " + _reportInfo.IP)
                .AppendLine("Description: " + _reportInfo.Description)
                .AppendLine();
        }

        private void BuildExceptionInfo()
        {
            for (var index = 0; index < _reportInfo.Exceptions.Count; index++)
            {
                var exception = _reportInfo.Exceptions[index];

                _stringBuilder.AppendLine(string.Format("[Exception Info {0}]", index + 1))
                    .AppendLine()
                    .AppendLine(ExceptionHierarchyToString(exception))
                    .AppendLine().AppendDottedLine().AppendLine();
            }
        }
        private void BuildSysInfo()
        {
            _stringBuilder.AppendLine("[System Info]").AppendLine();
            _stringBuilder.Append(SysInfoResultMapper.CreateStringList(_sysInfoResults));
            _stringBuilder.AppendDottedLine().AppendLine();
        }
        #endregion
 
        private static string ExceptionHierarchyToString(Exception exception)
        {
            var currentException = exception;
            var stringBuilder = new StringBuilder();
            var count = 0;

            while (currentException != null)
            {
                if (count++ == 0)
                    stringBuilder.AppendLine("Top-level Exception");
                else
                    stringBuilder.AppendLine("Inner Exception " + (count - 1));

                stringBuilder.AppendLine("Type:        " + currentException.GetType())
                             .AppendLine("Message:     " + currentException.Message)
                             .AppendLine("Source:      " + currentException.Source);

                if (currentException.StackTrace != null)
                    stringBuilder.AppendLine("Stack Trace: " + currentException.StackTrace.Trim());

                stringBuilder.AppendLine();
                currentException = currentException.InnerException;
            }

            var exceptionString = stringBuilder.ToString();
            return exceptionString.TrimEnd();
        }
    }

}
