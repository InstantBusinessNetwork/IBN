using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Sync.Core.Common
{

	public interface IFactoryMethod<T>
	{
		T Create(object obj);
	}


	public interface IAbstractFactory
	{
		T Create<T>(object obj);

		bool IsProduct<T>();
	}


	public abstract class AbstractFactory : IAbstractFactory
	{
		#region IAbstractFactory Members

		public T Create<T>(object obj)
		{
			IFactoryMethod<T> method = this as IFactoryMethod<T>;
			if (method != null)
			{
				return method.Create(obj);
			}

			throw new NotImplementedException(String.Format("Factory method for type {0} not implemented.", typeof(T).ToString()));
		}

		public bool IsProduct<T>()
		{
			return this is IFactoryMethod<T>;
		}

		#endregion
	}
}
