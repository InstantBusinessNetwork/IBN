using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Assignments.Schemas;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class WorkFlowInstanceSchema : System.Web.UI.UserControl
	{
		#region ObjectId
		protected PrimaryKeyId ObjectId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["Id"]))
					retval = PrimaryKeyId.Parse(Request.QueryString["Id"]);

				return retval;
			}
		}
		#endregion

		WorkflowInstanceEntity instance = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ObjectId != PrimaryKeyId.Empty)
				instance = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, ObjectId);

			if (!IsPostBack)
				BindData();

			BindCommands();
			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			MainHeader.Title = GetGlobalResourceObject("IbnFramework.BusinessProcess", "WorkflowSchema").ToString();

			string link = GetOwnerLink();
			string text = CHelper.GetIconText(GetGlobalResourceObject("IbnFramework.Common", "Back").ToString(), ResolveClientUrl("~/Images/IbnFramework/cancel.GIF"));
			if (!String.IsNullOrEmpty(link))
				MainHeader.AddLink(text, link);
		}
		#endregion

		#region GetOwnerLink
		private string GetOwnerLink()
		{
			string retval = String.Empty;

			string ownerName = string.Empty;
			if (instance.OwnerDocumentId != null)
				ownerName = WorkflowInstanceEntity.FieldOwnerDocumentId;

			if (!String.IsNullOrEmpty(ownerName))
			{
				retval = CHelper.GetLinkObjectViewByOwnerName(ownerName, instance.OwnerDocumentId.ToString());
			}

			return retval;
		}
		#endregion

		#region BindCommands
		private void BindCommands()
		{
			Dictionary<string, string> dicNew = new Dictionary<string, string>();
			dicNew.Add("parentName", "_parentName_");
			CommandParameters cpNew = new CommandParameters("WFNewActivityPopup", dicNew);

			Dictionary<string, string> dicEdit = new Dictionary<string, string>();
			dicEdit.Add("activityName", "_activityName_");
			CommandParameters cpEdit = new CommandParameters("WFEditActivityPopup", dicEdit);

			string newActivity = CommandManager.GetCurrent(this.Page).AddCommand("WorkflowInstance", string.Empty, string.Empty, cpNew);
//			newActivity = newActivity.Replace("\"", "&quot;");

			string editActivity = CommandManager.GetCurrent(this.Page).AddCommand("WorkflowInstance", string.Empty, string.Empty, cpEdit);
//			editActivity = editActivity.Replace("\"", "&quot;");
		} 
		#endregion

		#region BindData
		private void BindData()
		{
			SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(instance.SchemaId);
			object rootActivity = McWorkflowSerializer.GetObject(instance.Xaml);

			WorkflowDescription wfDescription = new WorkflowDescription((Guid)instance.PrimaryKeyId.Value,
				instance.Name,
				currentShemaMaster,
				rootActivity);

			ActivityGrid.DataSource = WorkflowActivityWrapper.GetActivityList(wfDescription, rootActivity);
			ActivityGrid.DataBind();
		} 
		#endregion

		#region GetSubject
		protected string GetSubject(object subject, object isBlock, object state, object level)
		{
			int indent = 30;

			string cssClass = string.Empty;
			if ((bool)isBlock)
				cssClass = "block";

			if ((WorkflowActivityWrapper.ActivityStatus)state == WorkflowActivityWrapper.ActivityStatus.Executing)
			{
				if (!String.IsNullOrEmpty(cssClass))
					cssClass += " ";
				cssClass += "active";
			}
			else if ((WorkflowActivityWrapper.ActivityStatus)state != WorkflowActivityWrapper.ActivityStatus.Initialized)
			{
				if (!String.IsNullOrEmpty(cssClass))
					cssClass += " ";
				cssClass += "inactive";
			}
			if (!String.IsNullOrEmpty(cssClass))
				cssClass = String.Format(CultureInfo.InvariantCulture, " class=\"{0}\"", cssClass);

			string retval = String.Format(CultureInfo.InvariantCulture,
				"<span{0} style=\"padding-left:{1}px;\">{2}</span>",
				cssClass,
				(int)level * indent,
				CHelper.GetResFileString((string)Eval("Subject")));

			return retval;
		} 
		#endregion

		#region GetUserName
		protected string GetUserName(object user)
		{
			string retval = string.Empty;
			if (user != null && user != DBNull.Value)
				retval = CHelper.GetResFileString(CommonHelper.GetUserName((int)user));

			return retval;
		} 
		#endregion

		#region GetDueDate
		protected string GetDueDate(object dueDate)
		{
			string retval = string.Empty;
			if (dueDate != null && dueDate != DBNull.Value)
				retval = String.Concat(((DateTime)dueDate).ToShortDateString(), " ", ((DateTime)dueDate).ToShortTimeString());

			return retval;
		} 
		#endregion

		#region GetMenu
		protected string GetMenu(object id, object isBlock)
		{
			string retval = string.Empty;

			retval = string.Format(CultureInfo.InvariantCulture,
				"<a href=\"#\" onclick=\"openMenu('{0}', '{1}', this)\" title='{2}'><img src='{3}' width='16px' height='16px' border='0'></a>",
				id,
				(bool)isBlock,
				GetGlobalResourceObject("IbnFramework.BusinessProcess", "Insert"),
				ResolveClientUrl("~/layouts/images/Newitem.gif")
			);

			return retval;
		} 
		#endregion
	}
}