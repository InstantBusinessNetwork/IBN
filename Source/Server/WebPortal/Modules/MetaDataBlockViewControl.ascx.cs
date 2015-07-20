namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.MetaDataPlus;
	using Mediachase.MetaDataPlus.Configurator;
	using Mediachase.IBN.Business;
	using Mediachase.WebSaltatoryControl;
	using Mediachase.IBN.Business.WebDAV.Common;
	using System.Globalization;

	/// <summary>
	///		Summary description for MetaDataViewControl.
	/// </summary>
	public partial class MetaDataBlockViewControl : System.Web.UI.UserControl, IPersistControl, IPersistPropertyControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(MetaDataBlockViewControl).Assembly);
		
		protected string _name = string.Empty;
		protected Hashtable _mfhash = new Hashtable();
		protected ArrayList _mflist = new ArrayList();

		#region ClassName
		public string ClassName
		{
			get
			{
				return ControlManager.CurrentView.Id;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				if (ControlManager.CurrentView == null ||
					!ControlManager.CurrentView.CustomizeEnabled)
					BindCustomFields();
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

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			//			BindToolBar();
		}
		#endregion

		#region BindCustomFields
		private void BindCustomFields()
		{
			int ObjectId = GetObjectId();
			string MetaClassName = GetMetaClassName();
			MetaObject obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName);
			if (obj != null)
			{
				int cur_weight = 0;
				for (int irow = 0; irow < _mflist.Count; irow++)
					tblCustomFields.Rows.Add(new HtmlTableRow());
				foreach (MetaField field in obj.MetaClass.UserMetaFields)
				{
					if (ContainsMetaField(field.Name))
					{
						cur_weight = _mflist.IndexOf(field.Name);
						HtmlTableRow row = new HtmlTableRow();
						HtmlTableCell cellTitle = new HtmlTableCell();
						HtmlTableCell cellValue = new HtmlTableCell();

						cellTitle.NoWrap = true;
						cellTitle.VAlign = "Top";
						cellTitle.InnerHtml = String.Format("<span class=ibn-label>{0}:</span>", field.FriendlyName);
						cellValue.Width = "90%";
						object fieldValue = obj[field.Name];

						if (fieldValue != null)
						{
							switch (field.DataType)
							{
								case MetaDataType.File:
									Mediachase.MetaDataPlus.MetaFile mf = (Mediachase.MetaDataPlus.MetaFile)fieldValue;
									int ContentTypeId = DSFile.GetContentTypeByFileName(mf.Name);
									string sInnerHTML = "<img src='../Common/ContentIcon.aspx?IconID=" +
										+ContentTypeId +
										"' border='0' align='middle' width='16px' height='16px' />&nbsp;" + mf.Name;

									string metaFileUrl = WebDavUrlBuilder.GetMetaDataPlusWebDavUrl(ObjectId, MetaClassName, field.Name, true);
									string sNameLocked = Util.CommonHelper.GetLockerText(metaFileUrl);

									cellValue.InnerHtml = String.Format("<a href='{0}'>{1}</a> {2}", metaFileUrl, sInnerHTML, sNameLocked);

									//cellValue.InnerHtml = "<a href='../Modules/DownloadMetaFile.aspx?Id="+ObjectId.ToString()+
									//    "&Class="+MetaClassName+"&Field="+field.Name+"'>"+sInnerHTML+"</a>";
									break;
								case MetaDataType.ImageFile:
									cellValue.InnerHtml = "<img align='middle' border='0' src='../Modules/GetMetaImageFile.aspx?Id=" + ObjectId.ToString() +
										"&Class=" + MetaClassName + "&Field=" + field.Name + "' />";
									break;
								case MetaDataType.Binary:
								case MetaDataType.VarBinary:
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", "[BinaryData]");
									break;
								case MetaDataType.Bit:
								case MetaDataType.Boolean:
									if ((bool)fieldValue)
										cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", LocRM.GetString("BooleanYes"));
									else
										cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", LocRM.GetString("BooleanNo"));
									break;
								case MetaDataType.Date:
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", ((DateTime)fieldValue).ToShortDateString());
									break;
								case MetaDataType.Email:
									cellValue.InnerHtml = String.Format("<a href='mailto:{0}'>{0}</a>", fieldValue.ToString());
									break;
								case MetaDataType.Image:
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", "[Image]");
									break;
								case MetaDataType.LongHtmlString:
									cellValue.InnerHtml = String.Format("<span class=ibn-description>{0}</span>", fieldValue.ToString());
									break;
								case MetaDataType.DateTime:
								case MetaDataType.SmallDateTime:
								case MetaDataType.Timestamp:
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0} {1}</span>", ((DateTime)fieldValue).ToShortDateString(), ((DateTime)fieldValue).ToShortTimeString());
									break;
								case MetaDataType.Url:
									cellValue.InnerHtml = String.Format("<a href='{0}' target='_blank'>{0}</a>", Server.UrlDecode(fieldValue.ToString()));
									break;
								case MetaDataType.Money:
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", ((decimal)fieldValue).ToString("f"));
									break;
								case MetaDataType.Float:
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", fieldValue.ToString());
									break;
								case MetaDataType.EnumSingleValue:
								case MetaDataType.DictionarySingleValue:
									MetaDictionaryItem item = (MetaDictionaryItem)fieldValue;
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", item.Value);
									break;
								case MetaDataType.EnumMultivalue:
								case MetaDataType.DictionaryMultivalue:
									MetaDictionaryItem[] items = (MetaDictionaryItem[])fieldValue;
									string sItems = String.Empty;
									foreach (MetaDictionaryItem mdItem in items)
									{
										if (sItems != "")
											sItems += "<br>";
										sItems += mdItem.Value;
									}
									sItems = String.Format(CultureInfo.InvariantCulture, 
										"<span class=ibn-value>{0}</span>",
										sItems);
									cellValue.InnerHtml = sItems;
									break;
								default:
									try
									{
										cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", fieldValue.ToString());
									}
									catch
									{
										cellValue.InnerHtml = "";
									}
									break;
							}
						}

						cellTitle.Attributes.Add("class", "text");
						cellValue.Attributes.Add("class", "text");
						row.Cells.Add(cellTitle);
						row.Cells.Add(cellValue);

						//tblCustomFields.Rows.Add(row);
						tblCustomFields.Rows.Insert(cur_weight, row);
						tblCustomFields.Rows.RemoveAt(cur_weight + 1);
					}
				}
			}
		}
		#endregion

		#region GetObjectId()
		private int GetObjectId()
		{
			string ObjectName = "";
			if (this.ClassName.StartsWith("ProjectsEx_"))
				ObjectName = "ProjectId";//ListsEx_
			if (this.ClassName.StartsWith("ListsEx_"))
				ObjectName = "ID";
			switch (this.ClassName)
			{
				case "UsersEx":
					ObjectName = "UserId";
					break;
				case "IncidentsEx":
					ObjectName = "IncidentId";
					break;
				case "DocumentsEx":
					ObjectName = "DocumentId";
					break;
				case "TaskEx":
					ObjectName = "TaskId";
					break;
				case "ToDoEx":
					ObjectName = "ToDoId";
					break;
				case "EventsEx":
					ObjectName = "EventId";
					break;
				case "PortfolioEx":
					ObjectName = "ProjectGroupId";
					break;
			}
			if (this.ClassName == "UsersEx" && Request.QueryString[ObjectName] == null)
				return Mediachase.IBN.Business.Security.CurrentUser.UserID;
			else
				return int.Parse(Request.QueryString[ObjectName]);
		}
		#endregion

		#region GetMetaClassName()
		private string GetMetaClassName()
		{
			return this.ClassName;
		}
		#endregion

		#region IPersistControl Members

		public void LoadSettings(System.Xml.XmlDocument settings)
		{
			XmlNode nameNode = settings.SelectSingleNode("MetaDataBlockViewControl/Name");
			XmlNodeList mfNodeList = settings.SelectNodes("MetaDataBlockViewControl/MetaField");

			if (nameNode != null)
				_name = nameNode.InnerText;

			string MetaClassName = GetMetaClassName();
			MetaClass mc = MetaClass.Load(MetaClassName);

			//bool bContains;
			//ArrayList delNodes = new ArrayList();

			foreach (XmlNode mfNode in mfNodeList)
			{
				//bContains = false;
				foreach (MetaField field in mc.UserMetaFields)
				{
					if (field.Name == mfNode.InnerText)
					{
						_mfhash.Add(mfNode.InnerText, null);
						_mflist.Add(mfNode.InnerText);
						//bContains = true;
					}
				}
				//if(!bContains)
				//	delNodes.Add(mfNode);
			}
			/*			
			for (int i=0; i<delNodes.Count; i++)
			{
				XmlNode mfNode = (System.Xml.XmlNode)delNodes[i];
				mfNode.ParentNode.RemoveChild(mfNode);
			}

			ControlManager.CurrentView.SetSettings(this.ClassName, this.Index, settings);
			*/
		}

		public bool ContainFileds
		{
			get
			{
				if (_mfhash.Count > 0)
					return true;
				return false;
			}
		}

		public void SaveSettings(System.Xml.XmlDocument settings)
		{

		}

		#endregion

		public string Name
		{
			get
			{
				string retVal = _name;
				if (retVal == string.Empty)
					retVal = "&nbsp;";
				return retVal;
			}
		}

		public string Description
		{
			get
			{

				string retVal = string.Empty;

				//int ObjectId = GetObjectId();
				string MetaClassName = GetMetaClassName();
				//MetaObject obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName);
				MetaClass mc = MetaClass.Load(MetaClassName);
				if (mc != null)
				{
					ArrayList list_fields = new ArrayList();
					for (int iweight = 0; iweight < _mflist.Count; iweight++)
						list_fields.Add("temp_field");
					int cur_weight = 0;

					foreach (MetaField field in mc.UserMetaFields)
					{
						if (ContainsMetaField(field.Name))
						{
							cur_weight = _mflist.IndexOf(field.Name);
							list_fields.Insert(cur_weight, field.FriendlyName);
							list_fields.RemoveAt(cur_weight + 1);
							/*
							if(retVal != string.Empty)
								retVal += ", ";
							retVal += field.FriendlyName;
							*/
						}
					}

					foreach (object field in list_fields)
					{
						if (retVal != string.Empty)
							retVal += ", ";
						retVal += (string)field;
					}
				}

				if (retVal == string.Empty)
					retVal = "&nbsp;";

				retVal = "<span class='text'>" + retVal + "</span>";
				return retVal;
			}
		}

		public bool ContainsMetaField(string metaFieldName)
		{
			return _mfhash.ContainsKey(metaFieldName);
		}

	}
}
