namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for EMailIssueBoxes.
	/// </summary>
	public partial class EMailIssueBoxes : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		private string constString = "400, 200, false";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			divMessage.Visible = false;
			BindToolbars();
			if(!Page.IsPostBack)
				BindDG();
			
		}

		#region BindDG
		private void BindDG()
		{
			IncidentBox[] ibList = IncidentBox.List();
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("IncidentBoxId", typeof(int)));
			dt.Columns.Add(new DataColumn("IsDefault", typeof(bool)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("IdentifierMask", typeof(string)));
			DataRow dr;
			foreach(IncidentBox ib in ibList)
			{
				dr = dt.NewRow();
				dr["IncidentBoxId"] = ib.IncidentBoxId;
				dr["Name"] = ib.Name;
				dr["IsDefault"] = ib.IsDefault;
				dr["IdentifierMask"] = ib.IdentifierMask;
				dt.Rows.Add(dr);
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";

			int i = 1;
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tDefaultBox");
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tName");
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tIdentMask");

			dgBoxes.DataSource = dv;
			dgBoxes.DataBind();

			foreach (DataGridItem dgi in dgBoxes.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
				{
					ib.Attributes.Add("onclick","return confirm('"+LocRM.GetString("tWarningIncBox")+"')");
					ib.Attributes.Add("title", LocRM.GetString("tDelete"));
				}
			}
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			secHeader.Title = LocRM.GetString("tIssBoxes");
			secHeader.AddLink(string.Format("<img border='0' width='16px' src='{0}' height='16px' align='absmiddle'/> ", this.Page.ResolveUrl("~/layouts/images/folder.gif")) + LocRM.GetString("tIssueBoxNew"), "javascript:OpenWindow('" + this.Page.ResolveUrl("~/Admin/EmailIssueBoxEdit.aspx") + "'," + constString + ")");
		}
		#endregion

		#region DG Protected Strings
		protected string GetRuleButton(int id)
		{
			IncidentBoxRule[] ibList = IncidentBoxRule.List(id);
			string Tooltip = LocRM.GetString("tIssBoxRules");
			if(ibList.Length>0)
				return String.Format("<a href=\"javascript:OpenWindow('{2}', 640,480,false)\"><img border='0' align='absmiddle' src='{0}' title='{1}'/></a>",
					this.Page.ResolveUrl("~/layouts/images/rules.gif"),
					Tooltip,
					String.Format("{0}?IssBoxId={1}",
					this.Page.ResolveUrl("~/Admin/EMailIssueBoxRulesView.aspx"), id));
			else
				return String.Format("<a href='{2}'><img border='0' align='absmiddle' src='{0}' title='{1}'/></a>",
					this.Page.ResolveUrl("~/layouts/images/rulesnew.gif"),
					Tooltip,
					String.Format("{0}?IssBoxId={1}",
						this.Page.ResolveUrl("~/Admin/EMailIssueBoxRules.aspx"), id));
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgBoxes.DeleteCommand += new DataGridCommandEventHandler(dgBoxes_DeleteCommand);
		}
		#endregion

		private void dgBoxes_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			try
			{
				IncidentBox.Delete(sid);
				Response.Redirect(this.Page.ResolveUrl("~/Admin/EMailIssueBoxes.aspx"));
			}
			catch
			{
				lblCantDelete.Text = LocRM.GetString("tIssboxCantDeleted");
				divMessage.Visible = true;
			}
		}

	}
}
