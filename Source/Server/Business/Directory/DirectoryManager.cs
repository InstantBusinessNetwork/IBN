using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Business.Directory
{
	/// <summary>
	/// Represents Directory Manager.
	/// </summary>
	public static class DirectoryManager
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CreatePrincipal
		/// <summary>
		/// Creates the principal.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <param name="name">The name.</param>
		internal static void CreatePrincipal(DirectoryPrincipalType principalType, PrimaryKeyId primaryKeyId, string name)
		{
			DirectoryPrincipalEntity principal = (DirectoryPrincipalEntity)BusinessManager.InitializeEntity(DirectoryPrincipalEntity.ClassName);

			principal["DirectoryPrincipalId"] = primaryKeyId;
			principal.Type = (int)principalType;
			principal.Name = name;

			BusinessManager.Create(principal);
		} 
		#endregion

		#region DeletePrincipal
		/// <summary>
		/// Deletes the principal.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
		internal static void DeletePrincipal(PrimaryKeyId primaryKeyId)
		{
			DirectoryPrincipalEntity principal = (DirectoryPrincipalEntity)BusinessManager.Load(DirectoryPrincipalEntity.ClassName, primaryKeyId);
			BusinessManager.Delete(principal);
		} 
		#endregion

		#region UpdatePrincipal
		/// <summary>
		/// Deletes the principal.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
		internal static void UpdatePrincipal(PrimaryKeyId primaryKeyId, string name)
		{
			DirectoryPrincipalEntity principal = (DirectoryPrincipalEntity)BusinessManager.Load(DirectoryPrincipalEntity.ClassName, primaryKeyId);
			principal.Name = name;
			BusinessManager.Update(principal);
		}
		#endregion

		#endregion
	}
}
