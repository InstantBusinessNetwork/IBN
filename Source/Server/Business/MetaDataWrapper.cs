using System;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for MetaData.
	/// </summary>
	public class MetaDataWrapper
	{

		public const string SystemRoot = "Mediachase.MetaDataPlus.System";
		public const string UserRoot = "Mediachase.MetaDataPlus.User";

		#region LoadMetaObject
		public static MetaObject LoadMetaObject(int ObjectId, string MetaClassName)
		{
			return MetaObject.Load(ObjectId, MetaClassName);
		}

		public static MetaObject LoadMetaObject(int ObjectId, string MetaClassName, int ModifierId, DateTime Modified)
		{
			return MetaObject.Load(ObjectId, MetaClassName, ModifierId, Modified);
		}
		#endregion

		#region NewMetaObject
		public static MetaObject NewMetaObject(int ObjectId, string MetaClassName)
		{
			return MetaObject.NewObject(ObjectId, MetaClassName, Security.CurrentUser.UserID, DateTime.UtcNow);
		}
		#endregion

		#region AcceptChanges
		public static int AcceptChanges(MetaObject obj)
		{
			obj.AcceptChanges();

			if (obj.MetaClass.Namespace == "Mediachase.IBN40.List") // ƒобавление/изменение записи в списке
			{
				string[] splitter = obj.MetaClass.Name.Split('_');
				try
				{
					int ListId = int.Parse(splitter[1]);
					SystemEvents.AddSystemEvents(SystemEventTypes.List_Updated_Data, ListId);
				}
				catch{}
			}

			return obj.Id;
		}
		#endregion

		#region GetMetaFieldNameSpace
		public static string GetMetaFieldNameSpace(int FieldId)
		{
			return MetaField.Load(FieldId).Namespace;
		}
		#endregion

		#region GetMetaFieldName
		public static string GetMetaFieldFriendlyName(int FieldId)
		{
			return MetaField.Load(FieldId).FriendlyName;
		}
		#endregion
	}
}
