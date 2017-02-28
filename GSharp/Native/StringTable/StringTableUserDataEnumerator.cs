using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.StringTable
{
    internal class StringTableUserDataEnumerator<TClass> : IEnumerator<Entry<TClass>>
    {
        private StringTable table;
        private StringUserDataTable<TClass> userdata;
        public StringTableUserDataEnumerator(StringTable<TClass> table)
        {
            this.table = table;
            this.userdata = table.UserData;
        }


        private int position = -1;
        Entry<TClass> IEnumerator<Entry<TClass>>.Current
        {
            get
            {
                return new Entry<TClass>(table[position], userdata[position]);
            }
        }

        public object Current
        {
            get
            {
                return (this as IEnumerator<Entry<TClass>>).Current;
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
