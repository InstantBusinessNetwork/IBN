using System;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Resources;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for SpreadSheetView.
	/// </summary>
	public class SpreadSheetView
	{
		private  SpreadSheetDocument _document = null;
		
		private static ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.SpreadSheets", typeof(SpreadSheetView).Assembly);

		// View Filter
		private  int _minYear = int.MinValue;
		private  int _maxYear = int.MaxValue;

		private  int _fromYear = int.MinValue;
		private  int _toYear = int.MaxValue;

		private  int _viewColumnIndex = 0;
		private  int _viewColumnLength = 0;

		// View Fields
		string _minColumnId = "9999-";
		string _maxColumnId = "0000-";

		private ArrayList _columns = new ArrayList();

		private Column[] _viewColumns = null;
		private Row[]	_viewRows = null;

		private Hashtable _columnIndexCache = null;
		private Hashtable _rowIndexCache = null;

		private ArrayList _changedCellList = new ArrayList();

		private ExpressionParser _expressionParser = new ExpressionParser();

		#region Control Id Format Constants

//		internal const string Year = "{0:0000}";
//		internal const string YearWeek = "{0:0000}-W{1:00}";
//		internal const string YearQuarter = "{0:0000}-Q{1:0}";
//		internal const string YearMonth = "{0:0000}-{1:00}";
//		internal const string Total = "TT";

		#endregion

		
		/// <summary>
		/// Initializes a new instance of the <see cref="SpreadSheetView"/> class.
		/// </summary>
		/// <param name="Document">The document.</param>
		public SpreadSheetView(SpreadSheetDocument Document)
		{
			_document = Document;

			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpreadSheetView"/> class.
		/// </summary>
		/// <param name="Document">The document.</param>
		/// <param name="FromYear">From year.</param>
		/// <param name="ToYear">To year.</param>
		public SpreadSheetView(SpreadSheetDocument Document, 
			int FromYear, int ToYear)
		{
			_document = Document;
			_fromYear = FromYear;
			_toYear = ToYear;

			Init();
		}

		/// <summary>
		/// Gets the document.
		/// </summary>
		/// <value>The document.</value>
		public SpreadSheetDocument Document
		{
			get 
			{
				return _document;
			}
		}

		/// <summary>
		/// Gets from year.
		/// </summary>
		/// <value>From year.</value>
		public int FromYear
		{
			get 
			{
				return _fromYear;
			}
		}

		/// <summary>
		/// Gets to year.
		/// </summary>
		/// <value>To year.</value>
		public int ToYear
		{
			get 
			{
				return _toYear;
			}
		}

		/// <summary>
		/// Gets the changed cell list.
		/// </summary>
		/// <value>The changed cell list.</value>
		public ArrayList ChangedCellList
		{
			get
			{
				return _changedCellList;
			}
		}

		/// <summary>
		/// Creates the column.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		private Column CreateColumn(string Id, string Name)
		{
			Column	retVal = new Column();

			retVal.Id = Id;
			retVal.Name = Name;

			_columns.Add(retVal);
			_viewColumns = null;

			return retVal;
		}

		/// <summary>
		/// Inits this instance.
		/// </summary>
		protected virtual void Init()
		{
			_minColumnId = "9999";
			_maxColumnId = "0000";

			#region Calculate Data Boundary
			// Calculate Data Boundary
			foreach(Cell cell in this.Document.Cells)
			{
				if(cell.Position.ColumnId!="TT")
				{
					if(string.Compare(cell.Position.ColumnId,_minColumnId)<0)
						_minColumnId = cell.Position.ColumnId;
					if(string.Compare(cell.Position.ColumnId,_maxColumnId)>0)
						_maxColumnId = cell.Position.ColumnId;
				}
			}

			_minYear = int.Parse(_minColumnId.Substring(0,4));
			_maxYear = int.Parse(_maxColumnId.Substring(0,4));

			if(_minYear==9999)
				_minYear = _fromYear;
			if(_maxYear==0)
				_maxYear = _toYear;

			if(_fromYear==int.MinValue)
				_fromYear = _minYear;

			if(_toYear==int.MaxValue)
				_toYear = _maxYear;

			_minYear = Math.Min(_fromYear,_minYear);
			_maxYear = Math.Max(_toYear,_maxYear);
			#endregion

			// Fill Colimns
			switch(this.Document.DocumentType)
			{
				case SpreadSheetDocumentType.MonthQuarterYear:
				{
					#region SpreadSheetDocumentType.MonthQuarterYear
					for(int curYear = _minYear; curYear<=_maxYear; curYear++)
					{
						//Fill View boundary
						if(curYear==_fromYear)
							_viewColumnIndex = _columns.Count;

						if (curYear > _toYear && _viewColumnLength==0)
						{
							StringBuilder totalParams = new StringBuilder();

							for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
							{
								if(totalParams.Length>0)
									totalParams.Append('+');
								totalParams.AppendFormat("[{0:0000}-T]",curTYear);
							}

							Column total = CreateColumn("TT","Total");
							total.Expression = totalParams.ToString();

							_viewColumnLength = _columns.Count - _viewColumnIndex;
						}

						// Create Column
						StringBuilder yearTotalParams = new StringBuilder();

						for(int curQuarter = 1; curQuarter<=4; curQuarter++)
						{
							StringBuilder quarterTotalParams = new StringBuilder();

							for(int curMonth = (curQuarter-1)*3+1; curMonth<=curQuarter*3; curMonth++)
							{
								DateTime time = new DateTime(curYear, curMonth,1);

								Column newColumn = CreateColumn(string.Format("{0:0000}-Q{1:0}-{2:00}",curYear, curQuarter, curMonth),
									time.ToString("MMMM"));

								if(quarterTotalParams.Length>0)
									quarterTotalParams.Append('+');
								quarterTotalParams.AppendFormat("[{0}]", newColumn.Id);
							}

							// Quarter Total
							Column quarterTotal = CreateColumn(string.Format("{0:0000}-Q{1:0}-T",curYear, curQuarter),
								string.Format("Total",curYear, curQuarter));
							quarterTotal.Expression = quarterTotalParams.ToString();

							if(yearTotalParams.Length>0)
								yearTotalParams.Append('+');
							yearTotalParams.AppendFormat("[{0}]",quarterTotal.Id);
						}

						// Year Total
						Column yearTotal = CreateColumn(string.Format("{0:0000}-T",curYear),
							string.Format("Total",curYear));
						yearTotal.Expression = yearTotalParams.ToString();
					}

					// Final Total
					if(_viewColumnLength==0)
					{
						StringBuilder totalParams = new StringBuilder();

						for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
						{
							if(totalParams.Length>0)
								totalParams.Append('+');
							totalParams.AppendFormat("[{0:0000}-T]",curTYear);
						}

						Column total = CreateColumn("TT","Total");
						total.Expression = totalParams.ToString();

						_viewColumnLength = _columns.Count - _viewColumnIndex;
					}

					#endregion
				}
					break;
				case SpreadSheetDocumentType.WeekYear:
				{
					#region SpreadSheetDocumentType.WeekYear
					for(int curYear = _minYear; curYear<=_maxYear; curYear++)
					{
						//Fill View boundary
						if(curYear==_fromYear)
							_viewColumnIndex = _columns.Count;

						if (curYear > _toYear && _viewColumnLength == 0)
						{
							StringBuilder totalParams = new StringBuilder();

							for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
							{
								if(totalParams.Length>0)
									totalParams.Append('+');
								totalParams.AppendFormat("[{0:0000}-T]",curTYear);
							}

							Column total = CreateColumn("TT","Total");
							total.Expression = totalParams.ToString();

							_viewColumnLength = _columns.Count - _viewColumnIndex;
						}

						// Create Columns
						StringBuilder yearTotalParams = new StringBuilder();

						DateTime yearStart = Iso8601WeekNumber.GetFirstWeekOfYear(curYear);

						for(int curWeek = 1; curWeek<=52; curWeek++)
						{
							DateTime weekFrom = yearStart.AddDays(7 * (curWeek - 1));
							DateTime weekTo = weekFrom.AddDays(6);

							Column newColumn = CreateColumn(string.Format("{0:0000}-W{1:00}",curYear, curWeek),
								string.Format("{1:dd MMM}", curWeek, weekFrom, weekTo));

							if(yearTotalParams.Length>0)
								yearTotalParams.Append('+');
							yearTotalParams.AppendFormat("[{0}]",newColumn.Id);
						}

						// Year Total
						Column yearTotal = CreateColumn(string.Format("{0:0000}-T",curYear),
							string.Format("Total",curYear));
						yearTotal.Expression = yearTotalParams.ToString();
					}

					// Final Total
					if(_viewColumnLength==0)
					{
						StringBuilder totalParams = new StringBuilder();

						for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
						{
							if(totalParams.Length>0)
								totalParams.Append('+');
							totalParams.AppendFormat("[{0:0000}-T]",curTYear);
						}

						Column total = CreateColumn("TT","Total");
						total.Expression = totalParams.ToString();

						_viewColumnLength = _columns.Count - _viewColumnIndex;
					}
					#endregion
				}
					break;
				case SpreadSheetDocumentType.QuarterYear:
				{
					#region SpreadSheetDocumentType.QuarterYear
					for(int curYear = _minYear; curYear<=_maxYear; curYear++)
					{
						//Fill View boundary
						if(curYear==_fromYear)
							_viewColumnIndex = _columns.Count;

						if (curYear > _toYear && _viewColumnLength == 0)
						{
							StringBuilder totalParams = new StringBuilder();

							for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
							{
								if(totalParams.Length>0)
									totalParams.Append('+');
								totalParams.AppendFormat("[{0:0000}-T]",curTYear);
							}

							Column total = CreateColumn("TT","Total");
							total.Expression = totalParams.ToString();

							_viewColumnLength = _columns.Count - _viewColumnIndex;
						}

						// Create Column
						StringBuilder yearTotalParams = new StringBuilder();

						for(int curQuarter = 1; curQuarter<=4; curQuarter++)
						{
							StringBuilder quarterTotalParams = new StringBuilder();

							// Quarter Total
							Column quarterTotal = CreateColumn(string.Format("{0:0000}-Q{1:0}",curYear, curQuarter),
								string.Format("{2}{1:0}",curYear, curQuarter,LocRM.GetString("Quartal")));

							if(yearTotalParams.Length>0)
								yearTotalParams.Append('+');
							yearTotalParams.AppendFormat("[{0}]",quarterTotal.Id);
						}

						// Year Total
						Column yearTotal = CreateColumn(string.Format("{0:0000}-T",curYear),
							string.Format("Total",curYear));
						yearTotal.Expression = yearTotalParams.ToString();
					}

					// Final Total
					if(_viewColumnLength==0)
					{
						StringBuilder totalParams = new StringBuilder();

						for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
						{
							if(totalParams.Length>0)
								totalParams.Append('+');
							totalParams.AppendFormat("[{0:0000}-T]",curTYear);
						}

						Column total = CreateColumn("TT", LocRM.GetString("ProjectTotal"));
						total.Expression = totalParams.ToString();

						_viewColumnLength = _columns.Count - _viewColumnIndex;
					}

					#endregion
				}
					break;
				case SpreadSheetDocumentType.Year:
				{
					#region SpreadSheetDocumentType.QuarterYear
					for(int curYear = _minYear; curYear<=_maxYear; curYear++)
					{
						//Fill View boundary
						if(curYear==_fromYear)
							_viewColumnIndex = _columns.Count;

						if (curYear > _toYear && _viewColumnLength == 0)
						{
							StringBuilder totalParams = new StringBuilder();

							for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
							{
								if(totalParams.Length>0)
									totalParams.Append('+');
								totalParams.AppendFormat("[{0:0000}-T]",curTYear);
							}

							Column total = CreateColumn("TT","Total");
							total.Expression = totalParams.ToString();

							_viewColumnLength = _columns.Count - _viewColumnIndex;
						}

						// Year Total
						Column yearTotal = CreateColumn(string.Format("{0:0000}-T",curYear),
							string.Format("{0:0000}",curYear));
					}

					// Final Total
					if(_viewColumnLength==0)
					{
						StringBuilder totalParams = new StringBuilder();

						for(int curTYear = _minYear; curTYear<=_maxYear; curTYear++)
						{
							if(totalParams.Length>0)
								totalParams.Append('+');
							totalParams.AppendFormat("[{0:0000}-T]",curTYear);
						}

						Column total = CreateColumn("TT",LocRM.GetString("ProjectTotal"));
						total.Expression = totalParams.ToString();

						_viewColumnLength = _columns.Count - _viewColumnIndex;
					}

					#endregion
				}
					break;
				case SpreadSheetDocumentType.Total:
				{
					_viewColumnIndex = 0;
					_viewColumnLength = 1;
					Column total = CreateColumn("TT",LocRM.GetString("ProjectTotal"));
				}
					break;
			}
		}

		#region GetColumnByDate

		public static string GetColumnByDate(SpreadSheetDocumentType DocumentType, DateTime Date)
		{
			switch(DocumentType)
			{
				case SpreadSheetDocumentType.WeekYear:
					int YW = Iso8601WeekNumber.GetYearWeekNumber(Date);
					return string.Format("{0:0000}-W{1:00}", YW / 100, YW%100);
				case SpreadSheetDocumentType.MonthQuarterYear:
					if(Date.Month>=1 && Date.Month<4)
						return string.Format("{0:0000}-Q1-{1:00}", Date.Year, Date.Month);
					else if(Date.Month>=4 && Date.Month<7)
						return string.Format("{0:0000}-Q2-{1:00}", Date.Year, Date.Month);
					else if(Date.Month>=7 && Date.Month<10)
						return string.Format("{0:0000}-Q3-{1:00}", Date.Year, Date.Month);
					else 
						return string.Format("{0:0000}-Q4-{1:00}", Date.Year, Date.Month);
				case SpreadSheetDocumentType.QuarterYear:
					if(Date.Month>=1 && Date.Month<4)
						return string.Format("{0:0000}-Q1", Date.Year);
					else if(Date.Month>=4 && Date.Month<7)
						return string.Format("{0:0000}-Q2", Date.Year);
					else if(Date.Month>=7 && Date.Month<10)
						return string.Format("{0:0000}-Q3", Date.Year);
					else 
						return string.Format("{0:0000}-Q4", Date.Year);
				case SpreadSheetDocumentType.Year:
					return string.Format("{0:0000}-T", Date.Year);
				case SpreadSheetDocumentType.Total:
					return "TT";
			}

			throw new NotSupportedException("Not supported document type.");
		}
		#endregion

		#region GetDateByColumn

		internal static DateTime GetDateByColumn(string ColumnId)
		{
			// "{0:0000}-W{1:00}"		Year - Week Index
			// "{0:0000}-Q{1:0}-{2:00}" Year - Quarter - Month
			// "{0:0000}-Q{1:0}"		Year - Quarter 

			// "{0:0000}-Q{1:0}-T"		Year - Quarter - Total
			// "{0:0000}-T"				Year - Total
			// "TT"						Total

			if(ColumnId=="TT")
				return DateTime.MinValue;

			string[] split = ColumnId.Split('-');

			int Year = int.Parse(split[0]);

			if(split[1].StartsWith("W"))
			{
				int WI = int.Parse(split[1].Substring(1));

				DateTime yStart = new DateTime(Year,1,1);
				return yStart.AddDays((WI-1)*7);
			}
			else if(split[1].StartsWith("Q"))
			{
				int Quarter = int.Parse(split[1].Substring(1));

				if(split.Length==2)
				{
					return new DateTime(Year, (Quarter-1)*3 + 1, 1 );
				}
				else
				{
					if(split[2]=="T")
					{
						return new DateTime(Year, (Quarter-1)*3 + 1, 1 );
					}
					else
					{
						int Month = int.Parse(split[2]);
						return new DateTime(Year, Month, 1);
					}
				}
			}
			else if (split[1].StartsWith("T"))
			{
				return new DateTime(Year, 1, 1);
			}
			
			throw new NotSupportedException("Not supported document type.");
		}
		#endregion

		/// <summary>
		/// Gets the column id list.
		/// </summary>
		/// <value>The column id list.</value>
		public Column[] Columns
		{
			get 
			{
				if(_viewColumns==null)
				{
					_viewColumns = new Column[_viewColumnLength];

					for(int index=0; index<_viewColumnLength;index++)
						_viewColumns[index] = (Column)_columns[index+_viewColumnIndex];
				}

//				ArrayList retVal = new ArrayList();
//				retVal.AddRange(_viewColumns);

				return _viewColumns;
//				return retVal;
			}
		}

		/// <summary>
		/// Gets the row id list.
		/// </summary>
		/// <value>The row id list.</value>
		public Row[] Rows
		{
			get 
			{
				if(_viewRows==null)
				{
					ArrayList rowList = new ArrayList();

					foreach(Row row in this.Document.Template.Rows)
					{
						rowList.Add(row);

						if(row is Block)
						{
//							if(((Block)row).CanAddRow)
//							{
//								Row emptyCell = new Row(string.Format("{0}#ViewNewRow",row.Id),"*");
//								row.ChildRows.Add(emptyCell);
//							}

							foreach(Row chRow in ((Block)row).ChildRows)
							{
								rowList.Add(chRow);
							}
							
						}

					}

					_viewRows = (Row[])rowList.ToArray(typeof(Row));
				}

				return _viewRows;
			}
		}

		/// <summary>
		/// Gets the index of the column.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <returns></returns>
		public int GetColumnIndex(string ColumnId)
		{
			int realIndex = GetInnerColumnIndex(ColumnId);

			if(realIndex>=_viewColumnIndex && realIndex< (_viewColumnIndex + _viewColumnLength))
				return (realIndex - _viewColumnIndex);

			return -1;
		}

		/// <summary>
		/// Gets the index of the column.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <returns></returns>
		protected int GetInnerColumnIndex(string ColumnId)
		{
			if(_columnIndexCache==null)
			{
				_columnIndexCache = new Hashtable();

				for(int index =0; index<_columns.Count;index++)
				{
					_columnIndexCache.Add(((Column)_columns[index]).Id, index);
				}
			}

			if(_columnIndexCache.ContainsKey(ColumnId))
				return (int)_columnIndexCache[ColumnId];

			return -1;
		}

		/// <summary>
		/// Gets the index of the row.
		/// </summary>
		/// <param name="RowId">The row id.</param>
		/// <returns></returns>
		public int GetRowIndex(string RowId)
		{
			if(_rowIndexCache==null)
			{
				_rowIndexCache = new Hashtable();

				for(int index =0; index<this.Rows.Length;index++)
				{
					_rowIndexCache.Add(((Row)this.Rows[index]).Id, index);
				}
			}

			if(_rowIndexCache.ContainsKey(RowId))
				return (int)_rowIndexCache[RowId];
			return -1;
		}

		#region GetDefaultCellFormat
		protected CellFormat GetDefaultCellFormat(int ColumnIndex, int RowIndex)
		{
			CellFormat retVal = new CellFormat();

			Column	column = (Column)_columns[ColumnIndex];
			Row	row = (Row)this.Rows[RowIndex];

			bool ColumnExpression = false;
			string Expression = string.Empty;

			if(row.Expression!=string.Empty && (column.Expression==string.Empty || 
				!row.HasChildRows || row.ChildRows.Count>0))
			{
				retVal.Type = CellType.AutoCalc;

				ColumnExpression = false;
				Expression = row.Expression;

				retVal.Format = row.Format;
				retVal.ReadOnly = row.ReadOnly;
			}
			else if (column.Expression != string.Empty)
			{
				retVal.Type = CellType.AutoCalc;

				ColumnExpression = true;
				Expression = column.Expression;

				retVal.Format = column.Format;
				retVal.ReadOnly = column.ReadOnly;
			}
			//else if (Expression==string.Empty && row is Block)
			//{
			//    retVal.Type = CellType.AutoCalc;
			//}

			// Unpack Expression
			if(Expression!=string.Empty)
			{
				//Regex regex = new Regex(@"\[(?<CellUid>[^\]]+)]", RegexOptions.Compiled);

				Hashtable hash = new Hashtable();

                foreach (Match match in Regex.Matches(Expression, @"\[(?<CellUid>[^\]]+)]", RegexOptions.Compiled))
				{
					string strLightUid = match.Groups["CellUid"].Value;

					if(!hash.ContainsKey(strLightUid))
					{
						if(ColumnExpression)
						{
							hash.Add(strLightUid, string.Format("{0}:{1}", strLightUid, row.Id));
						}
						else
						{
							hash.Add(strLightUid, string.Format("{0}:{1}", column.Id, strLightUid));
						}
					}
				}

				StringBuilder sb = new StringBuilder(255);
				sb.Append(Expression);

				foreach(string key in hash.Keys)
				{
					sb.Replace(key, (string)hash[key]);
				}

				retVal.Expression =  sb.ToString();
			}

			return retVal;
		}

		protected CellFormat GetDefaultCellFormat(string ColumnId, string RowId )
		{
			return GetDefaultCellFormat(GetInnerColumnIndex(ColumnId), GetRowIndex(RowId));
		}
		#endregion

		#region GetCell
		/// <summary>
		/// Gets the <see cref="Double"/> with the specified column index.
		/// </summary>
		/// <value></value>
		public double this[int ColumnIndex, int RowIndex]
		{
			get 
			{
				Cell cell = this.GetCell(ColumnIndex, RowIndex);
				if(cell!=null)
					return cell.Value;

				return 0;
			}
		}

		/// <summary>
		/// Gets the <see cref="Double"/> with the specified column id.
		/// </summary>
		/// <value></value>
		public double this[string ColumnId, string RowId]
		{
			get 
			{
				Cell cell = this.GetCell(ColumnId, RowId);
				if(cell!=null)
					return cell.Value;

				return 0;
			}
		}

		/// <summary>
		/// Gets the cell.
		/// </summary>
		/// <param name="ColumnIndex">Index of the column.</param>
		/// <param name="RowIndex">Index of the row.</param>
		/// <returns></returns>
		public Cell GetCell(int ColumnIndex, int RowIndex)
		{
			return GetInnerCell(ColumnIndex + _viewColumnIndex, RowIndex);
		}

		/// <summary>
		/// Gets the inner cell.
		/// </summary>
		/// <param name="ColumnIndex">Index of the column.</param>
		/// <param name="RowIndex">Index of the row.</param>
		/// <returns></returns>
		protected Cell GetInnerCell(int ColumnIndex, int RowIndex)
		{
			Column	column = (Column)_columns[ColumnIndex];
			Row	row = (Row)this.Rows[RowIndex];

			Cell cell = this.Document.GetCell(column.Id, row.Id);

			if(cell!=null)
			{
				if(cell.Type==CellType.UserValue && cell.Expression==string.Empty)
				{
					CellFormat cellFormat = GetDefaultCellFormat(ColumnIndex, RowIndex);

					//cell.Type = cellFormat.Type;
					cell.Expression = cellFormat.Expression;
					cell.Format = cellFormat.Format;
					cell.ReadOnly = cellFormat.ReadOnly;
				}
			}
			else
			{
				CellFormat cellFormat = GetDefaultCellFormat(ColumnIndex, RowIndex);

				cell = new Cell(this.Document, column.Id, row.Id);
				cell.Type = cellFormat.Type;
				cell.Expression = cellFormat.Expression;
				cell.Format = cellFormat.Format;
				cell.ReadOnly = cellFormat.ReadOnly;

				this.Document.AddCell(cell);

				if(cell.Expression!=string.Empty)
					EvaluateAutoValue(cell);
			}
			return cell;
		}

		/// <summary>
		/// Gets the cell.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		/// <returns></returns>
		public Cell GetCell(string ColumnId, string RowId)
		{
			return GetInnerCell(GetInnerColumnIndex(ColumnId), GetRowIndex(RowId));
		}
		#endregion

		#region GetValue
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="ColumnIndex">Index of the column.</param>
		/// <param name="RowIndex">Index of the row.</param>
		/// <returns></returns>
		public string GetValue(int ColumnIndex, int RowIndex)
		{
			Cell cell = GetCell(ColumnIndex, RowIndex);
			if(cell!=null)
			{
				if(cell.Format!=string.Empty)
					return cell.Value.ToString(cell.Format);

				return cell.Value.ToString();
			}

			return string.Empty;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		/// <returns></returns>
		public string GetValue(string ColumnId, string RowId)
		{
			Cell cell = GetCell(ColumnId, RowId);
			if(cell!=null)
			{
				if(cell.Format!=string.Empty)
					return cell.Value.ToString(cell.Format);

				return cell.Value.ToString();
			}

			return string.Empty;
		}

		/// <summary>
		/// Evaluates the auto value.
		/// </summary>
		/// <param name="srcCell">The SRC cell.</param>
		private void EvaluateAutoValue(Cell srcCell)
		{
			string Expression = srcCell.Expression;
			srcCell.SetAutoValue(_expressionParser.Parse(Expression, this));
		}

		#endregion 

		#region SetValue
		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		/// <param name="Value">The value.</param>
		public void SetValue(string ColumnId, string RowId, string Value) 
		{
			SetInnerValue(GetInnerColumnIndex(ColumnId), GetRowIndex(RowId), Value);
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="ColumnIndex">Index of the column.</param>
		/// <param name="RowIndex">Index of the row.</param>
		/// <param name="Value">The value.</param>
		public void SetValue(int ColumnIndex, int RowIndex, string Value) 
		{
			SetInnerValue(ColumnIndex + _viewColumnIndex, RowIndex, Value);
		}

		/// <summary>
		/// Sets the inner value.
		/// </summary>
		/// <param name="ColumnIndex">Index of the column.</param>
		/// <param name="RowIndex">Index of the row.</param>
		/// <param name="Value">The value.</param>
		protected void SetInnerValue(int ColumnIndex, int RowIndex, string Value) 
		{
			if(Value==string.Empty)
			{
				//Reset Auto calculated Cell or Delete Common Cell
				Cell cell = GetInnerCell(ColumnIndex, RowIndex);
				if(cell!=null)
				{
					if(cell.Type == CellType.Common)
					{
						// Delete Cell
						this.Document.DeleteCell(cell);

						cell.Value = 0;

						RecalculateRelatedCells(cell);
					}
					else if(cell.Type == CellType.UserValue)
					{
						cell.Type = CellType.AutoCalc;

						// Restore Expression by Default
						if(cell.Expression==string.Empty)
						{
							CellFormat cellFormat = GetDefaultCellFormat(ColumnIndex, RowIndex);
							cell.Expression = cellFormat.Expression;
						}

						if(cell.Expression!=string.Empty)
							EvaluateAutoValue(cell);

						RecalculateRelatedCells(cell);
					}
					else if(cell.Type == CellType.AutoCalc)
					{
						if(cell.Expression!=string.Empty)
							EvaluateAutoValue(cell);

						RaiseCellValueChanged(cell);
					}

				}
				return;
			}

			double dblValue = 0;

			try
			{
				dblValue = double.Parse(Value);
			}
			catch
			{
				dblValue = double.Parse(Value, System.Globalization.CultureInfo.InvariantCulture);
			}

			{
				Cell cell = GetInnerCell(ColumnIndex, RowIndex);
				if(cell==null)
				{
					cell = new Cell(this.Document, ((Column)_columns[ColumnIndex]).Id, this.Rows[RowIndex].Id);
					this.Document.AddCell(cell);
				}

				if(cell.Value==dblValue)
					return;

				if(cell.Type==CellType.AutoCalc)
					cell.Type = CellType.UserValue;

				cell.Value = dblValue;

				RecalculateRelatedCells(cell);
			}

		}

		#endregion 

		/// <summary>
		/// Recalculates the related cells.
		/// </summary>
		/// <param name="srcCell">The SRC cell.</param>
		private void RecalculateRelatedCells(Cell srcCell)
		{
			RaiseCellValueChanged(srcCell);

			ArrayList changedCells = new ArrayList();

			for(int colIndex =0;colIndex<this.Columns.Length; colIndex++)
			{
				for(int rowIndex =0;rowIndex<this.Rows.Length; rowIndex++)
				{
					Cell cell = this.GetCell(colIndex, rowIndex);

					if(cell!=null && cell.Type== CellType.AutoCalc)
					{
						//ExpressionInfo expInfo = ExpressionInfo.Parse(cell.Expression);
                        ExpressionInfo expInfo = cell.GetExpressionInfo();

						if(expInfo.ContainsParam(srcCell.Uid))
						{
							double oldValue = cell.Value;
							EvaluateAutoValue(cell);

							changedCells.Add(cell);
						}
//
//						int index = cell.Expression.IndexOf(srcCell.Uid);
//						if(index!=-1)
//						{
//							char endChar = cell.Expression[index+srcCell.Uid.Length];
//							if(endChar==',' || endChar==')' )
//							{
//								double oldValue = cell.Value;
//								EvaluateAutoValue(cell);
//
//								//if(oldValue!=cell.Value)
//								changedCells.Add(cell);
//							}
//						}
					}
				}
			}

			foreach(Cell chCell in changedCells)
			{
				RecalculateRelatedCells(chCell);
			}
		}

		/// <summary>
		/// Raises the cell value changed.
		/// </summary>
		/// <param name="cell">The cell.</param>
		private void RaiseCellValueChanged(Cell cell)
		{
			if(!_changedCellList.Contains(cell))
				_changedCellList.Add(cell);
//			else
//				throw new Exception("RaiseCellValueChanged Loop");
			//OnCellValueChanged(cell);
		}

		/// <summary>
		/// Adds the block row.
		/// </summary>
		/// <param name="BlockRowId">The block row id.</param>
		/// <param name="NewRowId">The new row id.</param>
		public void AddBlockRow(string BlockRowId, string NewRowId)
		{
			foreach(Row row in this.Document.Template.Rows)
			{
				if(row.HasChildRows && row.Id == BlockRowId)
				{
					//string newRowId = string.Format(NewRowId, BlockRowId, row.ChildRows.Count);

					string newRowName = ((Block)row).NewRowDefaultName;
					if(newRowName==string.Empty)
						newRowName = NewRowId;

					// OZ 2008-09-10 Fix problem with user value in block
					if (row.ChildRows.Count == 0)
					{
						// Delete all user values from block
						Cell[] cellList = this.Document.Cells;
						foreach (Cell cell in cellList)
						{
							if (cell.Position.RowId == BlockRowId)
							{
								this.SetValue(cell.Position.ColumnId,
								    cell.Position.RowId,
								    string.Empty);

								cell.Type = CellType.AutoCalc;
							}
						}
					}

					Row newRow = new Row(NewRowId, newRowName);
					newRow.Visibility = RowVisibility.User;
					row.ChildRows.Add(newRow);

					break;
				}
			}

			_viewRows = null;
			_rowIndexCache = null;
		}

		/// <summary>
		/// Deletes the block row.
		/// </summary>
		/// <param name="RowId">The row id.</param>
		public void DeleteBlockRow(string RowId)
		{
			foreach(Row row in this.Document.Template.Rows)
			{
				if(row.HasChildRows)
				{
					foreach(Row childRow in row.ChildRows)
					{
						if(childRow.Visibility==RowVisibility.User && childRow.Id==RowId)
						{
							ArrayList cell2Delete = new ArrayList();

							// Fill All Items from Deleted Row as NULL
							foreach(Cell cell in this.Document.Cells)
							{
								if(cell.Position.RowId==RowId)
								{
									cell2Delete.Add(cell);
									SetValue(cell.Position.ColumnId, cell.Position.RowId, "0");
								}
							}

							// Remove all cell with Deleted RowId from Document
							for(int index = _changedCellList.Count-1;index>=0;index--)
							{
								if(((Cell)_changedCellList[index]).Position.RowId==RowId)
								{
									this.Document.DeleteCell((Cell)_changedCellList[index]);
									_changedCellList.RemoveAt(index);
								}
							}

							// Remove Row
							row.ChildRows.Remove(childRow);
							this.Document.Template.DeleteUserRow(RowId);

							row.Expression = string.Empty;
							_viewRows = null;
							_rowIndexCache = null;

							// Clean Up Total Column automatically
							foreach(Row totalRow in this.Document.Template.Rows)
							{
								Cell totalCell = this.GetCell("TT",totalRow.Id);
								if(totalCell.Type==CellType.AutoCalc)
								{
									_changedCellList.Remove(totalCell);
									this.Document.DeleteCell(totalCell);
									totalCell = this.GetCell("TT",totalRow.Id);
									_changedCellList.Add(totalCell);
								}
							}
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Res the order block row.
		/// </summary>
		/// <param name="BlockId">The block id.</param>
		/// <param name="oldIndex">The old index.</param>
		/// <param name="newIndex">The new index.</param>
		public void ReOrderBlockRow(string BlockRowId, string oldRowId, string newRowId)
		{

			foreach(Row row in this.Document.Template.Rows)
			{
				if(row.HasChildRows && row.Id == BlockRowId)
				{
					int oldIndex = -1;
					int newIndex = -1;

					for(int index = 0; index < row.ChildRows.Count; index++)
					{
						Row rowItem = (Row)row.ChildRows[index];

						if(rowItem.Id==oldRowId)
							oldIndex = index;
						if(rowItem.Id==newRowId)
							newIndex = index;
					}

					//DV
					//if (newRowId == row.Id)
					//	newIndex = -1;

					if(oldIndex==newIndex)
						return;

					newIndex++;

					if(oldIndex==newIndex)
						return;

					Row chRow = (Row)row.ChildRows[oldIndex];

					if(newIndex>oldIndex)
						newIndex--;

					row.ChildRows.RemoveAt(oldIndex);

					if(newIndex>=row.ChildRows.Count)
						row.ChildRows.Add(chRow);
					else
						row.ChildRows.Insert(newIndex, chRow);

					break;
				}
			}


			_viewRows = null;
			_rowIndexCache = null;
		}

		#region GetReferencedCells
		/// <summary>
		/// Gets the referenced cells.
		/// </summary>
		/// <param name="srcCell">The SRC cell.</param>
		/// <returns></returns>
		public Cell[] GetReferencedCells(Cell srcCell)
		{
			ArrayList retVal = new ArrayList();

			GetReferencedCells(srcCell, retVal);

			return (Cell[])retVal.ToArray(typeof(Cell));
		}

		/// <summary>
		/// Gets the referenced cells.
		/// </summary>
		/// <param name="srcCell">The SRC cell.</param>
		/// <param name="refCellList">The ref cell list.</param>
		private void GetReferencedCells(Cell srcCell, ArrayList refCellList)
		{
			ArrayList foundList = new ArrayList();

			for(int colIndex =0;colIndex<this.Columns.Length; colIndex++)
			{
				for(int rowIndex =0;rowIndex<this.Rows.Length; rowIndex++)
				{
					Cell cell = this.GetCell(colIndex, rowIndex);

					if(cell!=null && cell.Type== CellType.AutoCalc)
					{
						//ExpressionInfo expInfo = ExpressionInfo.Parse(cell.Expression);
                        ExpressionInfo expInfo = cell.GetExpressionInfo();

						if(expInfo.ContainsParam(srcCell.Uid))
						{
							foundList.Add(cell);
						}
					}
				}
			}

			refCellList.AddRange(foundList);

			foreach(Cell chSrcCell in foundList)
			{
				GetReferencedCells(chSrcCell, refCellList);
			}
		}
		#endregion

		#region GetBackReferencedCells
		/// <summary>
		/// Gets the back referenced cells.
		/// </summary>
		/// <param name="srcCell">The SRC cell.</param>
		/// <returns></returns>
		public Cell[] GetBackReferencedCells(Cell srcCell)
		{
			ArrayList retVal = new ArrayList();

			GetReferencedCells(srcCell, retVal);

			return (Cell[])retVal.ToArray(typeof(Cell));
		}

		/// <summary>
		/// Gets the back referenced cells.
		/// </summary>
		/// <param name="srcCell">The SRC cell.</param>
		/// <param name="refCellList">The ref cell list.</param>
		private void GetBackReferencedCells(Cell srcCell, ArrayList refCellList)
		{
			ArrayList foundList = new ArrayList();

			//ExpressionInfo exInfo = ExpressionInfo.Parse(srcCell.Expression);
            ExpressionInfo exInfo = srcCell.GetExpressionInfo();

			foreach(string strParam in exInfo.Params)
			{
				string[] strPrmSplit = strParam.Split(':');

//				if(strPrmSplit[0].StartsWith("-"))
//				{
//					strPrmSplit[0] = strPrmSplit[0].Substring(1);
//				}

				Cell cell = this.GetCell(strPrmSplit[0], strPrmSplit[1]); 
				if(cell!=null)
				{
					foundList.Add(cell);
				}
			}

			refCellList.AddRange(foundList);

			foreach(Cell chSrcCell in foundList)
			{
				GetBackReferencedCells(chSrcCell, refCellList);
			}
		}
		#endregion


//		protected virtual void OnCellValueChanged(Cell cell)
//		{
//			if(this.CellValueChanged!=null)
//				this.CellValueChanged(cell, EventArgs.Empty);
//		}
	}
}
