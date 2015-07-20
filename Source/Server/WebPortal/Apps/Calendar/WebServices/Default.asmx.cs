using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using Mediachase.AjaxCalendar;
using System.Web.Script.Services;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Data;
using System.Globalization;
//using Mediachase.Ibn.Calendar;

namespace Mediachase.Ibn.Web.UI.Calendar.WebServices
{
	/// <summary>
	/// Summary description for Default
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	[ScriptService]
	public class Default : System.Web.Services.WebService, IItemsWebServiceInterface 
	{

		#region IItemsWebServiceInterface Members

		[WebMethod]
		public void CreateItem(string title, string startDate, string endDate, string description, bool isAllDay, object extentions, object calendarExtension)
		{
			
		}

		[WebMethod]
		public void DeleteItem(string uid, object calendarExtension)
		{
			CalendarEventEntity ceo = (CalendarEventEntity)BusinessManager.Load(CalendarEventEntity.ClassName, PrimaryKeyId.Parse(uid));
			BusinessManager.Delete(ceo);
		}

		[WebMethod]
		public CalendarItem[] LoadItems(string startDate, string endDate, object calendarExtension)
		{
			DateTime viewStartDate = DateTime.Now;
			DateTime viewEndDate = DateTime.Now;
			string[] arr = startDate.Split(new char[] { '.' });
			viewStartDate = new DateTime(int.Parse(arr[0]),int.Parse(arr[1]),
										int.Parse(arr[2]),int.Parse(arr[3]),int.Parse(arr[4]),
										int.Parse(arr[5]));
			arr = endDate.Split(new char[] { '.' });
			viewEndDate = new DateTime(int.Parse(arr[0]), int.Parse(arr[1]),
										int.Parse(arr[2]), int.Parse(arr[3]), int.Parse(arr[4]),
										int.Parse(arr[5]));

			List<CalendarItem> al = new List<CalendarItem>();
			FilterElementCollection fec = new FilterElementCollection();
			FilterElement fe = new FilterElement();
			fe.Type = FilterElementType.GreaterOrEqual;
			fe.Source = CalendarEventEntity.FieldStart;
			fe.Value = viewStartDate;
			fec.Add(fe);
			fe = new FilterElement();
			fe.Type = FilterElementType.LessOrEqual;
			fe.Source = CalendarEventEntity.FieldStart;
			fe.Value = viewEndDate;
			fec.Add(fe);

			EntityObject[] calList = BusinessManager.List(CalendarEventEntity.ClassName, fec.ToArray());
			foreach (EntityObject eo in calList)
			{
				CalendarEventEntity info = eo as CalendarEventEntity;
				CalendarItem it = new CalendarItem();
				it.Uid = info.PrimaryKeyId.ToString();
				it.StartDate = info.Start.ToString("yyyy.M.d.H.m.s");
				it.EndDate = info.End.ToString("yyyy.M.d.H.m.s"); ;
				it.Title = String.Format("<div class=\"ibn-propertysheet2\"><a href='{1}{2}'>{0}</a></div>", info.Subject, CHelper.GetAbsolutePath("/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=CalendarEvent&ObjectId="), it.Uid);
				it.Description = info.Body;
				it.Extensions = (info.RecurrenceId == DateTime.MinValue) ? "0" : "1";
				al.Add(it);

			}
			return al.ToArray();
		}

		[WebMethod]
		public void UpdateItem(string uid, string title, string startDate, string endDate, string description, bool isAllDay, object extentions, object calendarExtension)
		{
			DateTime viewStartDate = DateTime.Now;
			DateTime viewEndDate = DateTime.Now;
			string[] arr = startDate.Split(new char[] { '.' });
			viewStartDate = new DateTime(int.Parse(arr[0]), int.Parse(arr[1]),
										int.Parse(arr[2]), int.Parse(arr[3]), int.Parse(arr[4]),
										int.Parse(arr[5]));
			arr = endDate.Split(new char[] { '.' });
			viewEndDate = new DateTime(int.Parse(arr[0]), int.Parse(arr[1]),
										int.Parse(arr[2]), int.Parse(arr[3]), int.Parse(arr[4]),
										int.Parse(arr[5]));
			
			CalendarEventEntity ceo = (CalendarEventEntity)BusinessManager.Load(CalendarEventEntity.ClassName, PrimaryKeyId.Parse(uid));
			if (((VirtualEventId)ceo.PrimaryKeyId.Value).RealEventId == ceo.PrimaryKeyId
				&& extentions != null && extentions.ToString() == "resize")
			{
				arr = startDate.Split(new char[] { '.' });
				viewStartDate = DateTime.MinValue.AddHours(int.Parse(arr[3])).AddMinutes(int.Parse(arr[4]));

				arr = endDate.Split(new char[] { '.' });
				viewEndDate = DateTime.MinValue.AddHours(int.Parse(arr[3])).AddMinutes(int.Parse(arr[4]));
			}
			
			ceo.Start = viewStartDate;
			ceo.End = viewEndDate;
			BusinessManager.Update(ceo);
		}
		#endregion

	}
}
