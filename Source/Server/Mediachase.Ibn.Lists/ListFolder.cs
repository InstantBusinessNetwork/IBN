using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data;
using System.Globalization;

namespace Mediachase.Ibn.Lists
{
	partial class ListFolder
	{
		/// <summary>
		/// Gets the tree service.
		/// </summary>
		/// <returns></returns>
		public TreeService GetTreeService()
		{
			return this.GetService<TreeService>();
		}

		/// <summary>
		/// Gets the type of the folder.
		/// </summary>
		/// <value>The type of the folder.</value>
		public ListFolderType FolderType
		{
			get
			{
				if(this.ProjectId!=null)
					return ListFolderType.Project;

				if (this.Owner != null)
					return ListFolderType.Private;

				return ListFolderType.Public;
			}
		}

	}
}
