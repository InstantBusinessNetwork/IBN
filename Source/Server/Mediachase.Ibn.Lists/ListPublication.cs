using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;


namespace Mediachase.Ibn.Lists
{
	partial class ListPublication
	{
		#region List
		/// <summary>
		/// Lists the specified folder id.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <returns></returns>
		public static ListPublication[] List(int folderId)
		{
			return List(FilterElement.EqualElement("FolderId", folderId));
		}

		/// <summary>
		/// Lists the specified folder.
		/// </summary>
		/// <param name="folder">ListFolder.</param>
		/// <returns></returns>
		public static ListPublication[] List(MetaObject folder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			return List(FilterElement.EqualElement("FolderId", folder.PrimaryKeyId.Value));
		}
		#endregion
	}
}
