using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Sql;


namespace Mediachase.Ibn.Lists
{
	partial class ListInfo
	{
		#region List
		/// <summary>
		/// Lists the specified folder.
		/// </summary>
		/// <param name="folder">ListFolder.</param>
		/// <returns></returns>
		public static ListInfo[] List(MetaObject folder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			return List(folder.PrimaryKeyId.Value);
		}

		/// <summary>
		/// Lists the specified folder id.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <returns></returns>
		public static ListInfo[] List(int folderId)
		{
			FilterElement listSequrityFilterElement = new FilterElement(
				"ListInfoId",
				FilterElementType.In,
				new SelectCommandBuilderParameter("SELECT ListId FROM LISTINFO_SECURITY_ALL WHERE PrincipalId = @UserId AND AllowLevel >= 1", SqlHelper.SqlParameter("@UserId", SqlDbType.Int, Security.CurrentUserId)));

			return List(FilterElement.EqualElement("FolderId", folderId), listSequrityFilterElement);
		}
		#endregion

		#region Delete()
		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public override void Delete()
		{
			ListManager.DeleteList(this);
		}

		/// <summary>
		/// Inners the delete.
		/// </summary>
		internal void InnerDelete()
		{
			base.Delete();
		}
		#endregion

		protected override void OnSaving()
		{
			base.OnSaving();
			ListManager.SavingList(this);
		}

		protected override void OnSaved()
		{
			base.OnSaved();
			ListManager.SavedList(this);
		}
	}
}
