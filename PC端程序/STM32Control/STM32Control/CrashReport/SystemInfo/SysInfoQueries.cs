using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STM32Control.CrashReport.SystemInfo
{
    internal static class SysInfoQueries
    {
        public static readonly SysInfoQuery OperatingSystem = new SysInfoQuery("Operating System", "Win32_OperatingSystem", false);
        public static readonly SysInfoQuery Machine = new SysInfoQuery("Machine", "Win32_ComputerSystem", true);
    }
}
