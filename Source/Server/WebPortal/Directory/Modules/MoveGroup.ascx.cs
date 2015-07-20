namespace Mediachase.UI.Web.Directory.Modules
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

	/// <summary>
	///		Summary description for MoveGroup.
	/// </summary>
	public partial class MoveGroup : System.Web.UI.UserControl
	{
		ResourceManager LocRM;

		private int GroupID
		{
			get
			{
				try
				{
					return int.Parse(Request["GroupID"]);
				}
				catch
				{
					throw new Exception("GroupID is Reqired");
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strMoveGroup", typeof(MoveGroup).Assembly);

			btnMove.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnMove.CustomImage = this.Page.ResolveUrl("~/layouts/images/upload.gif");

			BindToolbar();
			if (!IsPostBack)
			{
				lblMoveTo.Text = LocRM.GetString("MoveTo");
				BindGroups();
			}
		}

		private void BindToolbar()
		{

			using (IDataReader reader = SecureGroup.GetGroup(GroupID))
			{
				while (reader.Read())
				{
					secHeader.Title = LocRM.GetString("tbTitle") + " '" + CommonHelper.GetResFileString((string)reader["GroupName"]) + "'";
				}
			}
			//secHeader.AddSeparator();
			btnMove.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");

		}

		private void BindGroups()
		{
			using (IDataReader reader = SecureGroup.GetListGroupsForMove(GroupID))
			{
				while (reader.Read())
				{
					ddGroupsList.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
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


		protected void btnMove_ServerClick(object sender, System.EventArgs e)
		{
			int GroupMoveTo = int.Parse(ddGroupsList.SelectedItem.Value);
			SecureGroup.UpdateGroup(GroupID, GroupMoveTo);
			Response.Redirect("../Directory/Directory.aspx?Tab=0");
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("../Directory/Directory.aspx?Tab=0");
		}
	}
}
