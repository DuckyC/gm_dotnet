using GSharp.Native.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using Libraria.Native;

namespace GSharp.Native.StringTable
{
    public class StringTable
    {
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
            var table = GetTableInternal(index);
            return new StringTableBare(table);
        }

        public static StringTable FindTable(string name)
        {
            var table = FindTableInternal(name);
            return new StringTableBare(table);
        }

        public static StringTable<TClass> GetTable<TClass>(int index)
        {
            var table = GetTableInternal(index);
            return new StringTable<TClass>(table);
        }

        public static StringTable<TClass> FindTable<TClass>(string name)
        {
            var table = FindTableInternal(name);
            return new StringTable<TClass>(table);
        }

        private static INetworkStringTable GetTableInternal(int index)
        {
            if (container == null) return null;
            var stringTablePointer = container.GetTable(index);
            if (stringTablePointer == IntPtr.Zero) return null;
            var stringTable = JIT.ConvertInstance<INetworkStringTable>(stringTablePointer);
            return stringTable;
        }

        private static INetworkStringTable FindTableInternal(string name)
        {
            if (container == null) return null;
            var stringTablePointer = container.FindTable(name);
            if (stringTablePointer == IntPtr.Zero) return null;
            var stringTable = JIT.ConvertInstance<INetworkStringTable>(stringTablePointer);
            return stringTable;
        }


        protected INetworkStringTable table;

        internal StringTable(INetworkStringTable table)
        {
            this.table = table;
        }

        public virtual string this[int index]
        {
            get
            {
                return GetString(index);
            }
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

        public void SetStringChangedCallback(IntPtr obj, IntPtr changeFunc)
        {
            table.SetStringChangedCallback(obj, changeFunc);
        }
        #endregion
    }

    public class StringTableBare : StringTable, IEnumerable<string>
    {
        public StringTableBare(INetworkStringTable table) : base(table)
        {
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new StringTableEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new StringTableEnumerator(this);
        }
    }

    public struct Entry<TClass>
    {
        public string String;
        public TClass UserData;
        public Entry(string str, TClass udata)
        {
            String = str;
            UserData = udata;
        }
    }

    public class StringTable<TClass> : StringTable, IEnumerable<Entry<TClass>>
    {
        public StringUserDataTable<TClass> UserData { get; protected set; }

        public StringTable(INetworkStringTable table) : base(table)
        {
            UserData = new StringUserDataTable<TClass>(table);
        }

        public IEnumerator<Entry<TClass>> GetEnumerator()
        {
            return new StringTableUserDataEnumerator<TClass>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<Entry<TClass>>).GetEnumerator();
        }

    }

}
