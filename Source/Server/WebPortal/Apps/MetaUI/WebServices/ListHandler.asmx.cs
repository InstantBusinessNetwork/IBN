using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.MobileControls;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Apps.MetaUI.Grid;

namespace Mediachase.Ibn.Web.UI.WebServices
{
	/// <summary>
	/// Summary description for ListHandler
	/// </summary>
	[WebService(Namespace = "http://ibn46.medichasae.ru/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ScriptService]
	public class ListHandler : System.Web.Services.WebService
	{
		private McMetaViewPreference mvPref;

		#region SaveMetaViewPreferences
		private void SaveMetaViewPreferences()
		{
			if (mvPref == null)
				throw new ArgumentException("mvPref is null @ SaveMetaViewPreferences");

			Mediachase.Ibn.Core.UserMetaViewPreference.Save(Mediachase.Ibn.Data.Services.Security.CurrentUserId, mvPref);
		}
		#endregion

		#region LoadMetaViewPreference
		private void LoadMetaViewPreference(string ContextKey)
		{
			MetaView view = GetViewByName(ContextKey);
			mvPref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(view, Mediachase.Ibn.Data.Services.Security.CurrentUserId);
		}
		#endregion

		#region GetViewByName
		public MetaView GetViewByName(string ContextKey)
		{
			MetaView retVal = null;

			retVal = Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ContextKey];

			if (retVal == null)
			{
				throw new ArgumentException(String.Format("Cant find view with name: {0}", ContextKey));
			}

			return retVal;
		}
		#endregion

		#region GetFieldNameByIndex
		/// <summary>
		/// Gets the index of the field name by.
		/// </summary>
		/// <param name="ColumnIndex">Index of the column.</param>
		/// <returns></returns>
		private string GetFieldNameByIndex(int ColumnIndex)
		{
			return mvPref.GetVisibleMetaField()[ColumnIndex].Name;
		}
		#endregion

		#region GetMinutesFromString
		/// <summary>
		/// Gets the minutes from string.
		/// </summary>
		/// <param name="Time">The time.</param>
		/// <returns></returns>
		private double GetMinutesFromString(string Time)
		{
			if (Time.Length != 5)
				throw new ArgumentException("Time");

			return Convert.ToInt32(Time.Substring(0, 2)) * 60 + Convert.ToInt32(Time.Substring(3, 2));
		}
		#endregion

		[WebMethod]
		[ScriptMethod(UseHttpGet = true)]
		public string Test2(string ContextKey)
		{
			return String.Format("true");
		}

		#region ColumnResize
		/// <summary>
		/// Columns the resize.
		/// </summary>
		/// <param name="ContextKey">Name of the view.</param>
		/// <param name="ColumnIndex">Index of the column.</param>
		/// <param name="NewSize">The new size.</param>
		[WebMethod]
		public void ColumnResize(string ContextKey, int ColumnIndex, int NewSize)
		{
			MetaGridContextKey gridContextKey = UtilHelper.JsonDeserialize<MetaGridContextKey>(ContextKey);

			LoadMetaViewPreference(gridContextKey.ViewName);
			mvPref.SetMetaFieldWidth(ColumnIndex - gridContextKey.CustomColumnsCount, NewSize);
			SaveMetaViewPreferences();
		}
		#endregion

		#region OrderChange
		/// <summary>
		/// Orders the change.
		/// </summary>
		/// <param name="ContextKey">Name of the view.</param>
		/// <param name="OldIndex">The old index.</param>
		/// <param name="NewIndex">The new index.</param>
		[WebMethod]
		public void OrderChange(string ContextKey, int OldIndex, int NewIndex)
		{
			MetaGridContextKey gridContextKey = UtilHelper.JsonDeserialize<MetaGridContextKey>(ContextKey);

			if (OldIndex - gridContextKey.CustomColumnsCount < 0)
				return;

			LoadMetaViewPreference(gridContextKey.ViewName);
			mvPref.ChangeMetaFieldOrder(OldIndex - gridContextKey.CustomColumnsCount, NewIndex - gridContextKey.CustomColumnsCount);
			SaveMetaViewPreferences();
		}
		#endregion

		#region HideColumn
		/// <summary>
		/// Hides the column.
		/// </summary>
		/// <param name="ContextKey">Name of the view.</param>
		/// <param name="ColumnName">Name of the column.</param>
		[WebMethod]
		public void HideColumn(string ContextKey, string ColumnName)
		{
			mvPref.HideMetaField(ColumnName);
		}
		#endregion

		#region LayoutResized
		/// <summary>
		/// Layouts the resized.
		/// </summary>
		/// <param name="ContextKey">Name of the view.</param>
		/// <param name="RegionName">Name of the region.</param>
		/// <param name="NewSize">The new size.</param>
		[WebMethod]
		public void LayoutResized(string ContextKey, string RegionName, int NewSize)
		{
			MetaGridContextKey gridContextKey = UtilHelper.JsonDeserialize<MetaGridContextKey>(ContextKey);

			LoadMetaViewPreference(gridContextKey.ViewName);
			string attrName = string.Empty;

			#region Convert west -> MarginLeft (etc.)
			switch (RegionName)
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

			mvPref.Attributes.Set(attrName, NewSize);
			SaveMetaViewPreferences();
		}
		#endregion

		#region UpdateGrid
		/// <summary>
		/// Updates the grid.
		/// </summary>
		/// <param name="ContextKey">Name of the view.</param>
		/// <param name="ChangesArray">The changes array.</param>
		[WebMethod]
		/// {"primaryKeyId":6, "column":1, "value":"11:21"}
		public void UpdateGrid(string ContextKey, object[] ChangesArray)
		{
			//List<string> fieldName = new List<string>();
			MetaGridContextKey gridContextKey = UtilHelper.JsonDeserialize<MetaGridContextKey>(ContextKey);
			List<MetaObject> changed = new List<MetaObject>();
			CHelper.AddToContext("GridUpdated", 1);

			LoadMetaViewPreference(gridContextKey.ViewName);
			MetaView CurrentView = GetViewByName(gridContextKey.ViewName);
			MetaObject[] list = CurrentView.List(mvPref);

			if (CurrentView.PrimaryGroupBy != null || CurrentView.SecondaryGroupBy == null)
			{
				if (CurrentView.SecondaryGroupBy != null)
					list = MetaViewGroupUtil.ExcludeCollapsed(MetaViewGroupByType.Secondary, CurrentView.SecondaryGroupBy, CurrentView.PrimaryGroupBy, mvPref, list);

				list = MetaViewGroupUtil.ExcludeCollapsed(MetaViewGroupByType.Primary, CurrentView.PrimaryGroupBy, null, mvPref, list);
			}


			foreach (Dictionary<string, object> obj in ChangesArray)
			{
				int primaryKeyId = Convert.ToInt32(obj["primaryKeyId"]);
				int columnId = Convert.ToInt32(obj["column"]);
				string value = Convert.ToString(obj["value"]);

				MetaObject tmp = GetMetaObjectById(list, primaryKeyId);
				MetaObject curObject = null;
				if (tmp != null)
					curObject = MetaObjectActivator.CreateInstance(tmp.GetMetaType(), primaryKeyId);
				else
					continue;

				curObject.Properties[GetFieldNameByIndex(columnId)].Value = GetMinutesFromString(value);

				changed.Add(curObject);
			}


			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				foreach (MetaObject changeObj in changed)
				{
					changeObj.Save();
				}

				tran.Commit();
			}
		}

		#region GetMetaObjectById
		private MetaObject GetMetaObjectById(MetaObject[] List, int? Id)
		{
			for (int i = 0; i < List.Length; i++)
			{
				if (List[i].PrimaryKeyId == Id)
					return List[i];
			}

			return null;
		}
		#endregion

		#endregion
	}
}
