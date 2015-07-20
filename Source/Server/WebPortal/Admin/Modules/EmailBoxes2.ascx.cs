namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.Pop3;
	using System.Reflection;

	/// <summary>
	///		Summary description for EmailBoxes2.
	/// </summary>
	public partial class EmailBoxes2 : System.Web.UI.UserControl
	{



		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBar();
			if (!Page.IsPostBack)
			{
				BindData();
			}
		}

		protected string GetStatus(object IsActive, object LastRequest, object LastSuccessfulRequest, object LastErrorText)
		{
			string retval = "";
			if ((bool)IsActive)
			{
				if ((DateTime)LastRequest != (DateTime)LastSuccessfulRequest)
					retval = "<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/icons/status_problem.gif") + "' height='16px' align='absmiddle' title='" + (string)LastErrorText + "'/>";
				else
					retval = "<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/icons/status_active.gif") + "' height='16px' align='absmiddle'/>";
			}
			else
			{
				retval = "<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/icons/status_stopped.gif") + "' height='16px' align='absmiddle'/>";
			}
			return retval;
		}

		public string GetPop3BoxHandlerName(Pop3Box box)
		{
			if (box.Handlers.Count > 0)
				return box.Handlers[0].Name;
			else
				return "none";
		}

		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tMailBoxes");
			secHeader.AddLink(string.Format("<img border='0' width='16px' src='{0}' height='16px' align='absmiddle'/> ", this.Page.ResolveUrl("~/layouts/images/folder.gif")) + LocRM.GetString("tAddBox"), this.Page.ResolveUrl("~/Admin/MailIncidents.aspx"));
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tCommonSettings"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin3"));
		}

		private void BindData()
		{
			/*
			dgBoxes.Columns[1].HeaderText = LocRM.GetString("tTitle");
			dgBoxes.Columns[2].HeaderText = LocRM.GetString("tServer");
			dgBoxes.Columns[3].HeaderText = LocRM.GetString("tPort");
			dgBoxes.Columns[4].HeaderText = LocRM.GetString("tUser");
			dgBoxes.Columns[5].HeaderText = LocRM.GetString("tAutoApprove");
			dgBoxes.Columns[6].HeaderText = LocRM.GetString("tAutoDelete");
			dgBoxes.Columns[7].HeaderText = LocRM.GetString("tUseExternal");
			*/
			dgBoxes.Columns[1].HeaderText = LocRM.GetString("tTitle");
			dgBoxes.Columns[2].HeaderText = LocRM.GetString("tStatus");
			dgBoxes.Columns[3].HeaderText = LocRM.GetString("tHandler");
			dgBoxes.Columns[4].HeaderText = LocRM.GetString("tLastReq");
			dgBoxes.Columns[5].HeaderText = LocRM.GetString("tLastSuccReq");
			//dgBoxes.DataSource = Mailbox.Get(false);
			Pop3Manager.Current.SelectedPop3Box = null;

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("IsActive", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsActiveInt", typeof(int)));
			dt.Columns.Add(new DataColumn("HandlerName", typeof(string)));
			dt.Columns.Add(new DataColumn("LastRequest", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("LastSuccessfulRequest", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("LastErrorText", typeof(string)));

			//dgBoxes.DataSource = Pop3Manager.Current.GetPop3BoxList();
			Pop3Box[] pop3list = Pop3Manager.Current.GetPop3BoxList();
			DataRow dr;
			foreach (Pop3Box pop3boxel in pop3list)
			{
				dr = dt.NewRow();
				dr["Id"] = pop3boxel.Id;
				dr["Name"] = pop3boxel.Name;
				dr["IsActive"] = pop3boxel.IsActive;
				dr["IsActiveInt"] = (pop3boxel.IsActive ? 1 : 0) + ((pop3boxel.IsActive && pop3boxel.LastRequest != pop3boxel.LastSuccessfulRequest) ? 1 : 0);
				dr["HandlerName"] = GetPop3BoxHandlerName(pop3boxel);
				dr["LastRequest"] = pop3boxel.LastRequest;
				dr["LastSuccessfulRequest"] = pop3boxel.LastSuccessfulRequest;
				dr["LastErrorText"] = pop3boxel.LastErrorText;
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;

			if (pc["EmailBoxes_Sorting"] == null)
				pc["EmailBoxes_Sorting"] = "Name";
			dv.Sort = pc["EmailBoxes_Sorting"].ToString();

			dgBoxes.DataSource = dv;

			dgBoxes.DataBind();
			foreach (DataGridItem dgi in dgBoxes.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
			}

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
			this.dgBoxes.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_ItemCommand);
			this.dgBoxes.SortCommand += new DataGridSortCommandEventHandler(dgBoxes_SortCommand);
		}
		#endregion

		protected void lbAddNewBox_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/MailIncidents.aspx");
		}

		private void dg_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int sid;

			if (e.CommandName == "ChangeStatus")
			{
				sid = int.Parse(e.Item.Cells[0].Text);
				Pop3Box box = Pop3Manager.Current.GetPop3Box(sid);
				box.IsActive = !box.IsActive;
				Pop3Manager.Current.UpdatePop3Box(box);
				//BindData();
				Response.Redirect("~/Admin/EmailBoxes.aspx");
			}

			if (e.CommandName == "Delete")
			{
				sid = int.Parse(e.Item.Cells[0].Text);
				try
				{
					Pop3Manager.Current.RemovePop3Box(sid);
					//Mailbox.Delete(sid);
				}
				catch
				{
					lblError.Text = "<br>" + LocRM.GetString("lblMailDeleteError") + "<br><br>";
				}
				finally
				{
					Response.Redirect("~/Admin/EmailBoxes.aspx");
				}
			}
		}

		private void dgBoxes_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (pc["EmailBoxes_Sorting"].ToString() == (string)e.SortExpression)
				pc["EmailBoxes_Sorting"] = (string)e.SortExpression + " DESC";
			else
				pc["EmailBoxes_Sorting"] = (string)e.SortExpression;
			BindData();
		}
	}
}
