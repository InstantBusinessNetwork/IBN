using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Bus = Mediachase.IBN.Business;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class MyAssignmentsByObject : System.Web.UI.UserControl
	{
		#region AssignmentId
		protected PrimaryKeyId AssignmentId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["AssignmentId"]))
					retval = PrimaryKeyId.Parse(Request.QueryString["AssignmentId"]);
				return retval;
			}
		}
		#endregion

		#region OwnerType
		protected string OwnerType
		{
			get
			{
				string retval = String.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["DocumentId"]))
					retval = AssignmentEntity.FieldOwnerDocumentId;

				return retval;
			}
		}
		#endregion

		#region OwnerId
		protected int OwnerId
		{
			get
			{
				int retval = -1;
				if (!String.IsNullOrEmpty(Request.QueryString["DocumentId"]))
					retval = int.Parse(Request.QueryString["DocumentId"]);

				return retval;
			}
		}
		#endregion

		#region SavedPath
		protected string SavedPath
		{
			get
			{
				string retval = string.Empty;
				if (ViewState["SavedPath"] != null)
					retval = (string)ViewState["SavedPath"];
				
				return retval;
			}
			set
			{
				ViewState["SavedPath"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(OwnerType) || OwnerId < 0)
			{
				this.Visible = false;
				return;
			}

			// We do it to ensure raise Page_PreRender, becase Page_PreRender doesn't work if Visible=false
			this.Visible = true;

			if (!IsPostBack)
			{
				BindData();
			}
			else
			{
				LoadAssignment();
			}
		}

		#region BindList
		private int BindList()
		{
			int itemsCount = 0;

			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(FilterElement.EqualElement(OwnerType, OwnerId));
			filters.Add(FilterElement.EqualElement(AssignmentEntity.FieldUserId, Bus.Security.CurrentUser.UserID));
			filters.Add(FilterElement.EqualElement(AssignmentEntity.FieldState, AssignmentState.Active));
//			if (AssignmentId != PrimaryKeyId.Empty)
//				filters.Add(FilterElement.NotEqualElement("PrimaryKeyId", AssignmentId));

			EntityObject[] assignments = BusinessManager.List(AssignmentEntity.ClassName, filters.ToArray());

			if (assignments != null && assignments.Length > 0)
			{
				itemsCount = assignments.Length;

				AssignmentGrid.DataSource = assignments;
				AssignmentGrid.DataBind();
			}

			return itemsCount;
		}
		#endregion

		#region BindAssignment
		private int BindAssignment()
		{
			int itemsCount = 0;

			if (AssignmentId != PrimaryKeyId.Empty)
			{
				FilterElementCollection filters = new FilterElementCollection();
				filters.Add(FilterElement.EqualElement(OwnerType, OwnerId));
				filters.Add(FilterElement.EqualElement(AssignmentEntity.FieldUserId, Bus.Security.CurrentUser.UserID));
				filters.Add(FilterElement.EqualElement(AssignmentEntity.FieldState, AssignmentState.Active));
				filters.Add(FilterElement.EqualElement("PrimaryKeyId", AssignmentId));

				EntityObject[] assignments = BusinessManager.List(AssignmentEntity.ClassName, filters.ToArray());
				if (assignments != null && assignments.Length > 0)
				{
					AssignmentEntity assignment = (AssignmentEntity)assignments[0];

					SavedPath = AssignmentEntity.GetControlPath(assignment);

					MCDataBoundControl control = (MCDataBoundControl)LoadAssignment();
					if (control != null)
					{
						control.DataItem = assignment;
						control.DataBind();
					}

					itemsCount = 1;
				}
				else
				{
					SavedPath = String.Empty;
					MainPlaceHolder.Controls.Clear();
					itemsCount = 0;
				}
			}

			return itemsCount;
		}
		#endregion

		#region LoadAssignment
		private Control LoadAssignment()
		{
			Control control = null;

			if (MainPlaceHolder.Controls.Count > 0)
				MainPlaceHolder.Controls.Clear();

			if (!String.IsNullOrEmpty(SavedPath))
			{
				control = LoadControl(SavedPath);
				MainPlaceHolder.Controls.Add(control);

				((IItemCommand)control).ItemCommand += new ItemCommandEventHandler(MyAssignmentsByObject_ItemCommand);
			}
			return control;
		}
		#endregion

		#region GetDateString
		protected string GetDateString(object obj)
		{
			string retval = string.Empty;
			if (obj != null && obj != DBNull.Value)
			{
				DateTime dt = (DateTime)obj;
				retval = String.Concat(dt.ToShortDateString(), " ", dt.ToShortTimeString());
			}
			return retval;
		}
		#endregion

		#region MyAssignmentsByObject_ItemCommand
		void MyAssignmentsByObject_ItemCommand(object sender, EventArgs e)
		{
			CHelper.AddToContext("RebindAssignments", true);
		} 
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			object rebindObj = CHelper.GetFromContext("RebindAssignments");
			if (rebindObj != null && (bool)rebindObj)
			{
				BindData();
			}
			// Check visibility
			else if (IsPostBack && AssignmentGrid.Rows.Count == 0 && String.IsNullOrEmpty(SavedPath))
			{
				this.Visible = false;
			}
		}
		#endregion

		#region BindData()
		private void BindData()
		{
			int listItems = BindList();
			int assignmentItems = BindAssignment();
			if (listItems + assignmentItems == 0)
				this.Visible = false;
			else if (listItems > 0 && assignmentItems > 0)
				DelimiterRow.Visible = true;
			else
				DelimiterRow.Visible = false;
		}
		#endregion

	}
}