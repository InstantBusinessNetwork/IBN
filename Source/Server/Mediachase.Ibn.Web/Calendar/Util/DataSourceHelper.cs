//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
namespace Mediachase.Web.UI.WebControls.Util
{
	public class DataSourceHelper
	{
		public static IEnumerable GetResolvedDataSource(object source, string member)
		{
			if(source != null && source is IListSource)
			{
				IListSource src = (IListSource)source;
				IList list = src.GetList();
				if(!src.ContainsListCollection)
				{
					return list;
				}
				if(list != null && list is ITypedList)
				{
					ITypedList tlist = (ITypedList)list;
					PropertyDescriptorCollection pdc = tlist.GetItemProperties(new PropertyDescriptor[0]);
					if(pdc != null && pdc.Count > 0)
					{
						PropertyDescriptor pd = null;
						if(member != null && member.Length > 0)
						{
							pd = pdc.Find(member, true);
						} 
						else
						{
							pd = pdc[0];
						}
						if(pd != null)
						{
							object rv = pd.GetValue(list[0]);
							if(rv != null && rv is IEnumerable)
							{
								return (IEnumerable)rv;
							}
						}
						throw new HttpException(
							String.Format("Can't find specified DataMember [{0}] in the DataSource", member));
					}
					throw new HttpException("DataSource doesn't contain any DataMembers");
				}
			}
			if(source is IEnumerable)
			{
				return (IEnumerable)source;
			}
			return null;
		}
	}
} 