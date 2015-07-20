using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Web;
using System.Web.UI.WebControls;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Directory.Modules
{
	/// <summary>
	/// Summary description for AddGroup.
	/// </summary>
	public partial class AddIMGroup : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblGroupTitle;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strAddIMGroup", typeof(AddIMGroup).Assembly);

		System.Drawing.Image img = null;

		#region GroupID
		private int GroupID
		{
			get
			{
				try
				{
					return int.Parse(Request["GroupID"]);
				}
				catch
				{
					return 0;
				}
			}
		} 
		#endregion

		#region CloneGroupID
		private int CloneGroupID
		{
			get
			{
				try
				{
					return int.Parse(Request["CloneGroupID"]);
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
			rfGroupTitle.ErrorMessage = LocRM.GetString("Reqired");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			cvImage.Text = LocRM.GetString("ImageSizeWarning");
			BindToolBar();
			if (!IsPostBack)
			{
				imgLogo.Src = this.Page.ResolveClientUrl(GlobalResourceManager.Strings["IMGroupLogoUrl"]);
				BindDefaultValues();
				if (GroupID > 0 || CloneGroupID > 0)
					BindSavedData();
			}
		}

		#region BindToolBar
		private void BindToolBar()
		{
			if (GroupID > 0)
				secHeader.Title = LocRM.GetString("EditGroup");
			else if (CloneGroupID > 0)
				secHeader.Title = LocRM.GetString("CloneGroup");
			else
			{
				using (DataTable table = IMGroup.GetGroup(GroupID))
				{
					if (table.Rows.Count > 0)
						secHeader.Title = LocRM.GetString("CreateIMGroup");
				}
			}

			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Back"), "../Directory/Directory.aspx");
		} 
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			int groupId = GroupID;
			int cloneGroupID = CloneGroupID;

			tbGroupTitle.Text = "";
			tbColor.Text = "2B6087";

			int id = 0;
			if (groupId > 0)
				id = groupId;
			else if (cloneGroupID > 0)
				id = cloneGroupID;
			imgLogo.Src = "../../Common/GroupLogo.aspx?GroupID=" + id;


			using (DataTable table = IMGroup.GetListIMGroup())
			{
				foreach (DataRow row in table.Rows)
				{
					int imGroupId = (int)row["IMGroupId"];
					string imGroupName = row["IMGroupName"].ToString();

					if (groupId == 0 || groupId != imGroupId)
					{
						string imGroupIdString = imGroupId.ToString(CultureInfo.InvariantCulture);

						lbVisible.Items.Add(new ListItem(imGroupName, imGroupIdString));
						lbCU.Items.Add(new ListItem(imGroupName, imGroupIdString));
					}
				}
			}
		} 
		#endregion

		#region BindSavedData
		private void BindSavedData()
		{
			int id = 0;
			if (GroupID > 0)
				id = GroupID;
			else if (CloneGroupID > 0)
				id = CloneGroupID;

			if (id > 0)
			{
				using (DataTable table = IMGroup.GetGroup(id))
				{
					if (table.Rows.Count > 0)
					{
						DataRow row = table.Rows[0];

						if (CloneGroupID == 0)
							tbGroupTitle.Text = HttpUtility.HtmlDecode(row["IMGroupName"].ToString());
						tbColor.Text = HttpUtility.HtmlDecode(row["Color"].ToString());
					}
				}

				using (DataTable table = IMGroup.GetListIMGroupsYouCanSee(id))
				{
					foreach (DataRow row in table.Rows)
					{
						CommonHelper.SafeMultipleSelect(lbVisible, ((int)row["IMGroupId"]).ToString(CultureInfo.InvariantCulture));
					}
				}

				using (DataTable table = IMGroup.GetListIMGroupsCanSeeYou(id))
				{
					foreach (DataRow row in table.Rows)
					{
						CommonHelper.SafeMultipleSelect(lbCU, ((int)row["IMGroupId"]).ToString(CultureInfo.InvariantCulture));
					}
				}
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			ArrayList YCS = new ArrayList();
			foreach (ListItem li in lbVisible.Items)
				if (li.Selected) YCS.Add(int.Parse(li.Value));

			ArrayList CSY = new ArrayList();
			foreach (ListItem li in lbCU.Items)
				if (li.Selected) CSY.Add(int.Parse(li.Value));

			byte[] GroupLogo = null;
			if (fLogo.PostedFile != null && fLogo.PostedFile.ContentLength > 0)
			{
				GroupLogo = new byte[fLogo.PostedFile.ContentLength];
				fLogo.PostedFile.InputStream.Read(GroupLogo, 0, fLogo.PostedFile.ContentLength);
			}
			else if (GroupID == 0)
			{
				FileInfo fi = new FileInfo(Server.MapPath(GlobalResourceManager.Strings["IMGroupLogoUrl"]));
				FileStream fs = fi.OpenRead();
				GroupLogo = new byte[fi.Length];
				int nBytesRead = fs.Read(GroupLogo, 0, (int)fi.Length);
			}

			int outid = 1;
			if (GroupID > 0)
			{
				IMGroup.Update(GroupID, tbGroupTitle.Text, tbColor.Text, GroupLogo, YCS, CSY);
				outid = GroupID;
			}
			else if (CloneGroupID > 0)
				outid = IMGroup.Clone(CloneGroupID, tbGroupTitle.Text, tbColor.Text, GroupLogo, YCS, CSY);
			else
				outid = IMGroup.Create(tbGroupTitle.Text, tbColor.Text, GroupLogo, YCS, CSY);

			if (outid != -1)
				Response.Redirect("../Directory/Directory.aspx?Tab=1&IMGroupID=0");
		} 
		#endregion

		#region btnCancel_Click
		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("../Directory/Directory.aspx?Tab=1&IMGroupID=0");
		} 
		#endregion

		#region cvImage_ServerValidate
		protected void cvImage_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;
			if (fLogo.PostedFile != null && fLogo.PostedFile.ContentLength > 0)
			{
				args.IsValid = false;
				try
				{
					img = System.Drawing.Image.FromStream(fLogo.PostedFile.InputStream);
					if (img.Width == 227 && img.Height == 36)
						args.IsValid = true;
				}
				catch
				{
				}

				fLogo.PostedFile.InputStream.Position = 0;
			}
		} 
		#endregion
	}
}
