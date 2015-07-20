namespace Mediachase.Ibn.Web.UI.MetaUI
{
	using System;
	using System.Data;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using System.Text;
	using System.IO;

	using Mediachase.Ibn.Data.Meta.Management;
	using Mediachase.Ibn.Core;

	/// <summary>
	/// Summary description for ListColumnFactory
	/// </summary>
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

		public DataControlField GetColumn(Page pageInstance, MetaField field, bool isPrimaryKey)
		{
			if (pageInstance == null)
				throw new ArgumentNullException("pageInstance");
			if (field == null)
				throw new ArgumentNullException("field");

			if (ControlPathResolver.Current == null)
				throw new ArgumentNullException("ControlPathResolver");

			TemplateField retVal = new TemplateField();

			if (!isPrimaryKey)
			{
				ResolvedPath resPath = ControlPathResolver.Current.Resolve(CHelper.GetMetaTypeName(field), "Grid", field.Owner.Name, field.Name, viewName);

				//retVal.ItemTemplate = PageInstance.LoadTemplate(MetaFieldControlPathResolver.Resolve(CHelper.GetMetaTypeName(Field)/*Field.TypeName*/, "Grid", Field.Owner.Name, Field.Name, viewName, string.Empty));
				if(resPath!= null)
					retVal.ItemTemplate = pageInstance.LoadTemplate(resPath.Path);
			}
			else
				retVal.ItemTemplate = pageInstance.LoadTemplate("~/Apps/MetaUI/Primitives/Text.Grid.@.@.PrimaryKey.ascx");

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

			if (ControlPathResolver.Current == null)
				throw new ArgumentNullException("ControlPathResolver");

			if (pageInstance == null)
				throw new ArgumentNullException("pageInstance");

			TemplateField retVal = new TemplateField();

			ResolvedPath resPath = ControlPathResolver.Current.Resolve(CHelper.GetMetaTypeName(field), "Grid", field.Owner.Name, field.Name, placeName);

			if (resPath != null)
				retVal.ItemTemplate = pageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}
		#endregion

		public DataControlField GetCssColumn(Page PageInstance, string MetaClassName)
		{
			TemplateField retVal = new TemplateField();
			
			if (ControlPathResolver.Current == null)
				throw new ArgumentNullException("ControlPathResolver");

			if (PageInstance == null)
				throw new ArgumentNullException("PageInstance");

			ResolvedPath resPath = ControlPathResolver.Current.Resolve("RowCssClass", "Grid", MetaClassName, String.Empty, viewName);

			//retVal.ItemTemplate = PageInstance.LoadTemplate(MetaFieldControlPathResolver.Resolve("RowCssClass", "Grid", MetaClassName, string.Empty, viewName, string.Empty));
			if(resPath!=null)
				retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}

		public DataControlField GetAllowEditColumn(Page PageInstance, string MetaClassName)
		{
			TemplateField retVal = new TemplateField();

			if (ControlPathResolver.Current == null)
				throw new ArgumentNullException("ControlPathResolver");

			if (PageInstance == null)
				throw new ArgumentNullException("PageInstance");

			ResolvedPath resPath = ControlPathResolver.Current.Resolve("AllowEdit", "Grid", MetaClassName, String.Empty, viewName);

			//retVal.ItemTemplate = PageInstance.LoadTemplate(MetaFieldControlPathResolver.Resolve("AllowEdit", "Grid", MetaClassName, string.Empty, viewName, string.Empty));
			if(resPath!=null)
				retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}

		public DataControlField GetCustomColumn(Page PageInstance, string MetaClassName, string ColumnType)
		{
			TemplateField retVal = new TemplateField();

			if (ControlPathResolver.Current == null)
				throw new ArgumentNullException("ControlPathResolver");

			ResolvedPath resPath = ControlPathResolver.Current.Resolve(ColumnType, "Grid", MetaClassName, viewName, String.Empty);

			if (resPath != null)
				retVal.ItemTemplate = PageInstance.LoadTemplate(resPath.Path);

			return retVal;
		}
		#endregion
	}
}