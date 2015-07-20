using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Core.Business.Mapping;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta.Schema;
using Mediachase.Ibn.Service;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.MetaDataPlus.Import;
using Mediachase.MetaDataPlus.Import.Parser;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityImport : System.Web.UI.UserControl
	{
		protected string HeaderText = String.Empty;
		protected string SubHeaderText = String.Empty;
		protected string StepText = String.Empty;
		private const int _maxStepCount = 3;
		private string prevStepId = String.Empty;
		ImportRequest _ir;
		
		#region _className
		private string _className
		{
			get
			{
				if (Request["ClassName"] != null)
					return Request["ClassName"];
				else
					return String.Empty;
			}
		} 
		#endregion

		#region _commandName
		private string _commandName
		{
			get
			{
				if (Request["CommandName"] != null)
					return Request["CommandName"];
				else
					return String.Empty;
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
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			DefineHeaderTexts();
			ucWizard.StepStyle.CssClass = (ucWizard.ActiveStep.ID == "step4") ? String.Empty : "wizardStep";
		}

		#region WizardDescribe
		/// <summary>
		/// Wizard Describing
		/// </summary>
		private void WizardDescribe()
		{
			ucWizard.DisplaySideBar = false;
			ucWizard.DisplayCancelButton = true;

			ucWizard.CancelButtonText = CHelper.GetResFileString("{IbnFramework.Common:tClose}");
			btnClose.Text = CHelper.GetResFileString("{IbnFramework.Common:tClose}");
			ucWizard.FinishCompleteButtonText = CHelper.GetResFileString("{IbnFramework.Common:tSave}");
			ucWizard.StartNextButtonText = CHelper.GetResFileString("{IbnFramework.Common:tNext}") + " >";
			ucWizard.StepNextButtonText = CHelper.GetResFileString("{IbnFramework.Common:tNext}") + " >";
			ucWizard.StepPreviousButtonText = CHelper.GetResFileString("{IbnFramework.Common:tPrev}") + " <";
			ucWizard.FinishPreviousButtonText = CHelper.GetResFileString("{IbnFramework.Common:tPrev}") + " <";
		}
		#endregion

		#region CreateDelegates
		/// <summary>
		/// Creates the delegates.
		/// </summary>
		private void CreateDelegates()
		{
			ucWizard.CancelButtonClick += new EventHandler(ucWizard_CancelButtonClick);
			ucWizard.ActiveStepChanged += new EventHandler(ucWizard_ActiveStepChanged);
			ucWizard.FinishButtonClick += new WizardNavigationEventHandler(ucWizard_FinishButtonClick);

			ddDelimeter.SelectedIndexChanged += new EventHandler(ddCSV_SelectedIndexChanged);
			ddTextQualifier.SelectedIndexChanged += new EventHandler(ddCSV_SelectedIndexChanged);
			ddEncoding.SelectedIndexChanged += new EventHandler(ddCSV_SelectedIndexChanged);

			dgMapping.ItemDataBound += new DataGridItemEventHandler(dgMapping_ItemDataBound);
			
			lbErrorLog.Click += new EventHandler(lbErrorLog_Click);
		}
		#endregion

		#region ApplyStepsLocalization
		/// <summary>
		/// Applies the steps localization.
		/// </summary>
		private void ApplyStepsLocalization()
		{
			lgdSourceType.AddText(GetGlobalResourceObject("IbnFramework.Common", "imSelectSourceType").ToString());
			lgdFile.AddText(GetGlobalResourceObject("IbnFramework.Common", "imSelectSourceFile").ToString());

			bhCSV.AddText(GetGlobalResourceObject("IbnFramework.Common", "tImportInformation").ToString());
			bhMapping.AddText(GetGlobalResourceObject("IbnFramework.Common", "tMapping").ToString());
		}
		#endregion

		#region DefineHeaderTexts
		/// <summary>
		/// Defines the header texts.
		/// </summary>
		private void DefineHeaderTexts()
		{
			HeaderText = CHelper.GetResFileString("{IbnFramework.Common:tWizardImportHeader}");
			SubHeaderText = CHelper.GetResFileString("{IbnFramework.Common:tWizardImportSubHeader}");
			if (ucWizard.ActiveStep.ID == "step1")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.Common:tStepByStep}"),
					"1", _maxStepCount.ToString());
			else if (ucWizard.ActiveStep.ID == "step3")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.Common:tStepByStep}"),
					"2", _maxStepCount.ToString());
			else if (ucWizard.ActiveStep.ID == "step4")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.Common:tStepByStep}"),
					"3", _maxStepCount.ToString());
			else
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.Common:tStepByStep}"),
					"&gt;3", _maxStepCount.ToString());
		}
		#endregion

		#region PreBindSteps
		/// <summary>
		/// Default data bind.
		/// </summary>
		private void PreBindSteps()
		{
			rbSourceType.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.Common", "imExcel").ToString(), "0"));
			rbSourceType.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.Common", "imXML").ToString(), "1"));
			rbSourceType.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.Common", "imCSV").ToString(), "2"));
			if(String.Compare(_className, ContactEntity.GetAssignedMetaClassName(), true) == 0)
				rbSourceType.Items.Add(new ListItem(" " + GetGlobalResourceObject("IbnFramework.Common", "imVCF").ToString(), "3"));

			CHelper.SafeSelect(rbSourceType, "0");

			ddDelimeter.Items.Add(new ListItem(",", ","));
			ddDelimeter.Items.Add(new ListItem(".", "."));
			ddDelimeter.Items.Add(new ListItem(";", ";"));
			ddDelimeter.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "tSpaceValue").ToString(), "space"));
			ddDelimeter.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "tTabValue").ToString(), "tab"));

			ddTextQualifier.Items.Add(new ListItem("\"", "\""));
			ddTextQualifier.Items.Add(new ListItem("'", "'"));

			ddEncoding.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "tDefault").ToString(), "Default"));
			ddEncoding.Items.Add(new ListItem("ASCII", "ASCII"));
			ddEncoding.Items.Add(new ListItem("UTF-8", "UTF8"));
			ddEncoding.Items.Add(new ListItem("Unicode", "Unicode"));
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

			IIncomingDataParser parser = new CsvIncomingDataParser(String.Empty, true, delimeter, textQualifier, true, enc);
			DataSet rawData = parser.Parse(Server.MapPath(hdnFilePath.Value), null);

			foreach (DataColumn column in rawData.Tables[0].Columns)
			{
				column.ColumnName = column.ColumnName.Trim(textQualifier);
			}

			return rawData;
		}
		#endregion

		#region GetMappingSource
		/// <summary>
		/// Gets the mapping source.
		/// </summary>
		/// <returns></returns>
		private DataTable GetMappingSource()
		{
			if (ViewState["_ir"] == null)
				return null;
			_ir = (ImportRequest)ViewState["_ir"];

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("metaFieldName", typeof(string)));
			dt.Columns.Add(new DataColumn("metaField", typeof(string)));
			dt.Columns.Add(new DataColumn("column", typeof(string)));
			DataRow dr;

			MetaClass mc = MetaDataWrapper.GetMetaClassByName(_className);
			MappingElementBuilder meb = new MappingElementBuilder(_ir.MappingDocument);
			foreach (MetaField mf in mc.Fields)
			{
				if (mf.InPrimaryKey || mf.IsBackReference || mf.IsLink || mf.IsReferencedField || 
					!SchemaAttribute.CheckIsUpdatable(mf))
					continue;

				if (mf.IsAggregation)
				{
					#region IsAggregation
					string aggrClassName = mf.Attributes.GetValue<string>(McDataTypeAttribute.AggregationMetaClassName);
					MetaClass aggrClass = MetaDataWrapper.GetMetaClassByName(aggrClassName);
					foreach (MetaField aggrField in aggrClass.Fields)
					{
						if (aggrField.InPrimaryKey || aggrField.IsBackReference || aggrField.IsLink ||
							aggrField.IsReferencedField ||
							!SchemaAttribute.CheckIsUpdatable(mf) ||
							aggrField.Attributes.ContainsKey(McDataTypeAttribute.AggregationMark))
							continue;

						dr = dt.NewRow();
						string uniqName = String.Format("{0}.{1}", mf.Name, aggrField.Name);
						dr["metaFieldName"] = uniqName;
						dr["metaField"] = CHelper.GetResFileString(aggrField.FriendlyName) + " (" + ((aggrField.GetOriginalMetaType() == null) ? aggrField.TypeName : CHelper.GetResFileString(aggrField.GetMetaType().FriendlyName)) + ")";

						MappingRule mr = meb.GetRuleByMetaField(uniqName);
						
						if (mr != null)
							dr["column"] = mr.ColumnName;
						else
							dr["column"] = String.Empty;
						dt.Rows.Add(dr);
					} 
					#endregion

					continue;
				}

				#region NonAggregation
				dr = dt.NewRow();
				dr["metaFieldName"] = mf.Name;
				dr["metaField"] = CHelper.GetResFileString(mf.FriendlyName) + " (" + ((mf.GetOriginalMetaType() == null) ? mf.TypeName : CHelper.GetResFileString(mf.GetMetaType().FriendlyName)) + ")";

				MappingRule mr1 = meb.GetRuleByMetaField(mf.Name);

				if (mr1 != null)
					dr["column"] = mr1.ColumnName;
				else
					dr["column"] = String.Empty;

				dt.Rows.Add(dr); 
				#endregion
			}
			return dt;
		}
		#endregion

		#region CSV Params Changing
		/// <summary>
		/// Handles the SelectedIndexChanged event of the ddCSV control.
		/// Parse and return IMDR for CSV file
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ddCSV_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataSet rawData = GetRawDataForCSV();
			InitializeMappingDocumentRequest imdr = new InitializeMappingDocumentRequest(_className, rawData, 0);
			InitializeMappingDocumentResponse resp = (InitializeMappingDocumentResponse)BusinessManager.Execute(imdr);
			MappingDocument md = resp.MappingDocument;
			ViewState["_ir"] = new ImportRequest(_className, rawData, md);
			BindDG();
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
			_ir = (ImportRequest)ViewState["_ir"];

			MappingElementBuilder meb = new MappingElementBuilder(_ir.MappingDocument);

			foreach (DataGridItem dgi in dg.Items)
			{
				if (excluded != null && excluded == dgi)
					continue;
				string mfName = dgi.Cells[0].Text;
				MetaField field = FormController.GetMetaField(_className, mfName);
				
				MappingRule mr = meb.GetRuleByMetaField(mfName);
				if (mr != null && mr.RuleType == MappingRuleType.DefaultValue)
				{
					if (field.IsEnum || field.IsReference)
					{
						DropDownList dd = (DropDownList)dgi.FindControl("ddColumn");
						if (dd != null && dd.SelectedValue != "-1")
							meb.AssignDefaultValueRule(mfName, dd.SelectedValue);
					}
					else
					{
						TextBox txt = (TextBox)dgi.FindControl("tbColumn");
						if (txt != null)
							meb.AssignDefaultValueRule(mfName, txt.Text);
					}
				}
			}

			ViewState["_ir"] = _ir;
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

				#region Commented  may be needed
				//string ownFieldName = mfName;
				//string aggrFieldName = String.Empty;
				//string aggrClassName = String.Empty;
				//MetaClass ownClass = MetaDataWrapper.GetMetaClassByName(_className);
				//MetaClass aggrClass = null;
				//MetaField ownField = null;
				//MetaField aggrField = null;
				//if (ownFieldName.Contains("."))
				//{
				//    string[] mas = ownFieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				//    ownFieldName = mas[0];
				//    aggrFieldName = mas[1];
				//    ownField = MetaDataWrapper.GetMetaFieldByName(ownClass, ownFieldName);
				//    aggrClassName = ownField.Attributes.GetValue<string>(McDataTypeAttribute.AggregationMetaClassName);
				//    aggrField = MetaDataWrapper.GetMetaFieldByName(aggrClassName, aggrFieldName);
				//}
				//else
				//    ownField = MetaDataWrapper.GetMetaFieldByName(ownClass, ownFieldName); 
				#endregion
				
				_ir = (ImportRequest)ViewState["_ir"];

				MetaField field = FormController.GetMetaField(_className, mfName);
				MappingElementBuilder meb = new MappingElementBuilder(_ir.MappingDocument);
				MappingRule mr = meb.GetRuleByMetaField(mfName);

				ddi.Items.Clear();
				ddi.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "tNotSetValue").ToString(), "-1"));
				if (field.GetOriginalMetaType() != null)
					ddi.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "tDefaultValue").ToString(), "0"));

				foreach (DataColumn dc in _ir.Data.Tables[0].Columns)
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
				switch (val)
				{
					case "-1":	//Not Set
						txt.Visible = false;
						dd.Visible = false;
						break;
					case "0":	//Default Value
						if (field.IsEnum)
						{
							dd.Visible = true;
							txt.Visible = false;
							dd.Items.Clear();
							MetaFieldType mft = field.GetMetaType();
							foreach (MetaEnumItem mei in mft.EnumItems)
								dd.Items.Add(new ListItem(CHelper.GetResFileString(mei.Name), mei.Handle.ToString()));
							if (!String.IsNullOrEmpty(mr.DefaultValue))
								CHelper.SafeSelect(dd, mr.DefaultValue);
						}
						else if (field.IsReference)
						{
							dd.Visible = true;
							txt.Visible = false;
							dd.Items.Clear();
							string refClassName = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName);
							MetaClass refClass = MetaDataWrapper.GetMetaClassByName(refClassName);
							EntityObject[] list = BusinessManager.List(refClassName, (new FilterElementCollection()).ToArray());

							dd.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "tNotSetValue").ToString(), "-1"));
							foreach (EntityObject eo in list)
								dd.Items.Add(new ListItem(CHelper.GetResFileString(eo[refClass.TitleFieldName].ToString()), eo.PrimaryKeyId.Value.ToString()));
							if (!String.IsNullOrEmpty(mr.DefaultValue))
								CHelper.SafeSelect(dd, mr.DefaultValue);
						}
						else
						{
							dd.Visible = false;
							txt.Visible = true;
							txt.Text = mr.DefaultValue;
						}
						break;
					default:	//CopyValue
						txt.Visible = false;
						dd.Visible = false;
						break;
				}

				//Update UpdatePanel with lbl & txt && dd (upValues)
				foreach (Control c in e.Item.Cells[3].Controls)
				{
					if (c is UpdatePanel)
						((UpdatePanel)c).Update();
				}

			}
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
			_ir = (ImportRequest)ViewState["_ir"];
			if (_ir != null)
			{
				MappingElementBuilder meb = new MappingElementBuilder(_ir.MappingDocument);
				DropDownList ddl = (DropDownList)sender;
				DataGridItem dgi = (DataGridItem)ddl.NamingContainer;

				DataGrid dg = (DataGrid)dgi.NamingContainer;
				FillDefaultValues(dg, dgi);
				_ir = (ImportRequest)ViewState["_ir"];

				string mf = dgi.Cells[0].Text;

				switch (ddl.SelectedValue)
				{
					case "-1":
						meb.RemoveRuleByMetaField(mf);
						break;
					case "0":
						MetaField field = FormController.GetMetaField(_className, mf);
						MappingRule mr = meb.AssignDefaultValueRule(mf, field.DefaultValue);
						if (field.IsEnum || field.IsReference)
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
						break;
					default:
						meb.AssignCopyValueRule(ddl.SelectedValue, mf);
						break;
				}
				ViewState["_ir"] = _ir;
				BindDG();
			}

		}
		#endregion

		#region ucWizard_ActiveStepChanged
		/// <summary>
		/// Handles the ActiveStepChanged event of the ucWizard control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ucWizard_ActiveStepChanged(object sender, EventArgs e)
		{
			//step1
			if (ucWizard.ActiveStep.ID == "step1")
			{
				ViewState["_ir"] = null;
			}

			//step2
			if (ucWizard.ActiveStep.ID == "step3")
			{
				#region upload file
				if (fSourceFile.PostedFile != null && fSourceFile.PostedFile.ContentLength > 0)
				{
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

				divCSV.Visible = (rbSourceType.SelectedIndex == 2);
				
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
						case 3:
							rawData = VCardUtil.ConvertFile(Server.MapPath(hdnFilePath.Value));
							break;
					}
				}
				catch (Exception ex)
				{
					CHelper.GenerateErrorReport(ex);
					ViewState["ServiceError"] = true;
					ViewState["ErrorFileName"] = Server.MapPath(hdnFilePath.Value);
					ucWizard.MoveTo(this.step4);
					return;
				}
				#endregion

				if (ViewState["_ir"] == null)
				{
					InitializeMappingDocumentRequest imdr = new InitializeMappingDocumentRequest(_className, rawData, 0);
					InitializeMappingDocumentResponse resp = (InitializeMappingDocumentResponse)BusinessManager.Execute(imdr);
					MappingDocument md = resp.MappingDocument;
					_ir = new ImportRequest(_className, rawData, md);
					ViewState["_ir"] = _ir;
				}
				BindDG();
			}

			if (ucWizard.ActiveStep.ID == "step4")
			{
				if (ViewState["ServiceError"] != null && (bool)ViewState["ServiceError"])
				{
					string fileName = ViewState["ErrorFileName"].ToString();
					if (fileName.EndsWith("xlsx") && !Is2007OfficeSystemDriverInstalled(fileName))
						lblResult.Text = GetGlobalResourceObject("IbnFramework.Common", "McOleDbServiceWarningXlsx").ToString();
					else
						lblResult.Text = GetGlobalResourceObject("IbnFramework.Common", "McOleDbServiceWarning").ToString();
				}
				foreach (Control c in ucWizard.ActiveStep.Controls)
				{
					if (c is Button)
					{
						Button btn = (Button)c;
						string param = String.Empty;
						if (!String.IsNullOrEmpty(_commandName))
							param = (new CommandParameters(_commandName)).ToString();
						string script = Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, param);
						script += " return false;";
						btn.OnClientClick = script;
					}
				}
			}
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
			_ir = (ImportRequest)ViewState["_ir"];
			if (_ir != null)
			{
				FillDefaultValues(dgMapping, null);
				_ir = (ImportRequest)ViewState["_ir"];
				ImportResponse irr = (ImportResponse)BusinessManager.Execute(_ir);
				MappingError[] mas = irr.Errors;
				if (mas.Length == 0)
					lblResult.Text = GetGlobalResourceObject("IbnFramework.Common", "tImportWasSuccessfull").ToString();
				else
				{
					ViewState["ErrorLog"] = MappingError.GetErrorLog(mas);
					string sAction = String.Format("<a href=\"{1};\">{0}</a>",
						GetGlobalResourceObject("IbnFramework.Common", "tErrorList").ToString(),
						this.Page.ClientScript.GetPostBackClientHyperlink(lbErrorLog, String.Empty));

					lblResult.Text = String.Format(
						GetGlobalResourceObject("IbnFramework.Common", "tImportWithErrors").ToString(),
							sAction);
				}
				string param = String.Empty;
				if (!String.IsNullOrEmpty(_commandName))
					param = (new CommandParameters(_commandName)).ToString();
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentFromFrameScript(this.Page, param);
			}
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
			string param = String.Empty;
			if (!String.IsNullOrEmpty(_commandName))
				param = (new CommandParameters(_commandName)).ToString();
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, param);
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
			string param = String.Empty;
			if (!String.IsNullOrEmpty(_commandName))
				param = (new CommandParameters(_commandName)).ToString();
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, param);
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
					fi.Delete();
			}
		}
		#endregion

		#region Is2007OfficeSystemDriverInstalled
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
		#endregion
	}
}