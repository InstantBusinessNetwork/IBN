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
	using Mediachase.Ibn;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Business.Customization;
	using Mediachase.Ibn.Data;

	/// <summary>
	///		Summary description for UserFullInfo.
	/// </summary>
	public partial class UserFullInfo : System.Web.UI.UserControl
	{

		private int UserID;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserView", typeof(UserFullInfo).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UserID = GetUserID();

			if (!IsPostBack)
			{
				ApplyLocalization();
				BindData();
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

		#region ApplyLocalization()
		private void ApplyLocalization()
		{
			tbEdit.AddText(LocRM.GetString("tbEditTitle"));
			lblLoginTitle.Text = LocRM.GetString("LoginTitle") + ":";
			lblPhoneTitle.Text = LocRM.GetString("PhoneTitle");
			lblFaxTitle.Text = LocRM.GetString("FaxTitle");
			lblMobileTitle.Text = LocRM.GetString("MobileTitle");
			lblJobTitleTitle.Text = LocRM.GetString("JobTitleTitle");
			lblDepartmentTitle.Text = LocRM.GetString("DepartmentTitle");
			lblLocationTitle.Text = LocRM.GetString("LocationTitle");
			lblIMGroupTitle.Text = LocRM.GetString("IMGroupTitle");
			lbWindowsLoginTitle.Text = LocRM.GetString("tWindowsLogin");
			lblCompanyTitle.Text = LocRM.GetString("Company");
			lblProfileTitle.Text = LocRM.GetString("Profile") + ":";
			if (User.CanUpdateUserInfo(UserID))
			{
				bool IsExternal = false;
				using (IDataReader reader = User.GetUserInfo(UserID, false))
				{
					if (reader.Read())
						IsExternal = (bool)reader["IsExternal"];
				}

				string sURL;
				if (IsExternal)
					sURL = String.Concat(Page.ResolveUrl("~/Directory/ExternalEdit.aspx"), "?Back=View&amp;UserID=", UserID);
				else
					sURL = String.Concat(Page.ResolveUrl("~/Directory/UserEdit.aspx"), "?Back=View&amp;UserID=", UserID);

				tbEdit.AddRightLink(
					String.Format(CultureInfo.InvariantCulture, "<img alt='' src='{0}'/> {1}", Page.ResolveUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("tbEditEdit")),
					sURL);
			}
		}
		#endregion

		#region BindData()
		private void BindData()
		{
			bool isExternal = false;
			bool isPending = false;

			try
			{
				using (IDataReader rdr = User.GetUserInfo(UserID))
				{
					//UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, IsPending, IsExternal
					if (rdr.Read())
					{
						lblLogin.Text = (string)rdr["Login"];
						lbWindowsLogin.Text = rdr["WindowsLogin"].ToString();
						isPending = (bool)rdr["IsPending"];
						if ((bool)rdr["IsExternal"])
						{
							isExternal = true;
							lblIMGroupTitle.Visible = false;
							lblLoginTitle.Visible = false;
							lblLogin.Visible = false;
						}
						else
						{
							isExternal = false;
						}

						if (isPending)
						{
							if (lblLogin.Text.IndexOf("___PENDING_USER___") >= 0)
							{
								lblLogin.Visible = false;
								lblLoginTitle.Visible = false;
							}
							lblIMGroupTitle.Visible = false;

							lblProfileTitle.Visible = false;
							lblProfile.Visible = false;
						}
						else if (isExternal)
						{
							lblIMGroupTitle.Visible = false;

							lblProfileTitle.Visible = false;
							lblProfile.Visible = false;
						}

						if (PortalConfig.UseIM && rdr["Login"].ToString().ToLower() != "alert")
						{
							if (rdr["IMGroupId"] != DBNull.Value)
							{
								int imGroupId = (int)rdr["IMGroupId"];
								string imGroupName = IMGroup.GetIMGroupName(imGroupId, null);
								if (imGroupName != null)
								{
									lblGroup.Text = string.Format(CultureInfo.InvariantCulture, "<a href='../Directory/Directory.aspx?Tab=1&amp;IMGroupID={0}'><img alt='' src='../layouts/images/icons/ibngroup.gif'/>{1}</a>", imGroupId, imGroupName);
								}
							}
						}
						else
						{
							lblGroup.Visible = false;
							lblIMGroupTitle.Visible = false;
						}

					}
					else
						Response.Redirect("../Common/NotExistingId.aspx?UserId=1");
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("~/Common/NotExistingID.aspx?AD=1");
			}

			using (IDataReader rdr = User.GetUserProfile(UserID))
			{
				//UserId, phone, fax, mobile, position, department, company, location, PictureUrl
				if (rdr.Read())
				{

					lblPhone.Text = (string)rdr["phone"];
					lblFax.Text = (string)rdr["fax"];
					lblMobile.Text = (string)rdr["mobile"];
					lblDepartment.Text = (string)rdr["department"];
					lblLocation.Text = (string)rdr["location"];
					lblJobTitle.Text = (string)rdr["position"];
					lblCompany.Text = (string)rdr["company"];
					imgPhoto.Src = "~/Common/GetUserPhoto.aspx?UserID=" + UserID.ToString() + "&amp;t=" + DateTime.Now.Millisecond.ToString();
				}
			}

			try
			{
				int profileId = -1;
				EntityObject[] userProfile = BusinessManager.List(CustomizationProfileUserEntity.ClassName, new FilterElement[] { FilterElement.EqualElement(CustomizationProfileUserEntity.FieldPrincipalId, UserID) });
				if (userProfile.Length > 0)
					profileId = ((CustomizationProfileUserEntity)userProfile[0]).ProfileId;

				EntityObject entity = BusinessManager.Load(CustomizationProfileEntity.ClassName, (PrimaryKeyId)profileId);
				if (entity != null)
					lblProfile.Text = CommonHelper.GetResFileString(((CustomizationProfileEntity)entity).Name);

			}
			catch { }
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
