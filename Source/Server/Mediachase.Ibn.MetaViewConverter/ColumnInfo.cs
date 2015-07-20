using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Converter
{
	internal class ColumnInfo : IComparable<ColumnInfo>
	{
		private string _name;
		private int _index;
		private int _width = 150;

		public ColumnInfo(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public int Index
		{
			set { _index = value; }
		}

		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		#region IComparable<ColumnInfo> Members

		public int CompareTo(ColumnInfo other)
		{
			int result;

			if (other == null)
			{
				result = -1;
			}
			else
				result = _index.CompareTo(other._index);

			return result;
		}

		#endregion
	}
}
