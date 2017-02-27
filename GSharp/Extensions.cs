using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GSharp
{
    public static class Extensions
    {
        /// <summary>
        /// Gets an absolute path to the relative path specified, relative to the exe
        /// </summary>
        /// <returns></returns>
        public static string AbsolutePath(string relative)
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess()?.MainModule?.FileName;
            var exeDirectory = Path.GetDirectoryName(exePath);
            return Path.Combine(exeDirectory, relative);
        }
    }
}
