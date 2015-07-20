using System;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for CellPosition.
	/// </summary>
	public class CellPosition
	{
		private string _columnId;
		private string _rowId;
	
		/// <summary>
		/// Initializes a new instance of the <see cref="CellPosition"/> class.
		/// </summary>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		public CellPosition(string ColumnId, string RowId)
		{
			_columnId = ColumnId;
			_rowId = RowId;
		}

		/// <summary>
		/// Gets the index of the cell.
		/// </summary>
		/// <value>The index of the cell.</value>
		public string ColumnId
		{
			get 
			{
				return _columnId;
			}
		}

		/// <summary>
		/// Gets the index of the row.
		/// </summary>
		/// <value>The index of the row.</value>
		public string RowId
		{
			get 
			{
				return _rowId;
			}
		}
	}
}
