using System;
using System.Drawing;
using System.Resources;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Web;
using System.Data;
using System.Collections.Generic;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.SpreadSheet;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for ProjectSpreadSheet.
	/// </summary>
	public class ProjectSpreadSheet
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectSpreadSheet"/> class.
		/// </summary>
		private ProjectSpreadSheet()
		{
		}

		/// <summary>
		/// Gets the spread sheet template directory.
		/// </summary>
		/// <returns></returns>
		public static string TemplateDirectory
		{
			get
			{
				return System.Web.HttpContext.Current.Server.MapPath("~/Projects/SpreadSheetTemplates/");
			}
		}

		internal static SpreadSheetDocument InitProjectDocument(int ProjectId, out int ProjectSpreadSheetId)
		{
			ProjectSpreadSheetId = 0;

			string SpreadSheetTemplateDirectory = ProjectSpreadSheet.TemplateDirectory;

			ProjectSpreadSheetRow[] SpreadSheets = ProjectSpreadSheetRow.List(ProjectId);

			if (SpreadSheets.Length == 0)
				return null;

			ProjectSpreadSheetRow prjSpreadSheetRow = SpreadSheets[0];

			ProjectSpreadSheetId = prjSpreadSheetRow.ProjectSpreadSheetId;

			// Load Document
			SpreadSheetDocument document = new SpreadSheetDocument((SpreadSheetDocumentType)prjSpreadSheetRow.DocumentType);

			// Step 1. Load Template
			document.Template.Load(Path.Combine(SpreadSheetTemplateDirectory, prjSpreadSheetRow.BaseTemplateName));

			if (prjSpreadSheetRow.UserRows != string.Empty)
			{
				try
				{
					document.Template.LoadXml(prjSpreadSheetRow.UserRows);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex);
				}
			}

			return document;
		}

		#region LoadView
		/// <summary>
		/// Loads the view.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="Index">The index.</param>
		/// <param name="FromYear">From year.</param>
		/// <param name="ToYear">To year.</param>
		/// <returns></returns>
		public static SpreadSheetView LoadView(int ProjectId, int Index, int FromYear, int ToYear)
		{
			// Load Document
			int ProjectSpreadSheetId;
			SpreadSheetDocument document = InitProjectDocument(ProjectId, out ProjectSpreadSheetId);

			if (document == null)
				return null;

			if (Index >= 0)
			{
				foreach (ProjectSpreadSheetDataRow row in
					ProjectSpreadSheetDataRow.List(ProjectSpreadSheetId, Index))
				{
					if (((CellType)row.CellType) != CellType.AutoCalc)
					{
						Cell cellItem = document.AddCell(row.ColumnId, row.RowId, (CellType)row.CellType, row.Value);
						cellItem.Tag = row.ProjectSpreadSheetDataId;
					}
				}
			}
			else
			{
				#region LoadFact View
				foreach (ActualFinances finance in ActualFinances.List(ProjectId, ObjectTypes.Project))
				{
					string ColumnId = SpreadSheetView.GetColumnByDate(document.DocumentType, finance.Date);
					string RowId = finance.RowId;

					Cell cell = document.GetCell(ColumnId, RowId);
					if (cell == null)
					{
						cell = document.AddCell(ColumnId, RowId, CellType.Common, 0);
					}

					cell.Value += finance.Value;
				}
				#endregion
			}

			// Create View and return
			SpreadSheetView retVal = new SpreadSheetView(document, FromYear, ToYear);

			return retVal;
		}

		#endregion

		#region SaveView
		/// <summary>
		/// Saves the view.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="Index">The index.</param>
		/// <param name="view">The view.</param>
		public static void SaveView(int ProjectId, int Index, SpreadSheetView view)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Update User Rows
				ProjectSpreadSheetRow prjSpreadSheetRow = null;
				ProjectSpreadSheetRow[] SpreadSheets = ProjectSpreadSheetRow.List(ProjectId);

				if (SpreadSheets.Length == 0)
				{
					//TODO:
					throw new NotImplementedException();
				}
				else
				{
					prjSpreadSheetRow = SpreadSheets[0];
				}

				prjSpreadSheetRow.UserRows = view.Document.Template.GetUserRowXml();
				prjSpreadSheetRow.Update();

				// Update Cell Data
				foreach (Cell cell in view.ChangedCellList)
				{
					if (cell.Type != CellType.AutoCalc)
					{
						ProjectSpreadSheetDataRow.Update(prjSpreadSheetRow.ProjectSpreadSheetId,
							Index,
							cell.Position.ColumnId,
							cell.Position.RowId,
							cell.Value,
							(int)cell.Type);
					}
					else
					{
						if (cell.Tag is int)
						{
							ProjectSpreadSheetDataRow.Delete((int)cell.Tag);
						}
					}
				}

				// Delete cell from removed rows
				foreach (Cell cell in view.Document.DeletedCells)
				{
					if (cell.Tag is int)
					{
						ProjectSpreadSheetDataRow.Delete((int)cell.Tag);
					}
				}
				view.Document.DeletedCells.Clear();

				//Clean Up Actual Finances
				foreach (string RowId in view.Document.Template.DeletedUserRows)
				{
					ActualFinancesRow.Delete(RowId);
				}

				// Save business score
				if (Index >= 0)
					SaveBusinessScore(ProjectId, Index, view.Document);

				tran.Commit();
			}
		}

		internal static void SaveBusinessScore(int ProjectId, int Index, SpreadSheetDocument Document)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Delete prev results by ProjectId and Index
				BusinessScoreDataRow.Delete(ProjectId, Index);

				if (Document.Cells.Length != 0)
				{
					// Fill Business Score Hash
					BusinessScore[] bsList = BusinessScore.List();
					Hashtable bsHash = new Hashtable();

					foreach (BusinessScore bs in bsList)
					{
						bsHash.Add(bs.Key, bs.BusinessScoreId);
					}

					bool bContainsBusinessScoreRow = false;

					foreach (Row row in Document.Template.Rows)
					{
						if (bsHash.ContainsKey(row.Id))
						{
							bContainsBusinessScoreRow = true;
							break;
						}
					}

					if (bContainsBusinessScoreRow)
					{
						SpreadSheetView bsView = new SpreadSheetView(Document);

						Hashtable businessScoreHash = new Hashtable();
						List<ExpressionInfo> userValueExpList = new List<ExpressionInfo>();

						List<string> checkRelaitedList = new List<string>();

						// Calculate Business Score
						foreach (Row row in Document.Template.Rows)
						{
							if (bsHash.ContainsKey(row.Id))
							{
								// Save Business Score
								foreach (Column column in bsView.Columns)
								{
									Cell dataCell = bsView.GetCell(column.Id, row.Id);

									switch (dataCell.Type)
									{
										case CellType.Common:
											BusinessScoreDataRow newRowCommon = new BusinessScoreDataRow();
											newRowCommon.BusinessScoreId = (int)bsHash[row.Id];
											newRowCommon.ProjectId = ProjectId;
											newRowCommon.Index = Index;
											newRowCommon.Date = SpreadSheetView.GetDateByColumn(column.Id);
											newRowCommon.Value = dataCell.Value;

											businessScoreHash.Add(dataCell.Uid, newRowCommon);
											break;
										case CellType.AutoCalc:
											if (column.Id.IndexOf("T") == -1 ||
												(Document.DocumentType == SpreadSheetDocumentType.Total) ||
												(Document.DocumentType == SpreadSheetDocumentType.Year && column.Id != "TT"))
											{
												//userValueExpList.Add(dataCell.Expression);
												userValueExpList.Add(dataCell.GetExpressionInfo());

												BusinessScoreDataRow newRowAutoCalc = new BusinessScoreDataRow();
												newRowAutoCalc.BusinessScoreId = (int)bsHash[row.Id];
												newRowAutoCalc.ProjectId = ProjectId;
												newRowAutoCalc.Index = Index;
												newRowAutoCalc.Date = SpreadSheetView.GetDateByColumn(column.Id);
												if (newRowAutoCalc.Date == DateTime.MinValue)
													newRowAutoCalc.Date = DateTime.Now;
												newRowAutoCalc.Value = dataCell.Value;

												businessScoreHash.Add(dataCell.Uid, newRowAutoCalc);
											}
											break;
										case CellType.UserValue:
											//userValueExpList.Add(dataCell.Expression);
											userValueExpList.Add(dataCell.GetExpressionInfo());
											checkRelaitedList.Add(dataCell.Uid);

											BusinessScoreDataRow newRowUserValue = new BusinessScoreDataRow();
											newRowUserValue.BusinessScoreId = (int)bsHash[row.Id];
											newRowUserValue.ProjectId = ProjectId;
											newRowUserValue.Index = Index;
											newRowUserValue.Date = SpreadSheetView.GetDateByColumn(column.Id);
											newRowUserValue.Value = dataCell.Value;

											businessScoreHash.Add(dataCell.Uid, newRowUserValue);
											break;
									}
								}
							}
						}

						// Check Relaited with checkRelaitedList Items

						// Remove Common Items by UserValue
						foreach (ExpressionInfo exInfo in userValueExpList)
						{
							//ExpressionInfo exInfo = ExpressionInfo.Parse(Expression);

							foreach (string CellUID in exInfo.Params)
							{
								if (businessScoreHash.ContainsKey(CellUID))
								{
									businessScoreHash.Remove(CellUID);
								}
							}
						}

						// Save Business Score
						foreach (BusinessScoreDataRow data in businessScoreHash.Values)
						{
							if (data.Value != 0)
								data.Update();
						}
					}
				}

				tran.Commit();
			}
		}
		#endregion

		#region Activity
		/// <summary>
		/// Determines whether the specified project id is active.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified project id is active; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsActive(int ProjectId)
		{
			return ProjectSpreadSheetRow.List(ProjectId).Length != 0;
		}

		/// <summary>
		/// Activates the specified project id.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="DocumentType">Type of the document.</param>
		/// <param name="BaseTemplateName">Name of the base template.</param>
		public static void Activate(int ProjectId, SpreadSheetDocumentType DocumentType, string BaseTemplateName)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (!IsActive(ProjectId))
				{
					ProjectSpreadSheetRow newItem = new ProjectSpreadSheetRow();

					newItem.ProjectId = ProjectId;
					newItem.DocumentType = (int)DocumentType;
					newItem.BaseTemplateName = BaseTemplateName;

					newItem.Update();
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Des the ativate.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		public static void Deactivate(int ProjectId)
		{
			Deactivate(ProjectId, true);
		}
		public static void Deactivate(int ProjectId, bool recalculate)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Delete ProjectSpreadSheet
				foreach (ProjectSpreadSheetRow row in ProjectSpreadSheetRow.List(ProjectId))
				{
					row.Delete();
				}

				// Delete actual finances
				foreach (ActualFinancesRow row in ActualFinancesRow.List(ProjectId, (int)ObjectTypes.Project))
				{
					row.Delete();
				}

				// Delete ProjectBasePlanInfo
				foreach (ProjectBasePlanInfoRow row in ProjectBasePlanInfoRow.List(ProjectId))
				{
					row.Delete();
				}

				// Delete ProjectTaskBasePlanRow
				ProjectTaskBasePlanRow.DeleteByProject(ProjectId);

				// Delete Business Score Data
				BusinessScoreDataRow.DeleteByProject(ProjectId);

				// O.R. [2008-08-08] Reset AreFinancesRegistered flag for TimeTrackingBlocks 
				using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
				{
					TimeTrackingBlock[] blocks = TimeTrackingBlock.List(FilterElement.EqualElement("ProjectId", ProjectId));
					foreach (TimeTrackingBlock block in blocks)
					{
						block.Properties["AreFinancesRegistered"].Value = false;
						block.Save();
					}
				}

				// recalculate TotalApproved
				if (recalculate)
					DbTimeTracking.RecalculateAllObjectsByProject(ProjectId);

				tran.Commit();
			}
		}
		#endregion

		#region Utility
		/// <summary>
		/// Key is BasePlanSlotId, Value is Created
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static Hashtable GetFilledSlotHash(int ProjectId)
		{
			Hashtable retVal = new Hashtable();

			foreach (ProjectBasePlanInfoRow row in ProjectBasePlanInfoRow.List(ProjectId))
			{
				DateTime userTime = Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, row.Created);
				retVal.Add(row.BasePlanSlotId, userTime);
			}

			return retVal;
		}

		/// <summary>
		/// Gets the task hash.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="BasePlanSlotId">The base plan slot id.</param>
		/// <returns></returns>
		public static Dictionary<int, DateTime> GetTaskHash(int ProjectId, int BasePlanSlotId)
		{
			Dictionary<int, DateTime> result = new Dictionary<int, DateTime>();

			foreach (ProjectTaskBasePlanRow row in ProjectTaskBasePlanRow.List(ProjectId, BasePlanSlotId))
			{
				DateTime userTime = Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, row.StartDate);
				result.Add(row.TaskId, userTime);
			}

			return result;
		}

		/// <summary>
		/// Gets the fact available rows.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <returns></returns>
		public static Row[] GetFactAvailableRows(int ProjectId)
		{
			ArrayList retVal = new ArrayList();

			int ProjectSpreadSheetId;
			SpreadSheetDocument document = InitProjectDocument(ProjectId, out ProjectSpreadSheetId);

			if (document != null)
			{
				foreach (Row row in document.Template.Rows)
				{
					if (row.Expression == string.Empty)
						retVal.Add(row);

					if (row.HasChildRows)
					{
						foreach (Row chRow in row.ChildRows)
						{
							if (chRow.Expression == string.Empty)
								retVal.Add(chRow);
						}
					}
				}
			}

			return (Row[])retVal.ToArray(typeof(Row));
		}

		/// <summary>
		/// Gets the row name by id hash.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <returns></returns>
		public static Hashtable GetRowNameByIdHash(int ProjectId)
		{
			Hashtable retVal = new Hashtable();

			int ProjectSpreadSheetId;
			SpreadSheetDocument document = InitProjectDocument(ProjectId, out ProjectSpreadSheetId);

			foreach (Row row in document.Template.Rows)
			{
				retVal.Add(row.Id, row.Name);

				if (row.HasChildRows)
				{
					foreach (Row chRow in row.ChildRows)
					{
						retVal.Add(chRow.Id, chRow.Name);
					}
				}
			}

			return retVal;
		}
		#endregion

		#region === DV Block ===

		private static ResourceManager spreadSheetRes = new ResourceManager("Mediachase.IBN.Business.Resources.SpreadSheets", typeof(ReminderTemplate).Assembly);

		#region GenScript_NewRow
		private static string GenScript_NewRow(Row row)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("dhtmlXGrid_NewRow('{0}', '{1}', '{2}', '{3}', '{4}');", row.Id, row.ChildRows.Count, "leaf.gif", GetAbsolutePath("Layouts/Images/delete.gif"), spreadSheetRes.GetString("DeleteFinanceMsg"));
			return sb.ToString();
		}
		#endregion

		#region GenScript_DeleteRow
		private static string GenScript_DeleteRow(Row row)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("if (confirm('{1}')) dhtmlXGrid_DeleteRow('{0}'); ", row.Id, spreadSheetRes.GetString("DeleteFinanceMsg"));
			return sb.ToString();
		}
		#endregion

		#region BindCell
		private static void BindCell(SpreadSheetView view, XmlDocument doc, XmlNode parent_node, string ColumnId, string RowId)
		{
			XmlNode cell_node = parent_node.AppendChild(doc.CreateElement("cell"));
			Cell _cell = view.GetCell(ColumnId, RowId);
			string Value = view.GetValue(ColumnId, RowId);

			if (_cell != null)
			{
				if (_cell.Type == CellType.AutoCalc)
				{
					cell_node.Attributes.Append(doc.CreateAttribute("class")).Value = "dhtmlxGrid_bold";
					//Value = "<span></span>"+Value;
				}
				else if (_cell.Type == CellType.UserValue)
				{
					cell_node.Attributes.Append(doc.CreateAttribute("class")).Value = "dhtmlxGrid_italic";
				}
				if (_cell.ReadOnly)
					Value = "<div></div>" + Value;

				//ѕустые р€ды. Name == string.Empty + ReadOnly
				if (view.Rows[view.GetRowIndex(_cell.Position.RowId)].Name == string.Empty && view.Rows[view.GetRowIndex(_cell.Position.RowId)].ReadOnly)
				{
					Value = "<div></div>&nbsp;";
				}
			}
			cell_node.Attributes.Append(doc.CreateAttribute("id")).Value = ColumnId;
			cell_node.InnerText = Value;
		}
		#endregion

		#region CreateViewDoc
		//Generate XML for one spreadsheet
		public static XmlDocument CreateViewDoc(SpreadSheetView view)
		{
			XmlDocument xmlDoc = new XmlDocument();

			XmlNode rows_node = xmlDoc.AppendChild(xmlDoc.CreateElement("rows"));

			for (int index = 0; index < view.Rows.Length; index++)
			{
				Row row = view.Rows[index];

				XmlNode row_node = rows_node.AppendChild(xmlDoc.CreateElement("row"));
				row_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = row.Id;

				XmlNode _cellparent = xmlDoc.CreateElement("cell");
				_cellparent.Attributes.Append(xmlDoc.CreateAttribute("colspan")).Value = "2";

				XmlNode _cellParentAddImage = null;

				#region Bind block row
				//Generate tree
				if (row is Block)
				{
					_cellParentAddImage = xmlDoc.CreateElement("cell");
					_cellParentAddImage.AppendChild(xmlDoc.CreateCDataSection(String.Format("<img border=0 onclick=\"{2}\" src='{1}'>", row.Name, GetAbsolutePath("Layouts/Images/listsnew.gif"), GenScript_NewRow(row))));

					index++;
					int endIndex = index + row.ChildRows.Count;

					for (; index < endIndex; index++) //Row childrow in ((Block)row).ChildRows)
					{

						Row childrow = view.Rows[index];

						//Generate XML for child rows
						XmlNode rowchild_node = row_node.AppendChild(xmlDoc.CreateElement("row"));
						rowchild_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = ((Row)childrow).Id;

						XmlNode cellcaption_node = xmlDoc.CreateElement("cell");
						cellcaption_node.InnerText = childrow.Name;
						rowchild_node.AppendChild(cellcaption_node);

						XmlNode cellbtn = xmlDoc.CreateElement("cell");
						cellbtn.AppendChild(xmlDoc.CreateCDataSection(String.Format("<img border=0 id='{2}_{3}' onclick=\"{2}\" src='{1}'>", childrow.Name, GetAbsolutePath("Layouts/Images/delete.gif"), GenScript_DeleteRow(childrow), childrow.Id, row.ChildRows.Count + 1)));
						rowchild_node.AppendChild(cellbtn);


						foreach (Column childcolum in view.Columns)
						{
							BindCell(view, xmlDoc, rowchild_node, childcolum.Id, childrow.Id);
							// TODO:
						}
					}

					index--;
				}//if
				#endregion

				#region Bind colspan + images
				if (row.ReadOnly && row.Expression != string.Empty && !(row is Block))
				{
					_cellparent.InnerText = "<b>" + row.Name + "</b>";
				}
				else
				{
					_cellparent.InnerText = row.Name;
				}


				//if not block row then remove colspan
				if (_cellParentAddImage != null)
				{
					_cellparent.Attributes.RemoveNamedItem("colspan");
				}

				row_node.AppendChild(_cellparent);

				if (_cellParentAddImage != null)
				{
					row_node.AppendChild(_cellParentAddImage);
				}
				else
				{
					row_node.AppendChild(xmlDoc.CreateElement("cell")).InnerText = string.Empty;
				}
				#endregion

				#region Bind cells values
				foreach (Column column in view.Columns)
				{
					BindCell(view, xmlDoc, row_node, column.Id, row.Id);
				}
				#endregion
			}

			return xmlDoc;
		}
		#endregion

		#region CreateViewDocForAnalysis
		public static XmlDocument CreateViewDocForAnalysis(SpreadSheetView view)
		{
			XmlDocument xmlDoc = new XmlDocument();

			XmlNode rows_node = xmlDoc.AppendChild(xmlDoc.CreateElement("rows"));
			//XmlNode head_node = rows_node.AppendChild(xmlDoc.CreateElement("head"));

			for (int index = 0; index < view.Rows.Length; index++)
			{
				Row row = view.Rows[index];

				XmlNode row_node = rows_node.AppendChild(xmlDoc.CreateElement("row"));
				row_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = row.Id;

				XmlNode _cellparent = xmlDoc.CreateElement("cell");

				//_cellparent.Attributes.Append(xmlDoc.CreateAttribute("colspan")).Value = "2";

				//XmlNode _cellParentAddImage = null;

				#region Bind block row
				//Generate tree
				if (row is Block)
				{
					index++;
					int endIndex = index + row.ChildRows.Count;
					_cellparent.InnerText = row.Name;

					for (; index < endIndex; index++) //Row childrow in ((Block)row).ChildRows)
					{

						Row childrow = view.Rows[index];

						//Generate XML for child rows
						XmlNode rowchild_node = row_node.AppendChild(xmlDoc.CreateElement("row"));
						//rowchild_node.AppendChild(xmlDoc.CreateCDataSection(String.Format("<a href='alert(1)'>{0}</a>", childrow.Name)));
						rowchild_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = ((Row)childrow).Id;

						XmlNode cellcaption_node = xmlDoc.CreateElement("cell");
						//cellcaption_node.InnerText = childrow.Name;

						//-cellcaption_node.AppendChild(xmlDoc.CreateCDataSection(String.Format("{0} <img border=0 id='{2}_{3}' onclick=\"{2}\" src='{1}'>", childrow.Name, GetAbsolutePath("Layouts/Images/delete.gif"), GenScript_DeleteRow(childrow), childrow.Id, row.ChildRows.Count + 1 ))); //.InnerText = childrow.Name;
						cellcaption_node.InnerText = childrow.Name;
						rowchild_node.AppendChild(cellcaption_node);

						foreach (Column childcolum in view.Columns)
						{
							BindCell(view, xmlDoc, rowchild_node, childcolum.Id, childrow.Id);
							// TODO:
						}
					}
					index--;
					//row_node.AppendChild(_cellParentAddImage);
				}//if
				#endregion

				if (row.ReadOnly && row.Expression != string.Empty && !(row is Block))
				{
					_cellparent.InnerText = "<b>" + row.Name + "</b>";
				}
				else
				{
					_cellparent.InnerText = row.Name;
				}
				row_node.AppendChild(_cellparent);

				#region Bind cells values
				foreach (Column column in view.Columns)
				{
					BindCell(view, xmlDoc, row_node, column.Id, row.Id);
				}
				#endregion
			}

			return xmlDoc;
		}
		#endregion

		#region CreateViewCompareDoc
		// Generate XML for compare two views
		public static XmlDocument CreateViewCompareDoc(SpreadSheetView view1, SpreadSheetView view2)
		{
			if (view1.Columns.Length != view2.Columns.Length || view1.Rows.Length != view2.Rows.Length)
				throw new ArgumentException("Incorect views", "view1, view2");

			XmlDocument xmlDoc = new XmlDocument();
			XmlNode rows_node = xmlDoc.AppendChild(xmlDoc.CreateElement("rows"));

			for (int index = 0; index < view1.Rows.Length; index++)
			{
				Row row = view1.Rows[index];
				Row row2 = view2.Rows[index];

				XmlNode row_node = rows_node.AppendChild(xmlDoc.CreateElement("row"));
				row_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = row.Id;

				XmlNode _cellparent = xmlDoc.CreateElement("cell");
				_cellparent.Attributes.Append(xmlDoc.CreateAttribute("colspan")).Value = "2";

				XmlNode _cellParentAddImage = null;
				//Generate tree
				#region Block rows generating
				if (row is Block)
				{
					_cellParentAddImage = xmlDoc.CreateElement("cell");
					_cellParentAddImage.AppendChild(xmlDoc.CreateCDataSection(String.Format("<img border=0 onclick=\"{2}\" src='{1}'>", row.Name, GetAbsolutePath("Layouts/Images/listsnew.gif"), GenScript_NewRow(row))));

					index++;
					int endIndex = index + row.ChildRows.Count;
					//_cellparent.AppendChild(xmlDoc.CreateCDataSection(String.Format("{0} <img border=0 onclick=\"{2}\" src='{1}'>", row.Name, GetAbsolutePath("Layouts/Images/listsnew.gif"), GenScript_NewRow(row))));

					for (; index < endIndex; index++) //Row childrow in ((Block)row).ChildRows)
					{

						Row childrow = view1.Rows[index];
						Row childrow2 = view2.Rows[index];

						//Generate XML for child rows
						XmlNode rowchild_node = row_node.AppendChild(xmlDoc.CreateElement("row"));
						rowchild_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = ((Row)childrow).Id;

						XmlNode cellcaption_node = xmlDoc.CreateElement("cell");
						cellcaption_node.InnerText = childrow.Name;
						rowchild_node.AppendChild(cellcaption_node);

						XmlNode cellbtn = xmlDoc.CreateElement("cell");
						cellbtn.AppendChild(xmlDoc.CreateCDataSection(String.Format("<img border=0 id='{2}_{3}' onclick=\"{2}\" src='{1}'>", childrow.Name, GetAbsolutePath("Layouts/Images/delete.gif"), GenScript_DeleteRow(childrow), childrow.Id, row.ChildRows.Count + 1)));
						rowchild_node.AppendChild(cellbtn);


						foreach (Column childcolum in view1.Columns)
						{
							#region Get cell value
							string val1, val2;
							val1 = view1.GetValue(childcolum.Id, childrow.Id);
							val2 = view2.GetValue(childcolum.Id, childrow2.Id);
							if (val1.Length > 0 && val2.Length == 0) val2 = "0";
							if (val1.Length == 0 && val2.Length > 0) val1 = "0";
							string Value = String.Format("{0}/{1}", val1, val2);
							if (val1.Length == 0 && val2.Length == 0) Value = string.Empty;
							#endregion

							XmlNode cell_node = rowchild_node.AppendChild(xmlDoc.CreateElement("cell"));
							Cell _cell = view1.GetCell(childcolum.Id, childrow.Id);
							if (_cell != null)
							{
								if (_cell.Type == CellType.AutoCalc)
									cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_bold";
								else if (_cell.Type == CellType.UserValue)
									cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_italic";
								if (_cell.ReadOnly)
									Value = "<div></div>" + Value;
							}
							cell_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = childcolum.Id;

							cell_node.InnerText = Value;
							// TODO:
						}
					}

					index--;
				}//if
				#endregion

				#region Add images + colspan
				_cellparent.InnerText = row.Name;
				//if not block row then remove colspan
				if (_cellParentAddImage != null)
				{
					_cellparent.Attributes.RemoveNamedItem("colspan");
				}
				if (row.ReadOnly && row.Expression != string.Empty && !(row is Block))
				{
					_cellparent.InnerText = "<b>" + _cellparent.InnerText + "</b>";
				}
				row_node.AppendChild(_cellparent);

				if (_cellParentAddImage != null)
				{
					row_node.AppendChild(_cellParentAddImage);
				}
				else // if not block row then add empty cell
				{
					row_node.AppendChild(xmlDoc.CreateElement("cell")).InnerText = "<div></div>&nbsp;";//string.Empty;
				}
				#endregion

				#region Bind columns values
				foreach (Column column in view1.Columns)
				{
					string CellUID = string.Format("{0}:{1}", column.Id, row.Id);

					#region Get cell value
					string val1, val2;
					val1 = view1.GetValue(column.Id, row.Id);
					val2 = view2.GetValue(column.Id, row2.Id);
					if (val1.Length > 0 && val2.Length == 0) val2 = "0";
					if (val1.Length == 0 && val2.Length > 0) val1 = "0";
					string Value = String.Format("{0}/{1}", val1, val2);
					if (val1.Length == 0 && val2.Length == 0) Value = string.Empty;
					#endregion

					Cell _cell = view1.GetCell(column.Id, row.Id);
					XmlNode cell_node = row_node.AppendChild(xmlDoc.CreateElement("cell"));

					if (_cell != null)
					{
						if (_cell.Type == CellType.AutoCalc)
							cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_bold";
						else if (_cell.Type == CellType.UserValue)
							cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_italic";
						if (_cell.ReadOnly)
							Value = "<div></div>" + Value;

						if (row.ReadOnly && row.Id.ToLower().Contains("Space"))
							Value = "<div></div>"; 

						//fix 2008-09-09 (когда у блока есть дочерние р€ды до делать €чейки read only)
						if (row is Block && row.ChildRows != null & row.ChildRows.Count > 0 && Value.IndexOf("<div></div>") < 0)
						{
							Value = "<div></div>" + Value;
						}
					}

					cell_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = column.Id;//CellUID;//column.Id;
					cell_node.InnerText = Value;
				}
				#endregion
			}

			return xmlDoc;
		}
		#endregion

		#region CreateViewCompareDocForAnalysis
		// Generate XML for compare two views
		public static XmlDocument CreateViewCompareDocForAnalysis(SpreadSheetView view1, SpreadSheetView view2)
		{
			if (view1.Columns.Length != view2.Columns.Length || view1.Rows.Length != view2.Rows.Length)
				throw new ArgumentException("Incorect views", "view1, view2");

			XmlDocument xmlDoc = new XmlDocument();
			XmlNode rows_node = xmlDoc.AppendChild(xmlDoc.CreateElement("rows"));

			for (int index = 0; index < view1.Rows.Length; index++)
			{
				Row row = view1.Rows[index];
				Row row2 = view2.Rows[index];

				XmlNode row_node = rows_node.AppendChild(xmlDoc.CreateElement("row"));
				row_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = row.Id;

				XmlNode _cellparent = xmlDoc.CreateElement("cell");
				//_cellparent.Attributes.Append(xmlDoc.CreateAttribute("colspan")).Value = "2";

				//XmlNode _cellParentAddImage = null;
				//Generate tree
				#region Block rows generating
				if (row is Block)
				{
					_cellparent.InnerText = row.Name;

					index++;
					int endIndex = index + row.ChildRows.Count;
					//_cellparent.AppendChild(xmlDoc.CreateCDataSection(String.Format("{0} <img border=0 onclick=\"{2}\" src='{1}'>", row.Name, GetAbsolutePath("Layouts/Images/listsnew.gif"), GenScript_NewRow(row))));

					for (; index < endIndex; index++) //Row childrow in ((Block)row).ChildRows)
					{

						Row childrow = view1.Rows[index];
						Row childrow2 = view2.Rows[index];

						//Generate XML for child rows
						XmlNode rowchild_node = row_node.AppendChild(xmlDoc.CreateElement("row"));
						//rowchild_node.AppendChild(xmlDoc.CreateCDataSection(String.Format("<a href='alert(1)'>{0}</a>", childrow.Name)));
						rowchild_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = ((Row)childrow).Id;

						XmlNode cellcaption_node = xmlDoc.CreateElement("cell");
						//cellcaption_node.InnerText = childrow.Name;

						//-cellcaption_node.AppendChild(xmlDoc.CreateCDataSection(String.Format("{0} <img border=0 id='{2}_{3}' onclick=\"{2}\" src='{1}'>", childrow.Name, GetAbsolutePath("Layouts/Images/delete.gif"), GenScript_DeleteRow(childrow), childrow.Id, row.ChildRows.Count + 1 ))); //.InnerText = childrow.Name;
						cellcaption_node.InnerText = childrow.Name;
						rowchild_node.AppendChild(cellcaption_node);

						//XmlNode cellbtn = xmlDoc.CreateElement("cell");
						//cellbtn.AppendChild(xmlDoc.CreateCDataSection(String.Format("<img border=0 id='{2}_{3}' onclick=\"{2}\" src='{1}'>", childrow.Name, GetAbsolutePath("Layouts/Images/delete.gif"), GenScript_DeleteRow(childrow), childrow.Id, row.ChildRows.Count + 1 )));
						//rowchild_node.AppendChild(cellbtn);


						foreach (Column childcolum in view1.Columns)
						{
							#region Get cell value
							string val1, val2;
							val1 = view1.GetValue(childcolum.Id, childrow.Id);
							val2 = view2.GetValue(childcolum.Id, childrow2.Id);
							if (val1.Length > 0 && val2.Length == 0) val2 = "0";
							if (val1.Length == 0 && val2.Length > 0) val1 = "0";
							string Value = String.Format("{0}/{1}", val1, val2);
							if (val1.Length == 0 && val2.Length == 0) Value = string.Empty;
							#endregion

							XmlNode cell_node = rowchild_node.AppendChild(xmlDoc.CreateElement("cell"));
							Cell _cell = view1.GetCell(childcolum.Id, childrow.Id);
							if (_cell != null)
							{
								if (_cell.Type == CellType.AutoCalc)
									cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_bold";
								else if (_cell.Type == CellType.UserValue)
									cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_italic";
								if (_cell.ReadOnly)
									Value = "<div></div>" + Value;

							}
							cell_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = childcolum.Id;

							cell_node.InnerText = Value;
							// TODO:
						}
					}

					index--;
					//row_node.AppendChild(_cellParentAddImage);
				}//if
				#endregion

				if (row.ReadOnly && row.Expression != string.Empty && !(row is Block))
				{
					_cellparent.InnerText = "<b>" + row.Name + "</b>";
				}
				else
				{
					_cellparent.InnerText = row.Name;
				}

				row_node.AppendChild(_cellparent);

				#region Bind columns values
				foreach (Column column in view1.Columns)
				{
					string CellUID = string.Format("{0}:{1}", column.Id, row.Id);

					#region Get cell value
					string val1, val2;
					val1 = view1.GetValue(column.Id, row.Id);
					val2 = view2.GetValue(column.Id, row2.Id);
					if (val1.Length > 0 && val2.Length == 0) val2 = "0";
					if (val1.Length == 0 && val2.Length > 0) val1 = "0";
					string Value = String.Format("{0}/{1}", val1, val2);
					if (val1.Length == 0 && val2.Length == 0) Value = string.Empty;
					#endregion

					Cell _cell = view1.GetCell(column.Id, row.Id);
					XmlNode cell_node = row_node.AppendChild(xmlDoc.CreateElement("cell"));

					if (_cell != null)
					{
						if (_cell.Type == CellType.AutoCalc)
							cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_bold";
						else if (_cell.Type == CellType.UserValue)
							cell_node.Attributes.Append(xmlDoc.CreateAttribute("class")).Value = "dhtmlxGrid_italic";
						if (_cell.ReadOnly)
							Value = "<div></div>" + Value;

						if (row.ReadOnly && row.Id.ToLower().Contains("Space"))
							Value = "<div></div>"; 

					}

					cell_node.Attributes.Append(xmlDoc.CreateAttribute("id")).Value = column.Id;//CellUID;//column.Id;
					cell_node.InnerText = Value;
				}
				#endregion
			}

			return xmlDoc;
		}
		#endregion

		#region CreateDocChanges
		// Generate XML for updating cells/rows
		public static XmlDocument CreateDocChanges(SpreadSheetView view)
		{
			XmlDocument xmlDoc = new XmlDocument();

			XmlNode rows_node = xmlDoc.AppendChild(xmlDoc.CreateElement("rows"));
			XmlNode user_node = rows_node.AppendChild(xmlDoc.CreateElement("userdata"));
			user_node.Attributes.Append(xmlDoc.CreateAttribute("name")).Value = "action";
			user_node.InnerText = "update";

			foreach (Cell cell in view.ChangedCellList)
			{
				XmlNode cell_node = rows_node.AppendChild(xmlDoc.CreateElement("cell"));
				Cell _cell = view.GetCell(cell.Position.ColumnId, cell.Position.RowId);
				string Value = cell.Value.ToString();
				if (_cell != null)
				{
					if (_cell.Type == CellType.AutoCalc)
						cell_node.Attributes.Append(xmlDoc.CreateAttribute("font")).Value = "Black";
					else if (_cell.Type == CellType.UserValue)
						cell_node.Attributes.Append(xmlDoc.CreateAttribute("font")).Value = "#CE3431";
					if (_cell.ReadOnly)
						Value = "<div></div>" + Value;
				}
				int colind = view.GetColumnIndex(cell.Position.ColumnId) + 2;
				cell_node.Attributes.Append(xmlDoc.CreateAttribute("columnindex")).Value = colind.ToString();

				cell_node.Attributes.Append(xmlDoc.CreateAttribute("rowid")).Value = cell.Position.RowId;
				cell_node.InnerText = Value;
			}

			return xmlDoc;
		}
		#endregion

		#region CreateDocChangesCompare
		// Generate XML for updating cells/rows
		public static XmlDocument CreateDocChangesCompare(SpreadSheetView view1, SpreadSheetView view2)
		{
			XmlDocument xmlDoc = new XmlDocument();

			XmlNode rows_node = xmlDoc.AppendChild(xmlDoc.CreateElement("rows"));
			XmlNode user_node = rows_node.AppendChild(xmlDoc.CreateElement("userdata"));
			user_node.Attributes.Append(xmlDoc.CreateAttribute("name")).Value = "action";
			user_node.InnerText = "update";

			foreach (Cell cell in view1.ChangedCellList)
			{
				XmlNode cell_node = rows_node.AppendChild(xmlDoc.CreateElement("cell"));
				Cell _cell = view1.GetCell(cell.Position.ColumnId, cell.Position.RowId);
				Cell _cell2 = view2.GetCell(cell.Position.ColumnId, cell.Position.RowId);
				string Value = String.Format("{0}/{1}", cell.Value.ToString(), _cell2.Value.ToString());
				if (_cell != null)
				{
					if (_cell.Type == CellType.AutoCalc)
						cell_node.Attributes.Append(xmlDoc.CreateAttribute("font")).Value = "Black";
					else if (_cell.Type == CellType.UserValue)
						cell_node.Attributes.Append(xmlDoc.CreateAttribute("font")).Value = "#CE3431";
					if (_cell.ReadOnly)
						Value = "<div></div>" + Value;
				}
				int colind = view1.GetColumnIndex(cell.Position.ColumnId) + 2;

				cell_node.Attributes.Append(xmlDoc.CreateAttribute("columnindex")).Value = colind.ToString();
				cell_node.Attributes.Append(xmlDoc.CreateAttribute("rowid")).Value = cell.Position.RowId;
				cell_node.InnerText = Value;
			}

			return xmlDoc;
		}
		#endregion

		#region GetAbsolutePath
		public static string GetAbsolutePath(string xs_Path)
		{
			string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

			StringBuilder builder = new StringBuilder();
			if (UrlScheme != null)
				builder.Append(UrlScheme);
			else
				builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");

			// Oleg Rylin: Fixing the problem with non-default port [6/20/2006]
			builder.Append(HttpContext.Current.Request.Url.Authority);

			builder.Append(HttpContext.Current.Request.ApplicationPath);
			builder.Append("/");
			if (xs_Path != string.Empty)
			{
				if (xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString();
		}
		#endregion

		#endregion

		#region SetUserRowName
		/// <summary>
		/// Sets the name of the user row.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="UserRowId">The user row id.</param>
		/// <param name="Value">The value.</param>
		public static void SetUserRowName(int ProjectId, string UserRowId, string Value)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				ProjectSpreadSheetRow[] SpreadSheets = ProjectSpreadSheetRow.List(ProjectId);

				if (SpreadSheets.Length == 0)
					return;

				ProjectSpreadSheetRow prjSpreadSheetRow = SpreadSheets[0];

				if (prjSpreadSheetRow.UserRows != string.Empty)
				{
					XmlDocument xmlDoc = new XmlDocument();

					xmlDoc.LoadXml(prjSpreadSheetRow.UserRows);

					XmlElement userRowNode = (XmlElement)xmlDoc.SelectSingleNode(string.Format("//Row[@Id='{0}']", UserRowId));

					userRowNode.SetAttribute("Name", Value);

					prjSpreadSheetRow.UserRows = xmlDoc.InnerXml;

					prjSpreadSheetRow.Update();
				}

				tran.Commit();
			}
		}
		#endregion

		#region CompareProjects: Project / Business Score
		public static SpreadSheetView CompareProjects(ArrayList ProjectIdList,
			SpreadSheetDocumentType DocumentType,
			int Index,
			int FromYear, int ToYear)
		{
			if (!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || 
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)))
				throw new AccessDeniedException();

			SpreadSheetDocument document = new SpreadSheetDocument(DocumentType);

			// Step 1. Create Template
			#region Create Template
			SpreadSheetTemplate template = document.Template;

			BusinessScore[] businessScoreList = BusinessScore.List();

			foreach (int projectId in ProjectIdList)
			{
				if (!IsActive(projectId))
					continue;

				string projectName = string.Empty;

				using (IDataReader reader = Project.GetProject(projectId, false))
				{
					if (reader.Read())
					{
						projectName = (string)reader["Title"];
					}
				}

				Block projectBlock = new Block(string.Format("Prj{0:00000}", projectId),
					projectName, false, true, string.Empty, string.Empty);

				foreach (BusinessScore score in businessScoreList)
				{
					Row scoreRow = new Row(string.Format("{0}_{1}", projectBlock.Id, score.Key),
						score.Name, true, string.Empty, string.Empty);

					projectBlock.ChildRows.Add(scoreRow);
				}

				string expression = projectBlock.Expression;

				template.Rows.Add(projectBlock);
			}

			#endregion

			// Step 2. Load Document
			#region Load Document
			Hashtable hashBusinessScoreKeyById = new Hashtable();
			Hashtable hashBusinessScoreIdByKey = new Hashtable();

			// Load hashBusinessScoreKeyById
			foreach (BusinessScore bs in businessScoreList)
			{
				hashBusinessScoreKeyById.Add(bs.BusinessScoreId, bs.Key);
				hashBusinessScoreIdByKey.Add(bs.Key, bs.BusinessScoreId);
			}

			if (Index >= 0)
			{
				foreach (BusinessScoreDataRow dataRow in BusinessScoreDataRow.List(Index))
				{
					if (ProjectIdList.Contains(dataRow.ProjectId) &&
						hashBusinessScoreKeyById.ContainsKey(dataRow.BusinessScoreId))
					{
						string ColumnId = SpreadSheetView.GetColumnByDate(DocumentType, dataRow.Date);
						string RowId = string.Format("Prj{0:00000}_{1}", dataRow.ProjectId, hashBusinessScoreKeyById[dataRow.BusinessScoreId]);

						Cell cell = document.GetCell(ColumnId, RowId);
						if (cell == null)
						{
							cell = document.AddCell(ColumnId, RowId, CellType.Common, 0);
						}

						cell.Value += dataRow.Value;
					}
				}
			}
			else
			{
				foreach (int ProjectId in ProjectIdList)
				{
					foreach (ActualFinances finance in ActualFinances.List(ProjectId, ObjectTypes.Project))
					{
						SpreadSheetView projectFactView = LoadView(ProjectId, -1, FromYear, ToYear);

						if (projectFactView != null)
						{
							foreach (string key in hashBusinessScoreIdByKey.Keys)
							{
								string srcColumnId = SpreadSheetView.GetColumnByDate(projectFactView.Document.DocumentType, finance.Date);

								int srcColumnIndex = projectFactView.GetColumnIndex(srcColumnId);
								int srcRowIndex = projectFactView.GetRowIndex(key);

								if (srcColumnIndex != -1 && srcRowIndex != -1)
								{
									Cell srcCell = projectFactView.GetCell(srcColumnIndex, srcRowIndex);

									if (srcCell != null)
									{
										string ColumnId = SpreadSheetView.GetColumnByDate(DocumentType, finance.Date);
										string RowId = string.Format("Prj{0:00000}_{1}", ProjectId, key);

										Cell destCell = document.GetCell(ColumnId, RowId);

										if (destCell == null)
										{
											destCell = document.AddCell(ColumnId, RowId, CellType.Common, 0);
											destCell.Value += srcCell.Value;
										}
										else
										{
											if (srcCell.Type != CellType.AutoCalc)
											{
												destCell.Value += srcCell.Value;
											}
										}

									}
								}
							}
						}
					}

					/*foreach(ActualFinances finance in ActualFinances.List(ProjectId, ObjectTypes.Project))
					{
						if(hashBusinessScoreIdByKey.ContainsKey(finance.RowId))
						{
							string ColumnId = SpreadSheetView.GetColumnByDate(DocumentType,finance.Date);
							string RowId = string.Format("Prj{0:00000}_{1}", ProjectId , finance.RowId);

							Cell cell = document.GetCell(ColumnId, RowId);

							if(cell==null)
							{
								cell = document.AddCell(ColumnId, RowId, CellType.Common, 0);
							}

							cell.Value += finance.Value;
						}
					}*/
				}
			}
			#endregion

			// Step 3. Create View And Return
			return new SpreadSheetView(document, FromYear, ToYear);
		}
		#endregion

		#region CompareProjectsReverse: Business Score / Project
		public static SpreadSheetView CompareProjectsReverse(ArrayList ProjectIdList,
			SpreadSheetDocumentType DocumentType,
			int Index,
			int FromYear, int ToYear)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				throw new AccessDeniedException();

			SpreadSheetDocument document = new SpreadSheetDocument(DocumentType);

			// Step 1. Create Template
			#region Create Template
			SpreadSheetTemplate template = document.Template;

			BusinessScore[] businessScoreList = BusinessScore.List();


			foreach (BusinessScore score in businessScoreList)
			{
				Block scoreBlcok = new Block(score.Key,
					score.Name, false, true, string.Empty, string.Empty);

				foreach (int projectId in ProjectIdList)
				{
					if (!IsActive(projectId))
						continue;

					string projectName = string.Empty;

					using (IDataReader reader = Project.GetProject(projectId, false))
					{
						if (reader.Read())
						{
							projectName = (string)reader["Title"];
						}

						Row projectRow = new Row(string.Format("{0}_Prj{1:00000}", score.Key, projectId),
							projectName, true, string.Empty, string.Empty);

						scoreBlcok.ChildRows.Add(projectRow);

					}
				}

				string expression = scoreBlcok.Expression;
				template.Rows.Add(scoreBlcok);
			}
			#endregion

			// Step 2. Load Document
			#region Load Document
			Hashtable hashBusinessScoreKeyById = new Hashtable();
			Hashtable hashBusinessScoreIdByKey = new Hashtable();

			// Load hashBusinessScoreKeyById
			foreach (BusinessScore bs in businessScoreList)
			{
				hashBusinessScoreKeyById.Add(bs.BusinessScoreId, bs.Key);
				hashBusinessScoreIdByKey.Add(bs.Key, bs.BusinessScoreId);
			}

			if (Index >= 0)
			{
				foreach (BusinessScoreDataRow dataRow in BusinessScoreDataRow.List(Index))
				{
					if (ProjectIdList.Contains(dataRow.ProjectId) &&
						hashBusinessScoreKeyById.ContainsKey(dataRow.BusinessScoreId))
					{
						string ColumnId = SpreadSheetView.GetColumnByDate(DocumentType, dataRow.Date);
						string RowId = string.Format("{1}_Prj{0:00000}", dataRow.ProjectId, hashBusinessScoreKeyById[dataRow.BusinessScoreId]);

						Cell cell = document.GetCell(ColumnId, RowId);
						if (cell == null)
						{
							cell = document.AddCell(ColumnId, RowId, CellType.Common, 0);
						}

						cell.Value += dataRow.Value;
					}
				}
			}
			else
			{
				foreach (int ProjectId in ProjectIdList)
				{
					foreach (ActualFinances finance in ActualFinances.List(ProjectId, ObjectTypes.Project))
					{
						SpreadSheetView projectFactView = LoadView(ProjectId, -1, FromYear, ToYear);

						if (projectFactView != null)
						{
							foreach (string key in hashBusinessScoreIdByKey.Keys)
							{
								string srcColumnId = SpreadSheetView.GetColumnByDate(projectFactView.Document.DocumentType, finance.Date);

								int srcColumnIndex = projectFactView.GetColumnIndex(srcColumnId);
								int srcRowIndex = projectFactView.GetRowIndex(key);

								if (srcColumnIndex != -1 && srcRowIndex != -1)
								{
									Cell srcCell = projectFactView.GetCell(srcColumnIndex, srcRowIndex);

									if (srcCell != null)
									{
										string ColumnId = SpreadSheetView.GetColumnByDate(DocumentType, finance.Date);
										string RowId = string.Format("{1}_Prj{0:00000}", ProjectId, key);

										Cell destCell = document.GetCell(ColumnId, RowId);

										if (destCell == null)
										{
											destCell = document.AddCell(ColumnId, RowId, CellType.Common, 0);
											destCell.Value += srcCell.Value;
										}
										else
										{
											if (srcCell.Type != CellType.AutoCalc)
											{
												destCell.Value += srcCell.Value;
											}
										}

									}
								}
							}
						}
					}
				}

				/*foreach(int ProjectId in ProjectIdList)
				{
					foreach(ActualFinances finance in ActualFinances.List(ProjectId, ObjectTypes.Project))
					{
						if(hashBusinessScoreIdByKey.ContainsKey(finance.RowId))
						{
							string ColumnId = SpreadSheetView.GetColumnByDate(DocumentType,finance.Date);
							string RowId = string.Format("{1}_Prj{0:00000}", ProjectId , finance.RowId);

							Cell cell = document.GetCell(ColumnId, RowId);

							if(cell==null)
							{
								cell = document.AddCell(ColumnId, RowId, CellType.Common, 0);
							}

							cell.Value += finance.Value;
						}
					}
				}*/
			}
			#endregion

			// Step 3. Create View And Return
			return new SpreadSheetView(document, FromYear, ToYear);
		}
		#endregion
	}
}
