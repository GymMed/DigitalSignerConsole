using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NamespaceRemover
{
    public sealed class NamespaceRemover
    {
        private static NamespaceRemover instance = null;

        private NamespaceRemover()
        {

        }

        public static NamespaceRemover Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NamespaceRemover();
                }
                return instance;
            }
        }

        public static void RemoveFirstAllNamespacesFromXml(ref string XmlString)
        {
            string pattern = "(?<=</)<(?:\\s*)[^:\\s>]+:|<(?:\\s*)[^:\\s>]+:";//@"(?<=<\/)<[^:]+:|<[^:]+:";// @"</?\w+:";

            XmlString = Regex.Replace(XmlString, pattern, m =>
            m.Value.StartsWith("</") ? "</" : "<");
        }
    }
}
