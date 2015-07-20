namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	/// <summary>
	///		Summary description for ToDoGeneral2.
	/// </summary>
	public partial  class ToDoGeneral2 : System.Web.UI.UserControl
	{
		
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", typeof(ToDoGeneral2).Assembly);


		#region ToDoID
		private int ToDoID
		{
			get 
			{
				try
				{
					return int.Parse(Request["ToDoID"]);
				}
				catch
				{
					throw new Exception("Invalid ToDo ID");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
		  if (Security.CurrentUser.IsExternal)
			res.Visible = false;
		}

	}
}
