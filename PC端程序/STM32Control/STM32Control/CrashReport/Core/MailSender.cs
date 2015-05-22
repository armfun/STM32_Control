using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace STM32Control.CrashReport.Core
{
    public class MailSender
    {
        public delegate void CompletedMethodDelegate(bool success);
        private readonly ExceptionReportInfo _reportInfo;
        internal MailSender(ExceptionReportInfo reportInfo)
        {
            _reportInfo = reportInfo;
        }

        public void SendSmtp(string exceptionReport)
        {
            EmailBuilder en = new EmailBuilder(_reportInfo);
            string x = en.EncryptCrypto(_reportInfo.SmtpPassword);
            string n = en.DecryptCrypto(_reportInfo.SmtpPassword).Trim();
            var smtpClient = new SmtpClient(_reportInfo.SmtpServer)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new System.Net.NetworkCredential(_reportInfo.SmtpFromAddress, _reportInfo.SmtpPassword),
                Host = _reportInfo.SmtpServer,
            };
            //smtpClient.EnableSsl = true;
            //smtpClient.Port = 465;
            //smtpClient.UseDefaultCredentials = false;
            var mailMessage = CreateMailMessage(exceptionReport);
            smtpClient.SendAsync(mailMessage, null);

        }
        private MailMessage CreateMailMessage(string exceptionReport)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_reportInfo.SmtpFromAddress, null),
                ReplyTo = new MailAddress(_reportInfo.SmtpFromAddress, null),
                Body = exceptionReport,
                Subject = EmailSubject.Replace("\r", " ").Replace("\n", " "),
            };

            mailMessage.To.Add(new MailAddress(_reportInfo.EmailReportAddress));
            AddAnyAttachments(mailMessage);

            return mailMessage;
        }

        private void AddAnyAttachments(MailMessage mailMessage)
        {
            AttachScreenshot(mailMessage);
        }

        private void AttachScreenshot(MailMessage mailMessage)
        {
            if (_reportInfo.ScreenshotAvailable)
                mailMessage.Attachments.Add(new Attachment(ScreenshotTaker.GetImageAsFile(_reportInfo.ScreenshotImage),
                                                           ScreenshotTaker.ScreenshotMimeType));
        }

        public string EmailSubject
        {
            get
            {
                return "[CrashReport]" + "Product:" + _reportInfo.AppName + " "
                    + " Version:" + _reportInfo.AppVersion + " " + "Description:" + _reportInfo.MainException.Message;
            }
        }
    }

}
