namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.Interfaces;

	public partial class DocumentTypesEdit : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDocTypes", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region iconId
		private int iconId
		{
			get
			{
				try
				{
					if (Request["ContentTypeID"] != null)
						return int.Parse(Request["ContentTypeID"]);
					else return -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (iconId > 0)
					BindSavedValues();
			}

			DataBind();

			btnCancel.Text = LocRM.GetString("Cancel");
			btnSubmit.Text = LocRM.GetString("Save");
			cbAllowWebDav.Text = "&nbsp;" + LocRM.GetString("AllowWebDav");
			cbInNewWindow.Text = "&nbsp;" + LocRM.GetString("AllowNewWindow");
			cbForceDownload.Text = "&nbsp;" + LocRM.GetString("AllowForceDownload");
			if (iconId != -1)
				secHeader.Title = LocRM.GetString("TbEdit");
			else
				secHeader.Title = LocRM.GetString("TbNew");

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSubmit.Attributes.Add("onclick", "DisableButtons(this);");
			btnSubmit.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region BindSavedValues
		private void BindSavedValues()
		{
			using (IDataReader rdr = ContentType.GetContentType(iconId))
			{
				if (rdr.Read())
				{
					if (rdr["IconFileId"] != DBNull.Value && (int)rdr["IconFileId"] > 0)
						imgIcon.ImageUrl = String.Concat("~/Common/ContentIcon.aspx?IconID=", iconId);

					if (rdr["BigIconFileId"] != DBNull.Value && (int)rdr["BigIconFileId"] > 0)
						imgBigIcon.ImageUrl = String.Concat("~/Common/ContentIcon.aspx?Big=1&IconID=", iconId);

					tbExtension.Visible = false;
					tbMimeType.Visible = false;
					rfExtension.Visible = false;
					rfMimeType.Visible = false;

					lblExtension.Text = rdr["Extension"].ToString();
					lblContentType.Text = rdr["ContentTypeString"].ToString();
					tbFriendlyName.Text = rdr["FriendlyName"].ToString();
					cbAllowWebDav.Checked = (bool)rdr["AllowWebDav"];
					cbInNewWindow.Checked = (bool)rdr["AllowNewWindow"];
					cbForceDownload.Checked = (bool)rdr["AllowForceDownload"];
				}
			}
		}
		#endregion

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			string iconFileName = null;
			string bigIconFileName = null;
			Stream iconStream = null;
			Stream bigIconStream = null;

			if (fIcon.PostedFile != null && fIcon.PostedFile.ContentLength > 0)
			{
				iconFileName = fIcon.PostedFile.FileName;
				iconStream = fIcon.PostedFile.InputStream;
			}
			if (fBigIcon.PostedFile != null && fBigIcon.PostedFile.ContentLength > 0)
			{
				bigIconFileName = fBigIcon.PostedFile.FileName;
				bigIconStream = fBigIcon.PostedFile.InputStream;
			}

			if (iconId > 0)
				ContentType.Update(iconId, tbFriendlyName.Text,
					iconFileName, iconStream,
					bigIconFileName, bigIconStream,
					cbAllowWebDav.Checked, cbInNewWindow.Checked, cbForceDownload.Checked);
			else
				ContentType.Create(tbExtension.Text, tbMimeType.Text, tbFriendlyName.Text,
					iconFileName, iconStream,
					bigIconFileName, bigIconStream,
					cbAllowWebDav.Checked, cbInNewWindow.Checked, cbForceDownload.Checked);

			Response.Redirect("~/Admin/DocumentTypes.aspx");
		}

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/DocumentTypes.aspx");
		}

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			if (iconId != -1)
				return LocRM.GetString("TbEdit");
			else
				return LocRM.GetString("TbNew");
		}
		#endregion
	}
}
