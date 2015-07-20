using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV.Util
{
    public interface IAbstractFactory
    {
        T Create<T>(object obj);
        T Create<T>(object obj1, object obj2);

        bool IsProduct<T>();
    }
}
