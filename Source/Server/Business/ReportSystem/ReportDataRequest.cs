using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Ibn.Core.Business;


namespace Mediachase.IBN.Business.ReportSystem
{
	public class ReportDataRequest : Request
	{
		public const string FilterParameterName = "Filter";

		public ReportEntity Report
		{
			get { return this.Target as ReportEntity; }
			set { this.Target = value; }
		}

		public string Filter
		{
			get { return this.Parameters[FilterParameterName].Value as string; }
			set { this.Parameters[FilterParameterName].Value = value; }
		}

		public ReportDataRequest(ReportEntity report, string filter)
			: base(ReportRequestMethod.GetReportData, report)
		{
			this.Parameters.Add(new RequestParameter(FilterParameterName, filter));
		}
	}
}
