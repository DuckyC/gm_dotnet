namespace dotnet
{
	partial class LuaViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("_G");
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.LuaStateViewer = new System.Windows.Forms.TreeView();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.LuaStateViewer);
			this.splitContainer1.Size = new System.Drawing.Size(770, 497);
			this.splitContainer1.SplitterDistance = 256;
			this.splitContainer1.TabIndex = 0;
			// 
			// LuaStateViewer
			// 
			this.LuaStateViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LuaStateViewer.Location = new System.Drawing.Point(0, 0);
			this.LuaStateViewer.Name = "LuaStateViewer";
			treeNode1.Name = "_G";
			treeNode1.Text = "_G";
			this.LuaStateViewer.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.LuaStateViewer.Size = new System.Drawing.Size(256, 497);
			this.LuaStateViewer.TabIndex = 0;
			// 
			// LuaViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(770, 497);
			this.Controls.Add(this.splitContainer1);
			this.Name = "LuaViewer";
			this.Text = "LuaViewer";
			this.Load += new System.EventHandler(this.LuaViewer_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView LuaStateViewer;
	}
}