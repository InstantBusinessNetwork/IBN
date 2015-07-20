using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV.Util
{
    public interface IFactoryMethod<T>
    {
        T Create(object obj);
        T Create(object obj1, object obj2);
    }
}
