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
using GSharp.Native.JIT;
using GSharp.GLuaNET;

namespace dotnet
{
	public partial class LuaViewer : Form
	{
		public static void Spawn(lua_state L)
		{
			Thread LVThread = new Thread(() =>
			{
				LuaViewer LV = new LuaViewer();
				LV.L = new GLua(L);
				Application.Run(LV);
			});
			LVThread.IsBackground = true; LVThread.Start();
		}

		public GLua L;

		public LuaViewer()
		{
			InitializeComponent();
		}

		private void LuaViewer_Load(object S, EventArgs E)
		{
			LuaStateViewer.NodeMouseDoubleClick += OnNodeDoubleClick;
		}
		
		void GetFullPath(List<string> FullPath, TreeNode N)
		{
			if (N.Parent != null)
				GetFullPath(FullPath, N.Parent);
			FullPath.Add(N.Name);
		}

		private void OnNodeDoubleClick(object S, TreeNodeMouseClickEventArgs E)
		{
			LuaStateViewer.BeginUpdate();
			E.Node.Nodes.Clear();

			List<string> FullPath = new List<string>();
			GetFullPath(FullPath, E.Node);

			L.GetTable(GLua.LUA_GLOBALSINDEX);
			for (int i = 0; i < FullPath.Count; i++)
			{
				L.GetField(-1, FullPath[i]);
				L.Remove(-2);
			}

			if (L.GetLuaType() ==  LuaType.Table)
			{
				L.PushNil();
				while (L.Next(-2) != 0)
				{
					E.Node.Nodes.Add(L.LuaBase.GetString(-2));
					L.Pop();
				}
			}

			LuaStateViewer.EndUpdate();
		}
	}
}
