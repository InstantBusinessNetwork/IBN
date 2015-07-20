using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Functor
{
    public interface IHasValue<T>
    {

       T Value { get; set;}

    }
}
