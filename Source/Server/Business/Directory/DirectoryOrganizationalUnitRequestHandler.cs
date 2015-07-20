using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.Ibn.Business.Directory
{
	/// <summary>
	/// Represents DirectoryOrganizationalUnit request handler.
	/// </summary>
	public class DirectoryOrganizationalUnitRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryOrganizationalUnitRequestHandler"/> class.
		/// </summary>
		public DirectoryOrganizationalUnitRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == DirectoryOrganizationalUnitEntity.ClassName)
			{
				DirectoryOrganizationalUnitEntity retVal = new DirectoryOrganizationalUnitEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region Create
		protected override void PreCreate(BusinessContext context)
		{
			base.PreCreate(context);

			// Check ParentId should be not null
			DirectoryOrganizationalUnitEntity target = ((DirectoryOrganizationalUnitEntity)context.Request.Target);

			if (target.ParentId == null)
				throw new ArgumentNullException("target.ParentId");
		}

		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);

			// Update Icon from parent
			DirectoryOrganizationalUnitEntity target = ((DirectoryOrganizationalUnitEntity)context.Request.Target);

			if (target.Icon == null)
			{
				// Load Icon From Parent
				DirectoryOrganizationalUnitEntity parent = (DirectoryOrganizationalUnitEntity)BusinessManager.Load(DirectoryOrganizationalUnitEntity.ClassName,target.ParentId.Value);

				target.Icon = new FileInfo(parent.Icon.Name, parent.Icon.OpenRead());
			}
		}

		protected override void Create(BusinessContext context)
		{
			base.Create(context);

			// Append to tree
			DirectoryOrganizationalUnitEntity target = ((DirectoryOrganizationalUnitEntity)context.Request.Target);

			BusinessObject newElement = (BusinessObject)context.Items[MetaObjectRequestHandler.SourceMetaObjectKey];
			BusinessObject parent = (BusinessObject)MetaObjectActivator.CreateInstance(DirectoryOrganizationalUnitEntity.ClassName, 
				target.ParentId.Value);

			TreeNode node = parent.GetService<TreeService>().AppendChild(newElement);
			parent.Save();
		}

		protected override void PostCreateInsideTransaction(BusinessContext context)
		{
			base.PostCreateInsideTransaction(context);

			// Add BusinessUnit To Principal
			DirectoryManager.CreatePrincipal(DirectoryPrincipalType.OrganizationalUnit,
				((CreateResponse)context.Response).PrimaryKeyId, 
				((DirectoryOrganizationalUnitEntity)context.Request.Target).Name);

			// Add BusinessUnitScope To Principal
			DirectoryManager.CreatePrincipal(DirectoryPrincipalType.OrganizationalUnitScope,
				(PrimaryKeyId)((DirectoryOrganizationalUnitEntity)context.Request.Target).OrganizationalUnitScopeId,
				((DirectoryOrganizationalUnitEntity)context.Request.Target).Name);
		}

		#endregion

		#region Delete
		protected override void PreDelete(BusinessContext context)
		{
			base.PreDelete(context);

		}

		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			DirectoryOrganizationalUnitEntity entity = (DirectoryOrganizationalUnitEntity)BusinessManager.Load(DirectoryOrganizationalUnitEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			if (entity.ParentId == null)
				throw new AccessDeniedException("Can not delete root.");

			// TODO: Child Business Unit

			// TODO: Delete Users

			// TODO: Delete Team

			// TODO: Delete Role

			// Remove BusinessUnit From Principal
			DirectoryManager.DeletePrincipal(context.GetTargetPrimaryKeyId().Value);

			// Remove BusinessUnitScopeId From Principal
			DirectoryManager.DeletePrincipal((PrimaryKeyId)entity.OrganizationalUnitScopeId);
		}
		#endregion

		#region Update
		protected override void PostUpdateInsideTransaction(BusinessContext context)
		{
			base.PostUpdateInsideTransaction(context);

			// Principal
			if (context.Request.Target.Properties.Contains(DirectoryOrganizationalUnitEntity.FieldName))
			{
				DirectoryOrganizationalUnitEntity entity = (DirectoryOrganizationalUnitEntity)BusinessManager.Load(DirectoryOrganizationalUnitEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

				DirectoryManager.UpdatePrincipal(context.GetTargetPrimaryKeyId().Value,
					(string)context.Request.Target[DirectoryOrganizationalUnitEntity.FieldName]);

				DirectoryManager.UpdatePrincipal((PrimaryKeyId)entity.OrganizationalUnitScopeId,
					(string)context.Request.Target[DirectoryOrganizationalUnitEntity.FieldName]);
			}
		}
		#endregion

		#region List
		protected virtual void SortEntityObjectTreeByName(List<EntityObject> items)
		{
			OrganizationalUnitTreeComparer comparer = new OrganizationalUnitTreeComparer(items);
			items.Sort(comparer);
		}

		protected override void List(BusinessContext context)
		{
			base.List(context);

			ListRequest request = (ListRequest)context.Request;
			ListResponse response = (ListResponse)context.Response;

			// Check if sorting contains OutlineNumber
			bool bSortByName = request.Sorting.Length == 1 && 
				request.Sorting[0].Source == DirectoryOrganizationalUnitEntity.FieldOutlineNumber;

			List<EntityObject> items = new List<EntityObject>(response.EntityObjects);

			// Load Name From GlobalResource
			foreach (DirectoryOrganizationalUnitEntity entity in items)
			{
				entity.Name = GlobalResource.GetString(entity.Name);
			}

			if (bSortByName)
			{
				SortEntityObjectTreeByName(items);
			}

			response.EntityObjects = items.ToArray();

		}
		#endregion

		#region Custom Method
		protected override void PreCustomMethod(BusinessContext context)
		{
			switch (context.GetMethod())
			{
				case DirectoryOrganizationalUnitMethod.Move:
					PreMove(context);
					break;
				default:
					base.PreCustomMethod(context);
					break;
			}
		}

		/// <summary>
		/// Pres the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCustomMethodInsideTransaction(BusinessContext context)
		{
			switch (context.GetMethod())
			{
				case DirectoryOrganizationalUnitMethod.Move:
					PreMoveInsideTransaction(context);
					break;
				default:
					base.PreCustomMethodInsideTransaction(context);
					break;
			}
		}

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void CustomMethod(BusinessContext context)
		{
			switch (context.GetMethod())
			{
				case DirectoryOrganizationalUnitMethod.Move:
					Move(context);
					break;
				default:
					base.CustomMethod(context);
					break;
			}
		}

		/// <summary>
		/// Posts the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethodInsideTransaction(BusinessContext context)
		{
			switch (context.GetMethod())
			{
				case DirectoryOrganizationalUnitMethod.Move:
					PostMoveInsideTransaction(context);
					break;
				default:
					base.PostCustomMethodInsideTransaction(context);
					break;
			}
		}

		/// <summary>
		/// Posts the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethod(BusinessContext context)
		{
			switch (context.GetMethod())
			{
				case DirectoryOrganizationalUnitMethod.Move:
					PostMove(context);
					break;
				default:
					base.PostCustomMethod(context);
					break;
			}
		}
		#endregion

		#region Move Method
		/// <summary>
		/// Pres the move method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PreMove(BusinessContext context)
		{
			// TODO: Check You can not Move to Child element

			
		}

		/// <summary>
		/// Pres the move method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PreMoveInsideTransaction(BusinessContext context)
		{
		}

		/// <summary>
		/// Moves the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void Move(BusinessContext context)
		{
			MoveTreeNodeRequest request = (MoveTreeNodeRequest)context.Request;

			if (request.Target.PrimaryKeyId.Value != request.NewParent)
			{
				// Call SP mc_DirectoryOrganizationalUnit_Move
				SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure,
					"mc_DirectoryOrganizationalUnit_Move",
					SqlHelper.SqlParameter("@NodeId", System.Data.SqlDbType.UniqueIdentifier, (Guid)request.Target.PrimaryKeyId.Value),
					SqlHelper.SqlParameter("@NewParentId", System.Data.SqlDbType.UniqueIdentifier, (Guid)request.NewParent));

				// TODO: Rebuild USER_GROUP 
			}
		}

		/// <summary>
		/// Posts the move method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PostMoveInsideTransaction(BusinessContext context)
		{
		}

		/// <summary>
		/// Posts the move method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PostMove(BusinessContext context)
		{
		}
		#endregion


		#endregion
	}
}
