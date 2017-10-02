using GSharp.GLuaNET;
using GSharp.LuaLibrary.Libraries;
using System;

namespace GSharp
{
    //public interface IString
    //{
    //    string GetChar(string str, double index);
    //}

    //public class stringImpl : IString
    //{
    //    public GLua GLua;

    //    public string GetChar(string str, double index)
    //    {
    //        GLua.GetGlobal("string");
    //        Lua.lua_getfield(LuaState, -1, "GetChar");
    //        LuaAdvanced.Push(LuaState, str);
    //        LuaAdvanced.Push(LuaState, index);
    //        Lua.lua_pcall(LuaState, 2, 1, 0);
    //        return LuaAdvanced.Pop(LuaState, -1, typeof(string)) as string;
    //    }
    //}

    public class fileImpl : IFile
    {
        public GLua GLua;
        public fileImpl(GLua l)
        {
            GLua = l;
        }

        public void Append(string name, string content)
        {
            throw new NotImplementedException();
        }

        public void CreateDir(string name)
        {
            throw new NotImplementedException();
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public void Exists(string name, string path)
        {
            throw new NotImplementedException();
        }

        public IFileFindReturnType Find(string name, string path, string sorting = "nameasc")
        {
            GLua.LuaBase.GetField(GLua.LUA_GLOBALSINDEX, "file");
            GLua.LuaBase.GetField(-1, "Find");
            GLua.Push(name);
            GLua.Push(path);
            GLua.Push(sorting);
            GLua.PCall(3, 2, 0);
            return GLua.GetReturnType<IFileFindReturnType>();
        }

        public void IsDir(string name, string path)
        {
            throw new NotImplementedException();
        }

        public void Open(string fileName, string fileMode, string path)
        {
            throw new NotImplementedException();
        }

        public void Read(string fileName, string path = "DATA")
        {
            throw new NotImplementedException();
        }

        public void Size(string fileName, string path)
        {
            throw new NotImplementedException();
        }

        public void Time(string path, string gamePath)
        {
            throw new NotImplementedException();
        }

        public void Write(string fileName, string content)
        {
            throw new NotImplementedException();
        }
    }


    //public class UserData
    //{
    //    public IntPtr ObjectLocation { get; set; }
    //    public static string MetaTable { get; set; }
    //    public GLua GLua { get; set; }

    //    public void PushUserData()
    //    {
    //        //Lua.lua_newuserdata(LuaState, Marshal.SizeOf(*ObjectLocation.ToPointer()));
    //    }
    //}

    //public class entityImpl : UserData, IEntity
    //{
    //    static entityImpl()
    //    {
    //        MetaTable = "Entity";
    //    }

    //    public void Extinguish()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Ignite(int length, int radius)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class playerImpl : entityImpl, IPlayer
    //{
    //    static playerImpl()
    //    {
    //        MetaTable = "Player";
    //    }

    //    public void Say(string text, bool teamOnly = false)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public string SteamID()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
