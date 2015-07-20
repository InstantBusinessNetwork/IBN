namespace Mediachase.UI.Web.Common.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Reflection;
	using System.Resources;

	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Lists;
	using Mediachase.Ibn.Web.Interfaces;
	using Mediachase.UI.Web.Util;
	

	/// <summary>
	///		Summary description for SystemEventsByObject.
	/// </summary>
	public partial class SystemEventsByObject : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(SystemEventsByObject).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strManageDictionaries", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region ObjectId
		private int ObjectId
		{
			get
			{
				return int.Parse(Request["ObjectId"]);
			}
		}
		#endregion

		#region ObjectTypeId
		private int ObjectTypeId
		{
			get
			{
				return int.Parse(Request["ObjectTypeId"]);
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBar();
			BindDG();
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

		#region BindToolBar
		private void BindToolBar()
		{
			string Title = Util.CommonHelper.GetObjectTypeName(ObjectTypeId) + ": ";
			switch (ObjectTypeId)
			{
				case (int)ObjectTypes.CalendarEntry:
					Title += CalendarEntry.GetEventTitle(ObjectId);
					break;
				case (int)ObjectTypes.Document:
					Title += Document.GetTitle(ObjectId);
					break;
				case (int)ObjectTypes.Issue:
					Title += Incident.GetTitle(ObjectId);
					break;
				case (int)ObjectTypes.List:
					Title += ListManager.GetListTitle(ObjectId);
					break;
				case (int)ObjectTypes.Project:
					Title += Project.GetProjectTitle(ObjectId);
					break;
				case (int)ObjectTypes.Task:
					Title += Task.GetTaskTitle(ObjectId);
					break;
				case (int)ObjectTypes.ToDo:
					Title += Mediachase.IBN.Business.ToDo.GetToDoTitle(ObjectId);
					break;
			}
			secHeader.Title = Title;

			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM2.GetString("Exit"), "javascript:window.close();");
		}
		#endregion

		private void BindDG()
		{
			dgUpdates.DataSource = SystemEvents.GetListSystemEventsByObjectDT(ObjectId, ObjectTypeId);
			dgUpdates.DataBind();
		}

		#region IPageTemplateTitle Members
		public string Modify(string oldValue)
		{
			return LocRM.GetString("UpdateHistory");
		}
		#endregion
	}
}
