using GSharp.Native;
using GSharp.Native.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GSharp
{
    public static class ClientConsole
    {
        private class IGameConsoleTextWriter : TextWriter
        {
            public IGameConsole GameConsole;
            public Color Color;
            public override Encoding Encoding { get { throw new NotImplementedException(); } }

            public IGameConsoleTextWriter(IGameConsole console)
            {
                GameConsole = console;
            }

            public override void Write(char value)
            {
                //console.Printf("%s", value.ToString());
                Tier0.ConColorMsg(0, ref Color, value.ToString());
            }

            public override void Write(string value)
            {
                Tier0.ConColorMsg(0, ref Color, value);
            }

            public override void WriteLine(string value)
            {
                Tier0.ConColorMsg(0, ref Color, value + "\n");
            }
        }

        static IGameConsoleTextWriter GameConsoleWriter;

        public static IGameConsole RerouteConsole()
        {
            if (GameConsoleWriter == null)
            {
                IGameConsole GameConsole = NativeInterface.Load<IGameConsole>("gameui.dll");
                GameConsole.Activate();
                Console.SetOut(GameConsoleWriter = new IGameConsoleTextWriter(GameConsole));
            }

            return GameConsoleWriter.GameConsole;
        }

        public static Color Color
        {
            get
            {
                if (GameConsoleWriter == null)
                    return new Color();
                return GameConsoleWriter.Color;
            }

            set
            {
                if (GameConsoleWriter != null)
                    GameConsoleWriter.Color = value;
            }
        }
    }
}