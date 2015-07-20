namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using Mediachase.Ibn.Web.Interfaces;
	using System.Reflection;

	/// <summary>
	///		Summary description for AddEditWebStubs.
	/// </summary>
	public partial class AddEditWebStubs : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected System.Web.UI.WebControls.Label lblSelected;
		protected System.Web.UI.WebControls.Label lblAvailable;
		protected System.Web.UI.HtmlControls.HtmlInputButton Button1;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strWebStubs", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region StubID
		private int StubID
		{
			get
			{
				try
				{
					if (Request["StubId"] != null)
						return int.Parse(Request["StubId"]);
					else return 0;
				}
				catch
				{
					return 0;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindControls();
				if (StubID > 0)
					BindSavedValues();
			}
			BindToolbar();
			btnSubmit.CustomImage = this.Page.ResolveUrl("~/Layouts/Images/saveitem.gif");
		}

		#region BindControls
		private void BindControls()
		{
			lstOpenWindow.Items.Add(new ListItem(LocRM.GetString("ExternalBrowser"), "1"));
			lstOpenWindow.Items.Add(new ListItem(LocRM.GetString("TearOff"), "2"));

			btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");
			btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + ");return false;");

			lbAvailableGroups.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			lbSelectedGroups.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");

			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(true, true, true, true, true, false, true, true, false, false, true)) //SecureGroup.GetListGroupsAsTreeForIBN())
			{
				while (reader.Read())
				{
					string prefix = String.Empty;
					//for (int i = 0; i < (int)reader["Level"]; i++)
					//    prefix += "";
					lbAvailableGroups.Items.Add(new ListItem(prefix + CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			btnSubmit.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSubmit.Attributes.Add("onclick", "DisableButtons(this);SaveGroups();");

			if (Request["Group"] == "0")
				trGroups.Visible = false;

			DeleteIconCheckBox.Text = LocRM.GetString("DeleteIcon");
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			using (IDataReader rdr = WebStubs.GetStub(StubID))
			{
				if (rdr.Read())
				{
					txtTitle.Text = (string)rdr["ToolTip"];
					txtUrl.Text = (string)rdr["Url"];
					txtShort.Text = ((string)rdr["Abbreviation"]).Trim();
					if ((int)rdr["HasIcon"] != 0)
					{
						imgIcon.Src = Page.ResolveUrl("~/Common/WebStub.aspx") + "?StubId=" + StubID;
						imgIcon.Style["display"] = "";
						DeleteIconCheckBox.Visible = true;
					}
					else
					{
						lblShort.Text = (string)rdr["Abbreviation"];
						lblShort.Style["display"] = "";
						DeleteIconCheckBox.Visible = false;
					}

					if ((bool)rdr["OpenInBrowser"])
					{
						lstOpenWindow.SelectedIndex = 0;
						txtWidth.Enabled = false;
						txtHeight.Enabled = false;
					}
					else
					{
						lstOpenWindow.SelectedIndex = 1;
						txtWidth.Enabled = true;
						txtHeight.Enabled = true;
					}
					txtWidth.Text = rdr["Width"].ToString();
					txtHeight.Text = rdr["Height"].ToString();
				}
			}

			using (IDataReader reader = WebStubs.GetListGroupsByStub(StubID))
			{
				while (reader.Read())
				{
					lbSelectedGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			for (int i = 0; i < lbSelectedGroups.Items.Count; i++)
			{
				if (lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value) != null)
					lbAvailableGroups.Items.Remove(lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value));
				iGroups.Value += lbSelectedGroups.Items[i].Value + ",";
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (StubID > 0)
				secHeader.Title = LocRM.GetString("EditStub");
			else
				secHeader.Title = LocRM.GetString("AddStub");
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			if (StubID > 0)
				return LocRM.GetString("EditStub");
			else
				return LocRM.GetString("AddStub");
		}
		#endregion

		#region btnSubmit_Click
		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{

			byte[] Icon = null;
			bool OpenInBrowser = false;

			if (lstOpenWindow.SelectedIndex == 0)
				OpenInBrowser = true;

			if (txtIcon.PostedFile != null && txtIcon.PostedFile.ContentLength > 0)
			{
				Icon = new byte[txtIcon.PostedFile.ContentLength];
				txtIcon.PostedFile.InputStream.Read(Icon, 0, txtIcon.PostedFile.ContentLength);
			}

			ArrayList grp = new ArrayList();
			string sGroups = iGroups.Value;

			while (sGroups.Length > 0)
			{
				grp.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
				sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
			}


			if (StubID == 0)
				if (Request["Group"] == "1")
					WebStubs.CreateGroupStub(txtShort.Text, txtTitle.Text, txtUrl.Text, OpenInBrowser, int.Parse(txtWidth.Text), int.Parse(txtHeight.Text), Icon, grp);
				else
					WebStubs.CreateUserStub(txtShort.Text, txtTitle.Text, txtUrl.Text, OpenInBrowser, int.Parse(txtWidth.Text), int.Parse(txtHeight.Text), Icon);

			else
				if (Request["Group"] == "1")
					WebStubs.UpdateGroupStub(StubID, txtShort.Text, txtTitle.Text, txtUrl.Text, OpenInBrowser, int.Parse(txtWidth.Text), int.Parse(txtHeight.Text), Icon, grp, DeleteIconCheckBox.Checked);
				else
					WebStubs.UpdateUserStub(StubID, txtShort.Text, txtTitle.Text, txtUrl.Text, OpenInBrowser, int.Parse(txtWidth.Text), int.Parse(txtHeight.Text), Icon, DeleteIconCheckBox.Checked);

			if (Request["Group"] == "1")
				Response.Redirect("~/Admin/WebStubs.aspx");
			else
				Response.Redirect("~/Directory/UserWebApps.aspx");

		}
		#endregion

		#region btnCancel_Click
		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			if (Request["Group"] == "1")
				Response.Redirect("~/Admin/WebStubs.aspx");
			else
				Response.Redirect("~/Directory/UserWebApps.aspx");
		}
		#endregion
	}
}
