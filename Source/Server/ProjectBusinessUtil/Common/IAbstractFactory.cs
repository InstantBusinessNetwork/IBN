using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Common
{
    public interface IAbstractFactory
    {
        T Create<T>(object obj);

        bool IsProduct<T>();
    }
}
