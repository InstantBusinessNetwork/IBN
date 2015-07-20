using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class WorkFlowInstanceDesign : System.Web.UI.UserControl
	{
		#region prop: wfName
		/// <summary>
		/// Gets the name of the wf.
		/// </summary>
		/// <value>The name of the wf.</value>
		protected string wfName
		{
			get
			{
				if (ViewState["wfName"] == null)
				{
					EntityObject eo = null;
					try
					{
						eo = BusinessManager.Load(WorkflowInstanceEntity.ClassName, PrimaryKeyId.Parse(Request["Id"]));
					}
					catch
					{
					}
					if (eo == null)
					{
						try
						{
							eo = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, PrimaryKeyId.Parse(Request["Id"]));
						}
						catch
						{
						}
					}
					ViewState["wfName"] = eo.Properties["Name"].Value.ToString();
				}

				return ViewState["wfName"].ToString();
			}
		} 
		#endregion

		#region prop: wfState
		/// <summary>
		/// Gets the state of the wf.
		/// </summary>
		/// <value>The state of the wf.</value>
		protected string wfState
		{
			get
			{
				if (ViewState["wfState"] == null)
				{
					EntityObject eo = null;
					try
					{
						eo = BusinessManager.Load(WorkflowInstanceEntity.ClassName, PrimaryKeyId.Parse(Request["Id"]));
					}
					catch
					{
					}
					if (eo == null)
					{
						try
						{
							eo = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, PrimaryKeyId.Parse(Request["Id"]));
						}
						catch
						{
						}
					}
					if (eo.Properties["State"] != null)
						ViewState["wfState"] = CHelper.GetResFileString(MetaEnum.GetFriendlyName(Mediachase.Ibn.Core.MetaDataWrapper.GetMetaFieldByName(WorkflowInstanceEntity.ClassName, "State").GetMetaType(), Convert.ToInt32(eo.Properties["State"].Value, CultureInfo.InvariantCulture)));
					else
						ViewState["wfState"] = string.Empty;
				}

				return ViewState["wfState"].ToString();
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			// O.R. [2009-07-28]: Check license and NET Framework version
			if (Mediachase.IBN.Business.Configuration.WorkflowModule && WorkflowActivityWrapper.IsFramework35Installed())
			{
#if DEBUG
				if (Request["Id"] != null)
				{
					Dictionary<string, string> dicNew = new Dictionary<string, string>();
					dicNew.Add("parentName", "_parentName_");
					CommandParameters cpNew = new CommandParameters("WFNewActivityPopup", dicNew);

					Dictionary<string, string> dicEdit = new Dictionary<string, string>();
					dicEdit.Add("activityName", "_activityName_");
					CommandParameters cpEdit = new CommandParameters("WFEditActivityPopup", dicEdit);

					string _newActivity = CommandManager.GetCurrent(this.Page).AddCommand("WorkflowInstance", string.Empty, string.Empty, cpNew);
					string _editActivity = CommandManager.GetCurrent(this.Page).AddCommand("WorkflowInstance", string.Empty, string.Empty, cpEdit);

					ctrlWFBuilder.NewActivityScript = _newActivity;
					ctrlWFBuilder.EditActivityScript = _editActivity;
					ctrlWFBuilder.AddText = CHelper.GetResFileString("{IbnFramework.BusinessProcess:NewActivity}");
					ctrlWFBuilder.EditText = CHelper.GetResFileString("{IbnFramework.BusinessProcess:EditActivity}");
					ctrlWFBuilder.DeleteText = CHelper.GetResFileString("{IbnFramework.BusinessProcess:DeleteActivity}");

					ctrlWFBuilder.WorkflowId = PrimaryKeyId.Parse(Request["Id"]);
					ctrlWFBuilder.DataBind();
				}

				LayoutExtender _extender = LayoutExtender.GetCurrent(this.Page);
				if (_extender != null)
				{
					_extender.NoHeightResize = true;
				}

				Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
					String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", McScriptLoader.Current.GetScriptUrl("~/styles/IbnFramework/assignment.css", this.Page)));
#endif
			}
		}
	}
}