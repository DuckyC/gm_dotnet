using GSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GSharp.Native.Classes
{
    [InterfaceVersion("GameConsole004")]
    public interface IGameConsole
    {
        // activates the console, makes it visible and brings it to the foreground
        [VTableSlot(1)]
        void Activate();

        [VTableSlot(2)]
        void Initialize();

        // hides the console
        [VTableSlot(3)]
        void Hide();

        // clears the console
        [VTableSlot(4)]
        void Clear();

        // return true if the console has focus
        [VTableSlot(5)]
        bool IsConsoleVisible();

        // prints a message to the console
        [VTableSlot(6)]
        void Printf(string format, string first);

        // printes a debug message to the console
        [VTableSlot(7)]
        void DPrintf(string format, string first);

        // printes a debug message to the console
        //[VTableSlot(8)]
        //void ColorPrintf(Color& clr, string format, ...);

        //[VTableSlot(9)]
        //void SetParent(VPANEL parent);
    }
}
