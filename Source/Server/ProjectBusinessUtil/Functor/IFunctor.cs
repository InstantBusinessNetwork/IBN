using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Functor
{
    public interface IFunctor<T>
    {
        object Execute(T param);
    }
}
