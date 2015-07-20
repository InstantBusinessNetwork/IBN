using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ComponentArt.Web.UI;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Lists.Mapping;
using Mediachase.Ibn.Service;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.MetaDataPlus.Import;
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Data.OleDb;


namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class ImportWizard : System.Web.UI.UserControl
	{
		protected string HeaderText = "";
		protected string SubHeaderText = "";
		protected string StepText = "";
		private const int _maxStepCount = 4;
		ListImportParameters lip = null;
		private string prevStepId = String.Empty;

		#region keyLIP
		private string _keyLIP
		{
			get
			{
				if (ViewState["_keyLIP"] == null)
					ViewState["_keyLIP"] = Guid.NewGuid().ToString("N");
				return ViewState["_keyLIP"].ToString();
			}
			set { ViewState["_keyLIP"] = value; }
		}
		#endregion

		#region _listId
		private int _listId
		{
			get
			{
				int retVal = -1;
				if (Request["ListId"] != null)
				{
					string className = Request["ListId"];
					if (ListManager.MetaClassIsList(className))
						retVal = ListManager.GetListIdByClassName(className);
				}
				return retVal;
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Page.Form.Enctype = "multipart/form-data";

			prevStepId = ucWizard.ActiveStep.ID;

			WizardDescribe();
			CreateDelegates();
			ApplyStepsLocalization();

			if (!Page.IsPostBack)
				PreBindSteps();

			if (ucWizard.ActiveStep.ID == "step3")
				AddMetaFieldLink();

			CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoList", "MC_ListApp_MetaFieldEdited");
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			//After new field added need to update Lip
			if (CHelper.GetFromContext("NeedToChangeDataSource") != null && (bool)CHelper.GetFromContext("NeedToChangeDataSource"))
			{
				//lip = (ListImportParameters)CHelper.GetFromContext(_keyLIP);
				lip = (ListImportParameters)Session[_keyLIP];
				ViewState["lip"] = lip;
				Session[_keyLIP] = lip;
				FillDefaultValues(dgMapping, null);
				BindDG();
			}

			DefineHeaderTexts();
			if (ucWizard.ActiveStep.ID == "step2")
			{
				fsNewList.Visible = rbNewList.Checked;
				fsUpdateList.Visible = rbUpdList.Checked;
				tdLists.Visible = rbUpdList.Checked;

				MoveTree.AutoPostBackOnSelect = rbUpdList.Checked;
			}

			ucWizard.StepStyle.CssClass = (ucWizard.ActiveStep.ID == "step4") ? "" : "wizardStep";
		}

		void Page_PreRenderComplete(object sender, EventArgs e)
		{
			if (CHelper.GetFromContext("NeedToChangeDataSource") != null)
				CHelper.RemoveFromContext("NeedToChangeDataSource");
		}

		#region CreateDelegates
		/// <summary>
		/// Creates the delegates.
		/// </summary>
		private void CreateDelegates()
		{
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			rbList.SelectedIndexChanged += new EventHandler(rbList_SelectedIndexChanged);
			ucWizard.CancelButtonClick += new EventHandler(ucWizard_CancelButtonClick);
			ucWizard.ActiveStepChanged += new EventHandler(ucWizard_ActiveStepChanged);
			ucWizard.FinishButtonClick += new WizardNavigationEventHandler(ucWizard_FinishButtonClick);

			ddDelimeter.SelectedIndexChanged += new EventHandler(ddCSV_SelectedIndexChanged);
			ddTextQualifier.SelectedIndexChanged += new EventHandler(ddCSV_SelectedIndexChanged);
			ddEncoding.SelectedIndexChanged += new EventHandler(ddCSV_SelectedIndexChanged);

			dgMapping.ItemDataBound += new DataGridItemEventHandler(dgMapping_ItemDataBound);
			dgMapping.DeleteCommand += new DataGridCommandEventHandler(dgMapping_DeleteCommand);

			lbErrorLog.Click += new EventHandler(lbErrorLog_Click);

			rbNewList.CheckedChanged += new EventHandler(rbListType_CheckedChanged);
			rbUpdList.CheckedChanged += new EventHandler(rbListType_CheckedChanged);

			MoveTree.NodeSelected += new ComponentArt.Web.UI.TreeView.NodeSelectedEventHandler(MoveTree_NodeSelected);
			dgLists.ItemDataBound += new DataGridItemEventHandler(dgLists_ItemDataBound);
		}
		#endregion

		#region BindLists
		private void BindLists(int parentFolderId)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ListId", typeof(int)));
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			ListInfo[] mas = ListInfo.List(FilterElement.EqualElement("IsTemplate", false), FilterElement.EqualElement("FolderId", parentFolderId));
			foreach (ListInfo li in mas)
			{
				DataRow dr = dt.NewRow();
				dr["ListId"] = li.PrimaryKeyId.Value;
				dr["Title"] = li.Title;
				dt.Rows.Add(dr);
			}

			dgLists.DataSource = dt.DefaultView;
			dgLists.DataBind();
		} 
		#endregion

		void dgLists_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			int listId = _listId;
			if (!String.IsNullOrEmpty(hdnListId.Value))
				listId = int.Parse(hdnListId.Value);
			CheckBox cb = (CheckBox)e.Item.FindControl("cbListItem");
			if (cb != null && !String.IsNullOrEmpty(cb.Text))
				cb.Text = "&nbsp;&nbsp;&nbsp;" + cb.Text;

			if (e.Item.Cells[0].Text == listId.ToString())
				cb.Checked = true;
		}

		void MoveTree_NodeSelected(object sender, TreeViewNodeEventArgs e)
		{
			if (rbUpdList.Checked)
				BindLists(int.Parse(e.Node.Value));
		}

		void rbListType_CheckedChanged(object sender, EventArgs e)
		{
			switch (rbList.SelectedValue)
			{
				case "2":
					trProject.Visible = true;
					break;
				default:
					trProject.Visible = false;
					break;
			}
			BindTree();
		}

		#region WizardDescribe
		/// <summary>
		/// Wizard Describing
		/// </summary>
		private void WizardDescribe()
		{
			ucWizard.DisplaySideBar = false;
			ucWizard.DisplayCancelButton = true;

			ucWizard.CancelButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tClose}");
			btnClose.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tClose}");
			ucWizard.FinishCompleteButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tSave}");
			ucWizard.StartNextButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tNext}") + " >";
			ucWizard.StepNextButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tNext}") + " >";
			ucWizard.StepPreviousButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrev}") + " <";
			ucWizard.FinishPreviousButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrev}") + " <";
		}
		#endregion

		#region ApplyStepsLocalization
		/// <summary>
		/// Applies the steps localization.
		/// </summary>
		private void ApplyStepsLocalization()
		{
			lgdSourceType.AddText(GetGlobalResourceObject("IbnFramework.ListInfo", "imSelectSourceType").ToString());
			lgdFile.AddText(GetGlobalResourceObject("IbnFramework.ListInfo", "imSelectSourceFile").ToString());

			lgdListType.AddText(GetGlobalResourceObject("IbnFramework.ListInfo", "imWantToCreate").ToString());
			lgdNewList.AddText(GetGlobalResourceObject("IbnFramework.ListInfo", "imNewListCreation").ToString());
			lgdUpdateList.AddText(GetGlobalResourceObject("IbnFramework.ListInfo", "imSelectExistingList").ToString());

			bhCSV.AddText(GetGlobalResourceObject("IbnFramework.ListInfo", "tImportInformation").ToString());
			bhMapping.AddText(GetGlobalResourceObject("IbnFramework.ListInfo", "tMapping").ToString());

			rbNewList.Text = " " + GetGlobalResourceObject("IbnFramework.ListInfo", "imStep3NewList").ToString();
			rbUpdList.Text = " " + GetGlobalResourceObject("IbnFramework.ListInfo", "imStep3UpdateExList").ToString();
		}
		#endregion

		#region DefineHeaderTexts
		/// <summary>
		/// Defines the header texts.
		/// </summary>
		private void DefineHeaderTexts()
		{
			HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:tListImportHeader}");
			SubHeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:tListImportSubHeader}");
			if (ucWizard.ActiveStep.ID == "step1")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tStepByStep}"),
					"1", _maxStepCount.ToString());
			else if (ucWizard.ActiveStep.ID == "step2")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tStepByStep}"),
					"2", _maxStepCount.ToString());
			else if (ucWizard.ActiveStep.ID == "step3")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tStepByStep}"),
					"3", _maxStepCount.ToString());
			else if (ucWizard.ActiveStep.ID == "step4")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tStepByStep}"),
					"4", _maxStepCount.ToString());
			else
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tStepByStep}"),
					"&gt;4", _maxStepCount.ToString());
		}
		#endregion

		#region PreBindSteps
		/// <summary>
		/// Default data bind.
		/// </summary>
		private void PreBindSteps()
		{
			rbSourceType.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.ListInfo", "imExcel").ToString(), "0"));
			rbSourceType.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.ListInfo", "imXML").ToString(), "1"));
			rbSourceType.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.ListInfo", "imCSV").ToString(), "2"));
			CHelper.SafeSelect(rbSourceType, "0");

			//ddlLists.DataSource = ListInfo.List(FilterElement.EqualElement("IsTemplate", false));
			//ddlLists.DataTextField = "Title";
			//ddlLists.DataValueField = "PrimaryKeyId";
			//ddlLists.DataBind();

			rbList.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.ListInfo", "tPrivateLists").ToString(), "0"));
			rbList.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.ListInfo", "tPublicLists").ToString(), "1"));
			if (Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
			{
				rbList.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.ListInfo", "tPrjLists").ToString(), "2"));
			}
			rbList.Width = Unit.Percentage(100);
			rbList.CellPadding = 5;

			trProject.Visible = false;

			#region Define Folder
			int folderId = ListManager.GetPublicRoot().PrimaryKeyId.Value;
			if(Request["ListFolderId"]!=null)
				folderId = int.Parse(Request["ListFolderId"]);
			if (_listId > 0)
			{
				ListInfo list = new ListInfo(_listId);
				if (list.FolderId.HasValue)
					folderId = list.FolderId.Value;
			}
			ListFolder fld = null;
			try
			{
				fld = new ListFolder(folderId);
			}
			catch
			{
				try
				{
					fld = new ListFolder(ListManager.GetProjectRoot(folderId).PrimaryKeyId.Value);
				}
				catch { }
			}
			if (fld != null)
			{
				if (fld.FolderType == ListFolderType.Private)
					CHelper.SafeSelect(rbList, "0");
				else if (fld.FolderType == ListFolderType.Public)
					CHelper.SafeSelect(rbList, "1");
				else
				{
					CHelper.SafeSelect(rbList, "2");
					trProject.Visible = true;
					ucProject.ObjectTypeId = 3;
					ucProject.ObjectId = fld.ProjectId.Value;
				}
				
				destFolderId.Value = fld.PrimaryKeyId.Value.ToString();
			}
			else
				CHelper.SafeSelect(rbList, "0");
			#endregion

			ddDelimeter.Items.Add(new ListItem(",", ","));
			ddDelimeter.Items.Add(new ListItem(".", "."));
			ddDelimeter.Items.Add(new ListItem(";", ";"));
			ddDelimeter.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ListInfo", "tSpaceValue").ToString(), "space"));
			ddDelimeter.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ListInfo", "tTabValue").ToString(), "tab"));

			ddTextQualifier.Items.Add(new ListItem("\"", "\""));
			ddTextQualifier.Items.Add(new ListItem("'", "'"));

			ddEncoding.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ListInfo", "tDefault").ToString(), "Default"));
			ddEncoding.Items.Add(new ListItem("ASCII", "ASCII"));
			ddEncoding.Items.Add(new ListItem("UTF-8", "UTF8"));
			ddEncoding.Items.Add(new ListItem("Unicode", "Unicode"));

			MetaFieldType enumListType = DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListTypeEnumName];
			MetaFieldType enumListStatus = DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListStatusEnumName];

			foreach (MetaEnumItem mei in enumListType.EnumItems)
				ddType.Items.Add(new ListItem(CHelper.GetResFileString(mei.Name), mei.Handle.ToString()));

			foreach (MetaEnumItem mei in enumListStatus.EnumItems)
				ddStatus.Items.Add(new ListItem(CHelper.GetResFileString(mei.Name), mei.Handle.ToString()));

			if (_listId > 0)
			{
				rbUpdList.Checked = true;
				hdnListId.Value = _listId.ToString();
				//CHelper.SafeSelect(ddlLists, _listId.ToString());
			}
			else
				rbNewList.Checked = true;
		}
		#endregion

		#region BindTree
		/// <summary>
		/// Binds the ListFolder tree.
		/// </summary>
		private void BindTree()
		{
			int iFolderId = -1;
			switch (rbList.SelectedValue)
			{
				case "1":
					iFolderId = ListManager.GetPublicRoot().PrimaryKeyId.Value;
					break;
				case "2":
					int prjId = ucProject.ObjectId;
					if (prjId > 0)
						iFolderId = ListManager.GetProjectRoot(prjId).PrimaryKeyId.Value;
					break;
				default:
					iFolderId = ListManager.GetPrivateRoot(Mediachase.IBN.Business.Security.CurrentUser.UserID).PrimaryKeyId.Value;
					break;
			}

			int selFolderId = iFolderId;
			if(!String.IsNullOrEmpty(destFolderId.Value))
				selFolderId = int.Parse(destFolderId.Value);

			MoveTree.Nodes.Clear();
			MoveTree.ClientSideOnNodeSelect = "onNodeClick";
			if (iFolderId > 0)
			{
				TreeViewNode node;
				ListFolder folder = new ListFolder(iFolderId);
				node = new TreeViewNode();
				if (folder.FolderType == ListFolderType.Private)
				{
					node.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrivateLists}");
					node.ID = "listfolder" + iFolderId.ToString() + "private";
				}
				else if (folder.FolderType == ListFolderType.Public)
				{
					node.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tPublicLists}");
					node.ID = "listfolder" + iFolderId.ToString();
				}
				else if (folder.FolderType == ListFolderType.Project)
				{
					node.Text = Task.GetProjectTitle(folder.ProjectId.Value);
					node.ID = "listfolder" + iFolderId.ToString() + "project";
				}
				node.Value = iFolderId.ToString();
				if (folder.HasChildren)
					BindSubNodes(node, folder, selFolderId);
				MoveTree.Nodes.Add(node);
				if(iFolderId == selFolderId)
					MoveTree.SelectedNode = node;
				destFolderId.Value = selFolderId.ToString();
				if (rbUpdList.Checked)
					BindLists(selFolderId);
			}
		}
		#endregion

		#region BindSubNodes
		private void BindSubNodes(TreeViewNode parentNode, ListFolder parent, int selectedId)
		{
			Mediachase.Ibn.Data.Services.TreeService ts = parent.GetService<Mediachase.Ibn.Data.Services.TreeService>();
			foreach (Mediachase.Ibn.Data.Services.TreeNode tN in ts.GetChildNodes())
			{
				MetaObject moFolder = tN.InnerObject;
				ListFolder folder = new ListFolder(moFolder.PrimaryKeyId.Value);
				int iFolderId = folder.PrimaryKeyId.Value;
				TreeViewNode node = new TreeViewNode();
				node.Text = folder.Title;

				bool IsPrivate = (folder.FolderType == ListFolderType.Private);
				if (folder.HasChildren)
				{
					BindSubNodes(node, folder, selectedId);
				}
				node.ID = "listfolder" + iFolderId.ToString();
				if (IsPrivate)
					node.ID += "private";
				node.Value = iFolderId.ToString();
				if (iFolderId == selectedId)
					MoveTree.SelectedNode = node;
				parentNode.Nodes.Add(node);
			}
		} 
		#endregion

		#region rbList_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the rbList control.
		/// Defines ListFolder Type - Private, Public, Project
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void rbList_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (rbList.SelectedValue)
			{
				case "2":
					trProject.Visible = true;
					break;
				default:
					trProject.Visible = false;
					break;
			}
			BindTree();
		}
		#endregion

		#region lbChangeProject_Click
		/// <summary>
		/// Bind ListFolder Tree For selected project
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbChangeProject_Click(object sender, EventArgs e)
		{
			BindTree();
		}
		#endregion

		#region CSV Params Changing
		/// <summary>
		/// Handles the SelectedIndexChanged event of the ddCSV control.
		/// Parse and return LIP for CSV file
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ddCSV_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataSet rawData = GetRawDataForCSV();
			if (rbNewList.Checked)
				lip = new ListImportParameters(int.Parse(destFolderId.Value), txtTitle.Text, rawData, 0);
			else
				lip = new ListImportParameters(int.Parse(hdnListId.Value), rawData, 0);

			ViewState["lip"] = lip;
			Session[_keyLIP] = lip;
			BindDG();
		}
		#endregion

		#region Get RawDataSet for CSV
		/// <summary>
		/// Gets the raw data for CSV.
		/// Parse CSV file with user defined parameters.
		/// </summary>
		/// <returns></returns>
		private DataSet GetRawDataForCSV()
		{
			char delimeter = ',';
			#region define delimeter
			switch (ddDelimeter.SelectedValue)
			{
				case ",":
					delimeter = ',';
					break;
				case ".":
					delimeter = '.';
					break;
				case ";":
					delimeter = ';';
					break;
				case "space":
					delimeter = ' ';
					break;
				case "tab":
					delimeter = '\t';
					break;
				default:
					break;
			}
			#endregion

			char textQualifier = '"';
			#region define Text Qualifier
			switch (ddTextQualifier.SelectedValue)
			{
				case "\"":
					textQualifier = '"';
					break;
				case "'":
					textQualifier = '\'';
					break;
				default:
					break;
			}
			#endregion

			Encoding enc = Encoding.Default;
			#region define encoding
			switch (ddEncoding.SelectedValue)
			{
				case "ASCII":
					enc = Encoding.ASCII;
					break;
				case "UTF8":
					enc = Encoding.UTF8;
					break;
				case "Unicode":
					enc = Encoding.Unicode;
					break;
				default:
					break;
			}
			#endregion

			IIncomingDataParser parser = new CsvIncomingDataParser("", true, delimeter, textQualifier, true, enc);
			DataSet rawData = parser.Parse(Server.MapPath(hdnFilePath.Value), null);
			return rawData;
		}
		#endregion

		/// <summary>
		/// Handles the ActiveStepChanged event of the ucWizard control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ucWizard_ActiveStepChanged(object sender, EventArgs e)
		{
			//step2
			if (ucWizard.ActiveStep.ID == "step2")
			{
				#region upload file
				if (fSourceFile.PostedFile != null && fSourceFile.PostedFile.ContentLength > 0)
				{
					txtTitle.Text = Path.GetFileNameWithoutExtension(fSourceFile.PostedFile.FileName);
					ProcessFileCache(Server.MapPath(Mediachase.UI.Web.Util.CommonHelper.ChartPath));
					String dir = Mediachase.UI.Web.Util.CommonHelper.ChartPath;
					string wwwpath = dir + Guid.NewGuid().ToString("N");
					wwwpath += Path.GetExtension(fSourceFile.PostedFile.FileName);

					hdnFilePath.Value = wwwpath;
					using (Stream sw = File.Create(Server.MapPath(wwwpath)))
					{
						fSourceFile.PostedFile.InputStream.Seek(0, SeekOrigin.Begin);
						System.IO.BinaryReader br = new System.IO.BinaryReader(fSourceFile.PostedFile.InputStream);
						int iBufferSize = 655360; // 640 KB
						byte[] outbyte = br.ReadBytes(iBufferSize);

						while (outbyte.Length > 0)
						{
							sw.Write(outbyte, 0, outbyte.Length);
							outbyte = br.ReadBytes(iBufferSize);
						}
						br.Close();
					}
				}
				#endregion

				BindTree();
			}

			//step3
			if (ucWizard.ActiveStep.ID == "step3")
			{
				trCSV.Visible = (rbSourceType.SelectedIndex == 2);
				trList.Visible = !trCSV.Visible;

				#region file parsing
				IIncomingDataParser parser = null;
				DataSet rawData = null;
				try
				{
					switch (rbSourceType.SelectedIndex)
					{
						case 0:
							IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), ConfigurationManager.AppSettings["McOleDbServiceString"]);
							rawData = helper.ConvertExcelToDataSet(Server.MapPath(hdnFilePath.Value));
							break;
						case 1:
							parser = new XmlIncomingDataParser();
							rawData = parser.Parse(Server.MapPath(hdnFilePath.Value), null);
							break;
						case 2:
							rawData = GetRawDataForCSV();
							break;
					}
				}
				catch(Exception ex)
				{
					CHelper.GenerateErrorReport(ex);
					ViewState["ServiceError"] = true;
					ViewState["ErrorFileName"] = Server.MapPath(hdnFilePath.Value);
					ucWizard.MoveTo(this.step4);
					return;
				}
				#endregion

				if (ViewState["lip"] != null && prevStepId == ucWizard.ActiveStep.ID)
					lip = (ListImportParameters)ViewState["lip"];
				else if (rbNewList.Checked)
				{
					lip = new ListImportParameters(int.Parse(destFolderId.Value), txtTitle.Text, rawData, 0);
					lblName.Text = String.Format("<b>{0}:</b>&nbsp;&nbsp;{1}",
						GetGlobalResourceObject("IbnFramework.ListInfo", "List").ToString(), txtTitle.Text);
					lip.Status = int.Parse(ddStatus.SelectedValue);
					lip.ListType = int.Parse(ddType.SelectedValue);
				}
				else
				{
					foreach (DataGridItem dgi in dgLists.Items)
					{
						CheckBox cb = (CheckBox)dgi.FindControl("cbListItem");
						if (cb != null && cb.Checked)
						{
							hdnListId.Value = dgi.Cells[0].Text;
							break;
						}
					}
					lip = new ListImportParameters(int.Parse(hdnListId.Value), rawData, 0);
					ListInfo li = new ListInfo(int.Parse(hdnListId.Value));
					lblName.Text = String.Format("<b>{0}:</b>&nbsp;&nbsp;{1}",
						GetGlobalResourceObject("IbnFramework.ListInfo", "List").ToString(), li.Title);
				}

				ViewState["lip"] = lip;
				Session[_keyLIP] = lip;
				BindDG();

				AddMetaFieldLink();
			}

			if (ucWizard.ActiveStep.ID == "step4")
			{
				if (ViewState["ServiceError"] != null && (bool)ViewState["ServiceError"])
				{
					string fileName = ViewState["ErrorFileName"].ToString();
					if (fileName.EndsWith("xlsx") && !Is2007OfficeSystemDriverInstalled(fileName))
						lblResult.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "McOleDbServiceWarningXlsx").ToString();
					else
						lblResult.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "McOleDbServiceWarning").ToString();
				}
				foreach (Control c in ucWizard.ActiveStep.Controls)
				{
					if (c is Button)
					{
						Button btn = (Button)c;
						string script = Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedWindowScript(this.Page, String.Empty);
						script += "return false;";
						btn.OnClientClick = script;
					}
				}
			}
		}

		#region AddMetaFieldLink
		/// <summary>
		/// Adds the meta field link.
		/// </summary>
		private void AddMetaFieldLink()
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			Dictionary<string, string> prms = new Dictionary<string, string>();
			prms.Add("key", _keyLIP);
			prms.Add("tick", Guid.NewGuid().ToString("N").ToLower());

			bhMapping.AddRightLink(
				String.Format("<img alt='' border='0' align='absmiddle' src='{1}' /> {0}",
					GetGlobalResourceObject("IbnFramework.ListInfo", "tAddField").ToString(),
					this.Page.ResolveClientUrl("~/layouts/images/newitem.gif")),
				String.Format("javascript:{{{0}}}", cm.AddCommand("", "", "ListInfoList", "MC_ListApp_AddMetaFieldFrame", prms)));
		}

		private string GetMetaFieldLink(string nameText, string field)
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			Dictionary<string, string> prms = new Dictionary<string, string>();
			prms.Add("key", _keyLIP);
			prms.Add("field", field);
			prms.Add("tick", Guid.NewGuid().ToString("N").ToLower());

			string url = String.Format("javascript:{{{0}}}", cm.AddCommand("", "", "ListInfoList", "MC_ListApp_AddMetaFieldFrame", prms));
			url = url.Replace("\"", "&quot;");
			return 
				String.Format("<a href=\"{1}\">{0}</a>",
					nameText, url);
		}
		#endregion

		#region BindDG
		/// <summary>
		/// Binds the DataGrid.
		/// </summary>
		private void BindDG()
		{
			DataTable dt = GetMappingSource();
			dgMapping.DataSource = dt.DefaultView;
			dgMapping.DataBind();
		}
		#endregion

		#region ItemDataBound
		/// <summary>
		/// Handles the ItemDataBound event of the dgMapping control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
		void dgMapping_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			DropDownList ddi = (DropDownList)e.Item.FindControl("ddColumns");
			if (ddi != null)
			{
				string mfName = e.Item.Cells[0].Text;
				MetaField field = lip.GetDestinationMetaField(mfName);
				MappingRule mr = lip.GetRuleByMetaField(mfName);

				ddi.Items.Clear();
				ddi.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ListInfo", "tNotSetValue").ToString(), "-1"));
				if (field.GetOriginalMetaType() != null)
					ddi.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ListInfo", "tDefaultValue").ToString(), "0"));
				foreach (DataColumn dc in lip.GetSourceColumns())
					ddi.Items.Add(new ListItem(dc.ColumnName, dc.ColumnName));

				string val = "-1";
				if (mr != null)
				{
					if (mr.RuleType == MappingRuleType.DefaultValue)
						val = "0";
					else if (mr.RuleType == MappingRuleType.CopyValue)
						val = mr.ColumnName;
				}
				CHelper.SafeSelect(ddi, val);

				TextBox txt = (TextBox)e.Item.FindControl("tbColumn");
				DropDownList dd = (DropDownList)e.Item.FindControl("ddColumn");
				Label lbl = (Label)e.Item.FindControl("lblColumn");
				switch (val)
				{
					case "-1":	//Not Set
						txt.Visible = false;
						dd.Visible = false;
						lbl.Visible = false;
						break;
					case "0":	//Default Value
						bool isChangeable = IsChangeableField(mfName);
						if (isChangeable)
						{
							if (field.IsEnum)
							{
								dd.Visible = true;
								txt.Visible = false;
								lbl.Visible = false;
								dd.Items.Clear();
								MetaFieldType mft = field.GetMetaType();
								foreach (MetaEnumItem mei in mft.EnumItems)
									dd.Items.Add(new ListItem(mei.Name, mei.Handle.ToString()));
								if (!String.IsNullOrEmpty(mr.DefaultValue))
									CHelper.SafeSelect(dd, mr.DefaultValue);
							}
							else
							{
								dd.Visible = false;
								txt.Visible = true;
								lbl.Visible = false;
								txt.Text = mr.DefaultValue;
							}
						}
						else
						{
							txt.Visible = false;
							dd.Visible = false;
							lbl.Visible = true;
							string value = mr.DefaultValue;
							if (field.IsEnum)
							{
								MetaFieldType mft = field.GetMetaType();
								foreach (MetaEnumItem mei in mft.EnumItems)
									if (mei.Handle.ToString() == mr.DefaultValue)
										value = mei.Name;
							}
							if (String.IsNullOrEmpty(value))
								value = GetGlobalResourceObject("IbnFramework.ListInfo", "tNotSetValue").ToString();
							lbl.Text = value;
						}
						break;
					default:	//CopyValue
						txt.Visible = false;
						dd.Visible = false;
						lbl.Visible = false;
						//lbl.Visible = true;
						//lbl.Text = mr.ColumnName;
						break;
				}
			}
		}
		#endregion

		#region DeleteMetaField
		/// <summary>
		/// Handles the DeleteCommand event of the DataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		void dgMapping_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			lip = (ListImportParameters)ViewState["lip"];
			string mfName = e.CommandArgument.ToString();
			lip.RemoveNewMetaField(mfName);
			lip.RemoveRuleByMetaField(mfName);
			ViewState["lip"] = lip;
			Session[_keyLIP] = lip;
			BindDG();
		}
		#endregion

		#region ddi_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the ddi control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddi_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ViewState["lip"] != null)
			{
				lip = (ListImportParameters)ViewState["lip"];
				DropDownList ddl = (DropDownList)sender;
				DataGridItem dgi = (DataGridItem)ddl.NamingContainer;

				DataGrid dg = (DataGrid)dgi.NamingContainer;
				FillDefaultValues(dg, dgi);

				string mf = dgi.Cells[0].Text;

				switch (ddl.SelectedValue)
				{
					case "-1":
						lip.RemoveRuleByMetaField(mf);
						break;
					case "0":
						MappingRule mr = lip.AssignDefaultValueRule(mf);
						MetaField field = lip.GetDestinationMetaField(mf);
						if (field.IsEnum)
						{
							DropDownList dd = (DropDownList)dgi.FindControl("ddColumn");
							CHelper.SafeSelect(dd, mr.DefaultValue);
						}
						else
						{
							TextBox txt = (TextBox)dgi.FindControl("tbColumn");
							if (txt != null)
								txt.Text = mr.DefaultValue;
						}
						Label lbl = (Label)dgi.FindControl("lblColumn");
						if (lbl != null)
							lbl.Text = mr.DefaultValue;
						break;
					default:
						lip.AssignCopyValueRule(ddl.SelectedValue, mf);
						break;
				}

				ViewState["lip"] = lip;
				Session[_keyLIP] = lip;
				BindDG();
			}

		}
		#endregion

		#region GetMappingSource
		/// <summary>
		/// Gets the mapping source.
		/// </summary>
		/// <returns></returns>
		private DataTable GetMappingSource()
		{
			if (lip == null)
				return null;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("metaFieldName", typeof(string)));
			dt.Columns.Add(new DataColumn("metaField", typeof(string)));
			dt.Columns.Add(new DataColumn("column", typeof(string)));
			dt.Columns.Add(new DataColumn("canDelete", typeof(bool)));
			dt.Columns.Add(new DataColumn("canBeDefault", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsTitleField", typeof(bool)));
			DataRow dr;

			MetaField[] mas = lip.GetDestinationMetaFields();
			foreach (MetaField mf in mas)
			{
				if (mf.InPrimaryKey)
					continue;
				
				bool canDelete = mf.Owner == null;

				dr = dt.NewRow();
				dr["metaFieldName"] = mf.Name;
				if (canDelete)
					dr["metaField"] = GetMetaFieldLink(mf.FriendlyName + " (" + ((mf.GetOriginalMetaType() == null) ? mf.TypeName : CHelper.GetResFileString(mf.GetMetaType().FriendlyName)) + ")", mf.Name);
				else
					dr["metaField"] = mf.FriendlyName + " (" + ((mf.GetOriginalMetaType() == null) ? mf.TypeName : CHelper.GetResFileString(mf.GetMetaType().FriendlyName)) + ")";

				MappingRule mr = lip.GetRuleByMetaField(mf.Name);
				if (mr != null)
					dr["column"] = mr.ColumnName;
				else
					dr["column"] = "";

				dr["canDelete"] = canDelete;
				dr["canBeDefault"] = !mf.IsNullable;

				dr["IsTitleField"] = lip.TitleFieldName == mf.Name;
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region ucWizard_FinishButtonClick
		/// <summary>
		/// Handles the FinishButtonClick event of the ucWizard control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.WizardNavigationEventArgs"/> instance containing the event data.</param>
		void ucWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
		{
			if (ViewState["lip"] != null)
			{
				lip = (ListImportParameters)ViewState["lip"];
				FillDefaultValues(dgMapping, null);
				MappingError[] mas = ListManager.Import(lip);
				if (mas.Length == 0)
					lblResult.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "tImportWasSuccessfull").ToString();
				else
				{
					ViewState["ErrorLog"] = ListManager.GetErrorLog(mas);
					string sAction = String.Format("<a href=\"{1};\">{0}</a>",
						GetGlobalResourceObject("IbnFramework.ListInfo", "tErrorList").ToString(),
						this.Page.ClientScript.GetPostBackClientHyperlink(lbErrorLog, ""));

					lblResult.Text = String.Format(
						GetGlobalResourceObject("IbnFramework.ListInfo", "tImportWithErrors").ToString(),
							sAction);
				}
				string cmd = String.Empty;
				if (Request["CommandName"] != null)
				{
					CommandParameters cp = new CommandParameters(Request["CommandName"]);
					cmd = cp.ToString();
				}
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentWindowScript(this.Page, cmd);
			}
		}
		#endregion

		#region ErrorLog
		void lbErrorLog_Click(object sender, EventArgs e)
		{
			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.Charset = "utf-8";
			Response.AddHeader("Content-Type", "application/octet-stream");
			Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", "ErrorLog.html"));
			Response.BinaryWrite(System.Text.Encoding.UTF8.GetBytes(ViewState["ErrorLog"].ToString()));
			Response.End();
		}
		#endregion

		#region FillDefaultValues
		/// <summary>
		/// Fills the default values for LIP.
		/// </summary>
		/// <param name="dg">The dg.</param>
		/// <param name="excluded">The excluded.</param>
		private void FillDefaultValues(DataGrid dg, DataGridItem excluded)
		{
			foreach (DataGridItem dgi in dg.Items)
			{
				if (excluded != null && excluded == dgi)
					continue;
				string mfName = dgi.Cells[0].Text;

				CheckBox cb = (CheckBox)dgi.FindControl("cbDefaultItem");
				if (cb != null && cb.Checked)
					lip.TitleFieldName = mfName;

				MappingRule mr = lip.GetRuleByMetaField(mfName);
				if (mr != null && mr.RuleType == MappingRuleType.DefaultValue)
				{
					bool isChangeable = IsChangeableField(mfName);

					if (isChangeable)
					{
						MetaField field = lip.GetDestinationMetaField(mfName);
						if (field.IsEnum)
						{
							DropDownList dd = (DropDownList)dgi.FindControl("ddColumn");
							if (dd != null)
								lip.AssignDefaultValueRule(mfName, dd.SelectedValue);
						}
						else
						{
							TextBox txt = (TextBox)dgi.FindControl("tbColumn");
							if (txt != null)
								lip.AssignDefaultValueRule(mfName, txt.Text);
						}
					}
					else
					{
						Label lbl = (Label)dgi.FindControl("lblColumn");
						if (lbl != null)
							lip.AssignDefaultValueRule(mfName, lbl.Text);
					}
				}
			}

			//Check for New Enum
			//foreach (DataGridItem dgi in dg.Items)
			//{
			//    string mfName = dgi.Cells[0].Text;
			//    MappingRule mr = lip.GetRuleByMetaField(mfName);
			//    CheckBox cb = (CheckBox)dgi.FindControl("cbIsEnum");
			//    MetaField field = lip.GetDestinationMetaField(mfName);
			//    if (cb != null && cb.Visible)
			//    {
			//        if(cb.Checked && !lip.NewEnums.Contains(mfName))
			//            lip.NewEnums.Add(mfName);
			//        if (!cb.Checked && lip.NewEnums.Contains(mfName))
			//            lip.NewEnums.Remove(mfName);
			//    }
			//}
		}
		#endregion

		#region IsChangeableField
		/// <summary>
		/// Determines whether [is changeable field] [the specified mf name].
		/// </summary>
		/// <param name="mfName">Name of the mf.</param>
		/// <returns>
		/// 	<c>true</c> if [is changeable field] [the specified mf name]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsChangeableField(string mfName)
		{
			bool isChangeable = true;
			foreach (MetaField field in lip.GetDestinationMetaFields())
				if (field.Name == mfName)
				{
					if (field.Owner != null)
					{
						isChangeable = false;
						break;
					}
				}
			return isChangeable;
		}
		#endregion

		#region ucWizard_CancelButtonClick
		/// <summary>
		/// Handles the CancelButtonClick event of the ucWizard control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ucWizard_CancelButtonClick(object sender, EventArgs e)
		{
			this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), Guid.NewGuid().ToString("N"),
				"window.close();", true);
		}
		#endregion

		#region btnClose_Click
		/// <summary>
		/// Handles the Click event of the btnClose control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnClose_Click(object sender, EventArgs e)
		{
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty);
		}
		#endregion


		#region ProcessFileCache
		/// <summary>
		/// Processes the file cache.
		/// </summary>
		/// <param name="path">The path.</param>
		public static void ProcessFileCache(string path)
		{
			DirectoryInfo dir = new DirectoryInfo(path);
			System.IO.FileInfo[] fiMas = dir.GetFiles("*.*");
			foreach (System.IO.FileInfo fi in fiMas)
			{
				if (String.Compare(fi.Name, "readme.txt", true) == 0)
					continue;

				if (fi.CreationTime < DateTime.Now.AddMinutes(-1440))
				{
					try
					{
						fi.Delete();
					}
					catch (Exception ex)
					{
						CHelper.GenerateErrorReport(ex);
					}
				}
			}
		}
		#endregion

		private bool Is2007OfficeSystemDriverInstalled(string fileName)
		{
			try
			{
				OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=Yes\"");
				con.Open();
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}