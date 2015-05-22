using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace STM32Control.CrashReport.SystemInfo
{
    internal class SysInfoRetriever
    {
        private ManagementObjectSearcher _sysInfoSearcher;
        private SysInfoResult _sysInfoResult;
        private SysInfoQuery _sysInfoQuery;

        public SysInfoResult Retrieve(SysInfoQuery sysInfoQuery)
        {
            _sysInfoQuery = sysInfoQuery;
            _sysInfoSearcher = new ManagementObjectSearcher(string.Format("SELECT * FROM {0}", _sysInfoQuery.QueryText));
            _sysInfoResult = new SysInfoResult(_sysInfoQuery.Name);

            foreach (ManagementObject managementObject in _sysInfoSearcher.Get())
            {
                _sysInfoResult.AddNode(managementObject.GetPropertyValue(_sysInfoQuery.DisplayField).ToString().Trim());
                _sysInfoResult.AddChildren(GetChildren(managementObject));
            }
            return _sysInfoResult;
        }

        private IEnumerable<SysInfoResult> GetChildren(ManagementBaseObject managementObject)
        {
            SysInfoResult childResult = null;
            ICollection<SysInfoResult> childList = new List<SysInfoResult>();

            foreach (var propertyData in managementObject.Properties)
            {
                if (childResult == null)
                {
                    childResult = new SysInfoResult(_sysInfoQuery.Name + "_Child");
                    childList.Add(childResult);
                }

                var nodeValue = string.Format("{0} = {1}", propertyData.Name, Convert.ToString(propertyData.Value));
                childResult.Nodes.Add(nodeValue);
            }

            return childList;
        }
    }

}
