namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.Ibn.Web.UI.WebControls;
	using System.Globalization;
	using System.Text;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Core;

	/// <summary>
	///		Summary description for RecipientsEditor.
	/// </summary>
	public partial class RecipientsEditor : System.Web.UI.UserControl
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
					throw new Mediachase.Ibn.AccessDeniedException();
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			AppluLocalization();
			BindToolbar();
			if (!IsPostBack)
			{
				GetIncidentData();
				BinddgMemebers();
			}

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_Client_Recip");
			string script = cm.AddCommand("Incident", "", "IncidentView", cp);
			script = script.Replace("\"", "&quot;");
			lblAddContacts.NavigateUrl = String.Format("javascript:{{{0}}}", script);
			lblAddContacts.Text = LocRM.GetString("ContactSelect");

			StringBuilder sb = new StringBuilder();
			sb.Append("function SelectClient_Refresh(params){");
			sb.Append("var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);");
			sb.Append("if(obj && obj.CommandArguments && obj.CommandArguments.SelectedValue)");
			sb.AppendFormat("__doPostBack('{0}', obj.CommandArguments.SelectedValue);", lbAddClient.UniqueID);
			sb.Append("}");

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
				sb.ToString(), true);
		}

		#region lbAddClient_Click
		protected void lbAddClient_Click(object sender, EventArgs e)
		{
			string s = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(s))
			{
				DataTable dt = (DataTable)ViewState["Participants"];
				string[] mas = s.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string contact in mas)
				{
					PrimaryKeyId id = PrimaryKeyId.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(contact));
					ContactEntity contactEntity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), id);
					if (contactEntity != null &&
						(!String.IsNullOrEmpty(contactEntity.EMailAddress1)
						|| !String.IsNullOrEmpty(contactEntity.EMailAddress2)
						|| !String.IsNullOrEmpty(contactEntity.EMailAddress3)))
					{
						DataRow dr = dt.NewRow();
						int iIndex = int.Parse(hdnCurrent.Value);
						dr["EMailIssueExternalRecipientId"] = --iIndex;
						hdnCurrent.Value = iIndex.ToString();
						if (!String.IsNullOrEmpty(contactEntity.EMailAddress1))
							dr["EMail"] = contactEntity.EMailAddress1;
						else if (!String.IsNullOrEmpty(contactEntity.EMailAddress2))
							dr["EMail"] = contactEntity.EMailAddress2;
						else if (!String.IsNullOrEmpty(contactEntity.EMailAddress3))
							dr["EMail"] = contactEntity.EMailAddress3;
						dt.Rows.Add(dr);
					}
				}
				ViewState["Participants"] = dt;
				BinddgMemebers();
			}
		} 
		#endregion

		#region AppluLocalization
		private void AppluLocalization()
		{
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			btnAdd.InnerText = LocRM.GetString("tAdd2");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = "&nbsp;";// LocRM.GetString("tExternalRecipients");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/saveitem.gif'/> " + LocRM.GetString("btnSave"), "javascript:FuncSave();");
			secHeader.AddSeparator();
			if (Request["closeFramePopup"] != null)
				secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Cancel"), String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Cancel"), "javascript:FuncCancel();");
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
			if(String.IsNullOrEmpty(txtEMail.Text.Trim()))
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
