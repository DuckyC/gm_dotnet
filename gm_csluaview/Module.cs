using GSharp;
using GSharp.Native;
using GSharp.Native.Classes;
using GSharp.Native.JIT;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using PATH = System.IO.Path;

namespace gm_csluaview
{
    public class LuaFile
    {
        public string Path { get; set; }
        public int CRC { get; set; }
        private string _Content;
        public string Content
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Content)) { _Content = GetContent(); }
                return _Content;
            }
        }

        private string GetContent()
        {
            try
            {
                var path = Extensions.AbsolutePath(PATH.Combine("garrysmod/cache/lua", CRC.ToString()+ ".lua"));
                var fs = new FileStream(path, FileMode.Open);
                fs.Seek(4, SeekOrigin.Begin); // skip crc

                var decoder = new SevenZip.Compression.LZMA.Decoder();

                byte[] Props = new byte[5];
                if (fs.Read(Props, 0, Props.Length) != Props.Length)
                    throw new InvalidOperationException("Compressed memory too short");
                decoder.SetDecoderProperties(Props);

                byte[] fileLengthBytes = new byte[8];
                fs.Read(fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                var outStream = new MemoryStream();
                decoder.Code(fs, outStream, fs.Length, fileLength, null);

                outStream.Position = 0;
                var reader = new StreamReader(outStream);
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                return "ERROR: " + e.ToString();
            }

        }
    }

    public unsafe class Module
    {
        [DllExport("gmod13_open", CallingConvention = CallingConvention.Cdecl)]
        public static int Open(lua_state L)
        {
            ClientConsole.RerouteConsole(); // route Console.WriteLine to ingame Console
            ClientConsole.Color = new Color(0, 150, 255);
            Console.WriteLine("Testing ingame console from C#!");

            var stringTable = StringTable.FindTable("client_lua_files");
            var luaFiles = new List<LuaFile>();
            for (int i = 0; i < stringTable.Count(); i++)
            {
                luaFiles.Insert(i, new LuaFile { Path = stringTable[i] });
            }
            for (int i = 0; i < stringTable.UserData.Count(); i++)
            {
                var crcptr = stringTable.UserData[i];
                luaFiles[i].CRC = *((int*)crcptr.ToPointer());
            }

            var luaShared = NativeInterface.Load<ILuaShared>("lua_shared.dll");
            var luaInterfacePointer = luaShared.GetLuaInterface(0);
            var luaInterface = JITEngine.GenerateClass<ILuaInterface>(luaInterfacePointer);
            luaInterface.RunStringEx("", "", "print[[HI THERE FROM RUNSTRING]]");

            return 0;
        }

        [DllExport("gmod13_close", CallingConvention = CallingConvention.Cdecl)]
        public static int Close(IntPtr L)
        {
            return 0;
        }
    }
}
