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
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class MetaClassFields : MCDataBoundControl
	{
		protected readonly string classNameKey = "ClassName";
		protected readonly string editFieldLinkKey = "EditFieldLink";
		protected readonly string editFieldLinkDefaultValue = "~/Apps/MetaDataBase/Pages/Admin/MetaFieldEdit.aspx";
		protected readonly string deleteCommand = "Dlt";
		protected readonly string sortKey = "MetaClassView_Sort";

		private Mediachase.IBN.Business.UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		#region DataItem
		public override object DataItem
		{
			get
			{
				return base.DataItem;
			}
			set
			{
				if (value is MetaClass)
					mc = value as MetaClass;

				base.DataItem = value;
			}
		}
		#endregion

		#region MetaClass mc
		private MetaClass _mc = null;
		private MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState[classNameKey] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState[classNameKey].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState[classNameKey] = value.Name;
				_mc = value;
			}
		}
		#endregion

		#region EditFieldLink
		public string EditFieldLink
		{
			get
			{
				string retval = editFieldLinkDefaultValue;
				if (ViewState[editFieldLinkKey] != null)
				{
					retval = ViewState[editFieldLinkKey].ToString();
				}
				return retval;
			}
			set
			{
				ViewState[editFieldLinkKey] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			if (mc != null)
			{
				NewLink.Text = CHelper.GetIconText(
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewField").ToString(),
					ResolveClientUrl("~/images/IbnFramework/newitem.gif"));

				NewLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture, 
					"{0}?class={1}", 
					EditFieldLink,
					mc.Name);

				BindData();
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("TypeName", typeof(string));
			dt.Columns.Add("IsSystem", typeof(bool));
			dt.Columns.Add("FieldTypeImageUrl", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));
			dt.Columns.Add("IsTitle", typeof(bool));

			string titleField = mc.TitleFieldName;

			foreach (MetaField field in mc.Fields)
			{
				if (field.TypeName == "Reference")
				{
					// Ibn 4.7 Lists fix.
					if (!Mediachase.Ibn.Lists.ListManager.MetaClassIsList(mc))
						continue;
				}

				DataRow row = dt.NewRow();
				row["Name"] = field.Name;

				row["IsTitle"] = (field.Name == titleField);
				row["FriendlyName"] = CHelper.GetMetaFieldName(field);
				row["TypeName"] = CHelper.GetResFileString(field.GetOriginalMetaType().FriendlyName);
				row["IsSystem"] = field.Attributes.ContainsKey(MetaClassAttribute.IsSystem) || mc.TitleFieldName == field.Name;
				if(field.IsLink)
					row["EditLink"] = String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaLinkEdit.aspx?class={0}&field={1}", mc.Name, field.Name);
				else
					row["EditLink"] = String.Format("{0}?class={1}&field={2}", EditFieldLink, mc.Name, field.Name);
				string postfix = string.Empty;
				if ((bool)row["IsSystem"])
				{
					postfix = "_sys";
				}
				if (field.TypeName == "ReferencedField")
				{
					row["FieldTypeImageUrl"] = String.Format(CultureInfo.InvariantCulture,
						"~/images/IbnFramework/metainfo/referencedfield{0}.gif",
						postfix);

					string refClassName = field.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();

					row["TypeName"] = String.Format(CultureInfo.InvariantCulture,
						"{0} ({1})",
						CHelper.GetResFileString(field.GetMetaType().FriendlyName),
						CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(refClassName).FriendlyName));
				}
				else if (field.TypeName == "Reference") // Ibn 4.7 Lists
				{
					row["FieldTypeImageUrl"] = String.Format(CultureInfo.InvariantCulture,
						"~/images/IbnFramework/metainfo/reference{0}.gif",
						postfix);
				}
/*				else if (field.TypeName == "BackReference")
				{
					row["FieldTypeImageUrl"] = String.Format(CultureInfo.InvariantCulture,
						"~/images/IbnFramework/metainfo/backreference{0}.gif",
						postfix);
				}
 */ 
				else
				{
					row["FieldTypeImageUrl"] = String.Format(CultureInfo.InvariantCulture,
						"~/images/IbnFramework/metainfo/metafield{0}.gif",
						postfix);
				}
				dt.Rows.Add(row);
			}

			DataView dv = dt.DefaultView;
			if (_pc[sortKey] == null)
				_pc[sortKey] = "FriendlyName";

			dv.Sort = _pc[sortKey];

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (GridViewRow row in grdMain.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");
			}

			if (!ShowSystemInfo)
			{
//				grdMain.Columns[0].Visible = false;
				grdMain.Columns[1].Visible = false;
			}
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == deleteCommand && mc != null)
			{
				MetaDataWrapper.DeleteMetaField(mc, e.CommandArgument.ToString());

				CHelper.RequireDataBind();
			}
		}
		#endregion

		#region grdMain_Sorting
		protected void grdMain_Sorting(object sender, GridViewSortEventArgs e)
		{
			if (_pc[sortKey] == e.SortExpression)
				_pc[sortKey] += " DESC";
			else
				_pc[sortKey] = e.SortExpression;
			BindData();
		}
		#endregion
	}
}