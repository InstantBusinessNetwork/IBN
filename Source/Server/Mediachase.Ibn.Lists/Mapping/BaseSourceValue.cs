using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.Specialized;

namespace Mediachase.Ibn.Lists.Mapping
{
    internal class BaseSourceValue : ISourceValue
    {
   
        /// <summary>
        /// Creates the generator.
        /// </summary>
        /// <param name="mapEl">The map el.</param>
        /// <returns></returns>
        public static BaseSourceValue CreateValueSourceInstance(MappingRule mapEl)
        {
            NameValueCollection param = new NameValueCollection();
            BaseSourceValue retVal = new CopySourceValue();

            if(!String.IsNullOrEmpty(mapEl.DefaultValue))
            {
                param.Add("value", mapEl.DefaultValue);
                retVal = new DefaultSourceValue();
                
            }
            else if(!String.IsNullOrEmpty(mapEl.RelationTo))
            {
                param.Add("referenceTo", mapEl.RelationTo);
                retVal = new RelationSourceValue();
            }

            retVal.Initialize(param);

            return retVal;
            
        }

        #region ISourceValue Members

        public virtual void Initialize(NameValueCollection param)
        {
           
        }

        public virtual object GetValue(DataRow row, DataColumn column,
                                     List<MappedObject> mappedObjectList)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
