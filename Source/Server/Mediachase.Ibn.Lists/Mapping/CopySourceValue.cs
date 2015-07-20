using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Data;

namespace Mediachase.Ibn.Lists.Mapping
{
    internal class CopySourceValue : BaseSourceValue
    {


        /// <summary>
        /// Initializes the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        public override void Initialize(NameValueCollection param)
        {
            //Nothig to do
        }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="mappedObjectList">The mapped object list.</param>
        /// <returns></returns>
        public override object GetValue(DataRow row, DataColumn column, 
                                        List<MappedObject> mappedObjectList)
        {
            return row[column];
        }
    }
}
