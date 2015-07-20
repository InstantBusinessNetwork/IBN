using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;

using Mediachase.IBN.Business;


namespace Mediachase.UI.Web.Projects
{
	// Page parameters:
	//
	// Users: String. Comma separated list of user identifiers.
	// Xml: Boolean. True = return text/xml, False = return image/png.
	// Vast: Boolean. True = vast scale, False = small scale.
	// X: Integer. The x coordinate of image portion.
	// Y: Integer. The y coordinate of image portion.
	// ItemsPerPage: Integer. The number of users per page.
	// PageNumber: Integer.
	// StartDate: DateTime in format "yyyy-MM-dd". The start date of the interval
	// CurDate: DateTime in format "yyyy-MM-dd HH:mm". The current date.
	// ObjectTypes: String. Comma separated list of object types identifiers.
	// HObjects: String. Comma separated list of object identifiers to highlight.
	// HTypes: String. Comma separated list of object types (corresponded to objects) to highlight.

	public partial class ResourceChartImage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			byte[] data = null;

			bool dataIsXml = Request["Xml"] != null;

			// users
			List<int> list = new List<int>();
			string users = Request["Users"];
			if (!string.IsNullOrEmpty(users))
			{
				string[] parts = users.Split(',');
				foreach (string part in parts)
				{
					int userId = int.Parse(part, CultureInfo.InvariantCulture);
					list.Add(userId);
				}
			}

			// object types
			List<ObjectTypes> objectTypes = new List<ObjectTypes>();
			string objectTypesString = Request["ObjectTypes"];
			if (!string.IsNullOrEmpty(objectTypesString))
			{
				string[] parts = objectTypesString.Split(',');
				foreach (string part in parts)
				{
					ObjectTypes objectType = (ObjectTypes)int.Parse(part, CultureInfo.InvariantCulture);
					objectTypes.Add(objectType);
				}
			}

			// vastScale
			bool vastScale = false;
			string vast = Request["Vast"];
			if (!string.IsNullOrEmpty(vast))
			{
				vastScale = bool.Parse(vast);
			}

			// startDate
			DateTime startDate = DateTime.Now.Date;
			string dateString = Request["StartDate"];
			if (!string.IsNullOrEmpty(dateString))
			{
				startDate = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
			}

			// curDate
			DateTime curDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
			string curDateString = Request["CurDate"];
			if (!string.IsNullOrEmpty(curDateString))
			{
				curDateString = Server.UrlDecode(curDateString);
				curDate = DateTime.ParseExact(curDateString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
			}

			// highlighted Items
			List<KeyValuePair<int, int>> highlightedItems = new List<KeyValuePair<int, int>>();
			string hObjects = Request["HObjects"];
			string hTypes = Request["HTypes"];
			if (!string.IsNullOrEmpty(hObjects) && !string.IsNullOrEmpty(hTypes))
			{
				string[] objects = hObjects.Split(',');
				string[] types = hTypes.Split(',');
				if (objects.Length == types.Length)
				{
					for (int i = 0; i < objects.Length; i++)
					{
						highlightedItems.Add(new KeyValuePair<int, int>(
							int.Parse(objects[i], CultureInfo.InvariantCulture), 
							int.Parse(types[i], CultureInfo.InvariantCulture)
							));
					}
				}
			}

			//if (Project.IsWebGanttChartEnabled())
			data = ResourceChart.Render(vastScale, curDate, startDate, list.ToArray(), objectTypes.ToArray(), highlightedItems, dataIsXml, Server.MapPath(@"~/styles/IbnFramework/ResourceChartStyle.xml"), ParseInteger("X", 0), ParseInteger("Y", 0), ParseInteger("ItemsPerPage", 100), ParseInteger("PageNumber", 0));

			if (data != null && data.Length > 0)
			{
				Response.Clear();
				Response.Cache.SetNoStore();
				if (dataIsXml)
					Response.ContentType = "text/xml";
				else
					Response.ContentType = "image/png";
				Response.OutputStream.Write(data, 0, data.Length);
				Response.End();
			}
			else
				Response.Redirect("~/Layouts/Images/Blank.gif", true);
		}

		private int ParseInteger(string name, int defaultValue)
		{
			int ret = defaultValue;

			string value = Request[name];
			if (!string.IsNullOrEmpty(value))
			{
				ret = int.Parse(value, CultureInfo.InvariantCulture);
			}

			return ret;
		}
	}
}
