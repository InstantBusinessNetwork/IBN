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
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.Ibn.Web.UI.WebControls;



namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for SaveBasePlanPopUp.
	/// </summary>
	public partial class SaveBasePlanPopUp : System.Web.UI.Page
	{
		#region Html Controls
		//protected Mediachase.UI.Web.Modules.DGExtension.DataGridExtended dgBaseSlots;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(SaveBasePlanPopUp).Assembly);
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			btnOk.ServerClick += new EventHandler(btnOk_ServerClick);
			btnCancel.ServerClick +=new EventHandler(btnCancel_ServerClick);

			btnCancel.Text = LocRM.GetString("tCancel");
			btnOk.Text = LocRM.GetString("Apply");
			btnOk.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");

			if (!IsPostBack)
				BindDropDown();
		}

		#region Bind: DropDown
		void BindDropDown()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("BasePlanSlotId", typeof(int)));

			Hashtable ht = ProjectSpreadSheet.GetFilledSlotHash(int.Parse(Request["ProjectId"]));
			int selvalue = -1;
			DateTime d_time = DateTime.Now;

			foreach (BasePlanSlot bps in BasePlanSlot.List())
			{
				if (bps.IsDefault) selvalue = bps.BasePlanSlotId;
				DataRow row = dt.NewRow();
				
				row["BasePlanSlotId"] = bps.BasePlanSlotId;

				if (ht.Contains(bps.BasePlanSlotId))
				{

					foreach(BasePlan bp in BasePlan.List(int.Parse(Request["ProjectId"])))
					{
						if (bp.BasePlanSlotId == bps.BasePlanSlotId)
						{
							d_time = bp.Created;
							break;
						}
					}

					row["Name"] = String.Format("{0} ({1}: {2})" ,bps.Name, LocRM.GetString("LastSaved"), d_time);
					//row["Created"] = ht[bps.BasePlanSlotId];
					
				}
				else
				{
					row["Name"] = bps.Name;
				}
				dt.Rows.Add(row);
			}

			ddBasePlan.DataSource = dt;
			ddBasePlan.DataTextField = "Name";
			ddBasePlan.DataValueField = "BasePlanSlotId";
			ddBasePlan.DataBind();
			ddBasePlan.SelectedValue = selvalue.ToString();
		}
		#endregion

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

		#region btnOk_ServerClick
		private void btnOk_ServerClick(object sender, EventArgs e)
		{
			BasePlan.Save(int.Parse(Request["ProjectId"]), int.Parse(ddBasePlan.SelectedValue));
			ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "window.opener.location.href=window.opener.location.href; window.close();", true);
		}
		#endregion

		#region btnCancel_ServerClick
		private void btnCancel_ServerClick(object sender, EventArgs e)
		{
		  ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "window.close();", true);
		}
		#endregion
	}
}
