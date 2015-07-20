using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents workflow parameters.
	/// </summary>
	public static class WorkflowParameters
	{
		public const string AssignmentOverdueActionName = "AssignmentOverdueAction";
		public const string AssignmentAutoCompleteCommentName = "AssignmentAutoCompleteComment";
		public const string OwnerReadOnly = "OwnerReadOnly";

		#region Custom Value
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static object GetValue(EntityObject entity, string key)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (key == null)
				throw new ArgumentNullException("key");

			if (entity.Properties.Contains(WorkflowInstanceEntity.FieldXSParameters))
			{
				AttributeCollection attr = McXmlSerializer.GetObject<AttributeCollection>((string)entity[WorkflowInstanceEntity.FieldXSParameters]);

				if (attr != null)
				{
					return attr.GetValue(key);
				}
			}

			return AssignmentOverdueAction.NoAction;
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public static void SetValue(EntityObject entity, string key, object value)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (key == null)
				throw new ArgumentNullException("key");

			if (!entity.Properties.Contains(WorkflowInstanceEntity.FieldXSParameters))
				throw new ArgumentException("Couldn't find XSParameters property", "entity");

			AttributeCollection attr = McXmlSerializer.GetObject<AttributeCollection>((string)entity[WorkflowInstanceEntity.FieldXSParameters]);

			if (attr == null)
				attr = new AttributeCollection();

			attr.Set(key, value);

			entity[WorkflowInstanceEntity.FieldXSParameters] = McXmlSerializer.GetString<AttributeCollection>(attr);
		} 
		#endregion

		#region AssignmentOverdueAction
		/// <summary>
		/// Gets the assignment overdue action.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static AssignmentOverdueAction GetAssignmentOverdueAction(EntityObject entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			int? retVal = GetValue(entity, AssignmentOverdueActionName) as int?;

			if (!retVal.HasValue)
				return AssignmentOverdueAction.NoAction;

			return (AssignmentOverdueAction)retVal.Value;
		}

		/// <summary>
		/// Sets the assignment overdue action.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="value">The value.</param>
		public static void SetAssignmentOverdueAction(EntityObject entity, AssignmentOverdueAction value)
		{
			SetValue(entity, AssignmentOverdueActionName, (int)value);
		} 
		#endregion

		#region AutoCompleteComment
		/// <summary>
		/// Gets the auto complete comment.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static string GetAutoCompleteComment(EntityObject entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			string retVal = GetValue(entity, AssignmentAutoCompleteCommentName) as string;

			if (retVal == null)
				return string.Empty;

			return retVal;
		}

		/// <summary>
		/// Sets the auto complete comment.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="value">The value.</param>
		public static void SetAutoCompleteComment(EntityObject entity, string value)
		{
			SetValue(entity, AssignmentAutoCompleteCommentName, value);
		} 
		#endregion

		#region SetOwnerReadOnly
		/// <summary>
		/// Gets the auto complete comment.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static bool GetOwnerReadOnly(EntityObject entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			bool? retVal = GetValue(entity, OwnerReadOnly) as bool?;

			if (!retVal.HasValue)
				return false;

			return retVal.Value;
		}

		/// <summary>
		/// Sets the auto complete comment.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="value">The value.</param>
		public static void SetOwnerReadOnly(EntityObject entity, bool value)
		{
			SetValue(entity, OwnerReadOnly, value);
		} 
		#endregion
	}
}
