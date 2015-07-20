using System;
using System.Collections;
using System.Text;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for Block.
	/// </summary>
	public class Block: Row
	{
		private bool _canAddRow = true;
		private ArrayList _childRows = new ArrayList();

		private string _newRowDefaultName = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="Block"/> class.
		/// </summary>
		public Block()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Block"/> class.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <param name="Name">The name.</param>
		public Block(string Id, string Name):base(Id, Name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Block"/> class.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <param name="Name">The name.</param>
		/// <param name="CanAddRow">if set to <c>true</c> [can add row].</param>
		/// <param name="ReadOnly">if set to <c>true</c> [read only].</param>
		/// <param name="Expression">The expression.</param>
		/// <param name="Format">The format.</param>
		public Block(string Id, string Name, bool CanAddRow, bool ReadOnly, string Expression, string Format):
			base(Id, Name, ReadOnly, Expression, Format)
		{
			this.CanAddRow = CanAddRow;
		}

		/// <summary>
		/// Gets a value indicating whether this instance has child rows.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has child rows; otherwise, <c>false</c>.
		/// </value>
		public override bool HasChildRows
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the child rows.
		/// </summary>
		/// <value>The child rows.</value>
		public override ArrayList ChildRows
		{
			get 
			{
				return _childRows;
			}
		}

		public bool CanAddRow
		{
			get
			{
				return _canAddRow;
			}
			set
			{
				_canAddRow = value;
			}
		}

		public string NewRowDefaultName
		{
			get 
			{
				return _newRowDefaultName;
			}
			set
			{
				_newRowDefaultName = value;
			}
		}

		protected string BuildExpression()
		{
			StringBuilder sb = new StringBuilder(255);

			bool bFirstParam = true;
			foreach(Row childRow in this.ChildRows)
			{
				if(!bFirstParam)
					sb.Append('+');
				sb.AppendFormat("[{0}]",childRow.Id);
				bFirstParam = false;
			}

			return sb.ToString();
		}

		/// <summary>
		/// Gets or sets the expression.
		/// </summary>
		/// <value>The expression.</value>
		public override string Expression
		{
			get
			{
				if(base.Expression==string.Empty)
				{
					base.Expression = BuildExpression();
				}

				return base.Expression;
			}
			set
			{
				base.Expression = value;
			}
		}

	}
}
