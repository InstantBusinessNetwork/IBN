using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class MultipleAdd : System.Web.UI.UserControl
	{
		private Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		protected int ProjectObjectType = (int)Mediachase.IBN.Business.ObjectTypes.Project;
		private string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;

		#region ViewName
		public string ViewName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["ViewName"] != null)
					retval = Request.QueryString["ViewName"];

				return retval;
			}
		}
		#endregion

		#region CurrentView
		private MetaView currentView;
		public MetaView CurrentView
		{
			get
			{
				if (currentView == null)
				{
					if (DataContext.Current.MetaModel.MetaViews[ViewName] == null)
						throw new ArgumentException(String.Format("Cant find meta view: {0}", ViewName));
					currentView = DataContext.Current.MetaModel.MetaViews[ViewName];
				}
				return currentView;
			}
		}
		#endregion

		#region BlockId
		public int BlockId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["BlockId"] != null)
				{
					int.TryParse(Request.QueryString["BlockId"], NumberStyles.Integer, CultureInfo.InvariantCulture, out retval);
				}
				return retval;
			}
		}
		#endregion

		#region blockInstanceId
		protected int blockInstanceId
		{
			get
			{
				int retval = -1;
				if (ViewState["blockInstanceId"] != null)
					retval = (int)ViewState["blockInstanceId"];
				return retval;
			}
			set
			{
				ViewState["blockInstanceId"] = value;
			}
		}
		#endregion

		#region ownerId
		protected int ownerId
		{
			get
			{
				int retval = -1;
				if (ViewState["ownerId"] != null)
					retval = (int)ViewState["ownerId"];
				return retval;
			}
			set
			{
				ViewState["ownerId"] = value;
			}
		}
		#endregion

		#region startDate
		protected DateTime startDate
		{
			get
			{
				DateTime retval = DateTime.MinValue;
				if (ViewState["startDate"] != null)
					retval = (DateTime)ViewState["startDate"];
				
				return retval;
			}
			set
			{
				ViewState["startDate"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ApplyStartValues();
				if (startDate != DateTime.MinValue)
				{
					DTCWeek.SelectedDate = startDate;
					DTCWeek.Enabled = false;
					WeekRow.Visible = false;
				}

				if (ownerId > 0)
				{
					BindSingleUser(ownerId);

					if (blockInstanceId > 0)
						BindSingleBlockTypeInstance(blockInstanceId);
					else
						BindBlockTypeInstancesByUser(ownerId);
				}
				else if (blockInstanceId > 0)
				{
					BindSingleBlockTypeInstance(blockInstanceId);
					BindUsersByBlockTypeInstance(blockInstanceId);
				}
				else
				{
					BindBlockTypeInstances();
					BindUsers();
				}

				BindGrid();
			}
		}

		#region ApplyStartValues
		private void ApplyStartValues()
		{
			if (BlockId > 0)
			{
				TimeTrackingBlock block = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), BlockId);
				if (block != null)
				{
					ownerId = block.OwnerId;
					startDate = block.StartDate;
					blockInstanceId = block.BlockTypeInstanceId;
					return;
				}
			}

			if (ViewName == "TT_MyGroupByWeekProject")	// My
			{
				ownerId = Mediachase.Ibn.Data.Services.Security.CurrentUserId;
			}
			else
			{
				int filterUser = GetUserFromFilter();
				if (filterUser > 0)
					ownerId = filterUser;
			}

			int filterInstance = GetInstanceFromFilter();
			if (filterInstance > 0)
				blockInstanceId = filterInstance;

			DateTime filterDate = GetStartDateFromFilter();
			if (filterDate != DateTime.MinValue)
				startDate = filterDate;
		} 
		#endregion

		#region BindSingleBlockTypeInstance
		private void BindSingleBlockTypeInstance(int blockTypeInstanceId)
		{
			TimeTrackingBlockTypeInstance blockTypeInstance = MetaObjectActivator.CreateInstance<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), blockTypeInstanceId);

			BlockInstanceList.Items.Add(new ListItem(blockTypeInstance.Properties[titledFieldName].Value.ToString(), blockTypeInstanceId.ToString()));

			BlockInstanceList.Enabled = false;	// lock
			ProjectRow.Visible = false;
		}
		#endregion

		#region BindBlockTypeInstancesByUser
		private void BindBlockTypeInstancesByUser(int userId)
		{
			string savedValue;
			if (!IsPostBack)
				savedValue = "0";
			else
				savedValue = BlockInstanceList.SelectedValue;

			BlockInstanceList.Items.Clear();

			DateTime startDate = DTCWeek.SelectedDate;
			//if (startDate == DateTime.MinValue)
			//    startDate = CHelper.GetWeekStartByDate(DTCWeek.Value);

			MetaClass mc = TimeTrackingBlockTypeInstance.GetAssignedMetaClass();
			// 1. Non-project
			#region 1.1. Preform the list of the Id (to pass it as array) and the Dictionary of TimeTrackingBlockTypeInstance
			List<int> idList = new List<int>();
			Dictionary<int, TimeTrackingBlockTypeInstance> blockTypeInstanceList = new Dictionary<int, TimeTrackingBlockTypeInstance>();
			foreach (TimeTrackingBlockTypeInstance blockTypeInstance in TimeTrackingManager.GetNonProjectBlockTypeInstances())
			{
				blockTypeInstanceList.Add(blockTypeInstance.PrimaryKeyId.Value, blockTypeInstance);
				idList.Add(blockTypeInstance.PrimaryKeyId.Value);
			}
			#endregion

			#region 1.2. Fillin the dropdown with elemets for which we have rights
			bool isHeaderAdded = false;
			SerializableDictionary<int, Collection<string>> objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(mc, idList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;
				if ((allowedRights.Contains(TimeTrackingManager.Right_AddMyTTBlock) && userId == Mediachase.Ibn.Data.Services.Security.CurrentUserId)
					|| allowedRights.Contains(TimeTrackingManager.Right_AddAnyTTBlock))
				{
					if (!isHeaderAdded)
					{
						BlockInstanceList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "NonProject").ToString(), "-1"));
						isHeaderAdded = true;
					}

					TimeTrackingBlockTypeInstance blockTypeInstance = blockTypeInstanceList[id];
					ListItem li = new ListItem("   " + blockTypeInstance.Properties[titledFieldName].Value.ToString(), id.ToString());
					BlockInstanceList.Items.Add(li);
				}
			}
			#endregion

			// 2. Projects
			#region 2.1. Preform the list of the Id (to pass it as array) and the Dictionary of TimeTrackingBlockTypeInstance
			idList = new List<int>();
			blockTypeInstanceList = new Dictionary<int, TimeTrackingBlockTypeInstance>();
			foreach (TimeTrackingBlockTypeInstance blockTypeInstance in TimeTrackingManager.GetProjectBlockTypeInstances())
			{
				blockTypeInstanceList.Add(blockTypeInstance.PrimaryKeyId.Value, blockTypeInstance);
				idList.Add(blockTypeInstance.PrimaryKeyId.Value);
			}
			#endregion

			#region 2.2. Fillin the dropdown with elemets for which we have rights
			isHeaderAdded = false;
			objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(mc, idList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;

				if ((allowedRights.Contains(TimeTrackingManager.Right_AddMyTTBlock) && userId == Mediachase.Ibn.Data.Services.Security.CurrentUserId)
					|| allowedRights.Contains(TimeTrackingManager.Right_AddAnyTTBlock))
				{
					if (!isHeaderAdded)
					{
						BlockInstanceList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ByProject").ToString(), "-2"));
						BlockInstanceList.Items.Add(new ListItem("   " + GetGlobalResourceObject("IbnFramework.TimeTracking", "AllProjects").ToString(), "0"));
						isHeaderAdded = true;
					}

					TimeTrackingBlockTypeInstance blockTypeInstance = blockTypeInstanceList[id];
					ListItem li = new ListItem("   " + blockTypeInstance.Properties[titledFieldName].Value.ToString(), id.ToString());
					BlockInstanceList.Items.Add(li);
				}
			}
			#endregion

			if (savedValue != null)
				CHelper.SafeSelect(BlockInstanceList, savedValue);
		}
		#endregion

		#region BindBlockTypeInstances
		private void BindBlockTypeInstances()
		{
			MetaClass mc = TimeTrackingBlockTypeInstance.GetAssignedMetaClass();

			// 1. Non-project
			#region 1.1. Preform the list of the Id (to pass it as array) and the Dictionary of TimeTrackingBlockTypeInstance
			List<int> idList = new List<int>();
			Dictionary<int, TimeTrackingBlockTypeInstance> blockTypeInstanceList = new Dictionary<int, TimeTrackingBlockTypeInstance>();
			foreach (TimeTrackingBlockTypeInstance blockTypeInstance in TimeTrackingManager.GetNonProjectBlockTypeInstances())
			{
				blockTypeInstanceList.Add(blockTypeInstance.PrimaryKeyId.Value, blockTypeInstance);
				idList.Add(blockTypeInstance.PrimaryKeyId.Value);
			}
			#endregion

			#region 1.2. Fillin the dropdown with elemets for which we have rights
			bool isHeaderAdded = false;
			SerializableDictionary<int, Collection<string>> objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(mc, idList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;
				if (allowedRights.Contains(TimeTrackingManager.Right_AddMyTTBlock) || allowedRights.Contains(TimeTrackingManager.Right_AddAnyTTBlock))
				{
					if (!isHeaderAdded)
					{
						BlockInstanceList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "NonProject").ToString(), "-1"));
						isHeaderAdded = true;
					}

					TimeTrackingBlockTypeInstance blockTypeInstance = blockTypeInstanceList[id];
					ListItem li = new ListItem("   " + blockTypeInstance.Properties[titledFieldName].Value.ToString(), id.ToString());
					BlockInstanceList.Items.Add(li);
				}
			}
			#endregion

			// 2. Projects
			#region 2.1. Preform the list of the Id (to pass it as array) and the Dictionary of TimeTrackingBlockTypeInstance
			idList = new List<int>();
			blockTypeInstanceList = new Dictionary<int, TimeTrackingBlockTypeInstance>();
			foreach (TimeTrackingBlockTypeInstance blockTypeInstance in TimeTrackingManager.GetProjectBlockTypeInstances())
			{
				blockTypeInstanceList.Add(blockTypeInstance.PrimaryKeyId.Value, blockTypeInstance);
				idList.Add(blockTypeInstance.PrimaryKeyId.Value);
			}
			#endregion

			#region 2.2. Fillin the dropdown with elemets for which we have rights
			isHeaderAdded = false;
			objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(mc, idList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;

				if (allowedRights.Contains(TimeTrackingManager.Right_AddMyTTBlock) || allowedRights.Contains(TimeTrackingManager.Right_AddAnyTTBlock))
				{
					if (!isHeaderAdded)
					{
						BlockInstanceList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ByProject").ToString(), "-2"));
						BlockInstanceList.Items.Add(new ListItem("   " + GetGlobalResourceObject("IbnFramework.TimeTracking", "AllProjects").ToString(), "0"));
						isHeaderAdded = true;
					}

					TimeTrackingBlockTypeInstance blockTypeInstance = blockTypeInstanceList[id];
					ListItem li = new ListItem("   " + blockTypeInstance.Properties[titledFieldName].Value.ToString(), id.ToString());
					BlockInstanceList.Items.Add(li);
				}
			}
			#endregion

			CHelper.SafeSelect(BlockInstanceList, "0");	// All Projects
		}
		#endregion


		#region BindSingleUser
		private void BindSingleUser(int userId)
		{
			Mediachase.IBN.Business.UserLight usr = Mediachase.IBN.Business.UserLight.Load(userId);
			UserList.Items.Add(new ListItem(usr.LastName + ", " + usr.FirstName, usr.UserID.ToString()));

			UserList.Enabled = false;		// lock
			UserRow.Visible = false;
		}
		#endregion

		#region BindUsersByBlockTypeInstance
		private void BindUsersByBlockTypeInstance(int blockTypeInstanceId)
		{
			string savedValue;
			if (!IsPostBack)
				savedValue = Mediachase.IBN.Business.Security.CurrentUser.UserID.ToString();
			else
				savedValue = UserList.SelectedValue;

			DateTime startDate = DTCWeek.SelectedDate;
			//if (startDate == DateTime.MinValue)
			//    startDate = CHelper.GetWeekStartByDate(DTCWeek.Value);

			UserList.Items.Clear();

			TimeTrackingBlockTypeInstance inst = MetaObjectActivator.CreateInstance<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), blockTypeInstanceId);

			if (Mediachase.Ibn.Data.Services.Security.CheckObjectRight(inst, TimeTrackingManager.Right_AddAnyTTBlock))
			{
				#region 1. Make the Dictionary of Principal
				Dictionary<int, string> allUsers = new Dictionary<int, string>();
				Principal[] principals = Principal.List(new FilterElementCollection(FilterElement.EqualElement("Card", "User"), FilterElement.EqualElement("Activity", 3)), new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc)));
				foreach (Principal p in principals)
				{
					allUsers.Add(p.PrimaryKeyId.Value, p.Name);
				}
				#endregion

				#region 2. Make the list of the Id (to pass it as array) and the Dictionary of TimeTrackingBlock
				List<int> idList = new List<int>();
				Dictionary<int, TimeTrackingBlock> allblocks = new Dictionary<int, TimeTrackingBlock>();
				TimeTrackingBlock[] blocks = TimeTrackingBlock.List(FilterElement.EqualElement("StartDate", startDate), FilterElement.EqualElement("BlockTypeInstanceId", blockTypeInstanceId));
				foreach (TimeTrackingBlock block in blocks)
				{
					idList.Add(block.PrimaryKeyId.Value);
					allblocks.Add(block.PrimaryKeyId.Value, block);
				}
				#endregion

				#region 3. Get the list of the existing blocks with rights and remove the forbidden items from allUsers
				SerializableDictionary<int, Collection<string>> objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(TimeTrackingBlock.GetAssignedMetaClass(), idList.ToArray());
				foreach (KeyValuePair<int, Collection<string>> item in objectRights)
				{
					int id = item.Key;
					Collection<string> allowedRights = item.Value;

					TimeTrackingBlock block = allblocks[id];
					int ownerId = block.OwnerId;

					if (!allowedRights.Contains(Mediachase.Ibn.Data.Services.Security.RightWrite))
					{
						allUsers.Remove(ownerId);
					}
				}
				#endregion

				#region 4. Fill in the dropdown
				foreach (KeyValuePair<int, string> item in allUsers)
				{
					UserList.Items.Add(new ListItem(item.Value, item.Key.ToString()));
				}
				#endregion
			}
			else
			{
				Mediachase.IBN.Business.UserLight usr = Mediachase.IBN.Business.Security.CurrentUser;
				UserList.Items.Add(new ListItem(usr.LastName + ", " + usr.FirstName, usr.UserID.ToString()));
			}

			if (savedValue != null)
				CHelper.SafeSelect(UserList, savedValue);
		}
		#endregion

		#region BindUsers
		private void BindUsers()
		{
			EnsureSelectInstance();

			string savedValue;
			if (!IsPostBack)
				savedValue = Mediachase.IBN.Business.Security.CurrentUser.UserID.ToString();
			else
				savedValue = UserList.SelectedValue;

			UserList.Items.Clear();

			if (BlockInstanceList.SelectedValue != "0")	// 0 = All Projects
			{
				TimeTrackingBlockTypeInstance inst = MetaObjectActivator.CreateInstance<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), int.Parse(BlockInstanceList.SelectedValue, CultureInfo.InvariantCulture));

				if (Mediachase.Ibn.Data.Services.Security.CheckObjectRight(inst, TimeTrackingManager.Right_AddAnyTTBlock))
				{
					Principal[] mas = Principal.List(new FilterElementCollection(FilterElement.EqualElement("Card", "User"), FilterElement.EqualElement("Activity", 3)), new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc)));
					foreach (Principal pl in mas)
						UserList.Items.Add(new ListItem(pl.Name, pl.PrimaryKeyId.ToString()));
				}
			}

			if (UserList.Items.Count == 0)
			{
				Mediachase.IBN.Business.UserLight usr = Mediachase.IBN.Business.Security.CurrentUser;
				UserList.Items.Add(new ListItem(usr.LastName + ", " + usr.FirstName, usr.UserID.ToString()));
			}

			if (savedValue != null)
				CHelper.SafeSelect(UserList, savedValue);
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			EnsureSelectInstance();

			if (BlockInstanceList.Items.Count > 0 && UserList.Items.Count > 0)
			{
				MainGrid.Visible = true;

				int instanceId = int.Parse(BlockInstanceList.SelectedValue, CultureInfo.InvariantCulture);
				int userId = int.Parse(UserList.SelectedValue, CultureInfo.InvariantCulture);
				DateTime startDate = DTCWeek.SelectedDate;
				//if (startDate == DateTime.MinValue)
				//    startDate = CHelper.GetWeekStartByDate(DTCWeek.Value);

				if (instanceId > 0)
				{
					TimeTrackingBlock block = TimeTrackingManager.GetTimeTrackingBlock(instanceId, startDate, userId);
					if (block == null || Mediachase.Ibn.Data.Services.Security.CanWrite(block))
					{
						MainGrid.DataSource = Mediachase.IBN.Business.TimeTracking.GetListTimeTrackingItemsForAdd_DataTable(instanceId, startDate, userId);
						MainGrid.DataBind();
					}
					else
					{
						MainGrid.Visible = false;
					}
				}
				else	// All Projects
				{
					BindAllProjectsGrid(startDate, userId);
				}
			}
			else
			{
				MainGrid.Visible = false;
			}

			foreach (DataGridItem dgi in MainGrid.Items)
			{
				if (dgi.Cells[1].Text == ProjectObjectType.ToString(CultureInfo.InvariantCulture))
				{
					dgi.BackColor = Color.FromArgb(240, 240, 240);
					dgi.Font.Bold = true;

					// Add the OnClick handler
					CheckBox cb = (CheckBox)dgi.FindControl("chkElement");
					if (cb != null)
						cb.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "CheckChildren(this, {0})", dgi.Cells[2].Text));
				}
			}
		}
		#endregion

		#region BindAllProjectsGrid
		private void BindAllProjectsGrid(DateTime startDate, int userId)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ObjectId", typeof(int)));
			dt.Columns.Add(new DataColumn("ObjectTypeId", typeof(int)));
			dt.Columns.Add(new DataColumn("ObjectName", typeof(string)));
			dt.Columns.Add(new DataColumn("BlockTypeInstanceId", typeof(int)));
			DataRow dr;

			#region 1. Make the list of all BlockTypeInstances
			Dictionary<int, string> allList = new Dictionary<int, string>();
			bool isProject = false;
			foreach (ListItem li in BlockInstanceList.Items)
			{
				if (!isProject)
				{
					// Check that we have reached the [ All Projects ]
					if (li.Value == "0")
						isProject = true;
					continue;
				}
				allList.Add(int.Parse(li.Value, CultureInfo.InvariantCulture), li.Text);
			}
			#endregion

			#region 2. Get the list of the existing blocks by StartDate and OwnerId
			List<int> idList = new List<int>();
			Dictionary<int, TimeTrackingBlock> existingBlocks = new Dictionary<int, TimeTrackingBlock>();
			foreach (TimeTrackingBlock block in TimeTrackingBlock.List(FilterElement.EqualElement("StartDate", startDate), FilterElement.EqualElement("OwnerId", userId), FilterElement.IsNotNullElement("ProjectId")))
			{
				idList.Add(block.PrimaryKeyId.Value);
				existingBlocks.Add(block.PrimaryKeyId.Value, block);
			}
			#endregion

			#region 3. Get the security info by existing blocks and remove the forbidden items from allList
			SerializableDictionary<int, Collection<string>> objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(TimeTrackingBlock.GetAssignedMetaClass(), idList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;

				TimeTrackingBlock block = existingBlocks[id];
				if (!allowedRights.Contains(Mediachase.Ibn.Data.Services.Security.RightWrite))
				{
					allList.Remove(block.BlockTypeInstanceId);
				}
			}
			#endregion

			#region 4. Fill in the DataTable
			foreach (KeyValuePair<int, string> item in allList)
			{
				bool isHeaderAdded = false;
				int instanceId = item.Key;
				string instanceName = item.Value;
				using (IDataReader reader = Mediachase.IBN.Business.TimeTracking.GetListTimeTrackingItemsForAdd(instanceId, startDate, userId))
				{
					while (reader.Read())
					{
						if (!isHeaderAdded)
						{
							dr = dt.NewRow();
							dr["ObjectId"] = instanceId;
							dr["ObjectTypeId"] = ProjectObjectType;
							dr["ObjectName"] = instanceName;
							dr["BlockTypeInstanceId"] = instanceId;
							dt.Rows.Add(dr);

							isHeaderAdded = true;
						}

						dr = dt.NewRow();
						dr["ObjectId"] = reader["ObjectId"];
						dr["ObjectTypeId"] = reader["ObjectTypeId"];
						dr["ObjectName"] = reader["ObjectName"];
						dr["BlockTypeInstanceId"] = reader["BlockTypeInstanceId"];
						dt.Rows.Add(dr);
					}
				}
			}
			#endregion

			MainGrid.DataSource = dt.DefaultView;
			MainGrid.DataBind();
		}
		#endregion

		#region BlockInstanceList_SelectedIndexChanged
		protected void BlockInstanceList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (UserList.Enabled)
			{
				EnsureSelectInstance();
				int selectedId = int.Parse(BlockInstanceList.SelectedValue, CultureInfo.InvariantCulture);
				if (selectedId > 0)
				{
					BindUsersByBlockTypeInstance(selectedId);
				}
				else
				{
					BindUsers();
				}
			}
			BindGrid();
		}
		#endregion

		#region UserList_SelectedIndexChanged
		protected void UserList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion

		#region AddButton_Click
		protected void AddButton_Click(object sender, EventArgs e)
		{
			List<int> objects = new List<int>();
			List<int> objectTypes = new List<int>();
			List<string> titles = new List<string>();
			List<int> blockTypeInstances = new List<int>();

			foreach (DataGridItem dgi in MainGrid.Items)
			{
				CheckBox cb = (CheckBox)dgi.FindControl("chkElement");
				if (cb != null && cb.Checked && dgi.Cells[1].Text != ProjectObjectType.ToString(CultureInfo.InvariantCulture))
				{
					objects.Add(int.Parse(dgi.Cells[0].Text, CultureInfo.InvariantCulture));
					objectTypes.Add(int.Parse(dgi.Cells[1].Text, CultureInfo.InvariantCulture));
					titles.Add(cb.Text);
					blockTypeInstances.Add(int.Parse(dgi.Cells[2].Text, CultureInfo.InvariantCulture));
				}
			}

			if (objects.Count > 0)
			{
				DateTime startDate = DTCWeek.SelectedDate;
				//if (startDate == DateTime.MinValue)
				//    startDate = CHelper.GetWeekStartByDate(DTCWeek.Value);

				if (BlockInstanceList.SelectedValue != "0")
				{
					TimeTrackingManager.AddEntries(
						int.Parse(BlockInstanceList.SelectedValue, CultureInfo.InvariantCulture),
						startDate,
						int.Parse(UserList.SelectedValue, CultureInfo.InvariantCulture),
						objects, objectTypes, titles);
				}
				else  // Multiple projects
				{
					TimeTrackingManager.AddEntries(
						startDate,
						int.Parse(UserList.SelectedValue, CultureInfo.InvariantCulture),
						objects, objectTypes, titles, blockTypeInstances);
				}
			}

			// After rebind the selected items will be disappeared.
			BindGrid();
			
			// Refresh parent window
			CommandParameters cp = new CommandParameters("MC_TimeTracking_MultipleAddFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentFromFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (IsPostBack && Request["__EVENTTARGET"] == DTCWeek.UniqueID)
			{
				if (ownerId > 0)
				{
					if (blockInstanceId < 0)
						BindBlockTypeInstancesByUser(ownerId);
				}
				else if (blockInstanceId > 0)
				{
					BindUsersByBlockTypeInstance(blockInstanceId);
				}

				BindGrid();
			}

			if (BlockInstanceList.Items.Count == 0)
			{
				NoItemsLabel.Visible = true;
				BlockInstanceList.Visible = false;
			}
			else
			{
				NoItemsLabel.Visible = false;
				BlockInstanceList.Visible = true;
			}
			lblWeek.Text = String.Format("{1} - {2} (#{0})", 
				Iso8601WeekNumber.GetWeekNumber(startDate), 
				CHelper.GetWeekStartByDate(startDate).ToString("d MMM yyyy"),
				CHelper.GetWeekEndByDate(startDate).ToString("d MMM yyyy"));
		}
		#endregion

		#region EnsureSelectInstance
		private void EnsureSelectInstance()
		{
			if (BlockInstanceList.Items.Count > 0)
			{
				int selectedValue = int.Parse(BlockInstanceList.SelectedValue, CultureInfo.InvariantCulture);
				if (selectedValue < 0)
					BlockInstanceList.SelectedIndex = BlockInstanceList.SelectedIndex + 1;
			}
		}
		#endregion

		#region GetInstanceFromFilter
		private int GetInstanceFromFilter()
		{
			int retval = -1;
			McMetaViewPreference prefs = CHelper.GetMetaViewPreference(CurrentView);
			if (prefs != null && prefs.Filters != null)
			{
				foreach (FilterElement filter in prefs.Filters.GetListBySource("blocktypeinstanceid"))
				{
					if (filter.Value != null)
					{
						retval = (int)filter.Value;
						break;
					}
				}
			}
			return retval;
		}
		#endregion

		#region GetUserFromFilter
		private int GetUserFromFilter()
		{
			int retval = -1;
			McMetaViewPreference prefs = CHelper.GetMetaViewPreference(CurrentView);
			if (prefs != null && prefs.Filters != null)
			{
				foreach (FilterElement filter in prefs.Filters.GetListBySource("ownerid"))
				{
					if (filter.Value != null)
					{
						retval = (int)filter.Value;
						break;
					}
				}
			}
			return retval;
		}
		#endregion

		#region GetStartDateFromFilter
		private DateTime GetStartDateFromFilter()
		{
			DateTime retval = DateTime.MinValue;
			McMetaViewPreference prefs = CHelper.GetMetaViewPreference(CurrentView);
			if (prefs != null && prefs.Filters != null)
			{
				foreach (FilterElement filter in prefs.Filters.GetListBySource("startdate"))
				{
					if (filter.Value != null)
					{
						retval = (DateTime)filter.Value;
						break;
					}
				}
			}
			return retval;
		}
		#endregion

		#region GetText
		protected string GetText(string title, int objectTypeId, int objectId)
		{
			string retval = title;
			if (objectTypeId == (int)ObjectTypes.Task
				|| objectTypeId == (int)ObjectTypes.ToDo
				|| objectTypeId == (int)ObjectTypes.CalendarEntry
				|| objectTypeId == (int)ObjectTypes.Issue
				|| objectTypeId == (int)ObjectTypes.Document)
				retval += String.Concat(" (#", objectId, ")");
			return retval;
		}
		#endregion

		#region CloseButton_Click
		protected void CloseButton_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters("MC_TimeTracking_MultipleAddFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}