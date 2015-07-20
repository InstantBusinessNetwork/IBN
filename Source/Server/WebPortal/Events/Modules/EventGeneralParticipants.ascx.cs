namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for EventGeneralParticipants.
	/// </summary>
	public partial  class EventGeneralParticipants : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventGeneral", typeof(EventGeneralParticipants).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(EventGeneralParticipants).Assembly);

		private DataTable dt;
		
		#region EventID
		protected int EventID
		{
			get 
			{
				return CommonHelper.GetRequestInteger(Request, "EventID", -1);
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BinddgMembers();
			BindToolbar();
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

		#region BinddgMembers
		private void BinddgMembers()
		{
			dt = CalendarEntry.GetListEventResourcesDataTable(EventID);

			bool isinres = false;

			int _UserID = 0;
			
			using (IDataReader rdr = CalendarEntry.GetEvent(EventID))
			{
				if (rdr.Read())
					_UserID = (int)rdr["ManagerId"];
			}

			foreach(DataRow dr in dt.Rows)
				if ((int)dr["PrincipalId"] == _UserID) isinres = true;

			if (!isinres)
			{
				DataRow dr = dt.NewRow();
			
				dr["PrincipalId"] = _UserID;
				dr["IsGroup"] = false;
				dr["MustBeConfirmed"] = false;
				dr["ResponsePending"] = false;

				dt.Rows.InsertAt(dr,0);
			}

			dgMembers.DataSource = dt;
			dgMembers.DataBind();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("EventParticipants"));
			if (CalendarEntry.CanUpdate(EventID))
			{
				// Resource Utilization
				if (dt.Rows.Count > 0)
				{
					ArrayList userList = new ArrayList();
					foreach (DataRow row in dt.Rows)
					{
						int principalId = (int)row["PrincipalId"];
						if (Mediachase.IBN.Business.User.IsGroup(principalId))
						{
							using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(principalId, false))
							{
								while (reader.Read())
								{
									if (!userList.Contains((int)reader["UserId"]))
									{
										userList.Add((int)reader["UserId"]);
									}
								}
							}
						}
						else
						{
							if (!userList.Contains(principalId))
							{
								userList.Add(principalId);
							}
						}
					}

					string users = String.Empty;
					foreach (int userId in userList)
					{
						if (!String.IsNullOrEmpty(users))
							users += ",";

						users += userId.ToString();
					}

					string text = String.Format(CultureInfo.InvariantCulture,
						"<img alt='' src='{0}'/> {1}",
						Page.ResolveUrl("~/Layouts/Images/ResUtil.png"),
						LocRM2.GetString("Utilization"));
					string link = String.Format(CultureInfo.InvariantCulture,
						"javascript:OpenPopUpNoScrollWindow('{0}?users={1}&amp;ObjectId={2}&amp;ObjectTypeId={3}',750,300)",
						Page.ResolveUrl("~/Common/ResourceUtilGraphForObject.aspx"),
						users,
						EventID,
						(int)ObjectTypes.CalendarEntry);
					secHeader.AddRightLink(text, link);
				}

				// Edit
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_EventParticipants");
				string cmd = cm.AddCommand("Event", "", "EventView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"), "javascript:" + cmd);
			}
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
			else return LocRM.GetString("Denied");
		}
		#endregion
	}
}
