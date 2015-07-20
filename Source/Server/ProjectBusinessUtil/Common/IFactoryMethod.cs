using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Common
{
    public interface IFactoryMethod<T>
    {
        T Create(object obj);
    }
}
