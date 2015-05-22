using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace STM32Control.CrashReport.Core
{
    public class ConfigReader
    {
        const string SMTP = "SMTP";

        private readonly ExceptionReportInfo _info;

        public ConfigReader(ExceptionReportInfo reportInfo)
        {
            _info = reportInfo;
        }
        private static string GetMailSetting(string configName)
        {
            return GetConfigSetting("Email", configName);
        }

        private static string GetConfigSetting(string sectionName, string configName)
        {
            var sections = ConfigurationManager.GetSection(@"ExceptionReporter/" + sectionName) as NameValueCollection;
            if (sections == null) return string.Empty;

            return sections[configName];
        }
        public void ReadMailSettings()
        {
            ReadMailMethod();
            ReadMailValues();
        }

        private void ReadMailValues()
        {
            _info.SmtpServer = GetMailSetting("SmtpServer").GetString(_info.SmtpServer);
            _info.SmtpUsername = GetMailSetting("SmtpUsername").GetString(_info.SmtpUsername);
            _info.SmtpPassword = GetMailSetting("SmtpPassword").GetString(_info.SmtpPassword);
            _info.SmtpFromAddress = GetMailSetting("from").GetString(_info.SmtpFromAddress);
            _info.EmailReportAddress = GetMailSetting("to").GetString(_info.EmailReportAddress);
            _info.TakeScreenshot = GetMailSetting("Screenshot").GetBool(_info.TakeScreenshot);
        }

        private void ReadMailMethod()
        {
            var mailMethod = GetMailSetting("method");
            if (string.IsNullOrEmpty(mailMethod)) return;

            _info.MailMethod = mailMethod.Equals(SMTP) ? ExceptionReportInfo.EmailMethod.SMTP :
                                                         ExceptionReportInfo.EmailMethod.SimpleMAPI;
        }

        internal static string GetConfigFilePath()
        {
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
        }
    }
}
