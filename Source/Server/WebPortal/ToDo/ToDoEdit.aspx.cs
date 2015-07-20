using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;

using Mediachase.UI.Web.Util;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.ToDo
{
	/// <summary>
	/// Summary description for ToDoEdit.
	/// </summary>
	public partial class ToDoEdit : System.Web.UI.Page
	{
		private int ToDoID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Request["ToDoID"] != null)
			{
				ToDoID = int.Parse(Request["ToDoID"]);
			}

			ApplyLocalization();
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strPageTitles", typeof(ToDoEdit).Assembly);
			if (ToDoID != 0)
				pT.Title = LocRM.GetString("tToDoEdit");
			else
				pT.Title = LocRM.GetString("tToDoAdd");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
