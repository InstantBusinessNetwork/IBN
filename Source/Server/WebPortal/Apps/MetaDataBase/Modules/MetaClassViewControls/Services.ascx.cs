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
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class Services : MCDataBoundControl
	{
		protected readonly string className = "ClassName";

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
		private MetaClass _mc = null;
		private MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState[className] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState[className].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState[className] = value.Name;
				_mc = value;
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
				BindData();
			base.DataBind();
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			// O.R.: Ibn 4.7 add-on
			Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			bool adminMode = true;
			if (pc["TimeTrackingMode"] != null && pc["TimeTrackingMode"] == "dev")
				adminMode = false;

			return !adminMode;
		}
		#endregion

		#region BindData
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("IsInstalled", typeof(bool)));
			DataRow dr;
			BusinessObjectServiceInfo[] mas = BusinessObjectServiceManager.GetRegisteredServices();
			foreach (BusinessObjectServiceInfo bosi in mas)
			{
				if (!bosi.CanInstall(mc))
					continue;
				dr = dt.NewRow();
				dr["Name"] = bosi.Name;
				dr["Description"] = bosi.Description;
				dr["IsInstalled"] = bosi.IsInstalled(mc);
				dt.Rows.Add(dr);
			}
			grdMain.DataSource = dt.DefaultView;
			grdMain.DataBind();
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			string bosiName = e.CommandArgument.ToString();
			if (e.CommandName == "Install")
				BusinessObjectServiceManager.InstallService(mc, bosiName);
			else if (e.CommandName == "Uninstall")
				BusinessObjectServiceManager.UninstallService(mc, bosiName);
			CHelper.RequireDataBind();
		}
		#endregion
	}
}