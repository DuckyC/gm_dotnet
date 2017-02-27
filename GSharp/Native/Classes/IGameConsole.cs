using GSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.Classes
{
    public interface IBaseInterface
    {
        void dtorIBaseInterfaec();
    }
    [ModuleName("gameui")]
    [InterfaceVersion("GameConsole004")]
    public interface IGameConsole : IBaseInterface
    {
        // activates the console, makes it visible and brings it to the foreground
        void Activate();
        void Initialize();
        // hides the console
        void Hide();
        // clears the console
        void Clear();
        // return true if the console has focus
        bool IsConsoleVisible();

		//void SetParent(VPANEL parent);

		// prints a message to the console, existed in GameConsole003 but doesn't anymore?
		//void Printf(string format, string first);

		// printes a debug message to the console, existed in GameConsole003 but doesn't anymore?
		//void DPrintf(string format, string first);
    
		// printes a debug message to the console, existed in GameConsole003 but doesn't anymore?
		//void ColorPrintf(Color& clr, string format, ...);


	}
}
