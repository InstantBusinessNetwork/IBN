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

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls
{
	public partial class ListInfoShortInfo : MCDataBoundControl
	{
		protected readonly string classNameKey = "ClassName";

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

		protected void Page_Load(object sender, EventArgs e)
		{
			ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(SaveButton);
		}

		#region DataBind
		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
		/// </summary>
		public override void DataBind()
		{
			ListInfo li = ListManager.GetListInfoByMetaClass(mc);

			if (!li.IsTemplate)
			{
				ListDataLink.Text = li.Title;
				ListDataLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName={0}", mc.Name);
				ListTemplate.Visible = false;
			}
			else
			{
				ListTemplate.Text = li.Title;
				ListDataLink.Visible = false;
			}

			RecordCountLabel.Text = MetaObject.GetTotalCount(mc).ToString();
			CreatedByLabel.Text = CommonHelper.GetUserStatus(li.CreatorId);
			CreatedLabel.Text = li.Created.ToShortDateString();

			StatusLabel.Text = (li.Properties["Status"].Value != null) ? CHelper.GetResFileString(MetaEnum.GetFriendlyName(MetaDataWrapper.GetEnumByName(ListManager.ListStatusEnumName), (int)li.Properties["Status"].Value)) : "";
			TypeLabel.Text = (li.Properties["ListType"].Value != null) ? CHelper.GetResFileString(MetaEnum.GetFriendlyName(MetaDataWrapper.GetEnumByName(ListManager.ListTypeEnumName), (int)li.Properties["ListType"].Value)) : ""; 

			string titleFieldName = mc.TitleFieldName;
			if (!String.IsNullOrEmpty(titleFieldName))
			{
				if (mc.Fields[titleFieldName] != null)
				{
					DefaultFieldButton.Text = CHelper.GetResFileString(mc.Fields[titleFieldName].FriendlyName);
					DefaultFieldLabel.Text = DefaultFieldButton.Text;
				}
				else
				{
					DefaultFieldButton.Text = titleFieldName;
				}
			}
			else
			{
				DefaultFieldButton.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "DefaultFieldNotSet").ToString();
				DefaultFieldLabel.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "DefaultFieldNotSet").ToString();
			}

			foreach (MetaField mf in mc.Fields)
			{
				McDataType type = mf.GetOriginalMetaType().McDataType;
				if (mf.IsNullable || type != McDataType.String)
					continue;
				string itemText = "";
				string name = mf.Name;
				itemText = CHelper.GetResFileString(mf.FriendlyName);;
				FieldsList.Items.Add(new ListItem(itemText, name));
			}
			if (!String.IsNullOrEmpty(titleFieldName))
				CHelper.SafeSelect(FieldsList, titleFieldName);

			if (FieldsList.Items.Count > 0)
			{
				DefaultFieldButton.Visible = true;
				DefaultFieldLabel.Visible = false;
			}
			else
			{
				DefaultFieldButton.Visible = false;
				DefaultFieldLabel.Visible = true;
			}
			
			FieldsList.Visible = false;
			SaveButton.Visible = false;
			CancelButton.Visible = false;

			base.DataBind();
		}
		#endregion

		#region SaveButton_Click
		/// <summary>
		/// Handles the Click event of the SaveButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
		protected void SaveButton_Click(object sender, ImageClickEventArgs e)
		{
			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				mc.TitleFieldName = FieldsList.SelectedValue;
				scope.SaveChanges();
			}

			DefaultFieldButton.Visible = true;
			FieldsList.Visible = false;
			SaveButton.Visible = false;
			CancelButton.Visible = false;

			CHelper.RequireDataBind();
		}
		#endregion

		#region CancelButton_Click
		/// <summary>
		/// Handles the Click event of the CancelButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
		protected void CancelButton_Click(object sender, ImageClickEventArgs e)
		{
			DefaultFieldButton.Visible = true;
			FieldsList.Visible = false;
			SaveButton.Visible = false;
			CancelButton.Visible = false;
		}
		#endregion

		#region DefaultFieldButton_Click
		/// <summary>
		/// Handles the Click event of the DefaultFieldButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void DefaultFieldButton_Click(object sender, EventArgs e)
		{
			DefaultFieldButton.Visible = false;
			FieldsList.Visible = true;
			SaveButton.Visible = true;
			CancelButton.Visible = true;
		}
		#endregion
	}
}