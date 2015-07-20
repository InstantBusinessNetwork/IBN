//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
namespace Mediachase.Web.UI.WebControls.Design
{
	using System;
	using System.Web.UI.Design;
	using System.Drawing.Design;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using Mediachase.Web.UI.WebControls;

	/// <summary>
	/// Designer class for Mediachase.Web.UI.WebControls.Calendar
	/// </summary>
	public class CalendarDesigner : ControlDesigner
	{
		private DesignerVerbCollection _Verbs;

		/// <summary>
		/// Gets or sets a value indicating whether or not the control can be resized.
		/// </summary>
		public override bool AllowResize
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the design time verbs supported by the component associated
		/// with the designer.
		/// </summary>
		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (_Verbs == null)
				{
					string addEvent = DesignUtil.GetStringResource("CalendarAddEvent");
					string addOwner = DesignUtil.GetStringResource("CalendarAddOwner");
					string addHoliday = DesignUtil.GetStringResource("CalendarAddHoliday");

					_Verbs = new DesignerVerbCollection(
						new DesignerVerb[]
						{
							new DesignerVerb(addEvent, new EventHandler(OnAddEvent)),
							//new DesignerVerb(addHoliday, new EventHandler(OnAddHoliday)),
							//new DesignerVerb(addOwner, new EventHandler(OnAddOwner)),
					});
				}

				return _Verbs;
			}
		}

		/// <summary>
		/// Called when the Add Event menu item is clicked.
		/// </summary>
		/// <param name="sender">The source object</param>
		/// <param name="e">Event arguments</param>
		private void OnAddEvent(object sender, EventArgs e)
		{
			Calendar cal = (Calendar)Component;
			PropertyDescriptor itemsDesc = DesignUtil.GetPropertyDescriptor(cal, "Items");
			if (itemsDesc != null)
			{
				// Tell the designer that we're changing the property
				RaiseComponentChanging(itemsDesc);

				// Do the change
				CalendarItem calItem = new CalendarItem(0, "CalendarItem", DateTime.Now, "http://www.mediachase.com", "some desc");
				cal.Items.Add(calItem);

				// Tell the designer that we've changed the property
				RaiseComponentChanged(itemsDesc, null, null);
				UpdateDesignTimeHtml();
			}
		}

		/// <summary>
		/// Called when the Add Owner menu item is clicked.
		/// </summary>
		/// <param name="sender">The source object</param>
		/// <param name="e">Event arguments</param>
		private void OnAddOwner(object sender, EventArgs e)
		{
			Calendar cal = (Calendar)Component;
			PropertyDescriptor itemsDesc = DesignUtil.GetPropertyDescriptor(cal, "Owners");
			if (itemsDesc != null)
			{
				// Tell the designer that we're changing the property
				RaiseComponentChanging(itemsDesc);

				// Do the change
				Owner owner = new Owner("Owner", "Owner");
				cal.Owners.Add(owner);

				// Tell the designer that we've changed the property
				RaiseComponentChanged(itemsDesc, null, null);
				UpdateDesignTimeHtml();
			}
		}

		/// <summary>
		/// Called when the Add Holiday menu item is clicked.
		/// </summary>
		/// <param name="sender">The source object</param>
		/// <param name="e">Event arguments</param>
		private void OnAddHoliday(object sender, EventArgs e)
		{
			Calendar cal = (Calendar)Component;
			PropertyDescriptor itemsDesc = DesignUtil.GetPropertyDescriptor(cal, "Holidays");
			if (itemsDesc != null)
			{
				// Tell the designer that we're changing the property
				RaiseComponentChanging(itemsDesc);

				// Do the change
				Holiday holiday = new Holiday(DateTime.Now.Date, "Holiday");
				cal.Holidays.Add(holiday);

				// Tell the designer that we've changed the property
				RaiseComponentChanged(itemsDesc, null, null);
				UpdateDesignTimeHtml();
			}
		}


		/// <summary>
		/// Retrieves the HTML to display in the designer.
		/// </summary>
		/// <returns>The design-time HTML.</returns>
		public override string GetDesignTimeHtml()
		{
			Calendar cal = (Calendar)Component;

			// If the Calendar is empty, then add a label with instructions
			//if (cal.Items.Count == 0)
			//{
			//    return CreatePlaceHolderDesignTimeHtml(DesignUtil.GetStringResource("CalendarNoEvents"));
			//}
            
			return base.GetDesignTimeHtml();
		}
	}
}

