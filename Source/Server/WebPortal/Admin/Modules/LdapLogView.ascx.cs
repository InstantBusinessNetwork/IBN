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

	/// <summary>
	///		Summary description for LdapLogView.
	/// </summary>
	public partial class LdapLogView : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region LogId
		private int LogId
		{
			get
			{
				if(Request["LogId"]!=null)
					return int.Parse(Request["LogId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
				BindValues();
			BindToolbar();
		}

		#region BindValues
		private void BindValues()
		{
			DataTable dt = Ldap.SyncLogGetList(LogId);
			if(dt.Rows.Count>0)
			{
				DataRow dr = dt.Rows[0];
				lblTitle.Text = String.Format("<a href='{2}?SetId={1}'>{0}</a>", 
					dr["Title"].ToString(), dr["LdapId"].ToString(),
					ResolveClientUrl("~/Admin/LdapSettingsView.aspx"));
				lblSynchDate.Text = ((DateTime)dr["dt"]).ToString("g");
			}
			BindDG();
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			int i = 1;
			dgFields.Columns[i++].HeaderText = LocRM.GetString("FieldName");
			dgFields.Columns[i++].HeaderText = LocRM.GetString("tNewValue");
			dgFields.Columns[i++].HeaderText = LocRM.GetString("tOldValue");

			dgFields.DataSource = Ldap.SyncLogGetFields(LogId).DefaultView;
			//UserId, UserName, FieldName, NewValue, OldValue
			dgFields.DataBind();

			foreach (DataGridItem dgi in dgFields.Items)
			{
				if(dgi.Cells[0].Text!="&nbsp;")
					dgi.BackColor = Color.FromArgb(0xD0, 0xD0, 0xD0);
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tLDAPLogView");
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tLDAPLogs"),
				this.Page.ResolveUrl("~/Admin/LdapLogs.aspx"));
		}
		#endregion

		#region Protected DG Strings
		protected string GetTitle(object UserId, object UserName, object FieldName)
		{
			string retVal = "";
			if(UserId!=DBNull.Value)
				retVal = "<b>"+UserName.ToString()+"</b>";
			else
				retVal = "<span style='width:50px'>&nbsp;</span>" + FieldName.ToString();
			return retVal;
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
