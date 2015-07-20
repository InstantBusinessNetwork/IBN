using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI;
using Mediachase.UI.Web.Util;
using System.Xml;
using System.Text;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class ExportImportWizardNew : System.Web.UI.UserControl
	{
		#region Fields
		protected string MainHeaderText = string.Empty;
		protected string SubHeaderText = string.Empty;
		protected string StepHeaderText = string.Empty;
		private DataTable _dtp = null;
		private DataTable dtpImport = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the project id.
		/// </summary>
		/// <value>The project id.</value>
		public int ProjectId
		{
			get
			{
				if (Request["ProjectId"] != null)
					return int.Parse(Request["ProjectId"].ToString());
				return -1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is project MS synchronized.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is project MS synchronized; otherwise, <c>false</c>.
		/// </value>
		private bool? isProjectMSSynchronized = null;
		public bool IsProjectMSSynchronized
		{
			get
			{
				if (!isProjectMSSynchronized.HasValue)
				{
					if (ProjectId > 0)
						isProjectMSSynchronized = Project.GetIsMSProject(ProjectId);
					else
						isProjectMSSynchronized = false;
				}
				return isProjectMSSynchronized.Value;
			}
		}

		private bool _toMSPrj
		{
			get
			{
				if (Request["ToMSPrj"] != null && Request["ToMSPrj"] == "1")
					return true;
				else
					return false;
			}
		}

		private int XmlId
		{
			get
			{
				if (ViewState["EIWizard_XmlId"] != null)
				{
					return int.Parse(ViewState["EIWizard_XmlId"].ToString());
				}
				return -1;
			}
			set
			{
				ViewState["EIWizard_XmlId"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{

			Page.Form.Enctype = "multipart/form-data";
			BindStaticLabels();
			if (!IsPostBack)
			{
				BindStepsInfo();
				BindStepsLabels();
			}
		}

		protected void BindStepsInfo()
		{
			if (EIWizard.ActiveStep.ID == "step1")
			{
				rblFirstStep.Items.Clear();

				if (_toMSPrj)
					lbSUpInIBN.Text = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tFirstSynchDescription").ToString();

				if (IsProjectMSSynchronized || _toMSPrj)
				{
					ListItem li;
					li = new ListItem(" " + GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tUpdateInMSProject").ToString(), "UpdateInMS");
					rblFirstStep.Items.Add(li);
					li = new ListItem(" " + GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tUpdateInIBN").ToString(), "UpdateInIBN");
					rblFirstStep.Items.Add(li);
					rblFirstStep.SelectedIndex = 0;
					divStep1NotSyncronized.Visible = false;
					divStep1Syncronized.Visible = true;
					//lbSUpInMS.Style.Add(HtmlTextWriterStyle.Display, "none");
					lbSUpInIBN.Style.Add(HtmlTextWriterStyle.Display, "none");

					//AK - 2008-04-11 -switch to synch mode
					if (_toMSPrj)
					{
						rblFirstStep.SelectedIndex = 1;
						rblFirstStep.Visible = false;
						lbSUpInIBN.Style.Add(HtmlTextWriterStyle.Display, "");
						lbSUpInMS.Style.Add(HtmlTextWriterStyle.Display, "none");
					}
				}
				else
				{
					ListItem li = new ListItem(" " + GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tExportToMSProject").ToString(), "Export");
					rblFirstStep.Items.Add(li);
					li = new ListItem(" " + GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tImportFromMSProject").ToString(), "Import");
					rblFirstStep.Items.Add(li);
					rblFirstStep.Items[0].Selected = true;
					divStep1NotSyncronized.Visible = true;
					divStep1Syncronized.Visible = false;
					lbNSExport.Style.Add(HtmlTextWriterStyle.Display, "none");
					//lbNSSync.Style.Add(HtmlTextWriterStyle.Display, "none");
				}
			}
		}

		protected void BindStaticLabels()
		{
			MainHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tTopTitle").ToString();
			if (!_toMSPrj)
				SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tWizardFirstStep").ToString();
			else
				SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "ToMSProjSync").ToString();

		}

		protected void BindStepsLabels()
		{
			switch (EIWizard.ActiveStep.ID)
			{
				case "step1":
					if (!_toMSPrj)
						SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tWizardFirstStep").ToString();
					else
						SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "ToMSProjSync").ToString();
					StepHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tStep").ToString() + " 1 "; //+ GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tOf").ToString()+" 3";
					break;
				case "step2":
					StepHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tStep").ToString() + " 2 ";// +GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tOf").ToString() + " 3";
					switch (rblFirstStep.SelectedValue)
					{
						case "Import":
						case "UpdateInIBN":
							SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tUploadMSProjectXMLToIBN").ToString();
							break;
						case "Export":
						case "UpdateInMS":
							SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tDownloadIBNProjectXML").ToString();
							break;
					}
					break;
				case "step3":
					StepHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tStep").ToString() + " 3 ";// +GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tOf").ToString() + " 3";
					SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "s1Title").ToString();
					break;
				case "step4":
					StepHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tStep").ToString() + " 4";
					switch (rblFirstStep.SelectedValue)
					{
						case "Export":
						case "UpdateInMS":
							StepHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tStep").ToString() + " 2";
							break;
					}
					SubHeaderText = SubHeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tFinishSynchronization").ToString();
					break;
			}

		}

		protected void Page_Init(object sender, EventArgs e)
		{
			this.EIWizard.ActiveStepChanged += new EventHandler(EIWizard_ActiveStepChanged);
			this.EIWizard.NextButtonClick += new WizardNavigationEventHandler(EIWizard_NextButtonClick);
			this.EIWizard.FinishButtonClick += new WizardNavigationEventHandler(EIWizard_FinishButtonClick);
			this.btnImportAnyway.Click += new EventHandler(btnImportAnyway_Click);
			this.dgMembers.ItemDataBound += new DataGridItemEventHandler(dgMembers_ItemDataBound);
			this.dgMembersImport.ItemDataBound += new DataGridItemEventHandler(dgMembersImport_ItemDataBound);
			this.btnImportSuccessClose.Click += new EventHandler(btnImportSuccessClose_Click);
		}

		void dgMembers_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddProjectTeam");
			if (ddl != null && _dtp != null)
			{

				ddl.Attributes["onchange"] = "removeDupValue(this.value, this.id)";
				ddl.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "NotSet").ToString(), "-1"));

				foreach (DataRow dr in _dtp.Rows)
				{
					ddl.Items.Add(new ListItem(String.Concat(dr["LastName"].ToString(), " ", dr["FirstName"].ToString()), dr["UserId"].ToString()));
				}

			}
		}

		void dgMembersImport_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddProjectTeam");
			if (ddl != null && dtpImport != null)
			{

				ddl.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "NotSet").ToString(), "-1"));
				foreach (DataRow dr in dtpImport.Rows)
				{
					ddl.Items.Add(new ListItem(String.Concat(dr["LastName"].ToString(), " ", dr["FirstName"].ToString()), dr["UserId"].ToString()));
				}

			}
		}

		void btnImportSuccessClose_Click(object sender, EventArgs e)
		{
			this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("try{{window.opener.location.href='{0}';}} catch(e){{}}window.close();", ResolveClientUrl("~/Projects/ProjectView.aspx") + "?ProjectId=" + ProjectId.ToString() + "&Tab=6&ABTab=GanttChart"), true);
		}

		void btnImportAnyway_Click(object sender, EventArgs e)
		{
			if (cbImortAnyway.Checked)
			{
				SaveValues(true);
			}
			this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("try{{window.opener.location.href='{0}';}} catch(e){{}}window.close();", ResolveClientUrl("~/Projects/ProjectView.aspx") + "?ProjectId=" + ProjectId.ToString() + "&Tab=6&ABTab=GanttChart"), true);
		}

		private void SaveValues(bool raiseError)
		{
			DataTable outTable = (DataTable)ViewState["outTable"];
			if (!raiseError)
				for (int i = 0; i < outTable.Rows.Count; i++)
				{
					int principalId = -1;
					DropDownList ddl = (DropDownList)dgMembers.Items[i].FindControl("ddProjectTeam");
					if (ddl != null && ddl.SelectedItem != null)
						principalId = int.Parse(ddl.SelectedItem.Value);
					outTable.Rows[i]["PrincipalId"] = principalId;
				}
			ViewState["outTable"] = outTable;
			try
			{
				/*if(_toMSPrj)	// - switch to synch mode
					Project.UpdateIsMSProject(ProjectId);*/
				Task.TasksImport2(outTable, ProjectId, XmlId,
								 (CompletionType)Enum.Parse(typeof(CompletionType),
								 ddCType.SelectedValue), raiseError);
			}
			catch (FoundDifferenceSynchronizedProjectException)
			{
				ViewState["ImportError"] = true;
			}
		}

		private void SaveValues2()
		{
			DataTable OutTable = (DataTable)ViewState["ImportDt"];
			for (int i = 0; i < OutTable.Rows.Count; i++)
			{
				int PrincipalId = -1;
				DropDownList ddl = (DropDownList)dgMembersImport.Items[i].FindControl("ddProjectTeam");
				if (ddl != null && ddl.SelectedItem != null)
					PrincipalId = int.Parse(ddl.SelectedItem.Value);
				OutTable.Rows[i]["PrincipalId"] = PrincipalId;
			}
			Task.TasksImport(OutTable, ProjectId, XmlId);
		}

		void EIWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
		{
			if (EIWizard.ActiveStep.ID == "step3")
			{
				if (divStep3Synchronization.Visible)
				{
					SaveValues(false);
					if (ViewState["ImportError"] != null)
					{
						divStepCompleteFailed.Visible = true;
						divStepCompleteSuccess.Visible = false;
						divImportSuccess.Visible = false;
						divStepCompleteFailed.Visible = true;
						return;
					}
					else
					{
						divStepCompleteFailed.Visible = false;
						divStepCompleteSuccess.Visible = true;
						divImportSuccess.Visible = true;
						if (IsProjectMSSynchronized)
							ImportSuccessLiteral.Text = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "SynchronizationComplete").ToString();
					}
				}
				if (divStep3Import.Visible)
				{
					SaveValues2();
				}
			}


		}

		void EIWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
		{
			if (EIWizard.ActiveStep.ID == "step1")
			{
				bool t = false;
				switch (rblFirstStep.SelectedValue)
				{
					case "UpdateInIBN":
						divStep3Synchronization.Visible = true;
						divStep3Import.Visible = false;
						divExportSuccess.Visible = false;
						break;
					case "Import":
						divStep2Import.Visible = true;
						divStep3Synchronization.Visible = false;
						divStep3Import.Visible = true;
						divExportSuccess.Visible = false;
						break;
					case "Export":
					case "UpdateInMS":
						divStep2Import.Visible = false;
						t = true;
						divStep3Synchronization.Visible = false;
						divStep3Import.Visible = false;
						divExportSuccess.Visible = true;
						divImportSuccess.Visible = false;
						break;
				}
				if (t)
				{
					EIWizard.MoveTo(this.step4);
				}
			}
			if (EIWizard.ActiveStep.ID == "step2")
			{
				if (divStep2Import.Visible)//upload xml from MS Project
				{
					Page.Validate();
					if (!Page.IsValid)
					{
						EIWizard.MoveTo(this.step4);
						return;
					}
					ViewState["ImportError"] = null;
					XmlId = -1;
					try
					{
						if (mcImportFile.PostedFile != null && mcImportFile.PostedFile.ContentLength > 0)
						{
							XmlTextReader xmlreader = new XmlTextReader(mcImportFile.PostedFile.InputStream);
							bool valid = CommonHelper.ValidateXMLWithMsProjectSchema(xmlreader);
							if (valid)
							{
								mcImportFile.PostedFile.InputStream.Position = 0;
								XmlId = Project.UploadImportXML(ProjectId, mcImportFile.PostedFile.FileName, mcImportFile.PostedFile.InputStream);
							}
							else
								cvFileError.IsValid = false;
						}
						else
							cvFileError.IsValid = false;
					}
					catch (Exception ex)
					{
						CHelper.GenerateErrorReport(ex);
						cvFileError.IsValid = false;
					}
					divStep3Export.Visible = false;

				}
				else//load xml from ibn
				{
					divStep3Synchronization.Visible = false;
					divStep3Import.Visible = false;
					divStep3Export.Visible = true;
				}
				//find button
				Control startPoint = EIWizard.Controls[0].Controls[EIWizard.Controls[0].Controls.Count - 1];
				if (startPoint != null)
				    startPoint = GetControlFromColl(startPoint.Controls);
				if (startPoint != null)
				{
					Button btn = (Button)startPoint;
					btn.Attributes.Add("onclick", "MakeHide();");
				}
			}


		}

		private Control GetControlFromColl(ControlCollection coll)
		{
			Control retVal = null;
			foreach (Control c in coll)
			{
				if (c is Button && c.ID == "FinishButton")
				{
					retVal = c;
					break;

				}
				else
				{
					retVal = GetControlFromColl(c.Controls);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}

		protected void BindDataGrid()
		{
			_dtp = Project.GetListTeamMemberNamesDataTable(ProjectId);
			DataTable dt = Task.TaskImportAssignments(ProjectId, XmlId);
			dgMembers.Columns[1].HeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "MSPUser").ToString();
			dgMembers.Columns[2].HeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "IBNUser").ToString();
			dgMembers.DataSource = dt.DefaultView;
			dgMembers.DataBind();
			ViewState["outTable"] = dt;
		}

		protected void BindImportDataGrid()
		{
			dtpImport = Project.GetListTeamMemberNamesWithManagerDataTable(ProjectId);

			dgMembersImport.Columns[1].HeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "MSPUser").ToString();
			dgMembersImport.Columns[2].HeaderText = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "IBNUser").ToString();

			DataTable dt = Task.TaskImportAssignments(ProjectId, XmlId);
			ViewState["ImportDt"] = dt;
			dgMembersImport.DataSource = dt.DefaultView;
			dgMembersImport.DataBind();
		}

		void EIWizard_ActiveStepChanged(object sender, EventArgs e)
		{
			BindStepsLabels();
			if (EIWizard.ActiveStep.ID == "step1")//bind radio button list hints
			{
				if (IsProjectMSSynchronized || _toMSPrj) // Sync mode
				{
					divStep1NotSyncronized.Visible = false;
					divStep1Syncronized.Visible = true;
					switch (rblFirstStep.SelectedValue)
					{
						case "UpdateInIBN":
							lbSUpInIBN.Style.Add(HtmlTextWriterStyle.Display, "");
							lbSUpInMS.Style.Add(HtmlTextWriterStyle.Display, "none");
							break;
						case "UpdateInMS":
							lbSUpInIBN.Style.Add(HtmlTextWriterStyle.Display, "none");
							lbSUpInMS.Style.Add(HtmlTextWriterStyle.Display, "");
							break;
					}
				}
				else	// Update mode
				{
					divStep1NotSyncronized.Visible = true;
					divStep1Syncronized.Visible = false;
					switch (rblFirstStep.SelectedValue)
					{
						case "Import":
							lbNSImport.Style.Add(HtmlTextWriterStyle.Display, "");
							lbNSExport.Style.Add(HtmlTextWriterStyle.Display, "none");
							break;
						case "Export":
							lbNSImport.Style.Add(HtmlTextWriterStyle.Display, "none");
							lbNSExport.Style.Add(HtmlTextWriterStyle.Display, "");
							break;
					}
				}
			}

			if (EIWizard.ActiveStep.ID == "step3")
			{
				if (!Page.IsValid)
				{
					EIWizard.MoveTo(this.step4);
					return;
				}
				if (divStep3Synchronization.Visible)
				{
					ddCType.Items.Clear();
					ddCType.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tCTAll").ToString(), "All"));
					ddCType.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tCTAny").ToString(), "Any"));
					BindDataGrid();
					foreach (DataGridItem dgi in dgMembers.Items)
					{
						DropDownList ddl = (DropDownList)dgi.FindControl("ddProjectTeam");
						CommonHelper.SafeSelect(ddl, dgi.Cells[3].Text);
					}
				}
				if (divStep3Import.Visible)
				{
					BindImportDataGrid();
					foreach (DataGridItem dgi in dgMembersImport.Items)
					{
						DropDownList ddl = (DropDownList)dgi.FindControl("ddProjectTeam");
						CommonHelper.SafeSelect(ddl, dgi.Cells[3].Text);
					}
				}

			}
			if (EIWizard.ActiveStep.ID == "step4")
			{
				if (!Page.IsValid)
				{
					ImportSuccessLiteral.Text = GetGlobalResourceObject("IbnFramework.ImportProjectWizard", "tWizardError").ToString();
				}

				if (divExportSuccess.Visible)
				{
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						"OpenWindow('" + this.ResolveClientUrl("~/Projects/GetIBNProjectXML.aspx") + "?ProjectId=" + ProjectId.ToString() + (IsProjectMSSynchronized ? "&Synchronized=true" : "") + "',600,400);", true);
				}
			}
		}

	}
}