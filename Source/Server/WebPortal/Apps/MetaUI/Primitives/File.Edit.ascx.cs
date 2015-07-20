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
	public partial class File_Edit : System.Web.UI.UserControl, IEditControl
	{
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

		protected void Page_Load(object sender, EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(File_Edit).Assembly);
			btnDelete.Text = String.Format(CultureInfo.InvariantCulture, 
				"<img src='{0}' width='16px' height='16px' align='middle' border='0' />", ResolveClientUrl("~/layouts/images/delete.gif"));
			btnDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
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

					// ============ Hard-coded for 4.6
					// ToDo: Unify for common functionality
					int contentTypeId = DSFile.GetContentTypeByFileName(fi.Name);
					string text = String.Format(CultureInfo.InvariantCulture,
						"<img src='{0}?IconID={1}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{2}",
						ResolveClientUrl("~/Common/ContentIcon.aspx"), contentTypeId, fi.Name);
					//================

                    //lblLink.Text = String.Format(CultureInfo.InvariantCulture,
                    //    "<a href='{0}?FileUID={1}'>{2}</a>",
                    //    ResolveClientUrl("~/Apps/MetaUI/Pages/Public/DownloadFile.aspx"),
                    //    fi.FileUID.ToString(),
                    //    text);
					string sLink = WebDavUrlBuilder.GetMetaDataWebDavUrl(fi.FileUID, true);

					string sNameLocked = Mediachase.UI.Web.Util.CommonHelper.GetLockerText(sLink);
					
					lblLink.Text = String.Format(CultureInfo.InvariantCulture, "<a href='{0}'{3}>{1}</a> {2}",
										sLink, text, sNameLocked,
										Mediachase.IBN.Business.Common.OpenFileInNewWindow(fi.Name) ? " target='_blank'" : "");
					lblLink.Visible = true;
					btnDelete.Visible = true;
				}
				else
				{
					lblLink.Text = "";
					lblLink.Visible = false;
					btnDelete.Visible = false;
				}
			}
			get
			{
				FileInfo retval = null;
				if (fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0)
				{
					retval = new FileInfo(fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream);
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
					rfFile.Visible = true;
			}
		}
		#endregion

		#region btnDelete_Click
		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			FileUid = null;
			btnDelete.Visible = false;
			lblLink.Visible = false;
		}
		#endregion
	}
}