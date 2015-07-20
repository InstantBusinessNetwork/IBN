using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.MetaUI
{

	public partial class ListHandler : System.Web.UI.Page
	{
		[WebMethod]
		public static string Test(string ViewName)
		{
			return String.Format("Called by {0}", ViewName);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request["ViewName"] != null)
			{
				MetaView view = GetViewByName(Request["ViewName"]);
				string ActionName = Request["Action"];

				//TODO: PrincipalId
				McMetaViewPreference pref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(view, 13/*Security.CurrentUserId*/);

				switch (ActionName)
				{
					case "ColumnResize":
						{
							pref.SetMetaFieldWidth(Convert.ToInt32(Request["columnIndex"]), Convert.ToInt32(Request["newSize"]));
							break;
						}
					case "OrderChange":
						{
							pref.ChangeMetaFieldOrder(Convert.ToInt32(Request["oldIndex"]), Convert.ToInt32(Request["newIndex"]));
							break;
						}
					case "HiddenChange":
						{
							pref.HideMetaField(Request["columnName"]);
							break;
						}
					case "EditColumns":
						{
							pref.ShowAllMetaField();
							string hiddenColumns = Request["hiddenColumns"];
							if (hiddenColumns.Length > 0)
							{
								foreach (string fieldName in hiddenColumns.Split(','))
								{
									pref.HideMetaField(fieldName);
								}
							}
							break;
						}
					case "ResizeLayout":
						{
							int newSize = Convert.ToInt32(Request["newSize"]);
							string attrName = string.Empty;

							#region Convert west -> MarginLeft (etc.)
							switch (Request["RegionId"])
							{
								case "west":
									{
										attrName = "MarginLeft";
										break;
									}
								case "east":
									{
										attrName = "MarginRight";
										break;
									}
								case "north":
									{
										attrName = "MarginTop";
										break;
									}
								case "south":
									{
										attrName = "MarginBottom";
										break;
									}
							}
							#endregion

							pref.Attributes.Set(attrName, newSize);
							break;
						}
				}

				//TODO: PrincipalId
				Mediachase.Ibn.Core.UserMetaViewPreference.Save(13/*Security.CurrentUserId*/, pref);
			}

			if (Request["CommandId"] != null)
			{
				Response.Write("Test toolbar action: " + Request["CommandId"]);
			}
		}

		public MetaView GetViewByName(string ViewName)
		{
			MetaView retVal = null;

			retVal = Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ViewName];

			if (retVal == null)
			{
				throw new ArgumentException(String.Format("Cant find view with name: {0}", ViewName));
			}

			return retVal;
		}
	}

}
