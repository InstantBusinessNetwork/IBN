using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookAddin.OutlookItemAdaptors
{
	public class OutlookFolder : OutlookItem
	{
		private Outlook.MAPIFolder _oFolder;

		public OutlookFolder(OutlookListener outlookListener, Outlook.MAPIFolder oFolder) 
			: base(outlookListener)
		{
			_oFolder = oFolder;
		}

		/// <summary>
		/// Adds the item.
		/// </summary>
		/// <param name="oItemType">Type of the o item.</param>
		/// <returns></returns>
		public OutlookItem AddItem(Outlook.OlItemType oItemType)
		{
			return base._outlookListener.AddFolderItem(_oFolder, oItemType);
		}
		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <value>The items.</value>
		public IEnumerable<OutlookItem> Items
		{
			get
			{
				if (base._outlookListener == null)
					throw new NullReferenceException("_outlookListener");

				return _outlookListener.GetFolderItems(_oFolder);
			}
		}

		#region Overrides
		public override object InnerOutlookObject
		{
			get { return _oFolder; }
		}
		#endregion
	}
}
