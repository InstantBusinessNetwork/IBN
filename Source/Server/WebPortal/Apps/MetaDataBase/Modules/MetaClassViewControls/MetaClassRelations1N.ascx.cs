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
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta.Schema;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class MetaClassRelations1N : MCDataBoundControl
	{
		protected readonly string classNameKey = "ClassName";
		protected readonly string editLinkKey = "EditLink";
		protected readonly string editLinkDefaultValue = "~/Apps/MetaDataBase/Pages/Admin/RelationEdit.aspx";
		protected readonly string deleteCommand = "Dlt";
		protected readonly string sortKey = "Rel1N_Sort";

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
				string retval = editLinkDefaultValue;
				if (ViewState[editLinkKey] != null)
				{
					retval = ViewState[editLinkKey].ToString();
				}
				return retval;
			}
			set
			{
				ViewState[editLinkKey] = value;
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
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewRelation1N").ToString(),
					ResolveClientUrl("~/images/IbnFramework/newitem.gif"));

				NewLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
					"{0}?class={1}&mode=1N",
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
			dt.Columns.Add("DisplayName", typeof(string));
			dt.Columns.Add("PrimaryName", typeof(string));
			dt.Columns.Add("RelatedName", typeof(string));
			dt.Columns.Add("FieldTypeImageUrl", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));
			dt.Columns.Add("IsSystem", typeof(bool));

			foreach (MetaField field in mc.FindReferencesTo(true))
			{
				// don't process references from bridges
				if (field.Owner.IsBridge)
					continue;

				DataRow row = dt.NewRow();
				row["Name"] = String.Format(CultureInfo.InvariantCulture,
					"{0},{1}",
					field.Owner.Name,
					field.Name);

				// DisplayName - get from attribute or use PluralName
				if (!String.IsNullOrEmpty(field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock, string.Empty))
					&& field.Attributes.ContainsKey(McDataTypeAttribute.ReferenceDisplayText))
				{
					row["DisplayName"] = CHelper.GetResFileString(field.Attributes[McDataTypeAttribute.ReferenceDisplayText].ToString());
				}
				else
				{
					row["DisplayName"] = CHelper.GetResFileString(field.Owner.PluralName);
				}

				// PrimaryName - current class
				row["PrimaryName"] = CHelper.GetResFileString(mc.FriendlyName);

				// RelatedName - link
				row["RelatedName"] = CHelper.GetMetaClassAdminPageLink(field.Owner, this.Page);

				// FieldTypeImageUrl
				string postfix = string.Empty;
				if (field.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					postfix = "_sys";

				row["FieldTypeImageUrl"] = String.Format(CultureInfo.InvariantCulture,
					"~/images/IbnFramework/metainfo/backreference{0}.gif",
					postfix);

				// Edit - we use related class and related field as params
				row["EditLink"] = String.Format(CultureInfo.InvariantCulture,
					"{0}?class={1}&refclass={2}&reffield={3}&mode=1N", 
					EditFieldLink, 
					mc.Name,
					field.Owner.Name, 
					field.Name);

				// IsSystem
				row["IsSystem"] = field.Attributes.ContainsKey(MetaClassAttribute.IsSystem) || field.Owner.TitleFieldName == field.Name;

				dt.Rows.Add(row);
			}

			DataView dv = dt.DefaultView;
			if (_pc[sortKey] == null)
				_pc[sortKey] = "DisplayName";

			dv.Sort = _pc[sortKey];

			MainGrid.DataSource = dv;
			MainGrid.DataBind();

			foreach (GridViewRow row in MainGrid.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("DeleteButton");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");
			}
		}
		#endregion

		#region MainGrid_RowCommand
		protected void MainGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e != null && e.CommandName == deleteCommand & e.CommandArgument != null)
			{
				string[] parts = e.CommandArgument.ToString().Split(',');

				MetaDataWrapper.DeleteReference(parts[0], parts[1]);

				CHelper.RequireDataBind();
			}
		}
		#endregion

		#region MainGrid_Sorting
		protected void MainGrid_Sorting(object sender, GridViewSortEventArgs e)
		{
			if (_pc[sortKey] == e.SortExpression)
				_pc[sortKey] += " DESC";
			else
				_pc[sortKey] = e.SortExpression;
			BindData();
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			// O.R. [2009-05-06] IBN Fix:
			bool retval = true;
			if (dataItem is MetaClass)
			{
				MetaClass mc = dataItem as MetaClass;
				if (mc.Name.ToLowerInvariant().Contains("timetracking") || mc.IsCard)
					retval = false;
			}

			return retval;
		} 
		#endregion
	}
}