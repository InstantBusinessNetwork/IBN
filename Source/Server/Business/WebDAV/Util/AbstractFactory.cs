using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV.Util
{
    public abstract class AbstractFactory : IAbstractFactory
    {
        #region IAbstractFactory Members

        public T Create<T>(object obj)
        {
            IFactoryMethod<T> method = this as IFactoryMethod<T>;
            if(method != null)
            {
                return method.Create(obj);
            }

            throw new NotImplementedException(String.Format("Factory method for type {0} not implemented.", typeof(T).ToString()));
        }

        public T Create<T>(object obj1, object obj2)
        {
            IFactoryMethod<T> method = this as IFactoryMethod<T>;
            if (method != null)
            {
                return method.Create(obj1, obj2);
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
