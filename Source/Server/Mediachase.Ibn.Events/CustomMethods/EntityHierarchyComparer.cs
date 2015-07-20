using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.CustomMethods
{
	/// <summary>
	/// Позволяет сравнинивать EntityObjectHierarhy по определенным полям
	/// </summary>
	class EntityHierarchyComparer
	{
		private Dictionary<string, string[]> _entityUsedPropMap;
		private EntityObjectHierarchy _entityHierarchy;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityHierarchyComparer"/> class.
		/// </summary>
		/// <param name="entityHierarchy">The entity hierarchy.</param>
		/// <param name="propMap">The prop map.</param>
		public EntityHierarchyComparer(EntityObjectHierarchy entityHierarchy, Dictionary<string, string[]> propMap)
		{
			_entityHierarchy = entityHierarchy;
			_entityUsedPropMap = propMap;
		}


		/// <summary>
		/// Compares the hierarchy.
		/// </summary>
		/// <param name="otherHierarchy">The other hierarchy.</param>
		/// <returns></returns>
		public bool CompareHierarchy(EntityObjectHierarchy otherHierarchy)
		{
			bool retVal = true;
			EntityComparer entityComp = CreateEntityComparer(_entityHierarchy.InnerEntity);
			
			if (entityComp == null || otherHierarchy.InnerEntity.MetaClassName != _entityHierarchy.InnerEntity.MetaClassName || 
				!entityComp.CompareEntity(otherHierarchy.InnerEntity))
			{
				retVal = false;
			}
			else
			{
				foreach (EntityObjectHierarchy childEntity in _entityHierarchy.Childrens)
				{
					if (childEntity.InnerEntity.MetaClassName != _entityHierarchy.InnerEntity.MetaClassName 
						&& IsRegisteredEntity(childEntity.InnerEntity))
					{
						EntityHierarchyComparer entityHierarchyComp = new EntityHierarchyComparer(childEntity, _entityUsedPropMap);
						if (otherHierarchy.Childrens.Find(entityHierarchyComp.CompareHierarchy) == null)
						{
							retVal = false;
							break;
						}
					}
				}
			}

			return retVal;
		}

		private bool IsRegisteredEntity(EntityObject entity)
		{
			return _entityUsedPropMap.ContainsKey(entity.MetaClassName);
		}

		private EntityComparer CreateEntityComparer(EntityObject entity)
		{
			EntityComparer retVal = null;
			string[] usedProp;
			if (_entityUsedPropMap.TryGetValue(entity.MetaClassName, out usedProp))
			{
				retVal = new EntityComparer(entity, usedProp);
			}

			return retVal;
		}

	}
}
