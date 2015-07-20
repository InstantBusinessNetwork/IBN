using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Events.CustomMethods;

namespace Mediachase.Ibn.Events.Plugins
{
	/// <summary>
	/// CalendarEvent Plugin. Обеспечивает авторизацию текущего пользователя для работы с объектами метакласса CalendarEvent
	/// </summary>
	public class SecurityPlugin : IPlugin
	{
		#region IPlugin Members
		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(BusinessContext context)
		{
			if (context != null)
			{
				bool forceBase = context.Request.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
				if (!forceBase)
				{
					try
					{
						switch (context.GetMethod())
						{
							case RequestMethod.List:
							case RequestMethod.Load:
								ReadRightCheck(context);
								break;
						}
					}
					catch (Exception)
					{
						throw;
					}
				}
			}

		}

		#endregion

		/// <summary>
		/// Reads the right check.
		/// </summary>
		/// <param name="context">The context.</param>
		private void ReadRightCheck(BusinessContext context)
		{
			//Если есть флаг отключить проверку авторизации то ничего не фильтруем
			if (!SkipSecurityCheckScope.IsActive)
			{
				string securityViewQuery = @"SELECT ObjectId FROM [dbo].[CalendarEvent_Security_Read]";
				//Добавляем в sql context текущего пользователя
				SetContextInfo(Security.CurrentUserId);
				//Для метода List необходимо отфильтровать список согласно security view
				if (context.GetMethod() == RequestMethod.List)
				{
					ListRequest listRequest = context.Request as ListRequest;
					FilterElementCollection filterColl = new FilterElementCollection();
					foreach (FilterElement origFilterEl in listRequest.Filters)
					{
						filterColl.Add(origFilterEl);
					}

					FilterElement filterEl = new FilterElement("PrimaryKeyId", FilterElementType.In, securityViewQuery);
					filterColl.Add(filterEl);
					//перезаписываем старую коллекцию фильтров, новой
					listRequest.Filters = filterColl.ToArray();

				}//Для метода Load необходмио предотвратить загрузку объета не имеющего соотв прав
				else if (context.GetMethod() == RequestMethod.Load)
				{
					
					LoadRequest loadRequest = context.Request as LoadRequest;
					PrimaryKeyId eventId = loadRequest.Target.PrimaryKeyId.Value;
					VirtualEventId vEventId = (VirtualEventId)eventId;
					if (vEventId.IsRecurrence)
					{
						eventId = vEventId.RealEventId;
					}

					if (BusinessObject.GetTotalCount(DataContext.Current.GetMetaClass(CalendarEventEntity.ClassName),
													  new FilterElement[] {
																			new FilterElement("PrimaryKeyId", FilterElementType.Equal, eventId), 
																			new FilterElement("PrimaryKeyId", FilterElementType.In, securityViewQuery) 
																		   }) == 0)
					{
						throw new Exception("Read access denied");
					}
				}

			}
		}

		/// <summary>
		///	Записывает в Sql контекст, id пользователя
		/// </summary>
		/// <param name="userId">The user id.</param>
		private static void SetContextInfo(int userId)
		{
			string query = String.Format("DECLARE @BinaryId BINARY(128) SET @BinaryId = CAST({0} AS BINARY(128)) SET CONTEXT_INFO @BinaryId", userId);
			Mediachase.Ibn.Data.Sql.SqlHelper.ExecuteNonQuery(DataContext.Current.SqlContext, System.Data.CommandType.Text, query);
		}

	}
}
