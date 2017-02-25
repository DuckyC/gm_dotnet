using GSharp.Native.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace dotnet
{
	public partial class LuaViewer : Form
	{
		public static void Spawn(lua_state L)
		{
			Thread LVThread = new Thread(() =>
			{
				LuaViewer LV = new LuaViewer();
				LV.L = L;
				Application.Run(LV);
			});
			LVThread.IsBackground = true; LVThread.Start();
		}

		public lua_state L;

		public LuaViewer()
		{
			InitializeComponent();
		}

		private void LuaViewer_Load(object sender, EventArgs e)
		{

		}
	}
}
