using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mediachase.Sync.Core.Common;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlHeaderListItem : UserControl
	{
		private ImageList _imgList;
		private Color _clrPanelStart = Color.FromArgb(239, 239, 239);
		private Color _clrPanelEnd = Color.FromArgb(202, 202, 202);

		private Color _clrOuterBorder = Color.FromArgb(29, 29, 29);
		private Color _clrInnerBorder = Color.FromArgb(158, 158, 158);

		private Color _clrSelectionStartColor = Color.FromArgb(152, 193, 233);
		private Color _clrSelectionEndColor = Color.FromArgb(134, 186, 237);
		private Color _clrSelectionStartColorStart = Color.FromArgb(104, 169, 234);
		private Color _clrSelectionEndColorEnd = Color.FromArgb(169, 232, 255);

		private Font _drawFont = new Font("Visitor TT2 BRK", 9);
		private Font _drawHighLightFont = new Font("Visitor TT2 BRK", 10, FontStyle.Bold);

		private Dictionary<eSettingItem_State, CycleCollection<int>> _stateImageMaping =
														new Dictionary<eSettingItem_State, CycleCollection<int>>();

		private StateMachine<eSettingItem_State> _stateSM;

		public CtrlHeaderListItem(ImageList imgList)
		{
			_imgList = imgList;

			InitializeComponent();
			
			this.pnlHeader.Paint += new PaintEventHandler(OnPaintHeaderPanel);

			State<eSettingItem_State> normalState = new State<eSettingItem_State>(eSettingItem_State.Normal, ChangeStateHandler);
			State<eSettingItem_State> hoverState = new State<eSettingItem_State>(eSettingItem_State.Hover, ChangeStateHandler);
			State<eSettingItem_State> disabledState = new State<eSettingItem_State>(eSettingItem_State.Disabled, ChangeStateHandler);
			State<eSettingItem_State> selectedState = new State<eSettingItem_State>(eSettingItem_State.Selected, ChangeStateHandler);

			var allStates = new State<eSettingItem_State>[] { normalState, hoverState, disabledState, selectedState };
			foreach (State<eSettingItem_State> state in allStates)
			{
				state.AvailTransitions.AddRange(allStates);
			}
			_stateSM = new StateMachine<eSettingItem_State>(selectedState);
			_stateSM.RegisteredStates.AddRange(allStates);
		}

		#region Properties
		public String Caption
		{
			get { return this.lblCaption.Text;	}
			set { this.lblCaption.Text = value; }
		}

		public Font FontCaption
		{
			get { return this.lblCaption.Font; }
			set { this.lblCaption.Font = value; }
		}

		public eSettingItem_State CurrentItemState
		{
			get
			{
				return _stateSM.CurrentState.stateName;
			}
			set
			{
				lock (this)
				{
					if (CurrentItemState != value)
					{
						_stateSM.SetState(value);
					}
				}
			}
		}

		public Color BgrColorPanelStart
		{
			get { return _clrPanelStart; }
			set { _clrPanelStart = value; }
		}
		public Color BgrColorPanelEnd
		{
			get { return _clrPanelEnd; }
			set { _clrPanelEnd = value; }
		}
		public Color BorderColorInner
		{
			get { return _clrInnerBorder; }
			set { _clrInnerBorder = value; }
		}
		public Color BorderColorOuter
		{
			get { return _clrOuterBorder; }
			set { _clrOuterBorder = value; }
		}
		public Color SelectionStartColor
		{
			get
			{
				return this._clrSelectionStartColor;
			}
			set
			{
				this._clrSelectionStartColor = value;
				this.Invalidate();
			}
		}
		public Color SelectionEndColor
		{
			get
			{
				return this._clrSelectionEndColor;
			}
			set
			{
				this._clrSelectionEndColor = value;
				Invalidate();
			}
		}
		public Color SelectionEndColorEnd
		{
			get
			{
				return this._clrSelectionEndColorEnd;
			}
			set
			{
				this._clrSelectionEndColorEnd = value;
				Invalidate();
			}
		}
		public Color SelectionStartColorStart
		{
			get
			{
				return this._clrSelectionStartColorStart;
			}
			set
			{
				this._clrSelectionStartColorStart = value;
				Invalidate();
			}
		}
		public Font NormalCaptionFont
		{ 
			get { return _drawFont; }
			set { _drawFont = value; }
		}

		public Font HighlightCaptionFont
		{
			get { return _drawHighLightFont; }
			set { _drawHighLightFont = value; }
		}

		#endregion

		public void RegisterStateImage(eSettingItem_State itemState, params int[] imgIndexes)
		{
			CycleCollection<int> imgIndexColl = CycleCollection<int>.CreateInstance(0, imgIndexes);
			_stateImageMaping[itemState] = imgIndexColl;
		}


		#region Private methods
		private void ChangeStateHandler(State<eSettingItem_State> prevState)
		{
			SetCurrentStateImage(CurrentItemState);
			FontCaption = CurrentItemState == eSettingItem_State.Selected
				|| CurrentItemState == eSettingItem_State.Hover ? _drawHighLightFont : _drawFont;
			Invalidate();
		}

		private void SetCurrentStateImage(eSettingItem_State state)
		{
			CycleCollection<int> activeStateImgColl;

			if (_stateImageMaping.TryGetValue(state, out activeStateImgColl))
			{
				IEnumerator<int> cycleIterator = activeStateImgColl.GetEnumerator();
				if (cycleIterator.MoveNext())
				{
					int index = cycleIterator.Current;
					if (index >= _imgList.Images.Count)
						throw new Exception("img index out of range");

					this.pbLogo.Image = _imgList.Images[index];
				}

			}
		}

		private void DrawHeaderPanel(Graphics gfx, Rectangle rc, Color clrStart, Color clrEnd)
		{
			rc.Inflate(-1, -1);
			using (GraphicsPath pathMenuPanel = VistaMenuControl.RoundRect((RectangleF)rc, 7, 7, 7, 7))
			{
				using (LinearGradientBrush lgb = new LinearGradientBrush(rc, clrStart, clrEnd, 90f))
				{
					gfx.SmoothingMode = SmoothingMode.HighQuality;
					gfx.CompositingQuality = CompositingQuality.HighQuality;

					gfx.FillPath(lgb, pathMenuPanel);

					if (CurrentItemState == eSettingItem_State.Selected || CurrentItemState == eSettingItem_State.Hover)
					{
						Rectangle rcItem = rc;
						rcItem.X = 5;
						rcItem.Y = 4;
						rcItem.Width = rc.Width - 2;
						rcItem.Height = this.pnlHeader.Height;

						Rectangle rcUpRect = rcItem;
						Rectangle rcDownRect = rcItem;

						rcUpRect.Height /= 2;
						rcDownRect.Height /= 2;
						rcDownRect.Y = rcUpRect.Height + 3; 

						VistaMenuControl.FillMenuItem(gfx, rcUpRect, rcDownRect, _clrSelectionStartColor, _clrSelectionEndColor,
													  _clrSelectionStartColorStart, _clrSelectionEndColorEnd, 7);
					}

				}
			}

		}

		protected virtual void OnPaintHeaderPanel(object sender, PaintEventArgs e)
		{

			DrawHeaderPanel(e.Graphics, this.ClientRectangle, _clrPanelStart, _clrPanelEnd);

			VistaMenuControl.DrawInnerBorder(e.Graphics, this.pnlHeader.ClientRectangle, _clrInnerBorder);
			VistaMenuControl.DrawOuterBorder(e.Graphics, this.pnlHeader.ClientRectangle, _clrOuterBorder);

		}

		#endregion

		#region Event Handler
		#endregion

		#region Overrides
		#endregion

	}
}
