using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlSyncConnection : CtrlExtendedListItemBase, ISettingView
	{
		private CtrlHeaderListItem _header;
	
		private Font _drawFont = new Font("Visitor TT2 BRK", 9);
		private Font _drawHighLightFont = new Font("Visitor TT2 BRK", 10, FontStyle.Bold);

		private Color _clrMenuPanelStart = Color.FromArgb(239, 239, 239);
		private Color _clrMenuPanelEnd = Color.FromArgb(202, 202, 202);

		public CtrlSyncConnection(ImageList imgList)
		{
			InitializeComponent();
			_header = new CtrlHeaderListItem(imgList);
			_header.Dock = DockStyle.Fill;
			this.pnlTop.Controls.Add(_header);

			this.pnlBottom.Paint += new PaintEventHandler(OnPaintExtendedPanel);
		}

		protected void OnPaintExtendedPanel(object sender, PaintEventArgs e)
		{
			VistaMenuControl.DrawGradientBckground(e.Graphics, this.pnlBottom.ClientRectangle, _clrMenuPanelStart, _clrMenuPanelEnd);
			VistaMenuControl.DrawInnerBorder(e.Graphics, this.pnlBottom.ClientRectangle, _header.BorderColorInner);
			VistaMenuControl.DrawOuterBorder(e.Graphics, this.pnlBottom.ClientRectangle, _header.BorderColorOuter);

		}

		public CtrlHeaderListItem Header
		{
			get { return _header; }
		}
	
		#region ISettingView Members

		public void SetSetting(object setting)
		{
			syncAppSetting acountSetting = setting as syncAppSetting;
			if (acountSetting != null)
			{
				this.tbServer.Text = acountSetting.ibnPortalUrl;
				this.tbUser.Text = acountSetting.ibnPortalLogin;
				this.tbPass.Text = acountSetting.ibnPortalPassword;
			}
		}

		public void HarvestSetting(ref object setting)
		{
			syncAppSetting acountSetting = setting as syncAppSetting;
			if (acountSetting != null)
			{
				acountSetting.ibnPortalUrl = this.tbServer.Text;
				acountSetting.ibnPortalLogin = this.tbUser.Text;
				acountSetting.ibnPortalPassword = this.tbPass.Text;
			}
		}

		#endregion

		private void pnlBottom_Paint(object sender, PaintEventArgs e)
		{

		}
	
	}
}
