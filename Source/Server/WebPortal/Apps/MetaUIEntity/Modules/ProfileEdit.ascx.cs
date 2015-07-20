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
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class ProfileEdit : System.Web.UI.UserControl
	{
		#region _className
		private string _className
		{
			get
			{
				if (Request["className"] != null)
					return Request["className"];
				return String.Empty;
			}
		}
		#endregion

		#region _placeName
		private string _placeName
		{
			get
			{
				if (Request["placeName"] != null)
					return Request["placeName"];
				return String.Empty;
			}
		}
		#endregion

		#region _showFilters
		private bool _showFilters
		{
			get
			{
				if (Request["showFilters"] != null && Request["showFilters"] == "0")
					return false;
				return true;
			}
		}
		#endregion

		#region _showGrouping
		private bool _showGrouping
		{
			get
			{
				if (Request["showGrouping"] != null && Request["showGrouping"] == "0")
					return false;
				return true;
			}
		}
		#endregion

		#region _uid
		private string _uid
		{
			get
			{
				if (Request["uid"] != null)
					return Request["uid"];
				return String.Empty;
			}
		}
		#endregion

		#region _commandName
		private string _commandName
		{
			get
			{
				if (Request["commandName"] != null)
					return Request["commandName"];
				return "MC_MUI_ProfileEdited";
			}
		}
		#endregion

		#region _isSystem
		private bool _isSystem
		{
			get
			{
				if (Request["isSystem"] != null && Request["isSystem"] == "1")
					return true;
				return false;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				DefaultDataBind();
				if (String.IsNullOrEmpty(_uid))
					hfValue.Value = "Panel3";
				else
					hfValue.Value = "Panel1";
				if (!String.IsNullOrEmpty(_uid))
					BindSavedValues();
			}
			ApplyLocalization();

			if (!_showFilters)
			{
				tab2Gap.Visible = false;
				tdTab2.Visible = false;
				panel2.Visible = false;
			}
			if (!_showGrouping)
			{
				tab4Gap.Visible = false;
				tdTab4.Visible = false;
				panel4.Visible = false;
			}
			if (_isSystem)
			{
				cbIsPublic.Checked = true;
				cbIsPublic.Visible = false;
			}
			btnClose.OnClientClick = Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true);

			//add command for filters
			CommandParameters cp = new CommandParameters("MC_MUI_EntityDDSmall");
			CommandManager.GetCurrent(this.Page).AddCommand(string.Empty, string.Empty, string.Empty, cp);
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("ChangeTab('{0}');", hfValue.Value), true);

			base.OnPreRender(e);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnSave.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tSave}");
			btnClose.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tClose}");

			lgdFinish.AddText(GetGlobalResourceObject("IbnFramework.Global", "SelectTitle").ToString());
			cbIsPublic.Text = " " + GetGlobalResourceObject("IbnFramework.Global", "IsPublicView").ToString();
			rfTitle.ErrorMessage = GetGlobalResourceObject("IbnFramework.Global", "TitleIsRequired").ToString();
		}
		#endregion

		#region DefaultDataBind
		/// <summary>
		/// Default data bind.
		/// </summary>
		private void DefaultDataBind()
		{
			cbIsPublic.Visible = Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.Administrator);

			List<ListItem> listItems = new List<ListItem>();

			MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(_className);
			foreach (MetaField field in mc.Fields)
			{
				if (field.Attributes.ContainsKey(McDataTypeAttribute.AggregationMark))
					continue;
				if (field.IsBackReference)
					continue;
				if (field.IsAggregation)
				{
					string aggrClassName = field.Attributes.GetValue<string>(McDataTypeAttribute.AggregationMetaClassName);
					MetaClass aggrClass = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(aggrClassName);
					foreach (MetaField aggrField in aggrClass.Fields)
					{
						ListItem li = new ListItem(CHelper.GetMetaFieldName(field) + "." + CHelper.GetMetaFieldName(aggrField), field.Name + "." + aggrField.Name);
						listItems.Add(li);
					}
				}
				else
				{
					ListItem li = new ListItem(CHelper.GetMetaFieldName(field), field.Name);
					listItems.Add(li);
				}
			}

			listItems.Sort(delegate(ListItem x, ListItem y) { return x.Text.CompareTo(y.Text); });

			ListSelector.Items.AddRange(listItems.ToArray());
			
			//ctrlFilter.Provider = new ListViewProfileExpressionProvider();
			ctrlFilter.ProviderName = "MetaDataProvider";
			ctrlFilter.DataSource = string.Format("{0}::{1}", _className, _placeName) + ";" + _uid;// "Contact::EntitySelect;EntitySelect";
			ctrlFilter.DataBind();
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			ListViewProfile profile = ListViewProfile.Load(_className, _uid, _placeName);
			txtTitle.Text = profile.Name;
			cbIsPublic.Checked = profile.IsPublic;
			int weight = 0;
			foreach (string name in profile.FieldSet)
			{
				ListItem liItem = ListSelector.Items.FindByValue(name);
				if (liItem != null)
				{
					liItem.Selected = true;
					liItem.Attributes.Add("Weight", (weight++).ToString());
				}
			}
		}
		#endregion

		#region SaveFilters
		private FilterExpressionNodeCollection SaveFilters(string expressionKey)
		{
			FilterExpressionNodeCollection coll = ctrlFilter.NodeCollection;
			ctrlFilter.Provider.SaveFilters(string.Format("{0}::{1}", _className, _placeName), expressionKey, ctrlFilter.NodeCollection[0].ChildNodes);

			return coll;
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (Page.IsValid)
			{
				ListViewProfile profile;
				if (!String.IsNullOrEmpty(_uid))
					profile = ListViewProfile.Load(_className, _uid, _placeName);
				else
					profile = new ListViewProfile();
				//fields
				List<string> fields = new List<string>();
				ColumnPropertiesCollection coll = new ColumnPropertiesCollection();
				List<ListItem> items = ListSelector.GetSelectedItems();
				foreach (ListItem item in items)
				{
					fields.Add(item.Value);
					coll.Add(new ColumnProperties(item.Value, "150", String.Empty));
				}
				profile.FieldSet = fields;
				profile.ColumnsUI = coll;

				string uid = (!String.IsNullOrEmpty(_uid)) ? _uid : Guid.NewGuid().ToString();
				profile.Id = uid;

				profile.Name = txtTitle.Text;
				profile.ReadOnly = false;

				int currentUserId = Mediachase.Ibn.Data.Services.Security.CurrentUserId;

				if (_isSystem)
				{
					profile.IsSystem = true;
					profile.IsPublic = true;
					ListViewProfile.SaveSystemProfile(_className, _placeName, currentUserId, profile);
				}
				else
				{
					profile.IsSystem = false;
					profile.IsPublic = cbIsPublic.Checked;
					ListViewProfile.SaveCustomProfile(_className, _placeName, currentUserId, profile);
				}
				SaveFilters(uid);

				CommandParameters cp = new CommandParameters(_commandName);
				cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("ViewUid", uid);
				cp.AddCommandArgument("ClassName", _className);
				cp.AddCommandArgument("PlaceName", _placeName);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion
	}
}