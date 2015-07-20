namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Resources;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for UserShortInfo.
	/// </summary>
	public partial class UserShortInfo : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserView", typeof(UserShortInfo).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblClientLogin.Text = String.Format(LocRM.GetString("IbnClientLogin"), IbnConst.ProductFamilyShort);
			if (!IsPostBack)
			{
				BindUserInfo();
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BindUserInfo
		private void BindUserInfo()
		{
			bool isActive = false;
			bool isExternal = false;
			bool isPending = false;

			int UserID = GetUserID();

			LoginRow.Visible = (UserID == Security.CurrentUser.UserID);

			try
			{
				using (IDataReader reader = User.GetUserInfo(UserID))
				{
					if (reader.Read())
					{
						lblFirstName.Text = HttpUtility.HtmlEncode(reader["FirstName"].ToString());
						lblLastName.Text = HttpUtility.HtmlEncode(reader["LastName"].ToString());
						lblEmail.Text = string.Format("<a href=\"mailto:{0}\">{1}</a>", HttpUtility.HtmlAttributeEncode(reader["Email"].ToString()), HttpUtility.HtmlEncode(reader["Email"].ToString()));
						IbnClientLoginLabel.Text = String.Format(CultureInfo.InvariantCulture,
							"<font color=\"red\"><b>{0}@{1}</b></font>",
							HttpUtility.HtmlEncode(Security.CurrentUser.Login),
							HttpUtility.HtmlEncode(Configuration.Domain));

						isActive = (bool)reader["IsActive"];
						isPending = (bool)reader["IsPending"];
						isExternal = (bool)reader["IsExternal"];

						if (!isActive)
						{
							lblUserActivity.Text = LocRM.GetString("NotActiveUser");
							lblUserActivity.Style.Add("visibility", "visible");
						}
						else if (isPending)
						{
							lblUserActivity.Text = LocRM.GetString("PendingUser");
							lblUserActivity.Style.Add("visibility", "visible");
						}
						else if (isExternal)
						{
							lblUserActivity.Text = LocRM.GetString("ExternalUser");
							lblUserActivity.Style.Add("visibility", "visible");
						}
					}
					else
						Response.Redirect("~/Common/NotExistingId.aspx?UserId=1");
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("~/Common/NotExistingID.aspx?AD=1");
			}

			lblGroupList.Text = "";

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("IsGroup", typeof(bool)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("LinkName", typeof(string)));
			DataRow dr;

			using (IDataReader rdr = User.GetListSecureGroup(UserID))
			{
				while (rdr.Read())
				{
					dr = dt.NewRow();
					int iGroupId = (int)rdr["GroupId"];
					dr["Id"] = iGroupId;
					dr["IsGroup"] = (iGroupId > 8);
					string groupName = CommonHelper.GetResFileString(rdr["GroupName"].ToString());
					dr["Name"] = groupName;
					dr["LinkName"] = CommonHelper.GetGroupLink(iGroupId, groupName) + "<br />";
					dt.Rows.Add(dr);
				}
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "IsGroup, Name";
			
			foreach (DataRowView drv in dv)
			{
				lblGroupList.Text += drv["LinkName"].ToString();
			}
		}
		#endregion

		#region GetUserID()
		private int GetUserID()
		{
			int userID = -1;
			try
			{
				if (Request["UserID"] != null)
					userID = int.Parse(Request["UserID"]);
				else
				{
					if (Request["AccountID"] != null)
					{
						int iID = 0;
						using (IDataReader reader = User.GetUserInfoByOriginalId(int.Parse(Request["AccountID"])))
						{
							if (reader.Read())
								iID = (int)reader["UserId"];
						}
						if (iID > 0)
							userID = iID;
						else
							userID = Security.CurrentUser.UserID;
					}
					else
						userID = Security.CurrentUser.UserID;
				}
			}
			catch
			{
				throw new Exception("Invalid User ID");
			}
			return userID;
		}
		#endregion
	}
}
