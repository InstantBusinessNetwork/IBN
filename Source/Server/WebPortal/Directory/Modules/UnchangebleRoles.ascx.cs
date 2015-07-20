namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using System.Collections;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for UnchangebleRoles.
	/// </summary>
	public partial  class UnchangebleRoles : System.Web.UI.UserControl
	{
		protected int GroupId = -1;
		protected int UserId = -1;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strDeleteUser", typeof(UnchangebleRoles).Assembly);
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnDeactivate.InnerText = LocRM.GetString("Deactivate");
			cbManager.Text = LocRM.GetString("Manager");
			btnReplace.InnerText = LocRM.GetString("btnReplace");
			btnDelFromGroup.InnerText = LocRM.GetString("DelFromGroup");
			UserId = int.Parse(Request["UserId"]);
			if(Request["SGroupId"]==null || Request["SGroupId"]=="")
				btnDelFromGroup.Visible=false;
			else
				GroupId = int.Parse(Request["SGroupId"].ToString());
			
			int i=0;
			using (IDataReader reader = User.GetListSecureGroup(UserId))
			{
				while(reader.Read())
					i++;
			}

			if(i==1) btnDelFromGroup.Visible=false;
			if (!IsPostBack)
			{
				BindDD();
			}
		}

		private void BindDD()
		{
			using (IDataReader rdr = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.Everyone))
			{
				while (rdr.Read())
				{
					if (rdr["UserId"].ToString() != Request["UserId"])
						ddReplace.Items.Add(new ListItem(String.Concat((string)rdr["LastName"], " ", (string)rdr["FirstName"]), rdr["UserId"].ToString()));
				}
			}
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void btnDeactivate_Click(object sender, System.EventArgs e)
		{
			User.UpdateActivity(int.Parse(Request["UserId"]), false);
			if (Request["BtnID"] != null)
        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.document.forms[0]." + Request["BtnID"] + ".click();}" +
					"catch (e){} window.close();", true);
			else
        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.top.frames['right'].location.href='../Directory/Directory.aspx?Tab=3';}" +
					"catch (e){} window.close();", true);
		}

		protected void btnReplace_Click(object sender, System.EventArgs e)
		{
			if (ddReplace.SelectedItem!=null)
			{
				int ToUserId = int.Parse(ddReplace.SelectedItem.Value);
				User.ReplaceRelations(UserId,ToUserId,cbManager.Checked);
			}	
			
			if (Request["BtnID"] != null)
        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.document.forms[0]." + Request["BtnID"] + ".click();}" +
					"catch (e){} window.close();", true);
			else
        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.top.frames['right'].location.href='../Directory/Directory.aspx?Tab=3';}" +
					"catch (e){} window.close();", true);
		}

		protected void btnDelFromGroup_Click(object sender, System.EventArgs e)
		{
			User.DeleteUserFromGroup(UserId, GroupId);

			if (Request["BtnID"] != null)
        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.document.forms[0]." + Request["BtnID"] + ".click();}" +
					"catch (e){} window.close();", true);
			else
        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.top.frames['right'].location.href='../Directory/Directory.aspx?Tab=3';}" +
					"catch (e){} window.close();", true);
		}
	}
}
