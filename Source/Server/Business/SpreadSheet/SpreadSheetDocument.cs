using System;
using System.Xml;
using System.Drawing;
using System.Collections;

namespace Mediachase.IBN.Business.SpreadSheet
{
	public enum SpreadSheetDocumentType
	{
		WeekYear = 1,
		MonthQuarterYear = 2, // By default
		QuarterYear = 3,
		Year = 4,
		Total = 5,
	}

	/// <summary>
	/// Summary description for SpreadSheetDocument.
	/// </summary>
	public class SpreadSheetDocument
	{
		private SpreadSheetDocumentType _type  = SpreadSheetDocumentType.MonthQuarterYear;
		private SpreadSheetTemplate _template = new SpreadSheetTemplate();
		private ArrayList _cells = new ArrayList();

		private ArrayList _deletedCells = new ArrayList();

		//private Hashtable _assignedCells = new Hashtable();
		private Hashtable _cellIndexHash = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpreadSheetDocument"/> class.
		/// </summary>
		public SpreadSheetDocument()
		{
		}

		private void CheckIndexHash()
		{
			if(_cellIndexHash==null)
			{
				_cellIndexHash = new Hashtable();

				for(int index = 0; index<_cells.Count;index++)
				{
					_cellIndexHash.Add(((Cell)_cells[index]).Uid, index);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpreadSheetDocument"/> class.
		/// </summary>
		/// <param name="DocumentType">Type of the document.</param>
		public SpreadSheetDocument(SpreadSheetDocumentType DocumentType)
		{
			_type = DocumentType;
		}

		/// <summary>
		/// Gets the type of the document.
		/// </summary>
		/// <value>The type of the document.</value>
		public SpreadSheetDocumentType DocumentType
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the cells.
		/// </summary>
		/// <value>The cells.</value>
		public Cell[] Cells
		{
			get 
			{
				return (Cell[])_cells.ToArray(typeof(Cell));
			}
		}

		/// <summary>
		/// Gets the deleted cells.
		/// </summary>
		/// <value>The deleted cells.</value>
		internal ArrayList DeletedCells
		{
			get
			{
				return _deletedCells;
			}
		}

		/// <summary>
		/// Gets the template.
		/// </summary>
		/// <value>The template.</value>
		public SpreadSheetTemplate Template
		{
			get 
			{
				return _template;
			}
		}

		/// <summary>
		/// Gets the <see cref="Cell"/> with the specified column id.
		/// </summary>
		/// <value></value>
		public Cell this[string ColumnId, string RowId]
		{
			get
			{
				return this.GetCell(ColumnId, RowId);
			}
		}

		/// <summary>
		/// Gets the cell.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		/// <returns></returns>
		public Cell GetCell(string ColumnId, string RowId)
		{
			string strCellKey = string.Format("{0}:{1}", ColumnId,RowId);

			CheckIndexHash();

			if(_cellIndexHash.ContainsKey(strCellKey))
			{
				return (Cell)_cells[(int)_cellIndexHash[strCellKey]];
			}

			return null;
		}

		#region Load
		public void LoadXml(string Xml)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(Xml);

			XmlNode templateNode = xmlDoc.SelectSingleNode("SpreadSheet/Template");
			_template.LoadFromXmlNode(templateNode);

			XmlNode dataNode = xmlDoc.SelectSingleNode("SpreadSheet/Data");
			LoadDataFromXmlNode(dataNode);
		}

		protected void LoadDataFromXmlNode(XmlNode dataNode)
		{
		}
		#endregion

		/// <summary>
		/// Adds the cell.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		/// <param name="Type">The type.</param>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		public Cell AddCell(string ColumnId, string RowId, CellType Type, double Value)
		{
			Cell newCell = new Cell(this, ColumnId, RowId);
			newCell.Type = Type;
			newCell.Value = Value;

			this.AddCell(newCell);

			return newCell;
		}

		/// <summary>
		/// Adds the cell.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		public Cell AddCell(string ColumnId, string RowId, double Value)
		{
			Cell newCell = new Cell(this, ColumnId, RowId);
			newCell.Value = Value;

			this.AddCell(newCell);

			return newCell;
		}

		/// <summary>
		/// Adds the cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		public void AddCell(Cell cell)
		{
			// Add
			_cells.Add(cell);
			_cellIndexHash = null;
		}

		/// <summary>
		/// Deletes the cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		public void DeleteCell(Cell cell)
		{
			_deletedCells.Add(cell);

			_cells.Remove(cell);
			_cellIndexHash = null;
		}

		/// <summary>
		/// Deletes all cells.
		/// </summary>
		public void DeleteAllCells()
		{
			foreach (Cell cell in _cells)
			{
				_deletedCells.Add(cell);
			}

			_cells.Clear();
			_cellIndexHash = null;
		}
	}
}
