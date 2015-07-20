namespace Mediachase.UI.Web.Documents.Modules
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
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for DocumentResources.
	/// </summary>
	public partial class DocumentResources : System.Web.UI.UserControl
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentResources).Assembly);

		#region DocumentID
		protected int DocumentID
		{
			get
			{
				try
				{
					return int.Parse(Request["DocumentID"]);
				}
				catch
				{
					throw new Exception("DocumentID is invalid");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = true;
			frManageResources.Attributes.Add("src", "ResourcesEditor.aspx?DocumentID=" + DocumentID + "&FrameId=" + frManageResources.ClientID);

			BinddgMembers();
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
			this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (dgMembers.Items.Count == 0)
			{
				this.Visible = false;
			}
			else
			{
				secHeader.AddText(LocRM.GetString("tbDocumentRes"));
				if (Document.CanModifyResources(DocumentID))
				{
					CommandManager cm = CommandManager.GetCurrent(this.Page);
					CommandParameters cp = new CommandParameters("MC_DM_DocRes");
					string cmd = cm.AddCommand("Document", "", "DocumentView", cp);
					cmd = cmd.Replace("\"", "&quot;");
					secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"), "javascript:" + cmd);
					//secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"), "javascript:ModifyResources(" + DocumentID + ")");
				}
			}
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("UserName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("CanManage");
			dgMembers.Columns[3].HeaderText = LocRM.GetString("Status");

			dgMembers.DataSource = Document.GetListDocumentResourcesDataTable(DocumentID);
			dgMembers.DataBind();
		}
		#endregion

		#region GetLink
		protected string GetLink(int PID, bool IsGroup)
		{
			if (IsGroup)
				return CommonHelper.GetGroupLink(PID);
			else
				return CommonHelper.GetUserStatus(PID);
		}
		#endregion

		#region GetStatus
		protected string GetStatus(object _mbc, object _rp, object _ic)
		{
			bool mbc = false;
			if (_mbc != DBNull.Value)
				mbc = (bool)_mbc;

			bool rp = false;
			if (_rp != DBNull.Value)
				rp = (bool)_rp;

			bool ic = false;
			if (_ic != DBNull.Value)
				ic = (bool)_ic;

			if (!mbc) return "";
			else
				if (rp) return LocRM.GetString("Waiting");
				else
					if (ic) return LocRM.GetString("Accepted");
					else return LocRM.GetString("Declined");
		}
		#endregion

		#region btnRefresh_Click
		private void btnRefresh_Click(object sender, EventArgs e)
		{
			BinddgMembers();
		}
		#endregion

		#region GetManageType
		protected string GetManageType(bool CanManage)
		{
			if (CanManage)
				return "<img alt='' src='../Layouts/Images/accept.gif' border='0' width='16' height='16' align='absmiddle'>";
			else
				return "";
		}
		#endregion
	}
}
