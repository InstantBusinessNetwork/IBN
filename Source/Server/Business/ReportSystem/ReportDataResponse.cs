using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

using Mediachase.Ibn.Core.Business;


namespace Mediachase.IBN.Business.ReportSystem
{
	public class ReportDataResponse : Response
	{
		private List<DataTable> _data;

		public ReadOnlyCollection<DataTable> Data
		{
			get { return _data.AsReadOnly(); }
		}

		public ReportDataResponse(List<DataTable> data)
		{
			_data = data;
		}
	}
}
