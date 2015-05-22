using STM32Control.CrashReport.SystemInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace STM32Control.CrashReport.Core
{
    public class ExceptionReportGenerator
    {
        private readonly ExceptionReportInfo _reportInfo;
        private readonly List<SysInfoResult> _sysInfoResults = new List<SysInfoResult>();

        public ExceptionReportGenerator(ExceptionReportInfo reportInfo)
        {
            if (reportInfo == null)
                throw new ExceptionReportGeneratorException("reportInfo cannot be null");

            _reportInfo = reportInfo;

            _reportInfo.ExceptionDate = DateTime.UtcNow;
            _reportInfo.UserName = Environment.UserName;
            _reportInfo.MachineName = Environment.MachineName;
            _reportInfo.RegionInfo = Thread.CurrentThread.CurrentCulture.EnglishName;
            _reportInfo.AppName = string.IsNullOrEmpty(_reportInfo.AppName) ? System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString() : _reportInfo.AppName;
            _reportInfo.AppVersion = string.IsNullOrEmpty(_reportInfo.AppVersion) ? System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() : _reportInfo.AppVersion;
            System.Net.WebClient client = new WebClient();
            try
            {
                _reportInfo.IP = client.DownloadString("http://myip.dnsomatic.com/");
            }
            catch
            {
                _reportInfo.IP = "";
            }
        }

        public ExceptionReport CreateExceptionReport()
        {
            IList<SysInfoResult> sysInfoResults = GetOrFetchSysInfoResults();
            EmailBuilder reportBuilder = new EmailBuilder(_reportInfo, _sysInfoResults);
            return reportBuilder.Build();
        }

        internal IList<SysInfoResult> GetOrFetchSysInfoResults()
        {
            if (_sysInfoResults.Count == 0)
                _sysInfoResults.AddRange(CreateSysInfoResults());

            return _sysInfoResults.AsReadOnly();
        }

        private static IEnumerable<SysInfoResult> CreateSysInfoResults()
        {
            SysInfoRetriever retriever = new SysInfoRetriever();
            List<SysInfoResult> results = new List<SysInfoResult>
						  {
							retriever.Retrieve(SysInfoQueries.OperatingSystem).Filter(
								new[]
								{
									"CodeSet", "CurrentTime", "FreePhysicalMemory",
									"OSArchitecture", "OSLanguage", "Version"
								}),
							retriever.Retrieve(SysInfoQueries.Machine).Filter(
								new[]
								{
									"Machine", "UserName", "TotalPhysicalMemory", "Manufacturer", "Model"
								}),
						  };
            return results;
        }

        internal class ExceptionReportGeneratorException : Exception
        {
            public ExceptionReportGeneratorException(string message)
                : base(message)
            { }
        }
    }
}
