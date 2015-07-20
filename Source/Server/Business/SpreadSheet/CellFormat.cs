using System;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for CellFormat
	/// </summary>
	public class CellFormat
	{
		private CellType _type = CellType.Common;

		private string _expression = string.Empty;
		private bool _readOnly = false;
		private string _format = string.Empty;

		public CellFormat()
		{
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public CellType Type
		{
			get 
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Gets or sets the expression.
		/// </summary>
		/// <value>The expression.</value>
		public string Expression
		{
			get 
			{
				return _expression;
			}
			set
			{
				_expression = value;
			}
		}

		/// <summary>
		/// Gets or sets the format.
		/// </summary>
		/// <value>The format.</value>
		public string Format
		{
			get 
			{
				return _format;
			}
			set
			{
				_format = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether [read only].
		/// </summary>
		/// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			get 
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
			}
		}
	}
}
