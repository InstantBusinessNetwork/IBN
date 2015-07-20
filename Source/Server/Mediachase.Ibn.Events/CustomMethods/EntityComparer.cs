using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using System.Diagnostics;

namespace Mediachase.Ibn.Events.CustomMethods
{
	/// <summary>
	/// Позволяет сравнивать на равенство EntityObject по определенным полям
	/// </summary>
	public class EntityComparer
	{
		private readonly string[] _usedProperties;
		private EntityObject _entity;
		public EntityComparer(EntityObject entity, string[] usedProperties)
		{
			_entity = entity;
			_usedProperties = usedProperties;
		}

		public bool CompareEntity(EntityObject otherEntity)
		{
			bool retVal = true;
			foreach (string propName in _usedProperties)
			{
				EntityObjectProperty leftProp = _entity.Properties[propName];
				EntityObjectProperty rightProp = otherEntity.Properties[propName];
				object leftValue = leftProp != null ? leftProp.Value : null;
				object rightValue = rightProp.Value != null ? rightProp.Value : null;
				if (leftValue != null && rightValue != null)
				{
					if (!leftValue.Equals(rightValue))
					{
						Trace.WriteLine(String.Format("Entity {0}[{1}:{2}] != {3}[{1}:{4}]",_entity.MetaClassName, propName, leftProp.Value, otherEntity.MetaClassName, rightProp.Value));
						retVal = false;
						break;
					}
				}
				else if (leftValue != rightValue)
				{
					Trace.WriteLine(String.Format("Entity {0}[{1}:{2}] != {3}[{1}:{4}]", _entity.MetaClassName, propName, leftProp.Value, otherEntity.MetaClassName, rightProp.Value));
					retVal = false;
					break;
				}
			}

			return retVal;
		}
	}
}
