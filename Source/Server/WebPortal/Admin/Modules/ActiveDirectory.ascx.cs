namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;

	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using System.Reflection;

	/// <summary>
	///		Summary description for ActiveDirectory.
	/// </summary>
	public partial class ActiveDirectory : System.Web.UI.UserControl
	{
		//protected Button btnAddNewItem;


		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region Properties

		public DataTable RangesTable
		{
			get
			{
				if (ViewState["AD_RangesTable"] != null)
					return (DataTable)ViewState["AD_RangesTable"];
				else
					return null;
			}
			set
			{
				ViewState["AD_RangesTable"] = value;
			}
		}

		public int MaxDBRangeId
		{
			get
			{
				if (ViewState["AD_MaxDBRangeId"] != null)
					return (int)ViewState["AD_MaxDBRangeId"];
				else
					return -1;
			}
			set
			{
				ViewState["AD_MaxDBRangeId"] = value;
			}
		}

		public int MaxAddedRangeId
		{
			get
			{
				if (ViewState["AD_MaxAddedRangeId"] != null)
					return (int)ViewState["AD_MaxAddedRangeId"];
				else
					return -1;

			}
			set
			{
				ViewState["AD_MaxAddedRangeId"] = value;
			}
		}

		public ArrayList DeletedRanges
		{
			get
			{
				if (ViewState["AD_DeletedRanges"] != null)
					return (ArrayList)ViewState["AD_DeletedRanges"];
				else
					return null;
			}
			set
			{
				ViewState["AD_DeletedRanges"] = value;
			}
		}

		public bool IsNewRange
		{
			get
			{
				if (ViewState["AD_IsNewRange"] != null)
					return (bool)ViewState["AD_IsNewRange"];
				else
					return false;
			}
			set
			{
				ViewState["AD_IsNewRange"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{

			if (RangesTable == null)
				RangesTable = GetRangesTable();
			if (PortalConfig.UseWinLogin)
			{
				trWinLoginEnabled.Visible = true;
				trWinLoginDisabled.Visible = false;
			}
			else
			{
				trWinLoginEnabled.Visible = false;
				trWinLoginDisabled.Visible = true;
			}
			if (!this.IsPostBack)
			{
				BindData(RangesTable);
			}
			BindSecHeader();
		}

		protected DataTable GetRangesTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Clear();
			dt.Columns.Add("RangeId", typeof(int));
			dt.Columns.Add("StartAddress", typeof(string));
			dt.Columns.Add("EndAddress", typeof(string));
			using (IDataReader reader = Mediachase.IBN.Business.ActiveDirectory.GetLocalAddressRanges())
			{
				int max = -1;
				while (reader.Read())
				{
					DataRow dr = dt.NewRow();

					dr["RangeId"] = int.Parse(reader["RangeId"].ToString());
					dr["StartAddress"] = reader["StartAddress"].ToString();
					dr["EndAddress"] = reader["EndAddress"].ToString();
					max = (int.Parse(reader["RangeId"].ToString()) > max) ? int.Parse(reader["RangeId"].ToString()) : max;
					dt.Rows.Add(dr);
				}
				MaxDBRangeId = max;
				MaxAddedRangeId = max + 1;
			}
			return dt;
		}


		protected void BindData(DataTable dt)
		{

			if (dt != null)
			{
				dgMain.DataSource = dt.DefaultView;
				dgMain.Columns[0].HeaderText = LocRM.GetString("tStartIP");
				dgMain.Columns[1].HeaderText = LocRM.GetString("tEndIP");
				dgMain.DataBind();
				foreach (DataGridItem dgi in dgMain.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
					if (ib != null)
						ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tDelete") + "?');");
				}
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
			this.dgMain.ItemCommand += new DataGridCommandEventHandler(dgMain_ItemCommand);
			this.lbEnableWinLogin.Click += new EventHandler(lbEnableWinLogin_Click);
			this.lbDisableWinLogin.Click += new EventHandler(lbDisableWinLogin_Click);


		}

		void lbDisableWinLogin_Click(object sender, EventArgs e)
		{
			PortalConfig.UseWinLogin = false;
			Response.Redirect("~/Admin/ActiveDirectory.aspx");
		}

		void lbEnableWinLogin_Click(object sender, EventArgs e)
		{
			PortalConfig.UseWinLogin = true;
			Response.Redirect("~/Admin/ActiveDirectory.aspx");
		}
		#endregion

		private void dgMain_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Add")
			{
				DataTable dt = RangesTable;
				if (dt != null)
				{
					DataRow dr = dt.NewRow();
					dr["RangeId"] = -1;//MaxAddedRangeId;
					MaxAddedRangeId++;
					dr["StartAddress"] = "";
					dr["EndAddress"] = "";
					dt.Rows.Add(dr);
					dgMain.EditItemIndex = dt.Rows.Count - 1;
					RangesTable = dt;
					BindData(RangesTable);
				}
			}
			if (e.CommandName == "Edit")
			{
				dgMain.EditItemIndex = e.Item.ItemIndex;
				BindData(RangesTable);
			}
			if (e.CommandName == "Cancel")
			{
				int RangeId = int.Parse(e.CommandArgument.ToString());
				dgMain.EditItemIndex = -1;
				Response.Redirect(this.Page.ResolveUrl("~/Admin/ActiveDirectory.aspx"));
			}
			if (e.CommandName == "Save")
			{
				Page.Validate();
				if (!Page.IsValid)
					return;
				int RangeId = int.Parse(e.CommandArgument.ToString());
				string StartIP, EndIP;
				StartIP = EndIP = string.Empty;
				TextBox tb = (TextBox)e.Item.FindControl("tbStartAddress");
				if (tb != null)
					StartIP = tb.Text.Trim();
				tb = (TextBox)e.Item.FindControl("tbEndAddress");
				if (tb != null)
					EndIP = tb.Text.Trim();
				if (RangeId < 0)
				{
					Mediachase.IBN.Business.ActiveDirectory.AddLocalAddressRange(StartIP, EndIP);
				}
				else
				{
					Mediachase.IBN.Business.ActiveDirectory.UpdateLocalAddressRange(RangeId, StartIP, EndIP);
				}
				dgMain.EditItemIndex = -1;
				Response.Redirect(this.Page.ResolveUrl("~/Admin/ActiveDirectory.aspx"));
			}
			if (e.CommandName == "Delete")
			{
				int RangeId = int.Parse(e.CommandArgument.ToString());
				Mediachase.IBN.Business.ActiveDirectory.DeleteLocalAddressRange(RangeId);
				Response.Redirect(this.Page.ResolveUrl("~/Admin/ActiveDirectory.aspx"));
			}
		}

		protected void BindSecHeader()
		{
			secHeader.Title = LocRM.GetString("tSecurity");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tCommonSettings"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin3"));
			secAddress.AddText(LocRM.GetString("tAllowedRange"));

			lbEnableWinLogin.Text = LocRM.GetString("tEnableWinLogin");
			lbDisableWinLogin.Text = LocRM.GetString("tWinLoginDisableMessage");
		}
	}
}
