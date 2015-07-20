namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using System.Reflection;
	using Mediachase.Ibn;

	/// <summary>
	///		Summary description for LdapSettings.
	/// </summary>
	public partial class LdapSettings : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
				BindDG();
			BindToolbar();
		}

		#region BindDG
		private void BindDG()
		{
			
			dgSets.Columns[1].HeaderText = LocRM.GetString("tTitle");
			dgSets.Columns[2].HeaderText = LocRM.GetString("tDomain");
			dgSets.Columns[3].HeaderText = IbnConst.ProductFamilyShort + " Key";
			dgSets.Columns[4].HeaderText = "LDAP Key";
			dgSets.Columns[5].HeaderText = LocRM.GetString("tAutosync");
			dgSets.Columns[6].HeaderText = LocRM.GetString("tLastSynch");

			dgSets.DataSource = Mediachase.IBN.Business.LdapSettings.Get(-1);
			/// Returns fields: LdapId, Title, Domain, Username, Password, Filter, IbnKey, LdapKey, Activate, Deactivate, Autosync, AutosyncStart, AutosyncInterval, LastSynchronization.
			dgSets.DataBind();

			foreach (DataGridItem dgi in dgSets.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('"+LocRM.GetString("tWarningDelSets")+"')");
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tLDAPSettings");
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/newitem.gif") + "'/> " + LocRM.GetString("tLDAPSettingsAdd"), "javascript:OpenWindow('LdapSettingsEdit.aspx',400,540,false)");
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tCommonSettings"), ResolveClientUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin3"));
		}
		#endregion

		protected string GetDate(object date)
		{
			string retVal = "";
			if(date!=DBNull.Value && ((DateTime)date).Year>1)
				retVal = (Mediachase.IBN.Business.User.GetLocalDate((DateTime)date)).ToString("g");
			return retVal;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgSets.DeleteCommand += new DataGridCommandEventHandler(dg_DeleteCommand);
		}
		#endregion

		#region Delete
		private void dg_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			Mediachase.IBN.Business.LdapSettings.Delete(sid);
			Response.Redirect("~/Admin/LdapSettings.aspx");
		}
		#endregion
	}
}
