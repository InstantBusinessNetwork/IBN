using System;

namespace IbnServerUpdate
{
	class IbnVersionInfo : IComparable
	{
		private int _version;
		private int _position;
		private string _data;
		private string _data2;

		public IbnVersionInfo(int version, int position, string data)
			: this(version, position, data, null)
		{
		}

		public IbnVersionInfo(int version, int position, string data, string data2)
		{
			_version = version;
			_position = position;
			_data = data;
			_data2 = data2;
		}

		public int Version
		{
			get { return _version; }
		}

		public string Data
		{
			get { return _data; }
		}

		public string Data2
		{
			get { return _data2; }
		}

		#region Implementation of IComparable
		public int CompareTo(object obj)
		{
			IbnVersionInfo vi = obj as IbnVersionInfo;

			int result = _version.CompareTo(vi._version);
			if(result == 0)
				result = _position.CompareTo(vi._position);

			return result;
		}
		#endregion
	}
}
