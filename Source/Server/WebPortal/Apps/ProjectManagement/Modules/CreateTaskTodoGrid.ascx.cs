using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI;
using System.Collections;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Apps.ProjectManagement.Modules
{
	public partial class CreateTaskTodoGrid : System.Web.UI.UserControl
	{
		#region ProjectId
		protected int ProjectId
		{
			get
			{
				int retVal = -1;
				if (Request["ProjectId"] != null && int.TryParse(Request["ProjectId"], out retVal))
					return retVal;
				return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			BindButtons();

			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region BindButtons
		private void BindButtons()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Common", "tSave").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/Layouts/images/saveitem.gif");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Common", "tClose").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/images/cancel.gif");
			btnCancel.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));

			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (Mediachase.IBN.Business.ToDo.CanCreate(ProjectId))
				TypeList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Task", "ToDoType").ToString(), (ObjectTypes.ToDo).ToString()));
			if (Mediachase.IBN.Business.Task.CanCreate(ProjectId))
				TypeList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Task", "TaskType").ToString(), (ObjectTypes.Task).ToString()));
			if (TypeList.Items.Count > 0)
				TypeList.Items[0].Selected = true;

			// Priority
			using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetListPriorities())
			{
				while (reader.Read())
				{
					string text = CHelper.GetResFileString(reader["PriorityName"].ToString());
					string value = reader["PriorityId"].ToString();
					PriorityList1.Items.Add(new ListItem(text, value));
					PriorityList2.Items.Add(new ListItem(text, value));
					PriorityList3.Items.Add(new ListItem(text, value));
					PriorityList4.Items.Add(new ListItem(text, value));
					PriorityList5.Items.Add(new ListItem(text, value));
					PriorityList6.Items.Add(new ListItem(text, value));
					PriorityList7.Items.Add(new ListItem(text, value));
					PriorityList8.Items.Add(new ListItem(text, value));
				}
			}
			string normalPriority = ((int)Priority.Normal).ToString();
			CHelper.SafeSelect(PriorityList1, normalPriority);
			CHelper.SafeSelect(PriorityList2, normalPriority);
			CHelper.SafeSelect(PriorityList3, normalPriority);
			CHelper.SafeSelect(PriorityList4, normalPriority);
			CHelper.SafeSelect(PriorityList5, normalPriority);
			CHelper.SafeSelect(PriorityList6, normalPriority);
			CHelper.SafeSelect(PriorityList7, normalPriority);
			CHelper.SafeSelect(PriorityList8, normalPriority);

			// Resources
			// UserId, FirstName, LastName, IsExternal
			using (IDataReader reader = Project.GetListTeamMemberNamesWithManager(ProjectId))
			{
				while (reader.Read())
				{
					string text = String.Concat(reader["LastName"].ToString(), " ", reader["FirstName"].ToString());
					string value = reader["UserId"].ToString();
					ResourcesList1.Items.Add(new ListItem(text, value));
					ResourcesList2.Items.Add(new ListItem(text, value));
					ResourcesList3.Items.Add(new ListItem(text, value));
					ResourcesList4.Items.Add(new ListItem(text, value));
					ResourcesList5.Items.Add(new ListItem(text, value));
					ResourcesList6.Items.Add(new ListItem(text, value));
					ResourcesList7.Items.Add(new ListItem(text, value));
					ResourcesList8.Items.Add(new ListItem(text, value));
				}
			}
			string currentUserId = Security.UserID.ToString();
			CHelper.SafeSelect(ResourcesList1, currentUserId);
			CHelper.SafeSelect(ResourcesList2, currentUserId);
			CHelper.SafeSelect(ResourcesList3, currentUserId);
			CHelper.SafeSelect(ResourcesList4, currentUserId);
			CHelper.SafeSelect(ResourcesList5, currentUserId);
			CHelper.SafeSelect(ResourcesList6, currentUserId);
			CHelper.SafeSelect(ResourcesList7, currentUserId);
			CHelper.SafeSelect(ResourcesList8, currentUserId);

			// Dates
			DateTime dt = DateTime.Today.AddHours(DateTime.UtcNow.Hour + 1);
			DateTime startDate = User.GetLocalDate(Security.CurrentUser.TimeZoneId, dt);
			DateTime finishDate = startDate.AddDays(1);

			StartDate1.SelectedDate = StartDate2.SelectedDate = StartDate3.SelectedDate = StartDate4.SelectedDate = StartDate5.SelectedDate = StartDate6.SelectedDate = StartDate7.SelectedDate = StartDate8.SelectedDate = startDate;
			FinishDate1.SelectedDate = FinishDate2.SelectedDate = FinishDate3.SelectedDate = FinishDate4.SelectedDate = FinishDate5.SelectedDate = FinishDate6.SelectedDate = FinishDate7.SelectedDate = FinishDate8.SelectedDate = finishDate;
		} 
		#endregion

		#region btnSave_ServerClick
		void btnSave_ServerClick(object sender, EventArgs e)
		{
			ProcessItem(Title1.Text.Trim(), StartDate1.SelectedDate, FinishDate1.SelectedDate, int.Parse(PriorityList1.SelectedValue), int.Parse(ResourcesList1.SelectedValue));
			ProcessItem(Title2.Text.Trim(), StartDate2.SelectedDate, FinishDate2.SelectedDate, int.Parse(PriorityList2.SelectedValue), int.Parse(ResourcesList2.SelectedValue));
			ProcessItem(Title3.Text.Trim(), StartDate3.SelectedDate, FinishDate3.SelectedDate, int.Parse(PriorityList3.SelectedValue), int.Parse(ResourcesList3.SelectedValue));
			ProcessItem(Title4.Text.Trim(), StartDate4.SelectedDate, FinishDate4.SelectedDate, int.Parse(PriorityList4.SelectedValue), int.Parse(ResourcesList4.SelectedValue));
			ProcessItem(Title5.Text.Trim(), StartDate5.SelectedDate, FinishDate5.SelectedDate, int.Parse(PriorityList5.SelectedValue), int.Parse(ResourcesList5.SelectedValue));
			ProcessItem(Title6.Text.Trim(), StartDate6.SelectedDate, FinishDate6.SelectedDate, int.Parse(PriorityList6.SelectedValue), int.Parse(ResourcesList6.SelectedValue));
			ProcessItem(Title7.Text.Trim(), StartDate7.SelectedDate, FinishDate7.SelectedDate, int.Parse(PriorityList7.SelectedValue), int.Parse(ResourcesList7.SelectedValue));
			ProcessItem(Title8.Text.Trim(), StartDate8.SelectedDate, FinishDate8.SelectedDate, int.Parse(PriorityList8.SelectedValue), int.Parse(ResourcesList8.SelectedValue));

			string cmd = String.Empty;
			if (Request["CommandName"] != null)
			{
				CommandParameters cp = new CommandParameters(Request["CommandName"]);
				cmd = cp.ToString();
			}

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cmd, true);
		}
		#endregion

		#region ProcessItem
		private void ProcessItem(string title, DateTime startDate, DateTime finishDate, int priorityId, int userId)
		{
			if (!String.IsNullOrEmpty(title))
			{
				ArrayList resources = new ArrayList();
				resources.Add(userId);

				if (TypeList.SelectedValue == (ObjectTypes.ToDo).ToString())
				{
					// ToDo
					PrimaryKeyId org_id = PrimaryKeyId.Empty;
					PrimaryKeyId contact_id = PrimaryKeyId.Empty;
					Mediachase.IBN.Business.Common.GetDefaultClient(PortalConfig.ToDoDefaultValueClientField, out contact_id, out org_id);
					Mediachase.IBN.Business.ToDo.Create(ProjectId, Security.CurrentUser.UserID, title, String.Empty, startDate, finishDate, priorityId,
						int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField),
						int.Parse(PortalConfig.ToDoDefaultValueCompetionTypeField),
						bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField),
						int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField),
						Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField),
						null, null, resources, contact_id, org_id);
				}
				else
				{
					// Task
					if (startDate != DateTime.MinValue && finishDate != DateTime.MinValue && startDate <= finishDate)
					{
						Task.Create(ProjectId, title, String.Empty, startDate, finishDate, priorityId, startDate == finishDate, 
							int.Parse(PortalConfig.TaskDefaultValueActivationTypeField),
							int.Parse(PortalConfig.TaskDefaultValueCompetionTypeField),
							bool.Parse(PortalConfig.TaskDefaultValueMustConfirmField),
							Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.TaskDefaultValueGeneralCategoriesField),
							resources, false, -1,
							int.Parse(PortalConfig.TaskDefaultValueTaskTimeField));
					}
				}
			}
		} 
		#endregion
	}
}