using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.IBN.Business;
using System.Resources;
using Mediachase.IBN.Business.WebDAV.Common;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Image_Edit : System.Web.UI.UserControl, IEditControl
	{
		private readonly int maxImageSize = 10000;

		#region FileUid
		protected Guid? FileUid
		{
			set
			{
				ViewState["FileUid"] = value;
			}
			get
			{
				Guid? retval = null;
				if (ViewState["FileUid"] != null)
					retval = (Guid)ViewState["FileUid"];
				return retval;
			}
		}
		#endregion

		#region Width
		protected int Width
		{
			get
			{
				int retval = -1;
				if (ViewState["Width"] != null)
					retval = (int)ViewState["Width"];
				return retval;
			}
			set
			{
				ViewState["Width"] = value;
			}
		}
		#endregion

		#region Height
		protected int Height
		{
			get
			{
				int retval = -1;
				if (ViewState["Height"] != null)
					retval = (int)ViewState["Height"];
				return retval;
			}
			set
			{
				ViewState["Height"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(Image_Edit).Assembly);
			btnDelete.Text = String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' width='16px' height='16px' align='middle' border='0' />", ResolveClientUrl("~/layouts/images/delete.gif"));
			btnDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");

			fAssetFile.Attributes.Add("onkeypress", String.Format("Photochange(this, '{0}');", imgPhoto.ClientID));
			fAssetFile.Attributes.Add("onpropertychange", String.Format("Photochange(this, '{0}');", imgPhoto.ClientID));
			fAssetFile.Attributes.Add("onclick", String.Format("Photochange(this, '{0}');", imgPhoto.ClientID));

			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "Photochange"))
			{
				string rn = "\r\n";
				string script = @"<script language=javascript>" + rn +
					"function Photochange(file, img)" + rn +
					"{" + rn +
					"	var strFile = file.value;" + rn +
					"	if (strFile != \"\")" + rn +
					"		img.src = \"file:///\" + strFile;" + rn +
					"}" + rn +
					"</script>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Photochange", script);
			}
 		}

		#region IEditControl Members

		#region AllowNulls
		public bool AllowNulls
		{
			get
			{
				bool retval = true;
				if (ViewState["AllowNulls"] != null)
					retval = (bool)ViewState["AllowNulls"];
				return retval;
			}
			set
			{
				ViewState["AllowNulls"] = value;
			}
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
			if (field.Attributes.ContainsKey(McDataTypeAttribute.ImageWidth))
				Width = (int)field.Attributes[McDataTypeAttribute.ImageWidth];
			if (field.Attributes.ContainsKey(McDataTypeAttribute.ImageHeight))
				Height = (int)field.Attributes[McDataTypeAttribute.ImageHeight];
		}
		#endregion

		#region FieldName
		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		}
		#endregion

		#region Label
		public string Label
		{
			get
			{
				return "";
			}
			set
			{

			}
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			get
			{
				return "";
			}
			set
			{

			}
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			get
			{
				return !fAssetFile.Enabled;
			}
			set
			{
				fAssetFile.Enabled = !value;
			}
		}
		#endregion

		#region RowCount
		public int RowCount
		{
			get
			{
				return 1;
			}
			set
			{

			}
		}
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			get
			{
				return false;
			}
			set
			{

			}
		}
		#endregion

		#region Value
		public object Value
		{
			set
			{
				if (value != null)
				{
					FileInfo fi = (FileInfo)value;
					FileUid = fi.FileUID;

                    //imgPhoto.Src = String.Format(CultureInfo.InvariantCulture,
                    //    "{0}?FileUID={1}&mode=image",
                    //    ResolveClientUrl("~/Apps/MetaUI/Pages/Public/DownloadFile.aspx"),
                    //    fi.FileUID.ToString());
                    imgPhoto.Src = String.Format(CultureInfo.InvariantCulture, "{0}", 
                                                 WebDavUrlBuilder.GetMetaDataWebDavUrl(fi.FileUID, true));
					btnDelete.Visible = true;
				}
				else
				{
					imgPhoto.Src = "~/layouts/images/transparentpoint.gif";
					btnDelete.Visible = false;
				}
			}
			get
			{
				FileInfo retval = null;
				if (fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0)
				{
					// Check the image size and resize if need
					if (Width > 0 || Height > 0)
					{
						int w = (Width > 0) ? Width : maxImageSize;
						int h = (Height > 0) ? Height : maxImageSize;

						string extension = "";

						// O.R. [2010-04-06] Non-image file fix.
						try
						{
							System.Drawing.Image img = Images.ProcessImage(fAssetFile.PostedFile, w, h, out extension);
							string filename = fAssetFile.PostedFile.FileName.Substring(0, fAssetFile.PostedFile.FileName.LastIndexOf(".")) + extension;

							System.IO.Stream mem = new System.IO.MemoryStream();
							img.Save(mem, img.RawFormat);
							mem.Position = 0;

							retval = new FileInfo(filename, mem);
						}
						catch{}
					}
					else
					{
						retval = new FileInfo(fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream);
					}
				}
				else if (FileUid != null)
				{
					retval = new FileInfo(FileUid.Value);
				}
				return retval;
			}
		}
		#endregion

		#region TabIndex
		public short TabIndex
		{
			set
			{
				fAssetFile.TabIndex = value;
			}
		}
		#endregion
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (!AllowNulls)
			{
				btnDelete.Visible = false;

				if (!FileUid.HasValue)
					rfPhoto.Visible = true;
			}

			if (String.IsNullOrEmpty(imgPhoto.Src))
				imgPhoto.Src = "~/layouts/images/transparentpoint.gif";
		}
		#endregion

		#region btnDelete_Click
		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			FileUid = null;
			btnDelete.Visible = false;
			imgPhoto.Src = "~/layouts/images/transparentpoint.gif";
		}
		#endregion
	}
}