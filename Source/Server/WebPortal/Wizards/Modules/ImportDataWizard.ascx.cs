using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Resources;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn;
using Mediachase.Ibn.Service;
using Mediachase.Ibn.Web.Interfaces;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.Import;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus.Import;
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Wizards.Modules
{
	/// <summary>
	/// Summary description for ImportDataWizard.
	/// </summary>
	public partial class ImportDataWizard : System.Web.UI.UserControl, IWizardControl
	{
		#region HTML Vars

		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strComWd", typeof(ImportDataWizard).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		ArrayList subtitles = new ArrayList();
		ArrayList steps = new ArrayList();
		private int _stepCount = 2;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindStep1();
		}

		#region BindStep1
		private void BindStep1()
		{
			pastStep.Value = "1";

			lgdSourceType.InnerText = LocRM.GetString("imSelectSourceType");
			lgdFile.InnerText = LocRM.GetString("imSelectSourceFile");
			lgdFields.InnerText = LocRM.GetString("imFieldsMatching");

			rbSourceType.Items.Add(new ListItem(" " + LocRM.GetString("imExcel"), "0"));
			rbSourceType.Items.Add(new ListItem(" " + LocRM.GetString("imXML"), "1"));

			rbSourceType.Items[0].Selected = true;
		}
		#endregion

		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount + 1; i++)
				((Panel)steps[i]).Visible = false;
			((Panel)steps[step - 1]).Visible = true;

			//if(step==1)
			//{
			//    pastStep.Value="1";
			//}

			#region step2
			if (step == 2)
			{
				if (pastStep.Value == "1" && fSourceFile.PostedFile != null && fSourceFile.PostedFile.ContentLength > 0)
				{
					ProcessFileCache(Server.MapPath(CommonHelper.ChartPath));
					String dir = CommonHelper.ChartPath;
					string wwwpath = dir + Guid.NewGuid().ToString();
					switch (rbSourceType.SelectedValue)
					{
						case "0":
							//wwwpath += ".xls";
							// OZ: Added XLS and XLSX extensions
							wwwpath += Path.GetExtension(fSourceFile.PostedFile.FileName);
							break;
						case "1":
							wwwpath += ".xml";
							break;
						default:
							break;
					}
					wwwPath.Value = wwwpath;
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

					IIncomingDataParser parser = null;
					DataSet rawData = null;
					switch (rbSourceType.SelectedIndex)
					{
						case 0:
							IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), ConfigurationManager.AppSettings["McOleDbServiceString"]);
							rawData = helper.ConvertExcelToDataSet(Server.MapPath(wwwPath.Value));
							break;
						case 1:
							parser = new XmlIncomingDataParser();
							rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
							break;
					}

					try
					{
						//rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
						DataTable dtSource = rawData.Tables[0];
						DataTable dt = new DataTable();
						dt.Columns.Add(new DataColumn("SourceField", typeof(string)));
						dt.Columns.Add(new DataColumn("IBNField", typeof(string)));
						dt.Columns.Add(new DataColumn("realIBNFieldName", typeof(string)));
						dt.Columns.Add(new DataColumn("IsIn", typeof(bool)));
						DataRow dr;
						MappingMetaClass mc = null;
						mc = new IncidentMappingMetaClass();
						foreach (ColumnInfo ci in mc.ColumnInfos)
						{
							dr = dt.NewRow();
							dr["SourceField"] = LocRM.GetString("imNotSet");
							if (ci.Field.IsSystem)
							{
								dr["IBNField"] = LocRM.GetString("tw" + ci.FieldName);
								dr["realIBNFieldName"] = ci.FieldName;
							}
							else
							{
								dr["IBNField"] = ci.FieldFriendlyName;
								dr["realIBNFieldName"] = ci.FieldName;
							}
							dr["IsIn"] = true;
							dt.Rows.Add(dr);
						}

						DataTable dtColumns = new DataTable();
						dtColumns.Columns.Add(new DataColumn("SourceField", typeof(string)));
						dtColumns.Columns.Add(new DataColumn("IsIn", typeof(bool)));
						foreach (DataColumn dc in dtSource.Columns)
						{
							dr = dtColumns.NewRow();
							dr["SourceField"] = dc.ColumnName;
							dr["IsIn"] = true;
							dtColumns.Rows.Add(dr);
						}
						ViewState["Fields"] = dt;
						ViewState["SourceFields"] = dtColumns;
						BindDG();
					}
					catch
					{
						step2.Visible = false;
						step4.Visible = false;
						step5.Visible = false;
						step6.Visible = true;
						((WizardTemplate)Page.Controls[0]).btnNext.Visible = false;
						((WizardTemplate)Page.Controls[0]).btnBack.Visible = false;
						lblFinalResult.Text = String.Format(LocRM.GetString("imErrorListText"),
							String.Format("<a href='mailto:{0}'>{0}</a>", GlobalResourceManager.Strings["SupportEmail"]));
						return;
					}
				}
				((WizardTemplate)Page.Controls[0]).btnNext.Disabled = false;
				if (ViewState["Fields"] != null)
					BindDG();
				else
				{
					((WizardTemplate)Page.Controls[0]).btnNext.Disabled = true;
					grdFields.DataSource = null;
					grdFields.DataBind();
				}

				pastStep.Value = "2";
			}
			#endregion

			#region step3 - not Lists
			if (step == 3)
			{
				DataTable dtFields = (DataTable)ViewState["Fields"];
				IIncomingDataParser parser = null;
				DataSet rawData = null;
				switch (rbSourceType.SelectedIndex)
				{
					case 0:
						IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), ConfigurationManager.AppSettings["McOleDbServiceString"]);
						rawData = helper.ConvertExcelToDataSet(Server.MapPath(wwwPath.Value));
						break;
					case 1:
						parser = new XmlIncomingDataParser();
						rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
						break;
				}

				//DataSet rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
				DataTable dtSource = rawData.Tables[0];
				MappingMetaClass mc = null;
				mc = new IncidentMappingMetaClass();
				MetaDataPlus.Import.Rule mapping = mc.CreateClassRule();
				DataTable dtColumns = (DataTable)ViewState["SourceFields"];
				foreach (DataRow dr in dtColumns.Rows)
				{
					string srcColumnName = dr["SourceField"].ToString();
					DataColumn dc = dtSource.Columns[srcColumnName];
					DataRow[] drF = dtFields.Select("SourceField = '" + srcColumnName + "'");
					if (drF.Length > 0)
					{

						string realIBNField = drF[0]["realIBNFieldName"].ToString();
						MetaField mf = mc.GetColumnInfo(realIBNField).Field;
						if (mf != null)
						{
							mapping.Add(new RuleItem(srcColumnName, dc.DataType, mf, FillTypes.CopyValue));
						}
					}
					else
						mapping.Add(new RuleItem(srcColumnName, dc.DataType));
				}

				try
				{
					FillResult fr = null;
					fr = ((IncidentMappingMetaClass)mc).FillData(FillDataMode.New, dtSource, mapping, 0);
					if (fr.ErrorRows > 0)
					{
						string sText = String.Format(LocRM.GetString("imSomeErrors"), fr.SuccessfulRows.ToString(), fr.ErrorRows.ToString());
						int index = 0;
						foreach (string sError in fr.Errors)
						{
							if ((++index) > 5)
								break;
							else
								sText += "<br>&nbsp;&nbsp;<li>" + sError + "</li>";
						}
						lblFirstResult.Text = sText;
					}
					else
					{
						lblFirstResult.Text = String.Format(LocRM.GetString("imWasDone"), fr.SuccessfulRows.ToString());
						lblFinalResult.Text = String.Format(LocRM.GetString("imWasDone"), fr.SuccessfulRows.ToString());
						((WizardTemplate)Page.Controls[0]).btnNext.Visible = false;
						((WizardTemplate)Page.Controls[0]).btnBack.Visible = false;
						//Page.RegisterStartupScript("onfourthload","<script language='javascript'>"+
						//"NextStep();"+
						//"function NextStep(){var obj = document.getElementById('"+((WizardTemplate)Page.Controls[0]).btnNext.ClientID+"'); obj.click();}</script>");
					}
				}
				catch (Exception ex)
				{
					lblFirstResult.Text = LocRM.GetString("imSimpleError") + " " + ex.Message;
				}
				pastStep.Value = "3";
			}
			#endregion

			#region step4 - not Lists
			if (step == 4)
			{
				DataTable dtFields = (DataTable)ViewState["Fields"];
				IIncomingDataParser parser = null;
				DataSet rawData = null;
				switch (rbSourceType.SelectedIndex)
				{
					case 0:
						IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), ConfigurationManager.AppSettings["McOleDbServiceString"]);
						rawData = helper.ConvertExcelToDataSet(Server.MapPath(wwwPath.Value));
						break;
					case 1:
						parser = new XmlIncomingDataParser();
						rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
						break;
				}

				//DataSet rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
				DataTable dtSource = rawData.Tables[0];
				MappingMetaClass mc = null;
				mc = new IncidentMappingMetaClass();
				MetaDataPlus.Import.Rule mapping = mc.CreateClassRule();
				foreach (DataRow dr in dtFields.Rows)
				{
					string srcColumnName = dr["SourceField"].ToString();
					DataColumn dc = dtSource.Columns[srcColumnName];
					string realIBNField = dr["realIBNFieldName"].ToString();
					if (realIBNField.Length > 0)
					{
						MetaField mf = mc.GetColumnInfo(realIBNField).Field;
						if (mf != null)
						{
							mapping.Add(new RuleItem(srcColumnName, dc.DataType, mf, FillTypes.CopyValue));
						}
					}
					else
						mapping.Add(new RuleItem(srcColumnName, dc.DataType));
				}

				try
				{
					FillResult fr = null;
					fr = ((IncidentMappingMetaClass)mc).FillData(FillDataMode.New, dtSource, mapping, -1);
					lblFinalResult.Text = String.Format(LocRM.GetString("imWasDone"), fr.SuccessfulRows.ToString());
				}
				catch (Exception ex)
				{
					lblFinalResult.Text = LocRM.GetString("imSimpleError") + " " + ex.Message;
				}
				pastStep.Value = "5";
			}
			#endregion
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

			//subtitles.Add(LocRM.GetString("imSubSelectObject"));
			subtitles.Add(LocRM.GetString("imSubSourceType"));
			subtitles.Add(LocRM.GetString("imSubObject"));
			subtitles.Add(LocRM.GetString("imSubResult"));
			subtitles.Add(LocRM.GetString("imSub6"));

			steps.Add(step2);
			steps.Add(step4);
			steps.Add(step5);
			steps.Add(step6);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grdFields.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.grdFields.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.grdFields.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			grdFields.Visible = true;
			tdSecondField.Visible = true;
			DataTable dtFields = (DataTable)ViewState["Fields"];
			DataTable dtColumns = (DataTable)ViewState["SourceFields"];

			grdFields.DataSource = dtFields.DefaultView;
			grdFields.DataBind();

			foreach (DataGridItem dgi in grdFields.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddSFields");
				if (ddl != null)
				{
					ddl.Items.Add(new ListItem(LocRM.GetString("imNotSet"), "0"));
					foreach (DataRow dr in dtColumns.Rows)
					{
						ddl.Items.Add(new ListItem(dr["SourceField"].ToString(), dr["SourceField"].ToString()));
					}
				}
			}
		}
		#endregion

		#region DataGridCommands
		private void dg_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			grdFields.EditItemIndex = e.Item.ItemIndex;
			grdFields.DataKeyField = "IBNField";
			BindDG();
			foreach (DataGridItem dgi in grdFields.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddSFields");
				if (ddl != null)
				{
					ListItem liItem = ddl.Items.FindByValue(e.Item.Cells[4].Text);
					if (liItem != null)
						liItem.Selected = true;
					else
						ddl.SelectedValue = "0";
					HtmlAnchor aLink = (HtmlAnchor)dgi.FindControl("linkTo");
					if (aLink != null)
						Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>window.location.href=window.location.href+'#" + aLink.ClientID + "';</script>");
				}
			}
		}

		private void dg_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddSFields");
			if (ddl != null)
			{
				HtmlAnchor aLink = (HtmlAnchor)e.Item.FindControl("linkTo");
				if (aLink != null)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>window.location.href=window.location.href+'#" + aLink.ClientID + "';</script>");
			}
			grdFields.EditItemIndex = -1;
			BindDG();
		}

		private void dg_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sIBNField = grdFields.DataKeys[e.Item.ItemIndex].ToString();
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddSFields");
			if (ddl != null)
			{
				DataTable dt = (DataTable)ViewState["Fields"];
				DataRow[] dr = dt.Select("IBNField = '" + sIBNField + "'");
				if (dr.Length > 0)
				{
					dr[0]["SourceField"] = (ddl.SelectedValue == "0") ? LocRM.GetString("imNotSet") : ddl.SelectedValue;
				}
				ViewState["Fields"] = dt;
				HtmlAnchor aLink = (HtmlAnchor)e.Item.FindControl("linkTo");
				if (aLink != null)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>window.location.href=window.location.href+'#" + aLink.ClientID + "';</script>");
			}
			grdFields.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("imTopTitle"); } }
		public bool ShowSteps { get { return false; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			ShowStep(stepNumber);
			Subtitle = (string)subtitles[stepNumber - 1];
			if (((WizardTemplate)Page.Controls[0]).btnNext.Visible)
				CancelText = null;
			else
				CancelText = LocRM.GetString("tClose");
		}

		public string GenerateFinalStepScript()
		{
			string backlink = ResolveClientUrl("~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx");
			return String.Format("try{{window.opener.top.right.location.href='{0}';}} catch (e) {{}} window.close();",
				backlink);
		}

		public void CancelAction()
		{

		}
		#endregion

		public static void ProcessFileCache(string _Path)
		{
			DirectoryInfo dir = new DirectoryInfo(_Path);
			FileInfo[] fi = dir.GetFiles("*.*");
			foreach (FileInfo _fi in fi)
			{
				if (String.Compare(_fi.Name, "readme.txt", true) == 0)
					continue;

				if (_fi.CreationTime < DateTime.Now.AddMinutes(-1440))
					_fi.Delete();
			}
		}
	}
}
