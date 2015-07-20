using System;
using System.Collections;
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
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class Reference_Manage : System.Web.UI.UserControl, IManageControl
	{
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
			LoadData();
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
		private void LoadData()
		{
			if (ClassList.Items.Count > 0)
				ClassList.Items.Clear();

			SortedList list = new SortedList();
			foreach (MetaClass mc in DataContext.Current.MetaModel.MetaClasses)
			{
				if (!mc.IsBridge && !mc.IsCard && !String.IsNullOrEmpty(mc.TitleFieldName))
				{
					string name = mc.Name;
					string friendlyName = CHelper.GetResFileString(mc.FriendlyName);
					string text = name;

					if (name != friendlyName)
						text = String.Format(CultureInfo.InvariantCulture, "{0} ({1})", friendlyName, name);

					list.Add(text, name);
				}
			}

			ClassList.DataSource = list;
			ClassList.DataTextField = "key";
			ClassList.DataValueField = "value";
			ClassList.DataBind();
 		}
		#endregion
	}
}