using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.StringTable
{
    internal class StringTableEnumerator : IEnumerator<string>, IEnumerator
    {
        private StringTable table;
        public StringTableEnumerator(StringTable table)
        {
            this.table = table;
        }


        private int position = -1;
        public object Current
        {
            get
            {
                return table[position];
            }
        }

        string IEnumerator<string>.Current
        {
            get
            {
                return table[position];
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < table.Count());
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            table = null;
        }
    }
}
