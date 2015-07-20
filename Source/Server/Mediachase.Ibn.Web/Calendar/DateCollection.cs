//------------------------------------------------------------------------------
// Copyright (c) 2004 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

using System;
using System.Collections;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Summary description for DateCollection.
	/// </summary>
	public class DateCollection : CollectionBase
	{
		private Calendar _Calendar = null;
		public DateCollection(Calendar calendar)
		{
			_Calendar = calendar;
		}

		public bool Contains(DateTime date)
		{
			return base.InnerList.Contains(date); 
		} 

		public DateTime this[int index] 
		{ 
			get 
			{
					return ((DateTime) base.InnerList[index]); 
			}
		} 

		public void Add(DateTime date)
		{
			if (!base.InnerList.Contains(date))
			{
				base.InnerList.Add(date);
				base.InnerList.Sort();
 			}
 		}  
	}
}