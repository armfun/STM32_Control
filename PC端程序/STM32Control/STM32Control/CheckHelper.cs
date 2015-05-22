using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace STM32Control
{
    public static class CheckHelper
    {
        /// <summary>
        /// 验证IP地址
        /// </summary>
        /// <param name="IP">IP的字符串</param>
        /// <returns></returns>
        public static bool IPCheck(string IP)
        {
            string num = "(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)";
            return Regex.IsMatch(IP, ("^" + num + "\\." + num + "\\." + num + "\\." + num + "$"));
        }

        /// <summary>
        /// 验证输入角度是否正确,180度
        /// </summary>
        /// <param name="Degree"></param>
        /// <returns></returns>
        public static bool DegreeCheck(string Degree)
        {
            //360度的正则表达式^\d$|^[1-9]\d$|^[1-2]\d\d$|^3[0-5]\d|^360$
            return Regex.IsMatch(Degree, "^\\d$|^[1-9]\\d$|^1[0-7]\\d$|^180$");
        }

        /// <summary>
        /// 验证端口号(0-1400)
        /// </summary>
        /// <param name="Port"></param>
        /// <returns></returns>
        public static bool PortCheck(string Port)
        {
            return Regex.IsMatch(Port, "^\\d$|^[1-9]\\d$|^[1-9]\\d{2}$|^1[1-3]\\d{2}$|^1400$");
        }
    }
}
