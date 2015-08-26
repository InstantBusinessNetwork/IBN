using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mediachase.Sync.Core.Common;

namespace OutlookAddin.OutlookUI
{
	public partial class CtrlExtendedListItemBase : UserControl, IExtendedListItem
	{
		internal delegate void SizeEventHandler(int deltaSize);
		internal delegate void ExtendedListItemHandler(CtrlExtendedListItemBase item, bool selected);

		private ImageList _imgList { get; set; }
		protected List<CycleCollection<int>> _animatedItems = new List<CycleCollection<int>>();

		#region Events
		internal event SizeEventHandler HeightChangedEvent;
		internal event ExtendedListItemHandler ItemSelectedEvent;

		protected virtual void OnItemSelected(CtrlExtendedListItemBase item, bool selected)
		{
			ExtendedListItemHandler tmp = ItemSelectedEvent;
			if (tmp != null)
			{
				tmp(item, selected);
			}

		}
		protected virtual void OnHeightChange(int deltaH)
		{
			SizeEventHandler tmp = HeightChangedEvent;
			if (tmp != null)
			{
				tmp(deltaH);
			}
		}
		#endregion

		public CtrlExtendedListItemBase()
		{
			InitializeComponent();
		}

		public bool IsSelected { get; set; }
		public virtual bool IsHovering { get; set; }


		#region IExtendedListItem Members

		public virtual void SelectedChanged(bool isSelected)
		{
			OnItemSelected(this, isSelected);
			IsSelected = isSelected;
		}

		public virtual void PositionChanged(int index)
		{
			//Nothig todo
		}

	

		#endregion

	
	}
}
