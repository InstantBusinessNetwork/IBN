using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Mediachase.Ibn.Data;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using Mediachase.Ibn.Assignments;

namespace Mediachase.Ibn.AssignmentsUI
{
	public class Global : System.Web.HttpApplication
	{
		#region Const
		internal const string ConnectionString = @"Data source=S2;Initial catalog=WfAssignment;User ID=dev;Password=;";//Integrated Security=SSPI;Persist Security Info=False;;
		#endregion

		protected void Application_Start(object sender, EventArgs e)
		{
            GlobalWorkflowRuntime.SqlWorkflowPersistenceServiceConnectionString = ConnectionString;
            GlobalWorkflowRuntime.StartRuntime();
		}


		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			DataContext.Current = new DataContext(ConnectionString);
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{
            GlobalWorkflowRuntime.StopRuntime();
		}
	}
}