using System;
using System.Collections;

namespace Mediachase.IBN.Business.SpreadSheet
{
	public enum CellType
	{
		Common		=	1,
		AutoCalc	=	2,
		UserValue	=	3
	}


	/// <summary>
	/// Summary description for Cell.
	/// </summary>
	public class Cell
	{
        private string _tmpUid = null;

		private SpreadSheetDocument _owner = null;
		private CellPosition _pos = null;
		private CellType _type = CellType.Common;
		private double _value = 0;

		private bool _readOnly = false;
		private string _format = string.Empty;
		private string _expression = string.Empty;

		private object _tag = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> class.
		/// </summary>
		public Cell(SpreadSheetDocument Owner, string ColumnId, string RowId)
		{
			_owner = Owner;
			_pos = new CellPosition(ColumnId, RowId);
		}

		#region Events
		public event EventHandler ValueChanged;

		private void RaiseValueChanged()
		{
			OnValueChanged();
		}

		protected virtual void OnValueChanged()
		{
			if(this.ValueChanged!=null)
				this.ValueChanged(this, EventArgs.Empty);
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets the owner.
		/// </summary>
		/// <value>The owner.</value>
		public SpreadSheetDocument Owner
		{
			get 
			{
				return _owner;
			}
		}

		/// <summary>
		/// Gets the uid.
		/// </summary>
		/// <value>The uid.</value>
		public string Uid
		{
			get 
			{
                if(_tmpUid==null)
				    _tmpUid =string.Format("{0}:{1}", this.Position.ColumnId, this.Position.RowId);

                return _tmpUid;
			}
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <value>The position.</value>
		public CellPosition Position
		{
			get 
			{
				return _pos;
			}
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
                _expressionInfo = null;
			}
		}

        private ExpressionInfo _expressionInfo = null;

        public ExpressionInfo GetExpressionInfo()
        {
            if (_expressionInfo == null)
                _expressionInfo = ExpressionInfo.Parse(this.Expression);

            return _expressionInfo;
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

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public double Value
		{
			get 
			{
				return this.InnerValue;
			}
			set
			{
				if(this.ReadOnly)
					throw new NotSupportedException();

				if(this.InnerValue==value)
					return;

				if(this.Type==CellType.AutoCalc)
					this.Type=CellType.UserValue;

				this.InnerValue = value;

				RaiseValueChanged();
			}
		}

		internal void SetAutoValue(double Value)
		{
			if(this.InnerValue==Value)
				return;

			this.InnerValue = Value;

			RaiseValueChanged();
		}

		/// <summary>
		/// Gets or sets the inner value.
		/// </summary>
		/// <value>The inner value.</value>
		protected virtual double InnerValue
		{
			get 
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public object Tag
		{
			get 
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}
		#endregion
	}
}
