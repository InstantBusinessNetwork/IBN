namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.MetaDataPlus;
	using Mediachase.MetaDataPlus.Configurator;
	using Mediachase.IBN.Business;
    using Mediachase.IBN.Business.WebDAV.Common;

	/// <summary>
	///		Summary description for MetaDataViewControl.
	/// </summary>
	public partial class MetaDataViewControl : System.Web.UI.UserControl
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(MetaDataViewControl).Assembly);

		#region ClassName
		private string className = "";
		public string ClassName
		{
			set
			{
				className = value.ToLower();
			}
			get
			{
				return className;
			}
		}
		#endregion

		#region Header
		private string header = "";
		public string Header
		{
			set
			{
				header = value;
			}
			get
			{
				return header;
			}
		}
		#endregion

		#region MarginTop
		private string marginTop = "";
		public string MarginTop
		{
			set
			{
				marginTop = value;
			}
			get
			{
				return marginTop;
			}
		}
		#endregion

		#region ShowHeader
		public bool ShowHeader
		{
			set
			{
				ViewState["ShowHeader"] = value;
			}
			get
			{
				bool retval = true;
				if (ViewState["ShowHeader"] != null)
					retval = (bool)ViewState["ShowHeader"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
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
			if (ShowHeader)
				BindToolBar();
			else
				tbMetaInfo.Visible = false;
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			tbMetaInfo.AddText((Header == "") ? LocRM.GetString("tabMetaData") : Header);

			bool CanUpdate = false;
			string postfix = "";
			if (Request["ProjectID"] != null)
			{
				CanUpdate = Project.CanUpdate(int.Parse(Request["ProjectID"]));
			}
			if (Request["IncidentID"] != null)
			{
				CanUpdate = Incident.CanUpdate(int.Parse(Request["IncidentID"]));
			}
			if (Request["DocumentID"] != null)
			{
				CanUpdate = Document.CanUpdate(int.Parse(Request["DocumentID"]));
			}
			if (Request["EventID"] != null)
			{
				CanUpdate = CalendarEntry.CanUpdate(int.Parse(Request["EventID"]));
			}
			if (Request["TaskID"] != null)
			{
				CanUpdate = Task.CanUpdate(int.Parse(Request["TaskID"]));
			}
			if (Request["ToDoID"] != null)
			{
				CanUpdate = ToDo.CanUpdate(int.Parse(Request["ToDoID"]));
			}

			if (CanUpdate)
			{
				bool HasMetaFields = false;
				MetaClass mc = MetaClass.Load(GetMetaClassName());
				if (mc != null && mc.UserMetaFields.Count > 0)
					HasMetaFields = true;
				if(HasMetaFields)
				{
					tbMetaInfo.AddRightLink(
						String.Format("<img alt='' src='../Layouts/Images/Edit.gif'/> {0}", LocRM.GetString("Edit")),
						String.Format("../Common/MetaDataEdit.aspx?id={0}&class={1}{2}", GetObjectId(), GetMetaClassName(), postfix));
				}
				if(HasMetaFields && tblCustomFields.Rows.Count==0)
					BindText2();
				else if(!HasMetaFields)
					BindText1();
			}
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
				foreach (MetaField field in obj.MetaClass.UserMetaFields)
				{
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
						switch(field.DataType) 
						{
							case MetaDataType.File:
								Mediachase.MetaDataPlus.MetaFile mf = (Mediachase.MetaDataPlus.MetaFile)fieldValue;
								int ContentTypeId = DSFile.GetContentTypeByFileName(mf.Name);
								string sInnerHTML = "<img alt='' src='../Common/ContentIcon.aspx?IconID="+ 
									+ ContentTypeId+
									"' border='0' align='middle' width='16px' height='16px' />&nbsp;" + mf.Name;
                                string metaFileUrl = WebDavUrlBuilder.GetMetaDataPlusWebDavUrl(ObjectId, MetaClassName, field.Name, true);
                                cellValue.InnerHtml = "<a href='" + metaFileUrl + "'>" + sInnerHTML + "</a>";
                                //cellValue.InnerHtml = "<a href='../Modules/DownloadMetaFile.aspx?Id="+ObjectId.ToString()+
                                //    "&Class="+MetaClassName+"&Field="+field.Name+"'>"+sInnerHTML+"</a>";
								break;
							case MetaDataType.ImageFile:
								cellValue.InnerHtml = "<img align='middle' border='0' src='../Modules/GetMetaImageFile.aspx?Id="+ObjectId.ToString()+
									"&Class="+MetaClassName+"&Field="+field.Name+"' />";
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
								string sItems = "<span class=ibn-value>";
								MetaDictionaryItem[] items = (MetaDictionaryItem[])fieldValue;
								foreach (MetaDictionaryItem mdItem in items)
								{
									if (sItems != "")
										sItems += "<br>";
									sItems += mdItem.Value;
								}
								sItems += "</span>";
								cellValue.InnerHtml = sItems;
								break;
							default:
								try{
									cellValue.InnerHtml = String.Format("<span class=ibn-value>{0}</span>", fieldValue.ToString());
								}
								catch{
									cellValue.InnerHtml = "";
								}
								break;
						}
					}
					
					row.Cells.Add(cellTitle);
					row.Cells.Add(cellValue);

					tblCustomFields.Rows.Add(row);
				}
			}
		}
		#endregion

		#region GetObjectId()
		private int GetObjectId()
		{
			string ObjectName = "";
			switch(ClassName) {
				case "users":
					ObjectName = "UserId";
					break;
				case "projects":
					ObjectName = "ProjectId";
					break;
				case "incidents":
				case "issues":
					ObjectName = "IncidentId";
					break;
				case "documents":
					ObjectName = "DocumentId";
					break;
				case "tasks":
					ObjectName = "TaskId";
					break;
				case "todo":
					ObjectName = "ToDoId";
					break;
				case "events":
					ObjectName = "EventId";
					break;
				case "assets":
					ObjectName = "AssetID";
					break;
				case "portfolio":
					ObjectName = "ProjectGroupId";
					break;
			}
			if (this.ClassName == "users" && Request.QueryString[ObjectName] == null)
				return Mediachase.IBN.Business.Security.CurrentUser.UserID;
			else
				return int.Parse(Request.QueryString[ObjectName]);
		}
		#endregion

		#region GetMetaClassName()
		private string GetMetaClassName()
		{
			string MetaClassName = "";
			switch(ClassName) 
			{
				case "users":
					MetaClassName = "UsersEx";
					break;
				case "projects":
					int ProjectTypeId = -1;
					int ProjectId = int.Parse(Request.QueryString["ProjectId"]);
					using (IDataReader reader = Project.GetProject(ProjectId))
					{
						if (reader.Read())
							ProjectTypeId = (int)reader["TypeId"];
					}
					MetaClassName = String.Format("ProjectsEx_{0}", ProjectTypeId);
					break;
				case "incidents":
				case "issues":
					MetaClassName = "IncidentsEx";
					break;
				case "documents":
					MetaClassName = "DocumentsEx";
					break;
				case "tasks":
					MetaClassName = "TaskEx";
					break;
				case "todo":
					MetaClassName = "ToDoEx";
					break;
				case "events":
					MetaClassName = "EventsEx";
					break;
				case "portfolio":
					MetaClassName = "PortfolioEx";
					break;
			}
			return MetaClassName;
		}
		#endregion

		private void BindText1()
		{
			HtmlTableRow row = new HtmlTableRow();
			HtmlTableCell cellText = new HtmlTableCell();
			cellText.Style.Add("padding","12px");
			cellText.InnerHtml = "<span class='ibn-alerttext'>"+LocRM.GetString("tYouCanAddCustomFields")+"</span>";
			row.Cells.Add(cellText);
			tblCustomFields.Rows.Add(row);
		}

		private void BindText2()
		{
			HtmlTableRow row = new HtmlTableRow();
			HtmlTableCell cellText = new HtmlTableCell();
			cellText.Style.Add("padding","12px");
			string sFields = "";
			foreach(MetaField mf in MetaClass.Load(GetMetaClassName()).UserMetaFields)
			{
				sFields += "&nbsp;" + mf.FriendlyName + ",";
			}
			if(sFields.Length>0)
				sFields = sFields.Substring(0, sFields.Length-1);
			cellText.InnerHtml = "<span class='ibn-alerttext'>"+LocRM.GetString("tYouCanAddValuesForCustomFields")+":&nbsp;<font color='black'>"+ sFields+"</font></span>";
			row.Cells.Add(cellText);
			tblCustomFields.Rows.Add(row);
		}
	}
}
