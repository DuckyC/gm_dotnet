using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GSharp.Native.Classes
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Color
	{
		public byte R, G, B, A;

		public Color(byte R, byte G, byte B, byte A)
		{
			this.R = R;
			this.G = G;
			this.B = B;
			this.A = A;
		}

		public Color(byte R, byte G, byte B) : this(R, G, B, 255)
		{
		}

		public static implicit operator Color(ConsoleColor ConColor)
		{
			switch (ConColor)
			{
				case ConsoleColor.Black:
					return new Color(0x00, 0x00, 0x0);
				case ConsoleColor.DarkBlue:
					return new Color(0x00, 0x00, 0x8B);
				case ConsoleColor.DarkGreen:
					return new Color(0x00, 0x64, 0x00);
				case ConsoleColor.DarkCyan:
					return new Color(0x00, 0x8B, 0x8B);
				case ConsoleColor.DarkRed:
					return new Color(0x8B, 0x00, 0x00);
				case ConsoleColor.DarkMagenta:
					return new Color(0x8B, 0x00, 0x8B);
				case ConsoleColor.DarkYellow:
					return new Color(0xD7, 0xC3, 0x2A);
				case ConsoleColor.Gray:
					return new Color(0x80, 0x80, 0x80);
				case ConsoleColor.DarkGray:
					return new Color(0xA9, 0xA9, 0xA9);
				case ConsoleColor.Blue:
					return new Color(0x00, 0x00, 0xFF);
				case ConsoleColor.Green:
					return new Color(0x00, 0xFF, 0x00);
				case ConsoleColor.Cyan:
					return new Color(0x00, 0xFF, 0xFF);
				case ConsoleColor.Red:
					return new Color(0xFF, 0x00, 0x00);
				case ConsoleColor.Magenta:
					return new Color(0xFF, 0x00, 0xFF);
				case ConsoleColor.Yellow:
					return new Color(0xFF, 0xFF, 0x00);
				case ConsoleColor.White:
					return new Color(0xFF, 0xFF, 0xFF);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
