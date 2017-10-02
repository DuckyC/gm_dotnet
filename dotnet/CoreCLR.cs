using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GSharp
{
    public static class CoreCLR
    {
        private static bool findAndCall(IntPtr L, string exportName)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    var attributes = method.GetCustomAttributes(typeof(DllExportAttribute), true);
                    if (attributes.Length == 1)
                    {
                        var attr = attributes[0] as DllExportAttribute;
                        if (attr.ExportName == exportName)
                        {
                            var ar = new object[1];
                            ar[0] = L;
                            method.Invoke(null, ar);
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        public static bool CallOpen(IntPtr L)
        {
            return findAndCall(L, "gmod13_open");
        }

        public static bool CallClose(IntPtr L)
        {
            return findAndCall(L, "gmod13_close");
        }
    }
}
