namespace Mediachase.UI.Web.Modules.EditControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.MetaDataPlus;
	using System.Resources;
	using Mediachase.IBN.Business.WebDAV.Common;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for FileValue.
	/// </summary>
	public partial class FileValue : System.Web.UI.UserControl, ICustomField
	{
		private bool FileExists = false;

		#region ObjectId
		public int? ObjectId
		{
			set
			{
				ViewState["ObjectId"] = value;
			}
			get
			{
				int? retval = null;
				if (ViewState["ObjectId"] != null)
					retval = (int)ViewState["ObjectId"];
				return retval;
			}
		}
		#endregion

		#region MetaClassName
		public string MetaClassName
		{
			set
			{
				ViewState["MetaClassName"] = value;
			}
			get
			{
				string retval = null;
				if (ViewState["MetaClassName"] != null)
					retval = ViewState["MetaClassName"].ToString();
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(FileValue).Assembly);
			btnDelete.Text = String.Format("<img src='{0}' width='16px' height='16px' align='middle' border='0' />", ResolveUrl("~/layouts/images/delete.gif"));
			btnDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
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
		}
		#endregion

		#region Implementation of ICustomField
		public object Value
		{
			set
			{
				if (value != null)
				{
					int ContentTypeId = DSFile.GetContentTypeByFileName(((Mediachase.MetaDataPlus.MetaFile)value).Name);
					string sInnerHTML = String.Format("<img src='{2}?IconID={0}' border='0' align='middle' width='16px' height='16px' />&nbsp;{1}",
						ContentTypeId, ((Mediachase.MetaDataPlus.MetaFile)value).Name,
						ResolveUrl("~/Common/ContentIcon.aspx"));

					string metaFileUrl = WebDavUrlBuilder.GetMetaDataPlusWebDavUrl(
						ObjectId.HasValue ? ObjectId.Value : Convert.ToInt32(Request.QueryString["id"]),
						!String.IsNullOrEmpty(MetaClassName) ? MetaClassName : Request.QueryString["class"], 
						FieldName, 
						true);

					string sNameLocked = Util.CommonHelper.GetLockerText(metaFileUrl);

					lblLink.Text = String.Format("<a href='{0}'>{1}</a> {2}", metaFileUrl, sInnerHTML, sNameLocked);
					lblLink.Visible = true;
					btnDelete.Visible = true;
					FileExists = true;
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
				if (fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0)
				{
					byte[] _Buffer = new byte[fAssetFile.PostedFile.ContentLength];
					fAssetFile.PostedFile.InputStream.Read(_Buffer, 0, fAssetFile.PostedFile.ContentLength);
					Mediachase.MetaDataPlus.MetaFile mf = new Mediachase.MetaDataPlus.MetaFile(fAssetFile.PostedFile.FileName, "", _Buffer);
					return mf;
				}
				else if (lblLink.Visible)
				{
					int objectId = ObjectId.HasValue ? ObjectId.Value : Convert.ToInt32(Request.QueryString["id"]);
					string metaClassName = !String.IsNullOrEmpty(MetaClassName) ? MetaClassName : Request.QueryString["class"];
					MetaObject obj = MetaDataWrapper.LoadMetaObject(objectId, metaClassName);
					return obj[FieldName];
				}
				else
					return null;
			}
		}

		private string fieldName = "";
		public string FieldName
		{
			set
			{
				fieldName = value;
			}
			get
			{
				return fieldName;
			}
		}

		private bool allowEmptyValues = false;
		public bool AllowEmptyValues
		{
			set
			{
				allowEmptyValues = value;
			}
			get
			{
				return allowEmptyValues;
			}
		}
		#endregion

		#region btnDelete_Click
		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			int objectId = ObjectId.HasValue ? ObjectId.Value : Convert.ToInt32(Request.QueryString["id"]);
			string metaClassName = !String.IsNullOrEmpty(MetaClassName) ? MetaClassName : Request.QueryString["class"];
			MetaObject obj = MetaDataWrapper.LoadMetaObject(objectId, metaClassName);
			obj[FieldName] = null;
			MetaDataWrapper.AcceptChanges(obj);
			Value = null;
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (!AllowEmptyValues)
			{
				btnDelete.Visible = false;

				rfFile.Visible = !FileExists;
			}
		}
		#endregion
	}
}
