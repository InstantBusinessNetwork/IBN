using System;

namespace Mediachase.Ibn.Reporting
{

	public class FilterEventArgs : EventArgs
	{
		private string _filterSql;
		private string _filterXml;

		public FilterEventArgs(string filterSql, string filterXml)
		{
			_filterSql = filterSql;
			_filterXml = filterXml;
		}

		public string FilterSql
		{
			get { return _filterSql; }
			set { _filterSql = value; }
		}

		public string FilterXml
		{
			get { return _filterXml; }
			set { _filterXml = value; }
		}
	}

	public interface IReportFilter
	{
		string FilterXml { get; set; }
		string FilterSql { get; }
		Uri WebServiceUri { get; set; }
		event EventHandler<FilterEventArgs> FilterChanged;
	}
}
