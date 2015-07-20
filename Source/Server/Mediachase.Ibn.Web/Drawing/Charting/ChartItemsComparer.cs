using System.Collections;
using System.Collections.Generic;

namespace Mediachase.Ibn.Web.Drawing.Charting
{
	public class ChartItemsComparer : IComparer<ChartItem>
	{
		#region IComparer<ChartItem> Members

		public int Compare(ChartItem x, ChartItem y)
		{
			if (x.Value > y.Value)
				return -1;
			if (x.Value == y.Value)
				return 0;
			return 1;
		}

		#endregion
	}
}
