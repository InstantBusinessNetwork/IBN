using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlCustomPanel : Panel
	{
		public CtrlCustomPanel()
		{
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer
					  | ControlStyles.UserPaint 
					  | ControlStyles.AllPaintingInWmPaint, true);

			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
		}
	}
}
