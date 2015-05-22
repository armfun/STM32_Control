using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace STM32Control.CrashReport.SystemInfo
{
    public static class SysInfoResultMapper
    {
        public static void AddTreeViewNode(TreeNode parentNode, SysInfoResult result)
        {
            var nodeRoot = new TreeNode(result.Name);

            foreach (var nodeValueParent in result.Nodes)
            {
                var nodeLeaf = new TreeNode(nodeValueParent);
                nodeRoot.Nodes.Add(nodeLeaf);

                foreach (var childResult in result.ChildResults)
                {
                    foreach (var nodeValue in childResult.Nodes)
                    {
                        nodeLeaf.Nodes.Add(new TreeNode(nodeValue));
                    }
                }
            }
            parentNode.Nodes.Add(nodeRoot);
        }

        public static string CreateStringList(IEnumerable<SysInfoResult> results)
        {
            var stringBuilder = new StringBuilder();

            foreach (var result in results)
            {
                stringBuilder.AppendLine(result.Name);

                foreach (var nodeValueParent in result.Nodes)
                {
                    stringBuilder.AppendLine("-" + nodeValueParent);

                    foreach (var childResult in result.ChildResults)
                    {
                        foreach (var nodeValue in childResult.Nodes)
                        {
                            stringBuilder.AppendLine("--" + nodeValue);		// the max no. of levels is 2, ie '--' is as deep as we go
                        }
                    }
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
