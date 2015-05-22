using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace STM32Control.CrashReport.Core
{
    public class ExceptionReportInfo
    {

        private readonly List<Exception> _exceptions = new List<Exception>();

        public Exception MainException
        {
            get { return _exceptions.Count > 0 ? _exceptions[0] : null; }
            set
            {
                _exceptions.Clear();
                _exceptions.Add(value);
            }
        }

        public IList<Exception> Exceptions
        {
            get { return _exceptions.AsReadOnly(); }
        }

        public void SetExceptions(IEnumerable<Exception> exceptions)
        {
            _exceptions.Clear();
            _exceptions.AddRange(exceptions);
        }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpFromAddress { get; set; }
        public string SmtpServer { get; set; }

        public string AppName { get; set; }

        public string AppVersion { get; set; }

        public string RegionInfo { get; set; }

        public string MachineName { get; set; }

        public string UserName { get; set; }

        public DateTime ExceptionDate { get; set; }

        public string UserExplanation { get; set; }

        public string EmailReportAddress { get; set; }

        public bool TakeScreenshot { get; set; }
        public string IP { get; set; }

        public string Description { get; set; }

        public bool IsSendPersonalInfo { get; set; }

        public Bitmap ScreenshotImage { get; set; }

        public EmailMethod MailMethod { get; set; }

        public bool ScreenshotAvailable
        {
            get { return TakeScreenshot && ScreenshotImage != null; }
        }

        public ExceptionReportInfo()
        {
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            MailMethod = EmailMethod.SMTP;
            SmtpServer = "smtp.sina.com";
            SmtpUsername = "bugreportstm32@sina.com";
            SmtpPassword = "1GiB-1MsEUtP";
            SmtpFromAddress = "bugreportstm32@sina.com";
            EmailReportAddress = "youngytj@sina.com";
            TakeScreenshot = false;
        }

        public enum EmailMethod
        {
            SimpleMAPI,
            SMTP
        };

        protected void DisposeManagedResources()
        {
            if (ScreenshotImage != null)
            {
                ScreenshotImage.Dispose();
            }
        }
    }
}
