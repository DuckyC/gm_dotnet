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
            private IGameConsole console;
            public IGameConsoleTextWriter(IGameConsole console)
            {
                this.console = console;
            }

            public override void Write(char value)
            {
                console.Printf("%s", value.ToString());
            }

            public override Encoding Encoding
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static void RerouteConsole()
        {
            var console = NativeInterface.Load<IGameConsole>("gameui.dll");
            console.Activate();//.Printf("%s", "C# Rerouted Console");
            //Console.SetOut(new IGameConsoleTextWriter(console));
        }
    }

    
}
