using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.GLuaNET
{
    public class LuaType
    {
        public static readonly LuaType None = new LuaType(-1, "None");
        public static readonly LuaType Nil = new LuaType(0, "Nil");
        public static readonly LuaType Boolean = new LuaType(1, "Boolean");
        public static readonly LuaType LightUserData = new LuaType(2, "LightUserData");
        public static readonly LuaType Number = new LuaType(3, "Number");
        public static readonly LuaType String = new LuaType(4, "String");
        public static readonly LuaType Table = new LuaType(5, "Table");
        public static readonly LuaType Function = new LuaType(6, "Function");
        public static readonly LuaType UserData = new LuaType(7, "UserData");
        public static readonly LuaType Thread = new LuaType(8, "Thread");

        readonly int type;
        readonly string name;
        private LuaType(int type, string name = "")
        {
            this.type = type;
            this.name = name;
        }

        public static implicit operator int(LuaType value)
        {
            return value.type;
        }

        public static implicit operator LuaType(int value)
        {
            return new LuaType(value);
        }
    }
}
