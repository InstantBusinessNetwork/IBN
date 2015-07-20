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
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta.Schema;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class MetaClassRelationsNN : MCDataBoundControl
	{
		protected readonly string classNameKey = "ClassName";
		protected readonly string editLinkKey = "EditLink";
		protected readonly string editLinkDefaultValue = "~/Apps/MetaDataBase/Pages/Admin/RelationNNEdit.aspx";
		protected readonly string deleteCommand = "Dlt";
		protected readonly string sortKey = "RelNN_Sort";

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
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewRelationNN").ToString(),
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
			dt.Columns.Add("DisplayName", typeof(string));
			dt.Columns.Add("CurrentName", typeof(string));
			dt.Columns.Add("RelatedName", typeof(string));
			dt.Columns.Add("FieldTypeImageUrl", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));
			dt.Columns.Add("IsSystem", typeof(bool));

			foreach (MetaClass bridgeClass in DataContext.Current.MetaModel.GetBridgesReferencedToClass(mc))
			{
				if (!bridgeClass.Attributes.ContainsKey(MetaClassAttribute.BridgeRef1Name)
					|| !bridgeClass.Attributes.ContainsKey(MetaClassAttribute.BridgeRef2Name))
					continue;

				string field1Name = bridgeClass.Attributes[MetaClassAttribute.BridgeRef1Name].ToString();
				string field2Name = bridgeClass.Attributes[MetaClassAttribute.BridgeRef2Name].ToString();

				MetaField bridgeField1;	// reference to current class
				MetaField bridgeField2;	// reference to another class
				if (bridgeClass.Fields[field1Name].Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty) == mc.Name)
				{
					bridgeField1 = bridgeClass.Fields[field1Name];
					bridgeField2 = bridgeClass.Fields[field2Name];
				}
				else
				{
					bridgeField1 = bridgeClass.Fields[field2Name];
					bridgeField2 = bridgeClass.Fields[field1Name];
				}

				if (bridgeField2 == null)
					continue;

				string relatedClassName = bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty);
				if (String.IsNullOrEmpty(relatedClassName))
					continue;

				MetaClass relatedClass = MetaDataWrapper.GetMetaClassByName(relatedClassName);

				DataRow row = dt.NewRow();
				row["Name"] = bridgeClass.Name;

				// DisplayName - get from attribute or use PluralName of related class
				if (!String.IsNullOrEmpty(bridgeField1.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock, string.Empty))
					&& bridgeField1.Attributes.ContainsKey(McDataTypeAttribute.ReferenceDisplayText))
				{
					row["DisplayName"] = CHelper.GetResFileString(bridgeField1.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText, string.Empty));
				}
				else
				{
					row["DisplayName"] = CHelper.GetResFileString(relatedClass.PluralName);
				}

				// CurrentName - current class
				row["CurrentName"] = CHelper.GetResFileString(mc.FriendlyName);

				// RelatedName - related class
				if (relatedClass.Name == mc.Name)	// if reletion with the same class
					row["RelatedName"] = CHelper.GetResFileString(mc.FriendlyName);
				else
					row["RelatedName"] = CHelper.GetMetaClassAdminPageLink(relatedClass, this.Page);


				// FieldTypeImageUrl
				string postfix = string.Empty;
				if (bridgeClass.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					postfix = "_sys";

				row["FieldTypeImageUrl"] = String.Format(CultureInfo.InvariantCulture,
					"~/images/IbnFramework/metainfo/bridge{0}.gif",
					postfix);

				// Edit - we use current class, bridge class and field as params
				row["EditLink"] = String.Format("{0}?class={1}&bridge={2}&field={3}", EditFieldLink, mc.Name, bridgeClass.Name, bridgeField1.Name);

				// IsSystem
				row["IsSystem"] = bridgeClass.Attributes.ContainsKey(MetaClassAttribute.IsSystem);

				dt.Rows.Add(row);

				// we can have two references to current class from one bridge
				if (bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty) == mc.Name)
				{
					row = dt.NewRow();
					row["Name"] = bridgeClass.Name;

					// DisplayName - get from attribute or use PluralName 
					if (!String.IsNullOrEmpty(bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock, string.Empty))
						&& bridgeField2.Attributes.ContainsKey(McDataTypeAttribute.ReferenceDisplayText))
					{
						row["DisplayName"] = CHelper.GetResFileString(bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText, string.Empty));
					}
					else
					{
						row["DisplayName"] = CHelper.GetResFileString(mc.PluralName);
					}

					// CurrentName - current class
					row["CurrentName"] = CHelper.GetResFileString(mc.FriendlyName);

					// RelatedName - related class
					row["RelatedName"] = CHelper.GetResFileString(mc.FriendlyName);

					// FieldTypeImageUrl
					postfix = string.Empty;
					if (bridgeClass.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
						postfix = "_sys";

					row["FieldTypeImageUrl"] = String.Format(CultureInfo.InvariantCulture,
						"~/images/IbnFramework/metainfo/bridge{0}.gif",
						postfix);

					// Edit - we use current class, bridge class and field as params
					row["EditLink"] = String.Format("{0}?class={1}&bridge={2}&field={3}", EditFieldLink, mc.Name, bridgeClass.Name, bridgeField2.Name);

					// IsSystem
					row["IsSystem"] = bridgeClass.Attributes.ContainsKey(MetaClassAttribute.IsSystem);

					dt.Rows.Add(row);
				}
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

			if (!ShowSystemInfo)
			{
				MainGrid.Columns[1].Visible = false;
			}
		}
		#endregion

		#region MainGrid_RowCommand
		protected void MainGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e != null && e.CommandName == deleteCommand & e.CommandArgument != null)
			{
				MetaDataWrapper.DeleteBridge(e.CommandArgument.ToString());

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