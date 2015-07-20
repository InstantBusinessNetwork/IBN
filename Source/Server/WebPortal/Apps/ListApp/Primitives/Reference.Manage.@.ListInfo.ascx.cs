using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ListApp.Primitives
{
	public partial class Reference_Manage_All_ListInfo : System.Web.UI.UserControl, IManageControl
	{
		private readonly string businessObjectsBlock = "[BusinessObjects]";
		private readonly string listsBlock = "[Lists]";

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			return null;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.ReferenceMetaClassName, ClassList.SelectedValue);

				if (chkUseObjectRoles.Visible && chkUseObjectRoles.Checked && BusinessObjectServiceManager.IsServiceInstalled(ClassList.SelectedValue, SecurityService.ServiceName))
					Attr.Add(McDataTypeAttribute.ReferenceUseSecurity, "");

				return Attr;
			}
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			LoadData(mc.Name);
			if (mc != null && BusinessObjectServiceManager.IsServiceInstalled(mc, SecurityService.ServiceName))
				chkUseObjectRoles.Visible = true;
			else
				chkUseObjectRoles.Visible = false;
		}

		public void BindData(MetaField mf)
		{
			string primaryClassName = mf.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
			ClassList.Items.Add(new ListItem(CHelper.GetResFileString(Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(primaryClassName).FriendlyName)));
			ClassList.Enabled = false;

			chkUseObjectRoles.Enabled = false;
			chkUseObjectRoles.Checked = Mediachase.Ibn.Data.Services.Security.AreObjectRolesAddedFromRefernce(mf);

			if (!BusinessObjectServiceManager.IsServiceInstalled(mf.Owner, SecurityService.ServiceName))
				chkUseObjectRoles.Visible = false;
		}
		#endregion

		#region LoadData
		private void LoadData(string className)
		{
			if (ClassList.Items.Count > 0)
				ClassList.Items.Clear();

			FillObjectList(ClassList);
		}

		private void FillObjectList(DropDownList lst)
		{
			Dictionary<string, string> items;
			List<KeyValuePair<string, string>> sortedItems;

			// Meta Classes
			items = new Dictionary<string, string>();
			foreach (MetaClass metaClass in DataContext.Current.MetaModel.MetaClasses)
			{
				if (!metaClass.IsBridge
					&& !metaClass.IsCard
					&& !String.IsNullOrEmpty(metaClass.TitleFieldName)
					&& CHelper.CheckMetaClassIsPublic(metaClass)
					&& !ListManager.MetaClassIsList(metaClass))
				{
					items.Add(metaClass.Name, CHelper.GetResFileString(metaClass.FriendlyName));
				}
			}

			// [BusinessObjects] Block
			if (items.Count > 0)
			{
				ListItem li = new ListItem(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BusinessObjectsBlock").ToString(), businessObjectsBlock);
				lst.Items.Add(li);
			}

			// SortByValue
			sortedItems = new List<KeyValuePair<string, string>>(items);
			sortedItems.Sort(
				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
			);

			// add metaclasses
			foreach (KeyValuePair<string, string> kvp in sortedItems)
			{
				string text = String.Format(CultureInfo.InvariantCulture, "   {0}", kvp.Value);
				lst.Items.Add(new ListItem(text, kvp.Key));
			}

			// Lists
			items = new Dictionary<string, string>();
			foreach (MetaClass metaClass in DataContext.Current.MetaModel.MetaClasses)
			{
				string name = metaClass.Name;
				if (!metaClass.IsBridge
					&& !metaClass.IsCard &&
					!String.IsNullOrEmpty(metaClass.TitleFieldName)
					&& ListManager.MetaClassIsList(name))
				{
					// Check Security
					int listId = ListManager.GetListIdByClassName(name);
					if (Mediachase.IBN.Business.ListInfoBus.CanRead(listId))
					{
						items.Add(name, CHelper.GetResFileString(metaClass.FriendlyName));
					}
				}
			}

			// [Lists] Block
			if (items.Count > 0)
			{
				ListItem li = new ListItem(GetGlobalResourceObject("IbnFramework.ListInfo", "ListsBlock").ToString(), listsBlock);
				lst.Items.Add(li);
			}

			// SortByValue
			sortedItems = new List<KeyValuePair<string, string>>(items);
			sortedItems.Sort(
				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
			);

			// add lists
			foreach (KeyValuePair<string, string> kvp in sortedItems)
			{
				string text = String.Format(CultureInfo.InvariantCulture, "   {0}", kvp.Value);
				lst.Items.Add(new ListItem(text, kvp.Key));
			}

			lst.SelectedIndex = 1;
		}
		#endregion

		#region ClassList_SelectedIndexChanged
		protected void ClassList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ClassList.SelectedValue == businessObjectsBlock || ClassList.SelectedValue == listsBlock)
			{
				ClassList.SelectedIndex = ClassList.SelectedIndex + 1;
			}
		}
		#endregion
	}
}