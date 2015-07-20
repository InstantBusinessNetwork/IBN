using System;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for ReportCategoryInfo.
	/// </summary>
	public class ReportCategoryInfo
	{
		private int _id = 0;
		private string _name = string.Empty;

		internal ReportCategoryInfo(int id, string name)
		{
			_id = id;
			_name = name;
		}

		public int Id
		{
			get
			{
				return _id;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

	}
}
