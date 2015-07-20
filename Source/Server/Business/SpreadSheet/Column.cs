using System;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for Column.
	/// </summary>
	public class Column
	{
		private string _name = string.Empty;
		private string _expression = string.Empty;
		private bool _readOnly = false;

		private string _id = string.Empty;
		private string _format = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="Column"/> class.
		/// </summary>
		public Column()
		{
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get 
			{
				return _name;
			}
			set 
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public string Id
		{
			get 
			{
				return _id;
			}
			set 
			{
				_id = value;
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
