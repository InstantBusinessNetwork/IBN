using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.MetaUI;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity
{
	public class ListColumnFactory : IFactory
	{
		private string viewName;

		public ListColumnFactory(string ViewName)
		{
			viewName = ViewName;
		}

		#region IFactory Members

		public DataControlField GetColumn(Page PageInstance, MetaField Field)
		{
			return GetColumn(PageInstance, Field, false);
		}

		public DataControlField GetColumn(Page PageInstance, MetaField Field, bool IsPrimaryKey)
		{
			if (PageInstance == null)
				throw new ArgumentNullException("pageInstance");
			if (Field == null)
				throw new ArgumentNullException("field");

			TemplateField retVal = new TemplateField();

			if (!IsPrimaryKey)
			{
				string className = Field.Owner.Name;
				if (ListManager.MetaClassIsList(className))
					className = "List_@";
				ResolvedPath resPath = ControlPathResolver.Current.Resolve(CHelper.GetMetaTypeName(Field), "GridEntity", className, Field.Name, viewName);

				if (resPath != null)
					retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);
			}
			else
				retVal.ItemTemplate = PageInstance.LoadTemplate("~/Apps/MetaUIEntity/Primitives/Text.GridEntity.@.@.PrimaryKey.ascx");

			return retVal;
		}

		#region GetColumn [O.R.: 2008-03-05]
		/// <summary>
		/// Gets the column.
		/// </summary>
		/// <param name="pageInstance">The page instance.</param>
		/// <param name="field">The field.</param>
		/// <param name="placeName">Name of the place.</param>
		/// <returns></returns>
		public DataControlField GetColumn(Page pageInstance, MetaField field, string placeName)
		{
			if (pageInstance == null)
				throw new ArgumentNullException("pageInstance");
			if (field == null)
				throw new ArgumentNullException("field");

			TemplateField retVal = new TemplateField();

			string className = field.Owner.Name;
			if (ListManager.MetaClassIsList(className))
				className = "List_@";

			ResolvedPath resPath = ControlPathResolver.Current.Resolve(CHelper.GetMetaTypeName(field), "GridEntity", className, field.Name, placeName);

			if (resPath != null)
				retVal.ItemTemplate = pageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}
		#endregion

		public DataControlField GetCssColumn(Page PageInstance, string MetaClassName)
		{
			TemplateField retVal = new TemplateField();

			string className = MetaClassName;
			if (ListManager.MetaClassIsList(className))
				className = "List_@";

			ResolvedPath resPath = ControlPathResolver.Current.Resolve("RowCssClass", "GridEntity", className, String.Empty, viewName);

			if (resPath != null)
				retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}

		public DataControlField GetAllowEditColumn(Page PageInstance, string MetaClassName)
		{
			TemplateField retVal = new TemplateField();

			string className = MetaClassName;
			if (ListManager.MetaClassIsList(className))
				className = "List_@";

			ResolvedPath resPath = ControlPathResolver.Current.Resolve("AllowEdit", "GridEntity", className, String.Empty, viewName);

			if (resPath != null)
				retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}

		public DataControlField GetCustomColumn(Page PageInstance, string MetaClassName, string ColumnType)
		{
			TemplateField retVal = new TemplateField();

			string className = MetaClassName;
			if (ListManager.MetaClassIsList(className))
				className = "List_@";

			ResolvedPath resPath = ControlPathResolver.Current.Resolve(ColumnType, "GridEntity", className, viewName, String.Empty);

			if (resPath != null)
				retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}

		#endregion
	}
}
