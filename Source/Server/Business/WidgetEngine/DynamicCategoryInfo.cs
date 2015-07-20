using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public class DynamicCategoryInfo : IComparable, IComparer<DynamicCategoryInfo>
	{
		#region prop: Uid
		private string _uid;
		/// <summary>
		/// Gets or sets the uid.
		/// </summary>
		/// <value>The uid.</value>
		public string Uid
		{
			get { return _uid; }
			set { _uid = value; }
		} 
		#endregion

		#region prop: FriendlyName
		private string _friendlyName;

		/// <summary>
		/// Gets or sets the name of the friendly.
		/// </summary>
		/// <value>The name of the friendly.</value>
		public string FriendlyName
		{
			get { return _friendlyName; }
			set { _friendlyName = value; }
		}
		#endregion

		#region prop: Weight
		private int _weight = Int32.MaxValue;
		/// <summary>
		/// Gets or sets the weight.
		/// </summary>
		/// <value>The weight.</value>
		public int Weight
		{
			get { return _weight; }
			set { _weight = value; }
		} 
		#endregion

		#region .ctor()
		public DynamicCategoryInfo()
		{
		}

		public DynamicCategoryInfo(string uid)
			: this()
		{
			this.Uid = uid;
		}

		public DynamicCategoryInfo(string uid, string friendlyName)
			: this(uid)
		{
			this.FriendlyName = friendlyName;
		}

		public DynamicCategoryInfo(string uid, string friendlyName, int weight)
			: this(uid, friendlyName)
		{
			this.Weight = weight;
		} 
		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			DynamicCategoryInfo dci = (DynamicCategoryInfo)obj;
			if (dci != null)
				return this.Uid.CompareTo(dci.Uid);

			return 0;
		}

		#endregion

		#region IComparer<DynamicCategoryInfo> Members

		public int Compare(DynamicCategoryInfo x, DynamicCategoryInfo y)
		{
			return x.Uid.CompareTo(y.Uid);
		}

		#endregion
	}
}
