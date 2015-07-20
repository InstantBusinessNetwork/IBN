namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for FinanceAccountsList.
	/// </summary>
	public partial class FinanceAccountsList : System.Web.UI.UserControl
	{
		#region HTML Vars
		#endregion

		private UserLightPropertyCollection pc=Security.CurrentUser.Properties;
		protected IFormatProvider culture = CultureInfo.InvariantCulture;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(FinanceAccountsList).Assembly);

		#region ProjectId
		protected int ProjectId
		{
			get
			{
				try{
					return int.Parse(Request["ProjectId"].ToString());
				}
				catch{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblHelp.Text = LocRM.GetString("tSum");
			ApplyLocalization();
			//txtAccountTitle.Text="1";
			if(!Page.IsPostBack)
			{
				lowBorder.Text = "0";
				highBorder.Text = "0";
				BindDG();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			dgAccounts.Columns[4].HeaderText = LocRM.GetString("tTitle");
			dgAccounts.Columns[5].HeaderText = LocRM.GetString("tTarget");
			dgAccounts.Columns[6].HeaderText = LocRM.GetString("Estimate");
			dgAccounts.Columns[7].HeaderText = LocRM.GetString("Actual");
			lgdBlock.InnerText = LocRM.GetString("tChVal");
			lgdEditItem.InnerText = LocRM.GetString("tEditAcc");
			btnUpdateAcc.Text = LocRM.GetString("tSave");
			cbSaveParent.Text = LocRM.GetString("tSaveParVal");
			btnUpdAccName.Text = LocRM.GetString("tSave");
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			DataTable dt = Finance.GetListAccountsByProjectCollapsed(ProjectId);

			if (pc["FinAcc_PageSize"]!=null)
				dgAccounts.PageSize = int.Parse(pc["FinAcc_PageSize"]);

			if (pc["FinAcc_Page"]!=null)
				dgAccounts.CurrentPageIndex = int.Parse(pc["FinAcc_Page"]);

			int pageindex = dgAccounts.CurrentPageIndex;
			int ppi = dt.Rows.Count / dgAccounts.PageSize;
			if  (dt.Rows.Count % dgAccounts.PageSize == 0)
				ppi = ppi - 1;
			
			if (pageindex <= ppi)
				dgAccounts.CurrentPageIndex = pageindex;
			else dgAccounts.CurrentPageIndex = 0;

			dgAccounts.DataSource = dt.DefaultView;
			dgAccounts.DataBind();

			foreach(DataGridItem dgi in dgAccounts.Items)
			{
				if(dgi.FindControl("ibDelete")!=null)
				{
					ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
					ibDelete.ToolTip = LocRM.GetString("tDelete");
					ibDelete.Attributes.Add("onclick","return confirm('"+LocRM.GetString("accWarning")+"')");
				}
			}
			if(!Project.CanEditFinances(ProjectId))
				dgAccounts.Columns[8].Visible = false;
		}
		#endregion

		#region DataGrid Strings
		protected string GetIcon(bool IsExpand)
		{
			if (IsExpand) 
				return "<img border=0 src='" + ResolveUrl("~/Layouts/images/minus.gif") +"'>";
			else 
				return "<img border=0 src='" + ResolveUrl("~/Layouts/images/plus.gif") +"'>";
		}

		protected string GetTitle(int AccountId, bool IsSummary, bool IsCollapsed, int OutlineLevel, string Title)
		{
			string predstr = "";
			if(OutlineLevel==1)
				Title = LocRM.GetString("tRoot");
			if(IsSummary)
			{
				string CEType = "0";
				if(IsCollapsed)
					CEType = "1";
				predstr = String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", 20*(OutlineLevel-1));
				return predstr+"<a href='javascript:CollapseExpand("+CEType+","+AccountId.ToString()+")'>"+GetIcon(!IsCollapsed)+"</a>&nbsp;&nbsp;"+Title;
			}
			else
			{
				return String.Format("<div class='text' style='padding-left:{0}px'><img alt='' src='../layouts/images/rect.gif' border='0' align='absmiddle' />&nbsp;&nbsp;&nbsp;"+Title+"</div>", 20*(OutlineLevel-1));
			}
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
			this.dgAccounts.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageChange);			
			this.dgAccounts.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChange);
			this.dgAccounts.DeleteCommand +=new DataGridCommandEventHandler(dgAccounts_DeleteCommand);
			this.btnUpdateAcc.Click += new EventHandler(btnUpdateAcc_Click);
			this.btnUpdAccName.Click += new EventHandler(btnUpdAccName_Click);
		}
		#endregion

		#region DataGrid_Events
		private void dg_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["FinAcc_Page"] = e.NewPageIndex.ToString();
			BindDG();
		}

		private void dg_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["FinAcc_PageSize"] = e.NewPageSize.ToString();
			BindDG();
		}

		private void dgAccounts_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int AccountId = int.Parse(e.CommandArgument.ToString());
			Finance.DeleteAccount(AccountId);
			BindDG();
		}
		#endregion

		#region Collapse_Expand
		protected void Collapse_Expand_Click(object sender, System.EventArgs e)
		{
			int AccId = int.Parse(hdnAccountId.Value);
			string CEType = hdnCollapseExpand.Value;
			if (CEType=="0")
				Finance.CollapseAccount(AccId);
			else
				Finance.ExpandAccount(AccId);
			BindDG();	
		}
		#endregion

		#region UpdateAccountValue
		private void btnUpdateAcc_Click(object sender, EventArgs e)
		{
			try
			{
				int AccId = int.Parse(hdnAccountId.Value);
				string CEType = hdnCollapseExpand.Value;
				decimal dValue = 0;
				try{
					dValue = decimal.Parse(txtSum.Text);
				}
				catch{}
				if (CEType=="0")
					Finance.UpdateTargetAccount(AccId, dValue, cbSaveParent.Checked);
				else
					Finance.UpdateEstimatedAccount(AccId, dValue, cbSaveParent.Checked);
			}
			catch (WrongDataException)
			{
			}
			BindDG();	
		}
		#endregion

		#region Create-Update Account
		private void btnUpdAccName_Click(object sender, EventArgs e)
		{
			if(Page.IsValid)
			{
				int AccId = int.Parse(hdnAccountId.Value);
				string CEType = hdnCollapseExpand.Value;
				if (CEType=="1")
					Finance.RenameAccount(AccId,txtAccountTitle.Text);
				else
					Finance.AddChildAccount(AccId, txtAccountTitle.Text);
			}
			BindDG();	
		}
		#endregion

    #region Page_PreRender
    private void Page_PreRender(object sender, EventArgs e)
		{
			rfAcTitle.IsValid = true;
			cvSum1.IsValid = true;
			cvSum2.IsValid = true;
		}
		#endregion
	}
}
