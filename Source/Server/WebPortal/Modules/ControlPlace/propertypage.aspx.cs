using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.IBN.Business;
using Mediachase.WebSaltatoryControl;
using System.Xml;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Modules.ControlPlace
{
	/// <summary>
	/// Summary description for propertypage.
	/// </summary>
	public partial class propertypage : System.Web.UI.Page
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		//private ControlWrapper _currentInfo = null;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected string PageId
		{
			get
			{
				return this.Request.QueryString["PageId"];
			}
		}

		protected string ControlId
		{
			get
			{
				return this.Request.QueryString["ControlId"];
			}
		}

		protected int Index
		{
			get
			{
				return int.Parse(this.Request.QueryString["Index"]);
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			ControlManager.RegisterProperyPage(this, this.PageId,
				this.ControlId,
				this.Index);

			if (!IsPostBack)
				LoadSettings();
			

			ApplyLocalization();
			if (!IsPostBack)
			{
				BindData();
			}

			btnCancel.Attributes.Add("onclick","javascript:window.close();");
			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.CustomImage = "../../Layouts/Images/saveitem.gif";
			btnCancel.CustomImage = "../../Layouts/Images/Deny.gif";
		}

		public string BlockName
		{
			get
			{
				return (string)txtBlockName.Text;
			}
			set
			{
				txtBlockName.Text = value;
			}
		}

		public string[] SelectedMetaFields
		{
			get
			{
				return (string[])this.ViewState["SelectedMetaFields"];
			}
			set
			{
				this.ViewState["SelectedMetaFields"] = value;
			}
		}

		public void LoadSettings()
		{
			XmlDocument settings = ControlManager.CurrentView.GetSettings(this.ControlId, this.Index);

			XmlNode nameNode = settings.SelectSingleNode("MetaDataBlockViewControl/Name");
			XmlNodeList mfNodeList = settings.SelectNodes("MetaDataBlockViewControl/MetaField");

			if(nameNode!=null)
				this.BlockName = nameNode.InnerText;
			else
				this.BlockName = string.Empty;
 
			ArrayList list = new ArrayList();

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
						list.Add(mfNode.InnerText);
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

			this.SelectedMetaFields = (string[])list.ToArray(typeof(string));
		}

		public void SaveSettings()
		{
			XmlDocument settings = new XmlDocument();

			settings.LoadXml("<MetaDataBlockViewControl><Name></Name></MetaDataBlockViewControl>");

			XmlNode nameNode = settings.SelectSingleNode("MetaDataBlockViewControl/Name");
			nameNode.InnerText = this.BlockName;

			foreach(string item in this.SelectedMetaFields)
			{
				XmlElement el = settings.CreateElement("MetaField");
				el.InnerText = item;
				settings.SelectSingleNode("MetaDataBlockViewControl").AppendChild(el);
			}

			ControlManager.CurrentView.SetSettings(this.ControlId, this.Index, settings);
		}


		#region ApplyLocalization
		private void ApplyLocalization()
		{
			fieldsHeader.Title = LocRM.GetString("BlockPropertyAvailableFields");

			dgSelectedFields.Columns[1].HeaderText = LocRM.GetString("Field");
			dgSelectedFields.Columns[2].HeaderText = LocRM.GetString("DataType");
			dgSelectedFields.Columns[3].HeaderText = LocRM.GetString("Weight");

			dgAvailableFields.Columns[2].HeaderText = LocRM.GetString("Field");
			dgAvailableFields.Columns[3].HeaderText = LocRM.GetString("DataType");
		}
		#endregion

		#region BindToolBars
		private void BindToolBars()
		{
			
			blocknameHeader.Title = LocRM.GetString("tBlockName");
			elementHeader.Title = LocRM.GetString("tBlockPropertyCurrentlyUsed");
			
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

			this.dgSelectedFields.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_EditCommand);
			this.dgSelectedFields.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_DeleteCommand);
			this.dgAvailableFields.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAvailableFields_ItemCommand);
			this.dgAvailableFields.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAvailableFields_DeleteCommand);
			this.dgSelectedFields.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_update);
			this.dgSelectedFields.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSelectedFields_cancel);
			this.dgAvailableFields.SortCommand += new DataGridSortCommandEventHandler(dgAvailableFields_SortCommand);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			using (IDataReader reader = MetaClass.GetDataReader())
			{
				while (reader.Read())
				{
					int MetaClassId = (int)reader["MetaClassId"];
					bool IsSystem = (bool)reader["IsSystem"];
					string TableName = reader["TableName"].ToString().ToLower();
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
				}
			}

			BindSelectedFields();
			BindAvailableFields();
		}
		#endregion

		#region BindSelectedFields
		private void BindSelectedFields()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FieldId", typeof(int));
			dt.Columns.Add("FieldName", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("DataType", typeof(string));
			dt.Columns.Add("Weight", typeof(string));

			MetaClass SelectedMetaClass = GetSelectedMetaClass();
			if (SelectedMetaClass != null)
			{
				ArrayList list = new ArrayList();
				list.AddRange(this.SelectedMetaFields);
				MetaFieldCollection mfc = SelectedMetaClass.MetaFields;
				foreach (MetaField field in mfc)
				{
					if (field.IsUser && list.Contains(field.Name))
					{
						DataRow row = dt.NewRow();
						row["FieldId"] = field.Id;
						row["FieldName"] = field.Name;
						row["FriendlyName"] = field.FriendlyName;
						MetaType mdType = MetaType.Load(field.DataType);
						row["DataType"] = mdType.FriendlyName;
						row["Weight"] = 1 + list.IndexOf(field.Name);
						dt.Rows.Add(row);
					}
				}
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "Weight ASC";
			dgSelectedFields.DataSource = dv;
			dgSelectedFields.DataBind();

			foreach (DataGridItem dgi in dgSelectedFields.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('" + LocRM.GetString("DeleteWarning") +"')");
			}
		}
		#endregion

		#region BindAvailableFields
		private void BindAvailableFields()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FieldId", typeof(int));
			dt.Columns.Add("FieldName", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("DataType", typeof(string));
			dt.Columns.Add("DataTypeId", typeof(int));
			dt.Columns.Add("sortDataType", typeof(int));
			dt.Columns.Add("CanDelete", typeof(bool));

			MetaFieldCollection mfc;

			MetaClass SelectedMetaClass = GetSelectedMetaClass();
			mfc = SelectedMetaClass.MetaFields;

			foreach (MetaField field in mfc)
			{
				if (field.IsUser)
				{
					DataRow row = dt.NewRow();
					row["FieldId"] = field.Id;
					row["FieldName"] = field.Name;
					row["FriendlyName"] = field.FriendlyName;
					MetaType mdType = MetaType.Load(field.DataType);
					row["DataType"] = mdType.FriendlyName;
					int iType = mdType.Id;
					if(mdType.MetaDataType == MetaDataType.DictionaryMultivalue ||
						mdType.MetaDataType == MetaDataType.DictionarySingleValue ||
						mdType.MetaDataType == MetaDataType.EnumMultivalue ||
						mdType.MetaDataType == MetaDataType.EnumSingleValue ||
						mdType.MetaDataType == MetaDataType.StringDictionary)
						iType = 0;
					row["DataTypeId"] = iType;
					row["sortDataType"] = GetSortDataType(mdType);
					row["CanDelete"] = (field.OwnerMetaClassIdList.Count == 0);
					dt.Rows.Add(row);
				}
			}

			DataView dv = dt.DefaultView;
			if(pc["BlockProperty_AvailableFields_Sort"] == null)
				pc["BlockProperty_AvailableFields_Sort"] = "sortDataType";
			dv.Sort = pc["BlockProperty_AvailableFields_Sort"];

			dgAvailableFields.DataSource = dv;
			dgAvailableFields.DataBind();

			foreach (DataGridItem dgi in dgAvailableFields.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('" + LocRM.GetString("DeleteWarning") +"')");
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

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolBars();

			// Disabling checkboxes for existing fields
			MetaClass SelectedMetaClass = GetSelectedMetaClass();
			if (SelectedMetaClass != null)
			{
				ArrayList list = new ArrayList();
				list.AddRange(this.SelectedMetaFields);

				foreach (DataGridItem item in dgAvailableFields.Items)
				{
					bool CheckBoxEnabled = true;
					string FieldName = item.Cells[5].Text;

					if (list.Contains(FieldName))
					{
						CheckBoxEnabled = false;
					}

					foreach(Control control in item.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							checkBox.Enabled = CheckBoxEnabled;
						}
					}
					ImageButton ib=(ImageButton)item.FindControl("ibCopy");
					if(ib!=null)
						ib.Visible = CheckBoxEnabled;
				}
			}
		}
		#endregion

		#region dgAvailableFields_ItemCommand
		private void dgAvailableFields_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			MetaClass SelectedMetaClass = GetSelectedMetaClass();

			ArrayList list = new ArrayList();
			list.AddRange(this.SelectedMetaFields);

			if (e.CommandName == "Copy" && SelectedMetaClass != null)
			{
				foreach (DataGridItem item in dgAvailableFields.Items)
				{
					foreach(Control control in item.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							if (checkBox.Enabled && checkBox.Checked)
							{
								list.Add(item.Cells[5].Text);//FiledName
								checkBox.Checked = false;
							}
						}
					}
				}
				//Response.Redirect("propertypage.aspx?"+this.Request.QueryString.ToString(), true);
			}
			if (e.CommandName == "CopyOne" && SelectedMetaClass != null)
			{
				foreach(Control control in e.Item.Cells[1].Controls)
				{
					if (control is CheckBox)
					{
						CheckBox checkBox = (CheckBox)control;
						list.Add(e.Item.Cells[5].Text);//FiledName
						checkBox.Checked = false;
					}
				}
				//Response.Redirect("propertypage.aspx?"+this.Request.QueryString.ToString(), true);
			}
			this.SelectedMetaFields = (string[])list.ToArray(typeof(string));
			BindSelectedFields();
		}
		#endregion

		#region GetSelectedMetaClass
		private MetaClass GetSelectedMetaClass()
		{
			MetaClass selectedMetaClass = null;
			selectedMetaClass = MetaClass.Load(this.PageId);
			return selectedMetaClass;
		}
		#endregion

		#region DeleteCommand
		private void dgSelectedFields_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int FieldId = int.Parse(e.Item.Cells[0].Text);
			string FieldName = e.Item.Cells[5].Text;

			MetaClass mc = GetSelectedMetaClass();
			int CurrentPos = mc.GetFieldWeight(FieldId);

			ArrayList list = new ArrayList();
			list.AddRange(this.SelectedMetaFields);
			list.Remove(FieldName);
			this.SelectedMetaFields = (string[])list.ToArray(typeof(string));
			BindSelectedFields();
		}

		private void dgAvailableFields_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int FieldId = int.Parse(e.Item.Cells[0].Text);
			MetaField field = MetaField.Load(FieldId);
			if (field.OwnerMetaClassIdList.Count == 0)
				MetaField.Delete(FieldId);

			Response.Redirect("propertypage.aspx?"+this.Request.QueryString.ToString(), true);
		}
		#endregion

		#region dgSelectedFields_EditCommand
		private void dgSelectedFields_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgSelectedFields.EditItemIndex = e.Item.ItemIndex;
			BindSelectedFields();

			foreach (DataGridItem dgi in dgSelectedFields.Items)
			{
				DropDownList ddlPosition = (DropDownList)dgi.FindControl("ddlPosition");
				if(ddlPosition!=null)
				{
					MetaClass mc = GetSelectedMetaClass();
					int FieldsCount = this.SelectedMetaFields.Length;
					for (int i=1; i<=FieldsCount; i++)
					{
						ListItem li = new ListItem(i.ToString(), (i-1).ToString());
						ddlPosition.Items.Add(li);
					}
					string FieldName = e.Item.Cells[5].Text;
					ArrayList list = new ArrayList();
					list.AddRange(this.SelectedMetaFields);
					Util.CommonHelper.SafeSelect(ddlPosition, list.IndexOf(FieldName).ToString());
				}
			}
		}
		#endregion

		#region dgSelectedFields_cancel
		private void dgSelectedFields_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgSelectedFields.EditItemIndex = -1;
			BindSelectedFields();	
		}
		#endregion

		#region dgSelectedFields_update
		private void dgSelectedFields_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddlPosition");
			if (ddl!=null)
			{
				MetaClass mc= GetSelectedMetaClass();
				ArrayList list = new ArrayList();
				list.AddRange(this.SelectedMetaFields);
				string FieldName = e.CommandArgument.ToString();
				int NewPos = int.Parse(ddl.SelectedItem.Value);
				int CurPos = list.IndexOf(FieldName);
				
				string NewPosOldValue = list[NewPos].ToString();
				list.RemoveAt(CurPos);
				list.Insert(CurPos,NewPosOldValue);
				list[NewPos] = FieldName;
				this.SelectedMetaFields = (string[])list.ToArray(typeof(string));
			}
			dgSelectedFields.EditItemIndex = -1;
			BindSelectedFields();	
		}
		#endregion

		private void dgAvailableFields_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if((pc["BlockProperty_AvailableFields_Sort"] != null) && (pc["BlockProperty_AvailableFields_Sort"] == e.SortExpression))
				pc["BlockProperty_AvailableFields_Sort"] += " DESC";
			else pc["BlockProperty_AvailableFields_Sort"] = e.SortExpression;
		}

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			Page.Validate();

			if (!Page.IsValid)
			{
				lblError.Text="Error";
				return;
			}
			else lblError.Text="";

			SaveSettings();

      Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script language=javascript>"+
				"try {var str=window.opener.location.href;"+
				"window.opener.location.href=str;}" +
				"catch (e){} window.close();</script>");

		}
		#endregion
	}
}
