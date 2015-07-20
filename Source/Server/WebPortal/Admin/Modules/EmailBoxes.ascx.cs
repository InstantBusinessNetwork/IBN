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
	///		Summary description for EmailBoxes.
	/// </summary>
	public partial class EmailBoxes : System.Web.UI.UserControl
	{



		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBar();
			if (!Page.IsPostBack)
			{
				BindData();
			}
		}

		public int GetPop3BoxType(Pop3Box box)
		{
			if (box.Handlers.Contains("File.Pop3MessageHandler"))
				return 1;
			else if (box.Handlers.Contains("IssueRequest.Pop3MessageHandler"))
				return 0;
			else
				return -1;
		}

		public int GetPop3BoxPropertyValue(Pop3Box box, string name)
		{
			if (box.Parameters[name] != null)
				return int.Parse(box.Parameters[name]);
			else
				return 0;
		}

		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tMailBoxes");
			secHeader.AddLink("<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/icons/incident.gif") + "' height='16px' align='absmiddle'/> " + LocRM.GetString("tAddIncBox"), Page.ClientScript.GetPostBackClientHyperlink(lbAddNewIncBox, ""));
			secHeader.AddLink("<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/ifolder.gif") + "' height='16px' align='absmiddle'/> " + LocRM.GetString("tAddFldBox"), Page.ClientScript.GetPostBackClientHyperlink(lbAddNewFld, ""));
		}

		private void BindData()
		{
			dgBoxes.Columns[1].HeaderText = LocRM.GetString("tTitle");
			dgBoxes.Columns[2].HeaderText = LocRM.GetString("tServer");
			dgBoxes.Columns[3].HeaderText = LocRM.GetString("tPort");
			dgBoxes.Columns[4].HeaderText = LocRM.GetString("tUser");
			dgBoxes.Columns[5].HeaderText = LocRM.GetString("tAutoApprove");
			dgBoxes.Columns[6].HeaderText = LocRM.GetString("tAutoDelete");
			dgBoxes.Columns[7].HeaderText = LocRM.GetString("tUseExternal");
			//dgBoxes.DataSource = Mailbox.Get(false);
			Pop3Manager.Current.SelectedPop3Box = null;
			dgBoxes.DataSource = Pop3Manager.Current.GetPop3BoxList();
			dgBoxes.DataBind();
			foreach (DataGridItem dgi in dgBoxes.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
			}
		}

		protected string GetType(object oTitle, object oType)
		{
			string retval = "";
			if ((int)oType == 0)
			{
				retval = "<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/icons/incident.gif") + "' height='16px' align='absmiddle'/> " + oTitle.ToString();
			}
			else
			{
				retval = "<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/ifolder.gif") + "' height='16px' align='absmiddle'/> " + oTitle.ToString();
			}
			return retval;
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
			this.dgBoxes.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
		}
		#endregion

		protected void lbAddNewIncBox_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/MailIncidents.aspx?Type=0");
		}

		protected void lbAddNewFld_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/MailIncidents.aspx?Type=1");
		}

		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
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
}
