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
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for ImageFileValue.
	/// </summary>
	public partial class ImageFileValue : System.Web.UI.UserControl, ICustomField
	{
		private bool _pictExists = false;
		private ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(ImageFileValue).Assembly);

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

		string metaClassName = String.Empty;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnDelete.Text = String.Format("<img src='{0}' width='16px' height='16px' align='middle' border='0' />",
				ResolveClientUrl("~/layouts/images/delete.gif"));
			btnDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");

			fAssetFile.Attributes.Add("onkeypress", String.Format("fPhotochange(this, {0});", imgPhoto.ClientID));
			fAssetFile.Attributes.Add("onpropertychange", String.Format("fPhotochange(this, {0});", imgPhoto.ClientID));
			fAssetFile.Attributes.Add("onclick", String.Format("fPhotochange(this, {0});", imgPhoto.ClientID));

			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "Photochange"))
			{
				string rn = "\r\n";
				string script = @"<script language=javascript>" + rn +
					"function fPhotochange(file, img)" + rn +
					"{" + rn +
					"	var strFile = file.value;" + rn +
					"	if (strFile != \"\")" + rn +
					"		img.src = \"file:///\" + strFile;" + rn +
					"}" + rn +
					"</script>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Photochange", script);
			}
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
					imgPhoto.Src = String.Format("{0}?Id={1}&Class={2}&Field={3}",
						"~/Modules/GetMetaImageFile.aspx",
						ObjectId.HasValue ? ObjectId.Value.ToString() : Request.QueryString["id"], 
						!String.IsNullOrEmpty(MetaClassName) ? MetaClassName : Request.QueryString["class"], 
						FieldName);
					btnDelete.Visible = true;
					_pictExists = true;
				}
				else
				{
					imgPhoto.Src = "~/layouts/images/transparentpoint.gif";
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
				else if (imgPhoto.Src.IndexOf("GetMetaImageFile") >= 0)
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
			imgPhoto.Src = "~/layouts/images/transparentpoint.gif";
			btnDelete.Visible = false;
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (!AllowEmptyValues)
			{
				btnDelete.Visible = false;

				rfPhoto.Visible = !_pictExists;
			}

			if (String.IsNullOrEmpty(imgPhoto.Src))
				imgPhoto.Src = "~/layouts/images/transparentpoint.gif";
		}
		#endregion
	}
}
