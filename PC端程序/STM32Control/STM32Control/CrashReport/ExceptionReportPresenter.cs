using STM32Control.CrashReport.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace STM32Control.CrashReport
{
    public class ExceptionReportPresenter
    {
        EmailBuilder emailTextBuilder;
        ExceptionReportGenerator reportGenerator;
        public ExceptionReportPresenter(ExceptionReportInfo Info)
        {
            ReportInfo = Info;
            emailTextBuilder = new EmailBuilder(ReportInfo);
            reportGenerator = new ExceptionReportGenerator(ReportInfo);
        }

        public ExceptionReportInfo ReportInfo { get; private set; }

        public void SendReportByEmail()
        {
            string emailText = BuildEmailText();
            try
            {
                MailSender mailSender = new MailSender(ReportInfo);
                mailSender.SendSmtp(emailText);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.StackTrace, "Send Email Error:" + exception.Message, MessageBoxButton.OK);
            }
        }

        public void ScreenshotFileDelte()
        {
            DirectoryInfo fold = new DirectoryInfo(System.IO.Path.GetTempPath());
            if (fold.Exists)
            {
                FileInfo[] files = fold.GetFiles("ExceptionReport_Screenshot*.jpg");
                foreach (FileInfo f in files)
                {
                    try
                    {
                        f.Delete();
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        private string BuildEmailText()
        {
            string emailIntroString = emailTextBuilder.CreateIntro(ReportInfo.TakeScreenshot);
            StringBuilder entireEmailText = new StringBuilder(emailIntroString);

            ExceptionReport report = reportGenerator.CreateExceptionReport();
            entireEmailText.Append(report);

            return entireEmailText.ToString();
        }

    }
}
