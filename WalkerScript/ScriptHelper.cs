using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WalkerScript.Exceptions;

namespace WalkerScript
{
    public static class ScriptHelper
    {
        public static string GetAttribute(this XElement element, string attributeName, bool required = true)
        {
            var atr = element.Attribute(attributeName);
            if (atr == null)
            {
                if (required)
                    throw new InvalidScriptException("Missing attribute '{0}' in element '{1}'", attributeName, element.Name);
                return null;
            }

            return atr.Value;
        }

        public static Dictionary<string, string> GetAttributes(this XElement element, Func<string, string> evaluate = null)
        {
            Func<string, string> ev = evaluate ?? (a => a);
            return element.Attributes().ToDictionary(a => a.Name.ToString(), a => ev(a.Value));
        }

        public static Type LoadType(string typeId)
        {
            var t = typeId.Split(new[] { "," }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (t.Length == 1)
                return Type.GetType(t[0]);

            var asm = Assembly.Load(t[1]);
            return asm.GetType(t[0]);
        }
    }
}
