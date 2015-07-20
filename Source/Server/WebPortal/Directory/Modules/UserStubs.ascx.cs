namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for UserStubs.
	/// </summary>
	public partial  class UserStubs : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strWebStubs", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region UserID
		private int UserID
		{
			get 
			{
				try
				{
					if (Request["UserID"]!=null)
						return int.Parse(Request["UserID"]);
					else
					{
						if(Request["AccountID"]!=null)
						{
							int iID = 0;
							using (IDataReader reader = User.GetUserInfoByOriginalId(int.Parse(Request["AccountID"])))
							{
								if(reader.Read())
									iID = (int)reader["UserId"];
							}
							if(iID>0)
								return iID;
							else
								return Security.CurrentUser.UserID;
						}
						else
							return Security.CurrentUser.UserID;
					}
				}
				catch
				{
					throw new Exception("Invalid User ID");
				}
			}
		}
		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!PortalConfig.UseIM || UserID != Security.CurrentUser.UserID) 
			{
				this.Visible = false;
				return;
			}

			if (!IsPostBack)
			{
				BindDG();
				BindDGUser();
				
			}
			BindToolbar();
		}

		#region GetIcon
		protected string GetIcon(int HasIcon, string abb,int StubID)
		{
			if (HasIcon == 0)
				return abb;
			else
				return "<img width=32 height=32 border=0 src='../Common/WebStub.aspx?StubId=" + StubID +"'>";
		}
		#endregion

		#region GetGroups
		protected string GetGroups(int StubId)
		{
			StringBuilder sb = new StringBuilder();
			using (IDataReader rdr = WebStubs.GetListGroupsByStub(StubId))
			{
				
				while (rdr.Read())
				{
					sb.Append(CommonHelper.GetGroupLink((int)rdr["GroupId"], CommonHelper.GetResFileString((string)rdr["GroupName"])));
					sb.Append("<br>");
				}
			}
			return sb.ToString();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			tbHeader.AddText(LocRM.GetString("tTitle"));
			tbHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/webstub_create.gif'/> " + LocRM.GetString("Add"),"../Admin/AddEditWebStub.aspx?Group=0");
			sep1.Title = String.Concat("<b>",LocRM.GetString("Inherited"),"</b>");
			sep2.Title = String.Concat("<b>",LocRM.GetString("Your"),"</b>");
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgStubs.Columns[1].HeaderText = LocRM.GetString("Icon");
			dgStubs.Columns[2].HeaderText = LocRM.GetString("Title");
			dgStubs.Columns[3].HeaderText = LocRM.GetString("Url");
			dgStubs.Columns[4].HeaderText = LocRM.GetString("Groups");
			dgStubs.Columns[5].HeaderText = LocRM.GetString("Status");
			dgStubs.Columns[6].HeaderText = LocRM.GetString("Options");

			dgStubs.DataSource = WebStubs.GetListGroupStubsForUser();
			dgStubs.DataBind();
		}
		#endregion

		#region BindDGUser
		private void BindDGUser()
		{
			dgUserStubs.Columns[1].HeaderText = LocRM.GetString("Icon");
			dgUserStubs.Columns[2].HeaderText = LocRM.GetString("Title");
			dgUserStubs.Columns[3].HeaderText = LocRM.GetString("Url");
			dgUserStubs.Columns[4].HeaderText = LocRM.GetString("Options");

			dgUserStubs.DataSource = WebStubs.GetListStubsForUser();
			dgUserStubs.DataBind();

			foreach (DataGridItem dgi in dgUserStubs.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");

				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('"+ LocRM.GetString("Warning") +"')");
			}
		}
		#endregion

		#region GetActive
		protected string GetActive(int IsActive)
		{
			if (IsActive == 1)
				return LocRM.GetString("Active");
			else
				return LocRM.GetString("NotActive");
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgStubs.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgStubs_update);
			this.dgUserStubs.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgUserStubs_Delete);

		}
		#endregion

		#region dgStubs_update
		private void dgStubs_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int StubID = int.Parse(e.Item.Cells[0].Text);
			if ((string)e.CommandArgument == "1")
				WebStubs.Hide(StubID);
			else
				WebStubs.Show(StubID);
			BindDG();
		}
		#endregion

		#region dgUserStubs_Delete
		private void dgUserStubs_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int StubId = int.Parse(e.Item.Cells[0].Text);
			WebStubs.Delete(StubId);
			BindDGUser();
		}
		#endregion
	}
}
