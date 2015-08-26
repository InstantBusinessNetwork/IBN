using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookAddin.OutlookItemAdaptors
{
	public abstract class OutlookItem
	{
		protected OutlookListener _outlookListener;

		public OutlookItem(OutlookListener outlookListener)
		{
			_outlookListener = outlookListener;
		}

		protected void SetProp(object oItem, string propName, object value)
		{
			_outlookListener.SetOutlookPropVal(oItem, propName, value);
		}

		protected T GetProp<T>(object oItem, string propName)
		{
			T retVal = default(T);

			retVal = (T)_outlookListener.GetOutlookPropVal(oItem, propName);
			return retVal;
		}

		public abstract object InnerOutlookObject { get; }
	}
}
