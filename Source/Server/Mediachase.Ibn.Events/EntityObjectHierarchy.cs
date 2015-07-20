using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events
{
	[Serializable]
	public class EntityObjectHierarchy 
	{
		private List<EntityObjectHierarchy> _children;
		private EntityObject _entity;


		public EntityObjectHierarchy()
		{
			_children = new List<EntityObjectHierarchy>();
		}


		public EntityObjectHierarchy(EntityObject entity)
			: this()
		{
			_entity = entity;
		}

		public EntityObjectHierarchy(EntityObjectHierarchy entiyHierarchy)
			: this()
		{
			_entity = entiyHierarchy.InnerEntity;
			_children = entiyHierarchy._children;
		}

		public EntityObject InnerEntity
		{
			get
			{
				return _entity;
			}
		}

		public List<EntityObjectHierarchy> Childrens
		{
			get
			{
				return _children;
			}
		}
		
	}
}
