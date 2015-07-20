using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Resources;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;


namespace Mediachase.UI.Web.Incidents.Modules
{
	public partial class RecipientsEditor2 : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(RecipientsEditor).Assembly);

		#region IncidentID
		private int IncidentID
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			BindToolbar();
			if (!IsPostBack)
			{
				GetIncidentData();
				BinddgMemebers();
			}
		}

		#region ApplyLocalization
		/// <summary>
		/// Applies the localization.
		/// </summary>
		private void ApplyLocalization()
		{
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			btnAdd.InnerText = LocRM.GetString("tAdd2");
			btnSaveVisible.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_Save}");
			btnCancelVisible.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_Cancel}");

			btnSaveVisible.Attributes.Add("onclick", "javascript:FuncSave();");
			if (Request["closeFramePopup"] != null)
				btnCancelVisible.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				btnCancelVisible.Attributes.Add("onclick", "javascript:FuncCancel();");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{

		}
		#endregion

		#region GetIncidentData
		private void GetIncidentData()
		{
			EMailIssueExternalRecipient[] erList = Mediachase.IBN.Business.EMail.EMailIssueExternalRecipient.List(IncidentID);
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("EMailIssueExternalRecipientId", typeof(int)));
			dt.Columns.Add(new DataColumn("EMail", typeof(string)));
			DataRow dr;
			foreach (EMailIssueExternalRecipient er in erList)
			{
				dr = dt.NewRow();
				dr["EMailIssueExternalRecipientId"] = er.EMailIssueExternalRecipientId;
				dr["EMail"] = er.EMail;
				dt.Rows.Add(dr);
			}
			hdnCurrent.Value = "0";
			ViewState["Participants"] = dt;
		}
		#endregion

		#region BinddgMemebers
		private void BinddgMemebers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("tEMail");
			DataTable dt = (DataTable)ViewState["Participants"];
			DataView dv = dt.DefaultView;
			dv.Sort = "EMail";
			dgMembers.DataSource = dv;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarningPool") + "');");
				ib.ToolTip = LocRM.GetString("Delete");
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
			this.dgMembers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMembers_Delete);
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Participants"];
			Hashtable ht = new Hashtable();
			foreach (DataRow dr in dt.Rows)
				ht.Add((int)dr["EMailIssueExternalRecipientId"], dr["EMail"].ToString());
			EMailIssueExternalRecipient.Update(IncidentID, ht);

			if (Request["closeFramePopup"] != null)
			{
				CommandParameters cp = new CommandParameters("MC_HDM_RecipEdit");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "<script language=javascript>" +
					  "try {window.opener.location.href='IncidentView.aspx?IncidentId=" + IncidentID + "';}" +
					  "catch (e){}window.close();</script>");
		}
		#endregion

		#region btnAdd_Click
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			DataTable dt = (DataTable)ViewState["Participants"];
			DataRow dr = dt.NewRow();
			int iIndex = int.Parse(hdnCurrent.Value);
			dr["EMailIssueExternalRecipientId"] = --iIndex;
			hdnCurrent.Value = iIndex.ToString();
			dr["EMail"] = txtEMail.Text;
			dt.Rows.Add(dr);
			ViewState["Participants"] = dt;
			txtEMail.Text = "";
			BinddgMemebers();
		}
		#endregion

		#region dgMembers_Delete
		private void dgMembers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int EMailIssueExternalRecipientId = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = (DataTable)ViewState["Participants"];
			DataRow[] dr = dt.Select("EMailIssueExternalRecipientId = " + EMailIssueExternalRecipientId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Participants"] = dt;

			BinddgMemebers();
		}
		#endregion
	}
}