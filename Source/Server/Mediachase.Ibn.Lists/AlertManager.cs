using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;


namespace Mediachase.Ibn.Lists
{
	public static class AlertManager
	{
		public static void Init()
		{
			ListManager.ListCreated += new EventHandler<InfoEventArgs>(ListCreated);
			ListManager.ListDeleting += new EventHandler<InfoEventArgs>(ListDeleting);
			ListManager.ListSaving += new EventHandler<InfoEventArgs>(ListSaving);
			ChangeDetectionService.ObjectModified += new EventHandler<BusinessObjectEventArgs>(ObjectModified);
		}

		private static void ListCreated(object sender, InfoEventArgs e)
		{
			SystemEvents.AddSystemEvents(SystemEventTypes.List_Created, (int)e.Info.PrimaryKeyId.Value);
		}

		private static void ListDeleting(object sender, InfoEventArgs e)
		{
			SystemEvents.AddSystemEvents(SystemEventTypes.List_Deleted, (int)e.Info.PrimaryKeyId.Value);
		}

		private static void ListSaving(object sender, InfoEventArgs e)
		{
			if (e != null && e.Info != null && e.Info.ObjectState == MetaObjectState.Modified)
			{
				if (e.Info.Properties["Title"].IsChanged || e.Info.Properties["Description"].IsChanged)
				{
					SystemEvents.AddSystemEvents(SystemEventTypes.List_Updated_GeneralInfo, (int)e.Info.PrimaryKeyId.Value);
				}

				if (e.Info.Properties["Status"].IsChanged)
				{
					SystemEvents.AddSystemEvents(SystemEventTypes.List_Updated_Status, (int)e.Info.PrimaryKeyId.Value);
				}
			}
		}

		private static void ObjectModified(object sender, BusinessObjectEventArgs e)
		{
			MetaClass metaClass = e.Object.GetMetaType();
			if(ListManager.MetaClassIsList(metaClass))
				SystemEvents.AddSystemEvents(SystemEventTypes.List_Updated_Data, ListManager.GetListIdByClassName(metaClass.Name));
		}
	}
}
