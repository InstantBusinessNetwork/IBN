using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;

using Mediachase.IBN.Business;


namespace Mediachase.UI.Web.WebServices
{
	/// <summary>
	/// Summary description for Gannt.
	/// </summary>
	public class Gannt : System.Web.Services.WebService
	{
		public Gannt()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		[WebMethod]
		public XmlDocument GetData(int projectId)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(Gantt.GetData(projectId));
			return doc;
		}

		[WebMethod]
		public string GetDataString(int projectId)
		{
			return Mediachase.IBN.Business.Gantt.GetData(projectId);
		}

		[WebMethod]
		public void TaskMoveLeft(int taskId)
		{
			Task.MoveLeft(taskId);
		}

		[WebMethod]
		public void TaskMoveRight(int taskId)
		{
			Task.MoveRight(taskId);
		}

		[WebMethod]
		public void TaskDelete(int taskId)
		{
			Task.Delete(taskId);
		}

		[WebMethod]
		public void TaskDeleteAll(int taskId)
		{
			Task.Delete(taskId);
		}

		[WebMethod]
		public void TaskSetProgress(int taskId, int percentCompleted)
		{
			Task.UpdatePercent(taskId, percentCompleted, 0, DateTime.MinValue);
		}

		[WebMethod]
		public void TaskUpdateTimeline(int taskId, DateTime startDate, DateTime finishDate)
		{
			Task2.UpdateTimeline(taskId, startDate, finishDate);
		}

		[WebMethod]
		public void TaskMove(int taskId, int afterTaskNum)
		{
			Task.MoveTo(taskId, afterTaskNum);
		}

		[WebMethod]
		public void TaskCollapse(int taskId)
		{
			Task.Collapse(taskId);
		}

		[WebMethod]
		public void TaskExpand(int taskId)
		{
			Task.Expand(taskId);
		}


		#region Tests
		[WebMethod]
		public string Test1()
		{
			return "Hello World!";
		}
		[WebMethod]
		public string Test2(int number)
		{
			return string.Format("Number = {0}", number);
		}
		#endregion
	}
}
