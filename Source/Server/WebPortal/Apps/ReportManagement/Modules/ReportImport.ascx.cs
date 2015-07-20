using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Globalization;
using Mediachase.IBN.Business.ReportSystem;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.ReportManagement.Modules
{
	public partial class ReportImport : System.Web.UI.UserControl
	{
		private const string _filterDirPath = "~/Apps/ReportManagement/FilterControls";

		protected void Page_Load(object sender, EventArgs e)
		{
			BindButtons();
			if (!Page.IsPostBack)
				BindFilters();
			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			//if (Request["ReportId"] != null)
			//    BindValues();
		}

		#region BindButtons
		private void BindButtons()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		} 
		#endregion

		private void BindFilters()
		{
			ddFilters.Items.Clear();
			ddFilters.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Report", "FilterNotSet").ToString(), "-1"));
			foreach (string path in ReportManager.GetFilterControls(Server.MapPath(_filterDirPath)))
			{
				ddFilters.Items.Add(new ListItem(path, path));
			}
		}

		//private void BindValues()
		//{
		//    string id = Request["ReportId"];
		//    Guid uid = new Guid(id);
		//    PrimaryKeyId pkId = new PrimaryKeyId(uid);
		//    EntityObject eo = BusinessManager.Load(ReportEntity.GetAssignedMetaClassName(), pkId);
		//    ReportEntity re = eo as ReportEntity;
		//    //...
		//}

		void btnSave_ServerClick(object sender, EventArgs e)
		{
			if (fSourceFile.PostedFile != null && fSourceFile.PostedFile.ContentLength > 0)
			{
				StringBuilder sb = new StringBuilder();
				using (StringWriter sw = new StringWriter(sb))
				{
					fSourceFile.PostedFile.InputStream.Seek(0, SeekOrigin.Begin);
					StreamReader sr = new StreamReader(fSourceFile.PostedFile.InputStream);
					sw.Write(sr.ReadToEnd());
					sr.Close();
				}

				ReportEntity re = BusinessManager.InitializeEntity<ReportEntity>(ReportEntity.GetAssignedMetaClassName());
				re.Title = txtTitle.Text;
				re.Description = txtDescription.Text;
				re.TimeZoneId = Mediachase.IBN.Business.Security.CurrentUser.TimeZoneId;
				if(ddFilters.SelectedValue != "-1")
					re.FilterControl = ddFilters.SelectedValue;
				re.RdlText = sb.ToString();
				re.IsCustom = true;
				BusinessManager.Create(re);
				CommandParameters cp = new CommandParameters("MC_RM_Import");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}
	}
}