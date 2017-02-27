using GSharp.Native.Classes;
using GSharp.Native.JIT;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace GSharp.Native
{
    public class StringTable : IEnumerable<string>
    {

        private class StringTableEnumerator : IEnumerator<string>, IEnumerator
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

        public class StringUserDataTable
        {
            private INetworkStringTable table;

            internal StringUserDataTable(INetworkStringTable table)
            {
                this.table = table;
            }

            public IntPtr GetUserData(int index, IntPtr lengthOut = default(IntPtr))
            {
                return table.GetStringUserData(index, lengthOut);
            }

            public int Count()
            {
                return table.GetNumStrings();
            }

            public IntPtr this[int index]
            {
                get
                {
                    return GetUserData(index);
                }
            }
        }

        static private INetworkStringTableContainer container;
        static StringTable()
        {
#if CLIENT
            container = NativeInterface.Load<INetworkStringTableContainer>("engine", StringTableInterfaceName.CLIENT);
#elif SERVER
            container = NativeInterface.Load<INetworkStringTableContainer>("engine", StringTableInterfaceName.SERVER);
#else
#warning StringTable needs CLIENT or SERVER defined
#endif
        }

        public static StringTable GetTable(int index)
        {
            if (container == null) return null;
            var stringTablePointer = container.GetTable(index);
            if (stringTablePointer == IntPtr.Zero) return null;
            var stringTable = JITEngine.GenerateClass<INetworkStringTable>(stringTablePointer);
            if (stringTable == null) return null;
            return new StringTable(stringTable);
        }

        public static StringTable FindTable(string name)
        {
            if (container == null) return null;
            var stringTablePointer = container.FindTable(name);
            if (stringTablePointer == IntPtr.Zero) return null;
            var stringTable = JITEngine.GenerateClass<INetworkStringTable>(stringTablePointer);
            if (stringTable == null) return null;
            return new StringTable(stringTable);
        }


        private INetworkStringTable table;

        private StringUserDataTable _UserData;
        public StringUserDataTable UserData
        {
            get
            {
                if(_UserData == null) { _UserData = new StringUserDataTable(table); }
                return _UserData;
            }
        }

        public StringTable(INetworkStringTable table)
        {
            this.table = table;
        }

        public string this[int index]
        {
            get
            {
                return GetString(index);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new StringTableEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new StringTableEnumerator(this);
        }



        #region INetworkStringTable Passthrough
        public string GetTableName()
        {
            return table.GetTableName();
        }

        public int GetTableId()
        {
            return table.GetTableId();
        }

        public int Count()
        {
            return table.GetNumStrings();
        }

        public int GetMaxStrings()
        {
            return table.GetMaxStrings();
        }

        public int GetEntryBits()
        {
            return table.GetEntryBits();
        }

        public void SetTick(int tick)
        {
            table.SetTick(tick);
        }

        public bool ChangedSinceTick(int tick)
        {
            return table.ChangedSinceTick(tick);
        }

        public int AddString(bool bIsServer, string value, int length = -1, IntPtr userdata = default(IntPtr))
        {
            return table.AddString(bIsServer, value, length, userdata);
        }

        public string GetString(int stringNumber)
        {
            return table.GetString(stringNumber);
        }

        public void SetStringUserData(int stringNumber, int length, IntPtr userdata)
        {
            table.SetStringUserData(stringNumber, length, userdata);
        }

        public IntPtr GetStringUserData(int stringNumber, IntPtr length)
        {
            return table.GetStringUserData(stringNumber, length);
        }

        public int FindStringIndex(string str)
        {
            return table.FindStringIndex(str);
        }

        public void SetStringChangedCallback(IntPtr obj, pfnStringChanged changeFunc)
        {
            table.SetStringChangedCallback(obj, changeFunc);
        }
        #endregion
    }

}
