using System;
using System.Collections;

namespace Mediachase.IBN.Business.SpreadSheet
{
	public enum RowVisibility
	{
		Template = 1,
		User = 2
	}

	/// <summary>
	/// Summary description for Row.
	/// </summary>
	public class Row
	{
		private string _name = string.Empty;
		private string _expression = string.Empty;
		private bool _readOnly = false;

		private string _id = string.Empty;
		private string _format = string.Empty;

		private RowVisibility _type = RowVisibility.Template;


		/// <summary>
		/// Initializes a new instance of the <see cref="Row"/> class.
		/// </summary>
		public Row()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Row"/> class.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <param name="Name">The name.</param>
		public Row(string Id, string Name)
		{
			_id = Id;
			_name = Name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Row"/> class.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <param name="Name">The name.</param>
		/// <param name="ReadOnly">if set to <c>true</c> [read only].</param>
		/// <param name="Expression">The expression.</param>
		/// <param name="Format">The format.</param>
		public Row(string Id, string Name, bool ReadOnly, string Expression, string Format)
		{
			_id = Id;
			_name = Name;
			_readOnly = ReadOnly;
			_expression = Expression;
			_format = Format;
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
		/// Gets or sets the visibility.
		/// </summary>
		/// <value>The visibility.</value>
		public RowVisibility Visibility
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
		public virtual string Expression
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

		public virtual bool HasChildRows
		{
			get 
			{
				return false;
			}
		}

		public virtual ArrayList ChildRows
		{
			get 
			{
				throw new NotSupportedException();
			}
		}
	}
}
