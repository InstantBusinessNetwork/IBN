using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents assignment entity.
	/// </summary>
	partial class AssignmentEntity
	{
		/// <summary>
		/// Gets the control path.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static string GetControlPath(AssignmentEntity entity)
		{
			return "~/Apps/BusinessProcess/AssignmentControls/ApprovalWithComment.ascx";
		}

		private static void CreateMetaField()
		{
			DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;");

			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaClass metaClass = DataContext.Current.GetMetaClass(AssignmentEntity.ClassName);

				using (MetaFieldBuilder builder = new MetaFieldBuilder(metaClass))
				{
					builder.CreateInteger("ClosedBy", "Closed By", true, 0);

					builder.SaveChanges();
				}

				scope.SaveChanges();
			}
		}
	}
}
