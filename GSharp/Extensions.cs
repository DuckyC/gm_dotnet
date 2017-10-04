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


        //Source: https://github.com/cartman300/Libraria/blob/b38562abe1c45bc0e05101f664816702c0d8f072/LibrariaShared/ArrayListExtensions.cs
        public static void Replace<T>(this List<T> L, T Item, IEnumerable<T> Items)
        {
            int Idx = L.IndexOf(Item);
            L.RemoveAt(Idx);
            L.InsertRange(Idx, Items);
        }
    }
}
