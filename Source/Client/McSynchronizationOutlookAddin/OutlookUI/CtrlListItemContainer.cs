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
	public partial class CtrlListItemContainer : CustomListBox, ISettingView
	{
		public delegate void ListItemEventHandler(CtrlExtendedListItemBase item);

		#region Events
		public event ListItemEventHandler OnItemSelect;

		private void RaiseSelectedItem(CtrlExtendedListItemBase item)
		{
			ListItemEventHandler tmp = OnItemSelect;
			if (tmp != null)
			{
				tmp(item);
			}
		}
		#endregion

		public CtrlListItemContainer()
		{
			InitializeComponent();
			base.EnableSmothScrolling = false;
		}

		protected override void FireListItemClicked(Control listItem)
		{
			base.FireListItemClicked(listItem);
			CtrlExtendedListItemBase item = listItem as CtrlExtendedListItemBase;
			if (item != null)
			{
				RaiseSelectedItem(item);
			}
		}

		public CtrlExtendedListItemBase AddItem(CtrlExtendedListItemBase item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			item.HeightChangedEvent -= Item_OnHeightChanged;
			item.ItemSelectedEvent -= Item_OnItemSelectedChanged;
			item.HeightChangedEvent += Item_OnHeightChanged;
			item.ItemSelectedEvent += Item_OnItemSelectedChanged;
			HookEvents(item);
			//Call add item in CustomListBox imp
			base.AddItem(item);
			CtrlExtendedListItemBase prevSelectedItem = SelectedItem;
			SelectedItem = item;

			
			return item;
		}

		public void RemoveItem(CtrlExtendedListItemBase item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			if (item.IsSelected)
			{
				//Selected item and collapse deleted if is selected
				foreach (Control ctrl in Items)
				{
					if (ctrl != item)
					{
						SelectedItem = ctrl as CtrlExtendedListItemBase;
						break;
					}
				}
			}
			base.RemoveItem(item);
		}

		#region Handlers
		private void Item_OnItemSelectedChanged(CtrlExtendedListItemBase item, bool selected)
		{
					
		}
		private void Item_OnHeightChanged(int deltaSize)
		{

		}


		#endregion

		public CtrlExtendedListItemBase SelectedItem
		{
			get
			{
				CtrlExtendedListItemBase retVal = null;
				if (base.SelectedItems.Length != 0)
				{
					//Get first item
					retVal = (CtrlExtendedListItemBase)SelectedItems[0];
				}

				return retVal;
			}

			set
			{
				if (value != null)
				{
					SelectedItems = new Control[] { value };
					RaiseSelectedItem(value);
				}
			}
		}

		#region ISettingView Members

		public void SetSetting(object setting)
		{
			foreach (Control ctrl in Items)
			{
				ISettingView settingView = ctrl as ISettingView;
				if (settingView != null)
				{
					settingView.SetSetting(setting);
				}
			}
		}

		public void HarvestSetting(ref object setting)
		{
			foreach (Control ctrl in Items)
			{
				ISettingView settingView = ctrl as ISettingView;
				if (settingView != null)
				{
					settingView.HarvestSetting(ref setting);
				}
			}
		}

		#endregion

		private void CtrlListItem_MouseMove(object sender, MouseEventArgs e)
		{
			foreach (Control ctrl in base.Items)
			{
				Control tmpCtrl = (Control)sender;
				CtrlExtendedListItemBase listItem = null;
				do
				{
					listItem = tmpCtrl as CtrlExtendedListItemBase;
					if (listItem == null)
					{
						tmpCtrl = tmpCtrl.Parent;
						if (tmpCtrl == null)
							throw new NullReferenceException("CtrlExtendedListItemBase control not found");
					}
				}
				while (listItem == null);


				((CtrlExtendedListItemBase)ctrl).IsHovering = (listItem != null && listItem == ctrl);
				
			}
		}

		private void HookEvents(Control ctrl)
		{
			ctrl.MouseMove -= this.CtrlListItem_MouseMove;
			ctrl.MouseMove += this.CtrlListItem_MouseMove;
			foreach (Control child in ctrl.Controls)
			{
				HookEvents(child);
			}
		}
	}
}
