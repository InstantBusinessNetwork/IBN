namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for IncidentResources.
	/// </summary>
	public partial class IncidentResources : System.Web.UI.UserControl
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(IncidentResources).Assembly);

		#region IncidentID
		protected int IncidentID
		{
			get 
			{
				try
				{
					return int.Parse(Request["IncidentID"]);
				}
				catch
				{
					throw new Exception("IncidentID is Reqired");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = true;
			BinddgMembers();
			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			if (dgMembers.Items.Count == 0)
			{
				this.Visible = false;
			}
			else
			{
				secHeader.AddText(LocRM.GetString("tbIncidentRes"));
				if (Incident.CanModifyResourcesSecurity(IncidentID))
				{
					CommandManager cm = CommandManager.GetCurrent(this.Page);
					CommandParameters cp = new CommandParameters("MC_HDM_ResEdit");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					cmd = cmd.Replace("\"", "&quot;");

					secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"),
						"javascript:" + cmd);//"javascript:ShowWizard('ResourcesEditor.aspx?IncidentId=" + IncidentID + "', 650, 350);");
				}
			}
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("UserName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("Status");

			dgMembers.DataSource = Incident.GetListIncidentResourcesDataTable(IncidentID);
			dgMembers.DataBind();
		}
		#endregion

		#region GetLink
		protected string GetLink(int PID,bool IsGroup)
		{
			if (IsGroup)
				return CommonHelper.GetGroupLink(PID);
			else
				return CommonHelper.GetUserStatus(PID);
		}
		#endregion

		#region GetStatus
		protected string GetStatus(object _mbc, object _rp,object _ic)
		{
			bool mbc = false;
			if (_mbc!=DBNull.Value)
				mbc = (bool)_mbc;

			bool rp = false;
			if (_rp!=DBNull.Value)
				rp = (bool)_rp;

			bool ic = false;
			if (_ic!=DBNull.Value)
				ic = (bool)_ic;

			if (!mbc) return "";
			else
				if (rp) return LocRM.GetString("Waiting");
			else
				if (ic)  return LocRM.GetString("Accepted");
			else return LocRM.GetString("Declined");
		}
		#endregion

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
		}
		#endregion

	}
}
