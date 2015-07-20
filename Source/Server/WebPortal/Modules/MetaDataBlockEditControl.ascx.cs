namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Xml;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.MetaDataPlus;
	using Mediachase.MetaDataPlus.Configurator;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules.EditControls;
	using System.Web.UI;
	using Mediachase.WebSaltatoryControl;
	using Mediachase.Ibn.Web.Interfaces;
	using System.Linq;
	using System.Collections.Generic;

	/// <summary>
	///		Summary description for MetaDataBlockEditControl.
	/// </summary>
	public partial class MetaDataBlockEditControl : System.Web.UI.UserControl
	{
		//protected System.Web.UI.WebControls.Label lblBlockName;

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(MetaDataBlockEditControl).Assembly);
		private int ObjectId = -1;
		private string MetaClassName = "";
		private int ListId = -1;

		protected Hashtable _mfhash = new Hashtable();
		protected ArrayList _mflist = new ArrayList();

		#region Path_Img
		private string path_img = "../";
		public string Path_Img
		{
			set
			{
				string ValueToSet = value;
				if( ValueToSet.Length > 0 )
				{
					if(ValueToSet.Substring(ValueToSet.Length-1,1) != "/") ValueToSet += "/";
				}
				path_img = ValueToSet;
			}
			get
			{
				return path_img;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LoadRequestVariables();

			ControlManager.RegisterProperyPage(this.Page, MetaClassName,
				this.ControlId,
				this.Index);

      BindToolBar();
			LoadSettings();
			BindCustomFields();

			btnCancel.Attributes.Add("onclick","javascript:window.close();");
			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["id"] != null)
			{
				try
				{
					ObjectId = int.Parse(Request.QueryString["id"]);
				}
				catch {}
			}

			if (Request.QueryString["class"] != null)
				MetaClassName = Request.QueryString["class"];

			if (Request.QueryString["ListId"] != null)
			{
				try
				{
					ListId = int.Parse(Request.QueryString["ListId"]);
				}
				catch {}
			}
		}

		protected string PageId
		{
			get
			{
				return Request.QueryString["class"];
			}
		}

		protected string ControlId
		{
			get
			{
				return Request.QueryString["ControlPlaceId"];
			}
		}

		protected int Index
		{
			get
			{
				return int.Parse(Request.QueryString["Index"]);
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

		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			//tbMetaInfo.Title = LocRM.GetString("tabMetaData");
			btnSave.CustomImage = ResolveUrl("~/Layouts/Images/saveitem.gif");
			btnCancel.CustomImage = ResolveUrl("~/Layouts/Images/Deny.gif");
		}
		#endregion

		#region BlockName
		private string blockname = "";
		public string BlockName
		{
			get
			{
				return blockname;
			}
			set
			{
				blockname = value;
				//lblBlockName.Text = blockname;
				tbMetaInfo.Title = LocRM.GetString("tMDEBlock") + ": " + blockname;
			}
		}
		#endregion

		#region LoadSettings
		public void LoadSettings()
		{
			XmlDocument settings = ControlManager.CurrentView.GetSettings(this.ControlId, this.Index);

			XmlNode nameNode = settings.SelectSingleNode("MetaDataBlockViewControl/Name");
			XmlNodeList mfNodeList = settings.SelectNodes("MetaDataBlockViewControl/MetaField");

			if(nameNode!=null)
				this.BlockName = nameNode.InnerText;
			else
				this.BlockName = string.Empty;
 
			string MetaClassName = ControlManager.CurrentView.Id;
			MetaClass mc = MetaClass.Load(MetaClassName);

			bool bContains;
			ArrayList delNodes = new ArrayList();

			foreach(XmlNode mfNode in mfNodeList)
			{
				bContains = false;
				foreach (MetaField field in mc.UserMetaFields)
				{
					if(field.Name == mfNode.InnerText)
					{
						_mfhash.Add(mfNode.InnerText,null);
						_mflist.Add(mfNode.InnerText);
						bContains = true;
					}
				}
				if(!bContains)
					delNodes.Add(mfNode);
			}
			
			for (int i=0; i<delNodes.Count; i++)
			{
				XmlNode mfNode = (System.Xml.XmlNode)delNodes[i];
				mfNode.ParentNode.RemoveChild(mfNode);
			}

			ControlManager.CurrentView.SetSettings(this.ControlId, this.Index, settings);
		}
		#endregion

		#region ContainsMetaField
		public bool ContainsMetaField(string metaFieldName)
		{
			return _mfhash.ContainsKey(metaFieldName);
		}
		#endregion

		#region BindCustomFields
		private void BindCustomFields()
		{
			MetaObject obj = null;
			if (ObjectId > 0)
				obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName);
			if (obj == null)
				obj = MetaDataWrapper.NewMetaObject(ObjectId, MetaClassName);

			MetaClass mc = obj.MetaClass;

			SortedList<int, MetaField> userMetaFields = new SortedList<int, MetaField>();
			foreach (MetaField field in mc.UserMetaFields)
			{
				if (ContainsMetaField(field.Name))
				{
					int cur_weight = _mflist.IndexOf(field.Name);
					userMetaFields.Add(cur_weight, field);
				}
			}

			foreach (MetaField field in userMetaFields.Values)
			{
				HtmlTableRow row = new HtmlTableRow();
				HtmlTableCell cellTitle = new HtmlTableCell();
				HtmlTableCell cellValue = new HtmlTableCell();

				cellTitle.VAlign = "middle";
				cellTitle.InnerHtml = String.Format("<b>{0}</b>:", field.FriendlyName);
				object fieldValue = obj[field.Name];
				System.Web.UI.UserControl control = null;

				switch(field.DataType) 
				{
					case MetaDataType.Binary:
						cellValue.InnerText = "[BinaryData]";
						break;
					case MetaDataType.File:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/FileValue.ascx");
						break;
					case MetaDataType.ImageFile:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/ImageFileValue.ascx");
						break;
					case MetaDataType.DateTime:
						//control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DateTimeValue.ascx");
						Mediachase.UI.Web.Modules.EditControls.DateTimeValue control_datetime = (Mediachase.UI.Web.Modules.EditControls.DateTimeValue)Page.LoadControl("~/Modules/EditControls/DateTimeValue.ascx");
						control_datetime.Path_JS = "../../Scripts/";
						control = (System.Web.UI.UserControl)control_datetime;
						break;
					case MetaDataType.Money:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/MoneyValue.ascx");
						break;
					case MetaDataType.Float:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/FloatValue.ascx");
						break;
					case MetaDataType.Integer:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/IntValue.ascx");
						break;
					case MetaDataType.Boolean:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/BooleanValue.ascx");
						break;
					case MetaDataType.Date:
						//control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DateValue.ascx");
						Mediachase.UI.Web.Modules.EditControls.DateValue control_date = (Mediachase.UI.Web.Modules.EditControls.DateValue)Page.LoadControl("~/Modules/EditControls/DateValue.ascx");
						control_date.Path_JS = "../../Scripts/";
						control = (System.Web.UI.UserControl)control_date;
						break;
					case MetaDataType.Email:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/EmailValue.ascx");
						break;
					case MetaDataType.Url:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/URLValue.ascx");
						break;
					case MetaDataType.ShortString:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/ShortStringValue.ascx");
						break;
					case MetaDataType.LongString:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/LongStringValue.ascx");
						break;
					case MetaDataType.LongHtmlString:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/LongHTMLStringValue.ascx");
						break;
					case MetaDataType.DictionarySingleValue:
					case MetaDataType.EnumSingleValue:
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DictionarySingleValue.ascx");
						((DictionarySingleValue)control).InitControl(field.Id, (field.AllowNulls ? !field.IsRequired : field.AllowNulls));
						break;
					case MetaDataType.DictionaryMultivalue:
					case MetaDataType.EnumMultivalue:
						cellTitle.VAlign = "top";
						control = (System.Web.UI.UserControl)Page.LoadControl("~/Modules/EditControls/DictionaryMultivalue.ascx");
						((DictionaryMultivalue)control).InitControl(field.Id);
						break;
					default:
						if (fieldValue != null)
							cellValue.InnerText = fieldValue.ToString();
						break;
				}

				if (control != null)
				{
					cellValue.Controls.Add(control);
				
				}
			
				row.Cells.Add(cellTitle);
				row.Cells.Add(cellValue);

				tblCustomFields.Rows.Add(row);

				if (control != null)
				{
					ICustomField iCustomField = ((ICustomField)control);
					iCustomField.FieldName = field.Name;
					if (fieldValue != null)
						iCustomField.Value = fieldValue;
					iCustomField.AllowEmptyValues = !mc.GetFieldIsRequired(field);
				}
			}
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			
			MetaObject obj = null;
			if (ObjectId > 0)
				obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName, Security.CurrentUser.UserID, DateTime.UtcNow);
			if (obj == null)
				obj = MetaDataWrapper.NewMetaObject(ObjectId, MetaClassName);

			foreach (HtmlTableRow row in tblCustomFields.Rows)
			{
				HtmlTableCell cell = row.Cells[1];
				ICustomField ctrl = (ICustomField)cell.Controls[0];
				object FieldValue = ctrl.Value;
				string FieldName = ctrl.FieldName;

				obj[FieldName] = FieldValue;
			}
			
			ObjectId = MetaDataWrapper.AcceptChanges(obj);

      Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script language=javascript>"+
				"try {var str=window.opener.location.href;"+
				"window.opener.location.href=str;}" +
				"catch (e){} window.close();</script>");

		}
		#endregion

	}
}
