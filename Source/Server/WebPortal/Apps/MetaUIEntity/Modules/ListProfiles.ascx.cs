using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using System.Text;
using System.Globalization;
using Mediachase.Ibn.Lists;
using Mediachase.IbnNext.TimeTracking;
using System.Collections.Generic;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class ListProfiles : MCDataBoundControl
	{
		public const string ConstClassName = "ClassName";
		public const string ConstViewName = "ViewName";
		public const string ConstPlaceName = "PlaceName";
		public const string ConstPlaceNameValue = "EntityList";
		public const string ConstCommandNameEdit = "CommandName";
		public const string ConstCommandNameEditValue = "MC_MUI_ProfileEditAdmin";
		public const string ConstCommandNameNew = "CommandNewName";
		public const string ConstCommandNameNewValue = "MC_MUI_ProfileNewAdmin";
		public const string ConstDeleteCommand = "Dlt";
		public const string ConstResetCommand = "Rst";

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
					mc = (MetaClass)value;

				base.DataItem = value;
			}
		}
		#endregion

		#region MetaClass mc
		private MetaClass _mc;
		private MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState[ConstClassName] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState[ConstClassName].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState[ConstClassName] = value.Name;
				_mc = value;
			}
		}
		#endregion

		#region ClassName
		public string ClassName
		{
			get
			{
				if (ViewState[ConstClassName] == null)
					return String.Empty;
				return ViewState[ConstClassName].ToString();
			}
			set
			{
				ViewState[ConstClassName] = value;
			}
		}
		#endregion

		#region ViewName
		public string ViewName
		{
			get
			{
				if (ViewState[ConstViewName] == null)
					return String.Empty;
				return ViewState[ConstViewName].ToString();
			}
			set
			{
				ViewState[ConstViewName] = value;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get
			{
				if (ViewState[ConstPlaceName] == null)
					return ConstPlaceNameValue;
				return ViewState[ConstPlaceName].ToString();
			}
			set
			{
				ViewState[ConstPlaceName] = value;
			}
		}
		#endregion

		#region CommandEditName
		public string CommandEditName
		{
			get
			{
				if (ViewState[ConstCommandNameEdit] == null)
					return ConstCommandNameEditValue;
				return (string)ViewState[ConstCommandNameEdit];
			}
			set
			{
				ViewState[ConstCommandNameEdit] = value;
			}
		}
		#endregion

		#region CommandNewName
		public string CommandNewName
		{
			get
			{
				if (ViewState[ConstCommandNameNew] == null)
					return ConstCommandNameNewValue;
				return (string)ViewState[ConstCommandNameNew];
			}
			set
			{
				ViewState[ConstCommandNameNew] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand(String.Empty, String.Empty, PlaceName, "MC_MUI_NeedToDataBind");
			cm.AddCommand(String.Empty, String.Empty, PlaceName, CommandEditName);
		}

		#region DataBind
		public override void DataBind()
		{
			if (mc != null)
			{
				lnkNew.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "AddSystemView").ToString();
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters(CommandNewName);
				cp.CommandArguments = new Dictionary<string, string>();
				cp.AddCommandArgument("ClassName", mc.Name);
				string cmd = cm.AddCommand(mc.Name, ViewName, PlaceName, cp);
				lnkNew.NavigateUrl = String.Format("javascript:{{{0}}};", cmd);

				BindGrid();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				//hide list templates and timetracking classes (not entity objects)
				MetaClass mc = (MetaClass)dataItem;
				if (ListManager.MetaClassIsList(mc))
				{
					ListInfo li = ListManager.GetListInfoByMetaClassName(mc.Name);
					if (li != null && li.IsTemplate)
						return false;
				}
				else if (mc.Name == TimeTrackingEntry.GetAssignedMetaClass().Name ||
					mc.Name == TimeTrackingBlock.GetAssignedMetaClass().Name ||
					mc.Name == TimeTrackingBlockType.GetAssignedMetaClass().Name ||
					mc.Name == TimeTrackingBlockTypeInstance.GetAssignedMetaClass().Name)
					return false;
			}
			return base.CheckVisibility(dataItem);
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(string));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));
			dt.Columns.Add("CanDelete", typeof(bool));
			dt.Columns.Add("CanReset", typeof(bool));
			
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp;
			ListViewProfile[] list = ListViewProfile.GetSystemProfiles(mc.Name, PlaceName);
			foreach (ListViewProfile lvp in list)
			{
				DataRow dr = dt.NewRow();
				dr["Id"] = lvp.Id;
				dr["Name"] = CHelper.GetResFileString(lvp.Name);
				cp = new CommandParameters(CommandEditName);
				cp.CommandArguments = new Dictionary<string, string>();
				cp.AddCommandArgument("ClassName", mc.Name);
				cp.AddCommandArgument("uid", lvp.Id);
				string cmd = cm.AddCommand(mc.Name, ViewName, PlaceName, cp);
				dr["EditLink"] = String.Format("javascript:{{{0}}};", cmd);
				if (ListManager.MetaClassIsList(mc.Name))
					dr["CanDelete"] = list.Length > 1 && !lvp.ReadOnly;
				else
					dr["CanDelete"] = !lvp.ReadOnly;
				dr["CanReset"] = false;
				dt.Rows.Add(dr);
			}

			grdMain.DataSource = dt;
			grdMain.DataBind();

			foreach (DataGridItem row in grdMain.Items)
			{
				ImageButton ib;
				ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");

				ib = (ImageButton)row.FindControl("ibReset");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ResetToDefault").ToString() + "?')");
			}
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object source, DataGridCommandEventArgs e)
		{
			if (e == null)
				return;

			if (e.CommandName == ConstDeleteCommand)
			{
				string id = e.CommandArgument.ToString();
				ListViewProfile.DeleteCustomProfile(ClassName, id, PlaceName);
			}
			else if (e.CommandName == ConstResetCommand)
			{
				string id = e.CommandArgument.ToString();
				//TODO - reset
			}

			CHelper.RequireDataBind();
		}
		#endregion
	}
}