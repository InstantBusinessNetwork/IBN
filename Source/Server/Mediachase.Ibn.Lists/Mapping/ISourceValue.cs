using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Data;

namespace Mediachase.Ibn.Lists.Mapping
{
    public interface ISourceValue
    {
        void Initialize(NameValueCollection param);

        object GetValue(DataRow row, DataColumn column, List<MappedObject> mappedObjectList);

    }
}
