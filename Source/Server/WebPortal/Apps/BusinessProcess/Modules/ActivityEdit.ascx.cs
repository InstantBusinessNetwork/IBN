using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Assignments.Schemas;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class ActivityEdit : System.Web.UI.UserControl
	{
		#region InstanceId
		protected PrimaryKeyId InstanceId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["InstanceId"]))
					retval = PrimaryKeyId.Parse(Request.QueryString["InstanceId"]);

				return retval;
			}
		}
		#endregion

		#region ParentActivityName
		protected string ParentActivityName
		{
			get
			{
				string retval = string.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["Parent"]))
					retval = Request.QueryString["Parent"];

				return retval;
			}
		}
		#endregion

		#region ActivityName
		protected string ActivityName
		{
			get
			{
				string retval = string.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["Activity"]))
					retval = Request.QueryString["Activity"];

				return retval;
			}
		}
		#endregion

		#region ControlName
		protected string ControlName
		{
			get
			{
				string retval = string.Empty;
				if (ViewState["ControlName"] != null)
					retval = (string)ViewState["ControlName"];
				return retval;
			}
			set
			{
				ViewState["ControlName"] = value;
			}
		}
		#endregion

		WorkflowInstanceEntity instance = null;
		SchemaMaster shemaMaster = null;
		object rootActivity = null;
		object activity = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (InstanceId != PrimaryKeyId.Empty)
			{
				instance = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, InstanceId);
				shemaMaster = SchemaManager.GetShemaMaster(instance.SchemaId);
				rootActivity = McWorkflowSerializer.GetObject(instance.Xaml);

				if (!String.IsNullOrEmpty(ActivityName))
					activity = WorkflowActivityWrapper.GetActivityByName(rootActivity, ActivityName);
			}

			if (!IsPostBack)
			{
				BindInfo();
				BindData();
			}

			LoadControlToPlaceHolder(!IsPostBack);
		}

		#region BindInfo
		private void BindInfo()
		{
			SaveButton.CustomImage = ResolveUrl("~/layouts/images/saveitem.gif");
			SaveButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			CancelButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();

			if (!String.IsNullOrEmpty(Request["closeFramePopup"]))
				CancelButton.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				CancelButton.Attributes.Add("onclick", "window.close();");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (activity == null)	// new
			{
				ActivityMaster[] list = shemaMaster.GetAllowedActivities(ParentActivityName);
				if (list != null && list.Length > 0)
				{
					foreach (ActivityMaster item in list)
					{
						ActivityTypeList.Items.Add(new ListItem(CHelper.GetResFileString(item.Description.Comment), item.Description.Name));
					}

					ActivityMaster currentActivityMaster = list[0];
					CHelper.SafeSelect(ActivityTypeList, currentActivityMaster.Description.Name);
					ControlName = currentActivityMaster.Description.UI.CreateControl;
				}
			}
			else // edit
			{
				ActivityMaster currentActivityMaster = WorkflowActivityWrapper.GetActivityMaster(shemaMaster, activity, ActivityName);

				ActivityTypeList.Items.Add(new ListItem(CHelper.GetResFileString(currentActivityMaster.Description.Comment), currentActivityMaster.Description.Name));
				ActivityTypeList.Enabled = false;

				ControlName = currentActivityMaster.Description.UI.EditControl;
			}
		}
		#endregion

		#region LoadControlToPlaceHolder
		private void LoadControlToPlaceHolder(bool bindData)
		{
			if (MainPlaceHolder.Controls.Count > 0)
				MainPlaceHolder.Controls.Clear();

			if (!String.IsNullOrEmpty(ControlName))
			{
				MCDataSavedControl control = (MCDataSavedControl)LoadControl(ControlName);
				MainPlaceHolder.Controls.Add(control);

				if (bindData)
				{
					control.DataItem = activity;
					control.DataBind();
				}
			}
		}
		#endregion

		#region ActivityTypeList_SelectedIndexChanged
		protected void ActivityTypeList_SelectedIndexChanged(object sender, EventArgs e)
		{
			ActivityMaster currentActivityMaster = shemaMaster.GetActivityMaster(ActivityTypeList.SelectedValue);
			ControlName = currentActivityMaster.Description.UI.CreateControl;

			LoadControlToPlaceHolder(true);
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			if (activity == null) // new
			{
				ActivityMaster currentActivityMaster = shemaMaster.GetActivityMaster(ActivityTypeList.SelectedValue);
				activity = currentActivityMaster.InstanceFactory.CreateInstance();

				if (MainPlaceHolder.Controls.Count > 0)
				{
					MCDataSavedControl control = (MCDataSavedControl)MainPlaceHolder.Controls[0];
					control.Save(activity);
				}

				object parentActivity = WorkflowActivityWrapper.GetActivityByName(rootActivity, ParentActivityName);
				WorkflowActivityWrapper.AddActivity(parentActivity, activity);
			}
			else // edit
			{
				if (MainPlaceHolder.Controls.Count > 0)
				{
					MCDataSavedControl control = (MCDataSavedControl)MainPlaceHolder.Controls[0];
					control.Save(activity);
				}
			}

			// Save data
			instance.Xaml = McWorkflowSerializer.GetString(rootActivity);
			BusinessManager.Update(instance);

			// Close popup
			if (!String.IsNullOrEmpty(Request["closeFramePopup"]))
			{
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, string.Empty, true);
			}
			else
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"<script language='javascript'>" +
					"try {window.opener.location.href=window.opener.location.href;}" +
					"catch (e){} window.close();</script>");
			}
		}
		#endregion
	}
}