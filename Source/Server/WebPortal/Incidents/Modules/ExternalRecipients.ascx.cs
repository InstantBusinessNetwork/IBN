namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for ExternalRecipients.
	/// </summary>
	public partial class ExternalRecipients : System.Web.UI.UserControl
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(ExternalRecipients).Assembly);

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
				secHeader.AddText(LocRM.GetString("tExternalRecipients"));
				if (!Mediachase.IBN.Business.Security.CurrentUser.IsExternal && Mediachase.IBN.Business.Incident.CanUpdateExternalRecipients(IncidentID))
				{
					CommandManager cm = CommandManager.GetCurrent(this.Page);
					CommandParameters cp = new CommandParameters("MC_HDM_RecipEdit");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					cmd = cmd.Replace("\"", "&quot;");
					secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"), "javascript:" + cmd);
					//secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"),
					//    "javascript:ShowWizard('RecipientsEditor.aspx?IncidentId=" + IncidentID + "', 450, 350);");
				}
			}
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			dgMembers.Columns[0].HeaderText = LocRM.GetString("tEMail");

			EMailIssueExternalRecipient[] erList = EMailIssueExternalRecipient.List(IncidentID);
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("EMail", typeof(string)));
			DataRow dr;
			foreach(EMailIssueExternalRecipient er in erList)
			{
				dr = dt.NewRow();
				dr["EMail"] = er.EMail;
				dt.Rows.Add(dr);
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "EMail";
			dgMembers.DataSource = dv;
			dgMembers.DataBind();
		}
		#endregion

		#region GetLink
		protected string GetLink(object eMail)
		{
			int iUserId = Mediachase.IBN.Business.User.GetUserByEmail(eMail.ToString());
			if(iUserId>0)
				return Util.CommonHelper.GetUserStatus(iUserId);
			else
			{
				Mediachase.IBN.Business.Client client = Mediachase.IBN.Business.Common.GetClient(eMail.ToString());
				if (client != null)
				{
					if (client.IsContact)
						return CommonHelper.GetContactLink(this.Page, client.Id, client.Name);
					else
						return CommonHelper.GetOrganizationLink(this.Page, client.Id, client.Name);
				}
				else
					return "";
			}
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
