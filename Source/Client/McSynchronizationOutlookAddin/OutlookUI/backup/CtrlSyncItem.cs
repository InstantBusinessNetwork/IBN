using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlSyncItem : CtrlExtendedListItemBase
	{
		private bool _checkinout = false;
		private bool _enabled = true;
		private bool IsHovering { get; set; }
		private int _deltaSize = 0;
		private int _opacity = 0;
		private static Font[] _fontStyles = new Font[] { new Font(FontFamily.GenericSansSerif, 12.0F, FontStyle.Bold),
														new Font(FontFamily.GenericSansSerif, 12.0F, FontStyle.Regular)};
		public Outlook.OlItemType SyncItemType { get; private set; }

		protected Color _gradientDark = Color.FromArgb(178, 193, 140);
		protected Color _gradientLight = Color.FromArgb(234, 240, 207);
		protected Color _gradientHoverDark = Color.FromArgb(247, 192, 91);
		protected Color _gradientHoverLight = Color.FromArgb(255, 255, 220);
		protected Color _gradientSelectedDark = Color.FromArgb(239, 150, 21);
		protected Color _gradientSelectedLight = Color.FromArgb(251, 230, 148);


		public string ItemCaption { get; set; }
		private Font _itemTextFont;


		public CtrlSyncItem(Outlook.OlItemType itemType)
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.UserPaint |
						  ControlStyles.OptimizedDoubleBuffer , true);

			
			InitializeComponent();

			this.SyncItemType = itemType;

			this.SuspendLayout();
			_deltaSize = this.panel2.Height;
			this.panel2.Visible = false;
			this.Height = this.panel1.Height;
			this.ResumeLayout(false);

			HookAllEvents(this.panel1, false);
			HookAllEvents(this.panel2, false);
		}


		#region Properties
		public bool ItemEnabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}
		private bool CheckInOut
		{
			set 
			{
				_checkinout = value;
				_opacity = 0;
				base.animationTimer.Enabled = true;
			}
			get 
			{
				return _checkinout;
			}
		}
		#endregion

		#region Color properties
		public Color GradientNormalDark
		{
			get { return _gradientDark; }
			set { _gradientDark = value; }
		}
		public Color GradientNormalLight
		{
			get { return _gradientLight; }
			set { _gradientLight = value; }
		}
		public Color GradientHoverDark
		{
			get { return _gradientHoverDark; }
			set { _gradientHoverDark = value; }
		}
		public Color GradientHoverLight
		{
			get { return _gradientHoverLight; }
			set { _gradientHoverLight = value; }
		}
		public Color GradientSelectedDark
		{
			get { return _gradientSelectedDark; }
			set { _gradientSelectedDark = value; }
		}
		public Color GradientSelectedLight
		{
			get { return _gradientSelectedLight; }
			set { _gradientSelectedLight = value; }
		}
		#endregion



		#region Label properties
		public string SyncStatsText
		{
			get { return this.lblSyncStats.Text; }
			set { this.lblSyncStats.Text = value; }
		}
		#endregion
		#region Image properties
		public AnimatedFormImage ImgSyncItemLogo
		{
			get
			{
				AnimatedFormImage retVal = GetRegisteredImg(this.pbSyncItemLogo);
				if (retVal == null)
				{
					retVal = RegisterImage(pbSyncItemLogo);
				}
				return retVal;
			}
			private set
			{
			}
		}

		public AnimatedFormImage ImgSyncStatus
		{
			get
			{
				AnimatedFormImage retVal = GetRegisteredImg(this.pbSyncStatus);
				if (retVal == null)
				{
					retVal = RegisterImage(this.pbSyncStatus);
				}
				return retVal;
			}
			private set
			{
			}
		}
		#endregion

		#region Event Handlers

		#endregion

		#region Overrides
		protected override void OnHeightChange(int deltaH)
		{
			base.OnHeightChange(deltaH);
		}

		protected override void OnItemSelected(CtrlExtendedListItemBase item, bool selected)
		{
			base.OnItemSelected(item, selected);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!DesignMode)
			{
				base.animationTimer.Enabled = true;
			}
		}

		protected override void animationTimer_Tick(object sender, EventArgs e)
		{
			base.animationTimer_Tick(sender, e);

			//if (this.CheckInOut)
			//{
				_opacity += 40;//(int)((base.animationTimer.Interval / 1000.0) * 255.0);
				_opacity = Math.Min(255, _opacity);
				base.animationTimer.Enabled = _opacity < 255;
			//}
			//else
			//{
			//    _opacity -= (int)((base.animationTimer.Interval / 1000.0) * 255.0);
			//    _opacity = Math.Max(0, _opacity);
			//    base.animationTimer.Enabled = _opacity > 0;
			//}
			

			System.Diagnostics.Trace.WriteLine(this.Name+" interval = "+(base.animationTimer.Interval / 1000.0)+ " _opacity = " + _opacity);

			this.panel1.Invalidate();
		}

		public override void SelectedChanged(bool isSelected)
		{
			base.SelectedChanged(isSelected);
			if (!isSelected)
			{
				base.animationTimer.Enabled = true;
			}
		}
		#endregion

		#region Methods
		

		public int ThreadedUpdateSyncProgress(string status, int currentWork, int totalWork)
		{
			int percent = (int)(((double)(this.progressBarSync.Value - progressBarSync.Minimum) /
						   (double)(this.progressBarSync.Maximum - this.progressBarSync.Minimum)) * 100);
			using (Graphics gr = this.progressBarSync.CreateGraphics())
			{
				gr.DrawString(percent.ToString() + "%", SystemFonts.DefaultFont, Brushes.Black,
							  new PointF(this.progressBarSync.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
							  SystemFonts.DefaultFont).Width / 2.0F),
							  this.progressBarSync.Height / 2 - (gr.MeasureString(percent.ToString() + "%", SystemFonts.DefaultFont).Height / 2.0F)));
			}

			return 0;
		}
		#endregion

		private void HookAllEvents(Control ctrl, bool processAllChild)
		{
			ctrl.Paint		-= CtrlSyncItem_Paint;
			ctrl.Paint		+= CtrlSyncItem_Paint;
			ctrl.MouseEnter -= CtrlSyncItem_MouseEnter;
			ctrl.MouseEnter += CtrlSyncItem_MouseEnter;
			ctrl.MouseLeave -= CtrlSyncItem_MouseLeave;
			ctrl.MouseLeave += CtrlSyncItem_MouseLeave;

			if (processAllChild)
			{
				foreach (Control child in ctrl.Controls)
				{
					HookAllEvents(child, processAllChild);
				}
			}
		}

		private void CtrlSyncItem_Paint(object sender, PaintEventArgs e)
		{
		
			Brush br;
			Rectangle rect = this.panel1.ClientRectangle;

			Color color1 = GradientNormalLight;
			Color color2 = GradientNormalDark;

			if (this.ItemEnabled)
			{
				if (IsSelected)
				{
					if (IsHovering)
					{
						color1 = Color.FromArgb(_opacity, GradientSelectedDark);
						color2 = Color.FromArgb(_opacity, GradientSelectedLight); 
					}
					else
					{
						color1 = Color.FromArgb(_opacity, GradientSelectedLight);
						color2 = Color.FromArgb(_opacity, GradientSelectedDark);
					}
				}
				else
				{
					if (IsHovering)
					{
						color1 = Color.FromArgb(_opacity, GradientHoverLight);
						color2 = Color.FromArgb(_opacity, GradientHoverDark);
					}
				}
			}
			using (br = new LinearGradientBrush(rect, color1, color2, 90f))
			{
				e.Graphics.FillRectangle(br, rect);
			}

			if (!string.IsNullOrEmpty(ItemCaption))
			{
				e.Graphics.DrawString(this.ItemCaption, _fontStyles[IsSelected ? 0 : 1], Brushes.Black, 36, this.Height / 2);
			}
			System.Diagnostics.Trace.WriteLine(this.Name + " Paint with opacity " + _opacity +" "+color1.ToString());

			//if (image != null)
			//{
			//    graphics.DrawImage(image, 36 / 2 - image.Width / 2, y + this.Height / 2 - image.Height / 2, image.Width, image.Height);
			//}
		}

		private void CtrlSyncItem_MouseEnter(object sender, EventArgs e)
		{
			System.Diagnostics.Trace.WriteLine(this.Name + " MouseEnter");

			if (!IsSelected)
			{
				this.CheckInOut = true;
			}

			this.IsHovering = true;
			this.panel1.Invalidate();
			
		}

		private void CtrlSyncItem_MouseLeave(object sender, EventArgs e)
		{
			System.Diagnostics.Trace.WriteLine(this.Name + " MouseLeave");

			if (!IsSelected)
			{
				this.CheckInOut = false;
			}
			
			this.IsHovering = false;
			this.panel1.Invalidate();
		}
		

	}
}
