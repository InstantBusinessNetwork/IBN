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
	using System.Reflection;

	/// <summary>
	///		Summary description for LdapSettingsView.
	/// </summary>
	public partial class LdapSettingsView : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));



		#region SetId
		protected int SetId
		{
			get
			{
				if (Request["SetId"] != null)
					return int.Parse(Request["SetId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindValues();
			BindToolbars();
		}

		#region BindValues
		private void BindValues()
		{
			Mediachase.IBN.Business.LdapSettings lsets = Mediachase.IBN.Business.LdapSettings.Load(SetId);
			lblTitle.Text = lsets.Title;
			lblDomain.Text = lsets.Domain;
			lblFilter.Text = lsets.Filter;
			lblUser.Text = lsets.Username;
			lblIBN.Text = lsets.IbnKey;
			lblLDAP.Text = lsets.LdapKey;
			lblActivated.Text = String.Format("<img align='absmiddle' border='0' src='{1}' />&nbsp;{0}", LocRM.GetString("tActivate"),
				(lsets.Activate) ? ResolveUrl("~/layouts/images/accept_1.gif") : ResolveUrl("~/layouts/images/deny_1.gif"));
			lblDeactivated.Text = String.Format("<img align='absmiddle' border='0' src='{1}' />&nbsp;{0}", LocRM.GetString("tDeactivate"),
				(lsets.Deactivate) ? ResolveUrl("~/layouts/images/accept_1.gif") : ResolveUrl("~/layouts/images/deny_1.gif"));
			lblAutosinc.Text = String.Format("<img align='absmiddle' border='0' src='{1}' />&nbsp;{0}", LocRM.GetString("tAutosync"),
				(lsets.Autosync) ? ResolveUrl("~/layouts/images/accept_1.gif") : ResolveUrl("~/layouts/images/deny_1.gif"));
			lblLastSynch.Text = (lsets.LastSynchronization.Year > 1) ? Mediachase.IBN.Business.User.GetLocalDate(lsets.LastSynchronization).ToString("g") : "";
			if (lsets.Autosync)
			{
				lblStart.Text = (lsets.AutosyncStart.Year > 1) ? Mediachase.IBN.Business.User.GetLocalDate(lsets.AutosyncStart).ToString("g") : "";
				lblInterval.Text = lsets.AutosyncInterval.ToString();
			}
			else
				trAuto.Visible = false;

			DataTable dt = GetDT(lsets.Fields);

			int i = 1;
			dgFields.Columns[i++].HeaderText = LocRM.GetString("tIsBitField");
			dgFields.Columns[i++].HeaderText = LocRM.GetString("tIBNName");
			dgFields.Columns[i++].HeaderText = LocRM.GetString("tLdapName");
			dgFields.Columns[i++].HeaderText = LocRM.GetString("tSettings");

			dgFields.DataSource = dt.DefaultView;
			dgFields.DataBind();

			foreach (DataGridItem dgi in dgFields.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("DeleteWarning") + "')");

				DropDownList ddIbnName = (DropDownList)dgi.FindControl("ddIbnName");
				if (ddIbnName != null)
				{
					ddIbnName.DataSource = UserInfo.PropertyNamesIbnAll;
					ddIbnName.DataBind();
				}
				DropDownList ddLdapName = (DropDownList)dgi.FindControl("ddLdapName");
				if (ddLdapName != null)
				{
					ddLdapName.DataSource = UserInfo.PropertyNamesAdAll;
					ddLdapName.DataBind();
				}
			}
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			secHeader.Title = LocRM.GetString("tLDAPSettingsView");
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/upload.gif") + "'/> " + LocRM.GetString("tSynchronize"), "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSynch, ""));
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/edit.gif") + "'/> " + LocRM.GetString("tLDAPSettingsEdit"), "javascript:OpenWindow('LdapSettingsEdit.aspx?SetId=" + SetId.ToString() + "', 400, 540, false)");
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tLDAPSettings"), 
				ResolveUrl("~/Admin/LdapSettings.aspx"));
		}
		#endregion

		#region Protected DG Strings
		protected string GetIsBit(bool _is)
		{
			return String.Format("<img align='absmiddle' border='0' src='{0}' />",
				(_is) ? ResolveUrl("~/layouts/images/accept_1.gif") : ResolveUrl("~/layouts/images/deny_1.gif"));
		}

		protected string GetSets(bool _is, object bitmask, object isequal, object compareto)
		{
			string retVal = "";
			if (_is)
				retVal = "AND&nbsp;&nbsp;&nbsp;" + bitmask.ToString() + (((bool)isequal) ? " Equal " : " Not Equal ") + compareto.ToString();
			return retVal;
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
			this.dgFields.DeleteCommand += new DataGridCommandEventHandler(dgFields_DeleteCommand);
			this.dgFields.EditCommand += new DataGridCommandEventHandler(dgFields_EditCommand);
			this.dgFields.UpdateCommand += new DataGridCommandEventHandler(dgFields_UpdateCommand);
			this.dgFields.CancelCommand += new DataGridCommandEventHandler(dgFields_CancelCommand);
			this.dgFields.ItemCommand += new DataGridCommandEventHandler(dgFields_ItemCommand);
			this.lbSynch.Click += new EventHandler(lbSynch_Click);
		}
		#endregion

		#region DataGrid Events
		private void dgFields_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			LdapField.Delete(sid);
			Response.Redirect("~/Admin/LdapSettingsView.aspx?SetId=" + SetId);
		}

		private void dgFields_EditCommand(object source, DataGridCommandEventArgs e)
		{
			dgFields.EditItemIndex = e.Item.ItemIndex;
			dgFields.DataKeyField = "FieldId";
			BindValues();
			int sid = int.Parse(e.Item.Cells[0].Text);
			LdapField lf = LdapField.Load(sid);
			foreach (DataGridItem dgi in dgFields.Items)
			{
				DropDownList ddEqual = (DropDownList)dgi.FindControl("ddEqual");
				if (ddEqual != null)
					ddEqual.SelectedValue = lf.Equal ? "true" : "false";

				DropDownList ddIbnName = (DropDownList)dgi.FindControl("ddIbnName");
				if (ddIbnName != null)
					ddIbnName.SelectedValue = lf.IbnName;

				DropDownList ddLdapName = (DropDownList)dgi.FindControl("ddLdapName");
				if (ddLdapName != null)
				{
					try
					{
						ddLdapName.SelectedValue = lf.LdapName;
					}
					catch { }
					ddLdapName.Attributes.Add("onchange", "ChangeLdap(this)");
				}
			}
		}

		private void dgFields_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int ItemID = (int)dgFields.DataKeys[e.Item.ItemIndex];
			CheckBox cbIsBit = (CheckBox)e.Item.FindControl("cbIsBit");
			DropDownList ddIbnName = (DropDownList)e.Item.FindControl("ddIbnName");
			DropDownList ddLdapName = (DropDownList)e.Item.FindControl("ddLdapName");
			TextBox txtLdapName = (TextBox)e.Item.FindControl("txtLdapName");
			TextBox txtBitMask = (TextBox)e.Item.FindControl("txtBitMask");
			TextBox txtCompare = (TextBox)e.Item.FindControl("txtCompare");
			Label lblError = (Label)e.Item.FindControl("lblError");
			DropDownList ddEqual = (DropDownList)e.Item.FindControl("ddEqual");
			if (cbIsBit != null && ddIbnName != null && txtLdapName != null && txtBitMask != null &&
				txtCompare != null && ddEqual != null)
			{
				int iMask = 0;
				int iCompare = 0;
				if (cbIsBit.Checked)
				{
					try
					{
						iMask = int.Parse(txtBitMask.Text);
						iCompare = int.Parse(txtCompare.Text);
					}
					catch
					{
						lblError.Visible = true;
						return;
					}
				}
				LdapField.CreateUpdate(ItemID, SetId, cbIsBit.Checked, ddIbnName.SelectedValue,
					(txtLdapName.Text != "") ? txtLdapName.Text : ddLdapName.SelectedValue,
					iMask, bool.Parse(ddEqual.SelectedValue), iCompare);
			}
			Response.Redirect("~/Admin/LdapSettingsView.aspx?SetId=" + SetId);
		}

		private void dgFields_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			Response.Redirect("~/Admin/LdapSettingsView.aspx?SetId=" + SetId);
		}

		private void dgFields_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "NewField")
			{
				Mediachase.IBN.Business.LdapSettings lsets = Mediachase.IBN.Business.LdapSettings.Load(SetId);
				DataTable dt = GetDT(lsets.Fields);
				DataRow dr = dt.NewRow();
				dr["FieldId"] = -1;
				dr["IsBit"] = false;
				dr["IbnName"] = "Login";
				dr["LdapName"] = "";
				dr["BitMask"] = 0;
				dr["Equal"] = false;
				dr["CompareTo"] = 0;
				dt.Rows.Add(dr);

				dgFields.EditItemIndex = dt.Rows.Count - 1;
				dgFields.DataKeyField = "FieldId";
				dgFields.DataSource = dt.DefaultView;
				dgFields.DataBind();

				foreach (DataGridItem dgi in dgFields.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
					if (ib != null)
						ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("DeleteWarning") + "')");

					DropDownList ddIbnName = (DropDownList)dgi.FindControl("ddIbnName");
					if (ddIbnName != null)
					{
						ddIbnName.DataSource = UserInfo.PropertyNamesIbnAll;
						ddIbnName.DataBind();
					}

					DropDownList ddLdapName = (DropDownList)dgi.FindControl("ddLdapName");
					if (ddLdapName != null)
					{
						ddLdapName.DataSource = UserInfo.PropertyNamesAdAll;
						ddLdapName.DataBind();
						ddLdapName.Attributes.Add("onchange", "ChangeLdap(this)");

						TextBox txtLdapName = (TextBox)dgi.FindControl("txtLdapName");
						if (txtLdapName != null)
							txtLdapName.Text = ddLdapName.SelectedValue;
					}
				}
			}
		}
		#endregion

		#region GetDT
		private DataTable GetDT(ArrayList alFields)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("FieldId", typeof(int)));
			dt.Columns.Add(new DataColumn("IsBit", typeof(bool)));
			dt.Columns.Add(new DataColumn("IbnName", typeof(string)));
			dt.Columns.Add(new DataColumn("LdapName", typeof(string)));
			dt.Columns.Add(new DataColumn("BitMask", typeof(int)));
			dt.Columns.Add(new DataColumn("Equal", typeof(bool)));
			dt.Columns.Add(new DataColumn("CompareTo", typeof(int)));
			DataRow dr;
			foreach (LdapField lf in alFields)
			{
				dr = dt.NewRow();
				dr["FieldId"] = lf.FieldId;
				dr["IsBit"] = lf.BitField;
				dr["IbnName"] = lf.IbnName;
				dr["LdapName"] = lf.LdapName;
				dr["BitMask"] = lf.BitMask;
				dr["Equal"] = lf.Equal;
				dr["CompareTo"] = lf.CompareTo;
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		private void lbSynch_Click(object sender, EventArgs e)
		{
			int LogId = Ldap.Synchronize(SetId);
			Response.Redirect("~/Admin/LdapLogView.aspx?LogId=" + LogId);
		}
	}
}
