using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization;

namespace OutlookAddin.OutlookUI
{
	public partial class FormSyncConflictResolution : Form
	{
		protected CtrlHeaderListItem _header;
		private ItemConflictingEventArgs _conflict;

		private ConflictResolutionAction DefaultAction { get; set; }

		protected Font _drawFont = new Font("Trebuchet MS", 9f, FontStyle.Bold);

		public FormSyncConflictResolution()
		{
			InitializeComponent();

			DefaultAction = ConflictResolutionAction.SkipChange;

			CreateRadioButtons();

			_header = new CtrlHeaderListItem(this.imageList1);
			_header.FontCaption = _drawFont;
			_header.Dock = DockStyle.Fill;
			this.pnlTop.Controls.Add(_header);

			this.pnlBottom.Paint += new PaintEventHandler(OnPaintBottomPanel);
		}

		public ItemConflictingEventArgs CurrentConflicting
		{
			get
			{
				return _conflict;
			}
			set
			{
				if (value != null)
				{
					string strSrcData = value.SourceChangeData != null ? value.SourceChangeData.ToString() : "unknow";
					string strDstData = value.DestinationChangeData != null ? value.DestinationChangeData.ToString() : "unknow";
					this._header.Caption = string.Format(Resources.FormConflictResolution_Caption_Pattern, strSrcData, strDstData);
				}
				_conflict = value;
			}

		}
		public CtrlHeaderListItem Header
		{
			get { return _header; }
		}

		public ConflictResolutionAction SelectedAction
		{
			get
			{
				return (ConflictResolutionAction)RadioButtons.First(x => x.Checked).Tag;
			}
			set
			{
				CheckItem(RadioButtons.First(x => (ConflictResolutionAction)x.Tag == value));
			}
		}

		private void CreateRadioButtons()
		{
			this.groupBox1.Controls.Clear();
			int y = groupBox1.Padding.Top;
			int x = groupBox1.Padding.Left;
			foreach (Object enumValue in Enum.GetValues(typeof(ConflictResolutionAction)))
			{
				ConflictResolutionAction action = (ConflictResolutionAction)enumValue;
				RadioButton rBtn = new RadioButton();
				rBtn.Click += new EventHandler(radioBtn_OnClick);
				rBtn.Tag = action;
				rBtn.AutoCheck = false;
				rBtn.Checked = action == DefaultAction;
				rBtn.Text = action.ToString();
				groupBox1.Controls.Add(rBtn);

				rBtn.Location = new Point(x, y);
				y += rBtn.Height + 2;
			}
		}
		protected void OnPaintBottomPanel(object sender, PaintEventArgs e)
		{
			VistaMenuControl.DrawInnerBorder(e.Graphics, this.pnlBottom.ClientRectangle, _header.BorderColorInner);
			VistaMenuControl.DrawOuterBorder(e.Graphics, this.pnlBottom.ClientRectangle, _header.BorderColorOuter);

		}

		private void CheckItem(RadioButton radioBtn)
		{
			if (!radioBtn.Checked)
			{
				radioBtn.Checked = true;
				foreach (RadioButton otherRadioBtn in RadioButtons.Where(x => x != radioBtn))
				{
					otherRadioBtn.Checked = false;
				}
			}
		}

		private IEnumerable<RadioButton> RadioButtons
		{
			get
			{
				foreach (Control ctrl in groupBox1.Controls)
				{
					RadioButton radioBtn = ctrl as RadioButton;
					if (radioBtn != null)
					{
						yield return radioBtn;
					}
				}
			}
		}

		#region Event Handlers
		private void radioBtn_OnClick(object sender, EventArgs e)
		{
			RadioButton rBtn = sender as RadioButton;
			if (rBtn != null)
			{
				CheckItem(rBtn);
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}
		#endregion

		#region Overrides
		protected override void OnClosing(CancelEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}
		#endregion
	}
}
