namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.WebSaltatoryControl;
	using MetaDataPlus.Configurator;

	/// <summary>
	///		Summary description for MdpCustomization.
	/// </summary>
	public partial class MdpCustomization : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarList", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region EnableCustomize
		private bool enablecustomize = true;
		public bool EnableCustomize 
		{
			set 
			{
				enablecustomize = value;
			}
			get 
			{
				return enablecustomize;
			}
		}
		#endregion

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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			CntrlPlTop.Path_Img = ResolveClientUrl("~/Modules/ControlPlace/");
			CntrlPlTop.PropertyPageUrl = ResolveClientUrl("~/Modules/ControlPlace/propertypage.aspx");
			CntrlPlLeft.Path_Img = ResolveClientUrl("~/Modules/ControlPlace/");
			CntrlPlLeft.PropertyPageUrl = ResolveClientUrl("~/Modules/ControlPlace/propertypage.aspx");
			CntrlPlRight.Path_Img = ResolveClientUrl("~/Modules/ControlPlace/");
			CntrlPlRight.PropertyPageUrl = ResolveClientUrl("~/Modules/ControlPlace/propertypage.aspx");
			CntrlPlBottom.Path_Img = ResolveClientUrl("~/Modules/ControlPlace/");
			CntrlPlBottom.PropertyPageUrl = ResolveClientUrl("~/Modules/ControlPlace/propertypage.aspx");

			ApplyLocalization();
			if (!IsPostBack)
			{
				BindData();
			}

			BindToolBars();

			// Disabling checkboxes for existing fields
			string SelectedMetaClassName = "";

			bool cust = enablecustomize;
			if(Request.QueryString["Customize"] != null)
				cust = false;

			if(!cust)
			{
				SelectedMetaClassName = GetMetaClassName();
				trElemBlockHeader.Visible = false;
				trDropDowns.Visible = false;
				tblContent.Attributes.Remove("class");
				tblContent.Attributes.Add("class","ibn-propertysheet");
				tblControlPlaces.CellPadding = 0;
					
				tdCPTop.Style.Add("padding-bottom","3px");
				tdCPLeft.Style.Add("padding-right","3px");
				tdCPRight.Style.Add("padding-left","3px");
				tdCPBottom.Style.Add("padding-top","3px");
			}
			else
			{
				//customize
				MetaClass SelectedMetaClass = GetSelectedMetaClass();
				SelectedMetaClassName = SelectedMetaClass.Name;
				trElemBlockHeader.Visible = true;
				trDropDowns.Visible = true;
				tblContent.Attributes.Remove("class");
				tblContent.Attributes.Add("class","ibn-propertysheet ibn-stylebox2");
				tblControlPlaces.CellPadding = 3;
				tdCPTop.Style.Remove("padding-bottom");
				tdCPLeft.Style.Remove("padding-right");
				tdCPRight.Style.Remove("padding-left");
				tdCPBottom.Style.Remove("padding-top");
			}
			
			if (!Security.CurrentUser.IsExternal)
			{
				CntrlPlTop.EditLinkHtml = "<a href=\"javascript:OpenPopUpWindow('" + this.Page.ResolveClientUrl("~/Modules/ControlPlace/MetaDataEditPage.aspx") + "?id={0}&amp;class=" + SelectedMetaClassName + "&amp;ControlPlaceId={1}&amp;Index={2}');\"><img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Edit.gif") + "'/> " + LocRM.GetString("Edit") + "</a>";
				CntrlPlLeft.EditLinkHtml = "<a href=\"javascript:OpenPopUpWindow('" + this.Page.ResolveClientUrl("~/Modules/ControlPlace/MetaDataEditPage.aspx") + "?id={0}&amp;class=" + SelectedMetaClassName + "&amp;ControlPlaceId={1}&amp;Index={2}');\"><img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Edit.gif") + "'/> " + LocRM.GetString("Edit") + "</a>";
				CntrlPlRight.EditLinkHtml = "<a href=\"javascript:OpenPopUpWindow('" + this.Page.ResolveClientUrl("~/Modules/ControlPlace/MetaDataEditPage.aspx") + "?id={0}&amp;class=" + SelectedMetaClassName + "&amp;ControlPlaceId={1}&amp;Index={2}');\"><img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Edit.gif") + "'/> " + LocRM.GetString("Edit") + "</a>";
				CntrlPlBottom.EditLinkHtml = "<a href=\"javascript:OpenPopUpWindow('" + this.Page.ResolveClientUrl("~/Modules/ControlPlace/MetaDataEditPage.aspx") + "?id={0}&amp;class=" + SelectedMetaClassName + "&amp;ControlPlaceId={1}&amp;Index={2}');\"><img alt='' src='" + ResolveClientUrl("~/Layouts/Images/Edit.gif") + "'/> " + LocRM.GetString("Edit") + "</a>";
			}

			ControlManager.RegisterPage(this.Page, SelectedMetaClassName, cust);
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{

			//lblAvailableType.Text = LocRM.GetString("SelectType") + ":";
			lblSelectedType.Text = LocRM.GetString("SelectType") + ":";

		}
		#endregion

		#region BindToolBars
		private void BindToolBars()
		{
			// Left Toolbar Header
			int ClassId = -1;
			if (ddlSelectedType.Visible && ddlSelectedType.Items.Count > 0)
				ClassId = int.Parse(ddlSelectedType.SelectedItem.Value);
			else if (ddlSelectedElement.Items.Count > 0)
				ClassId = int.Parse(ddlSelectedElement.SelectedItem.Value);
			/*if (ClassId > 0)
			{
				MetaClass mc = MetaClass.Load(ClassId);
				MetaFieldCollection mfc = mc.MetaFields;
				elementHeader.Title = mc.Parent.FriendlyName;
				if (ddlSelectedType.Visible && ddlSelectedType.Items.Count > 0)
					elementHeader.Title += " - " + mc.FriendlyName;
			}*/
			elementHeader.Title = LocRM.GetString("tFieldsBlocks"); ;
			// Left Toolbar Button
			string text = String.Format("<img alt='' src='{1}' title='{0}'/> {0}" ,
				LocRM.GetString("tNewBlock"), this.Page.ResolveClientUrl("~/Layouts/Images/NewItem.gif"));
			string link = "javascript:CP_Move('','MetaDataBlockViewControl','');";
			elementHeader.AddLink(text, link);
			elementHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM2.GetString("BusData"), this.Page.ResolveClientUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin2"));
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

		#region BindData
		private void BindData()
		{
			//ddlAvailableElement.Items.Add(new ListItem(LocRM.GetString("Any"), "0"));

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Value", typeof(string)));
			DataRow dr;
			using (IDataReader reader = MetaClass.GetDataReader())
			{
				while (reader.Read())
				{
					int MetaClassId = (int)reader["MetaClassId"];
					bool IsSystem = (bool)reader["IsSystem"];
					string TableName = reader["TableName"].ToString().ToLower();
					if((TableName == "projects" || TableName=="taskex" || TableName=="portfolioex") && 
						!Configuration.ProjectManagementEnabled)
						continue;
					if(TableName=="incidentsex" && !Configuration.HelpDeskEnabled)
						continue;
					string FriendlyName = reader["FriendlyName"].ToString();
					string ParentTableName = "";
					string ParentFriendlyName = "";
					if (reader["ParentTableName"] != DBNull.Value)
						ParentTableName = reader["ParentTableName"].ToString().ToLower();
					if (reader["ParentFriendlyName"] != DBNull.Value)
						ParentFriendlyName = reader["ParentFriendlyName"].ToString();
					bool IsAbstract = false;
					if (reader["IsAbstract"] != DBNull.Value)
						IsAbstract = (bool)reader["IsAbstract"];
					string namespaceName = reader["namespace"].ToString();
					if (!IsSystem && ParentTableName != "projects" && ParentTableName != "list_items" && !IsAbstract && namespaceName == MetaDataWrapper.UserRoot)
					{
						dr = dt.NewRow();
						dr["Name"] = ParentFriendlyName;
						dr["Value"] = MetaClassId.ToString();
						dt.Rows.Add(dr);
						//ddlSelectedElement.Items.Add(new ListItem(ParentFriendlyName, MetaClassId.ToString()));
					}
					else if (TableName == "projects")
					{
						dr = dt.NewRow();
						dr["Name"] = FriendlyName;
						dr["Value"] = MetaClassId.ToString();
						dt.Rows.Add(dr);
						//ddlSelectedElement.Items.Add(new ListItem(FriendlyName, MetaClassId.ToString()));
					}
					else if (TableName == "list_items")
					{
						//ddlAvailableElement.Items.Add(new ListItem(FriendlyName, MetaClassId.ToString()));
					}
				}
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			ddlSelectedElement.DataSource = dv;
			ddlSelectedElement.DataTextField = "Name";
			ddlSelectedElement.DataValueField = "Value";
			ddlSelectedElement.DataBind();
			// Current Element
			if (pc["cust_SelectedElement"] != null)
			{
				int CurClassId = int.Parse(pc["cust_SelectedElement"]);
				MetaClass mc = MetaClass.Load(CurClassId);
				if (mc != null)
				{
					if (mc.Parent != null && mc.Parent.TableName.ToLower() == "projects")
						Util.CommonHelper.SafeSelect(ddlSelectedElement, mc.Parent.Id.ToString());
					else
						Util.CommonHelper.SafeSelect(ddlSelectedElement, CurClassId.ToString());
				}
			}

			BindSelectedType();
		}
		#endregion

		#region BindSelectedType
		private void BindSelectedType()
		{
			lblSelectedType.Visible = false;
			ddlSelectedType.Visible = false;
			
			if (ddlSelectedElement.Items.Count > 0)
			{
				int SelectedClassId = int.Parse(ddlSelectedElement.SelectedItem.Value);
				MetaClass mc = MetaClass.Load(SelectedClassId);
				if (mc.ChildClasses != null && mc.ChildClasses.Count > 0)
				{
					lblSelectedType.Visible = true;
					ddlSelectedType.Visible = true;

					ddlSelectedType.Items.Clear();

					MetaClassCollection children = mc.ChildClasses;
					foreach (MetaClass child in children)
						ddlSelectedType.Items.Add(new ListItem(child.FriendlyName, child.Id.ToString()));

					if (!IsPostBack && pc["cust_SelectedElement"] != null)
						Util.CommonHelper.SafeSelect(ddlSelectedType, pc["cust_SelectedElement"]);
				}
			}

			BindSelectedFields();
		}
		#endregion

		#region BindSelectedFields
		private void BindSelectedFields()
		{
			MetaClass SelectedMetaClass = GetSelectedMetaClass();
			if (SelectedMetaClass != null)
			{
				pc["cust_SelectedElement"] = SelectedMetaClass.Id.ToString();
			}
		}
		#endregion

		#region GetSortDataType
		private int GetSortDataType(MetaType mdType)
		{
			switch(mdType.Id)
			{
				case 31://ShortString
					return 0;
				case 32://LongString
					return 1;
				case 26://Integer
					return 2;
				case 9://Money
					return 3;
				case 28://Date
					return 4;
				case 4://DateTime
					return 5;
				case 34://DictionarySingleValue
					return 6;
				case 35://DictionaryMultivalue
					return 7;
				case 36://EnumSingleValue
					return 8;
				case 37://EnumMultivalue
					return 9;
				case 38://StringDictionary
					return 10;
				case 27://Boolean
					return 11;
				case 30://URL
					return 12;
				case 33://LongHtmlString
					return 13;
				case 29://Email
					return 14;
				case 39://File
					return 15;
				case 40://ImageFile
					return 16;
				default:
					return 100;
			}
		}
		#endregion

		#region SelectedIndexChanged
		protected void ddlSelectedElement_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BindSelectedType();
			MetaClass SelectedMetaClass = GetSelectedMetaClass();
			ControlManager.CurrentView.ChangeId(SelectedMetaClass.Name);
		}

		protected void ddlSelectedType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BindSelectedFields();
			MetaClass SelectedMetaClass = GetSelectedMetaClass();
			ControlManager.CurrentView.ChangeId(SelectedMetaClass.Name);
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
//			BindToolBars();
//
//			// Disabling checkboxes for existing fields
//			MetaClass SelectedMetaClass = GetSelectedMetaClass();
//			ControlManager.RegisterPage(this.Page, SelectedMetaClass.Name, true);
//			if (SelectedMetaClass != null)
//			{
//			}
			bool cp_top_empty = (ControlManager.CurrentView.ControlPlaces["CntrlPlTop"].ControlWrappers.Count==0);
			bool cp_left_empty = (ControlManager.CurrentView.ControlPlaces["CntrlPlLeft"].ControlWrappers.Count==0);
			bool cp_right_empty = (ControlManager.CurrentView.ControlPlaces["CntrlPlRight"].ControlWrappers.Count==0);
			bool cp_bottom_empty = (ControlManager.CurrentView.ControlPlaces["CntrlPlBottom"].ControlWrappers.Count==0);
			if(!cp_top_empty && cp_left_empty && cp_right_empty && cp_bottom_empty)
				tdCPTop.Style.Remove("padding-bottom");
			if(cp_top_empty && cp_left_empty && cp_right_empty && !cp_bottom_empty)
				tdCPBottom.Style.Remove("padding-top");
		}
		#endregion
		
		#region GetSelectedMetaClass
		private MetaClass GetSelectedMetaClass()
		{
			int SelectedClassId = -1;
			if (ddlSelectedType.Visible && ddlSelectedType.Items.Count > 0)
				SelectedClassId = int.Parse(ddlSelectedType.SelectedItem.Value);
			else if (ddlSelectedElement.Items.Count > 0)
				SelectedClassId = int.Parse(ddlSelectedElement.SelectedItem.Value);
			MetaClass selectedMetaClass = null;
			if (SelectedClassId > 0)
				selectedMetaClass = MetaClass.Load(SelectedClassId);
			return selectedMetaClass;
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
				case "lists":
					MetaClassName = String.Format("ListsEx_{0}", Request.QueryString["ListId"]);
					break;
				case "portfolio":
					MetaClassName = "PortfolioEx";
					break;
			}
			return MetaClassName;
		}
		#endregion
	}
}
