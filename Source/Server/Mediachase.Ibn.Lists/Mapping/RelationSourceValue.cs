using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Data;

namespace Mediachase.Ibn.Lists.Mapping
{
    internal class RelationSourceValue : BaseSourceValue
    {
        NameValueCollection _genParams;


        /// <summary>
        /// Initializes the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        public override void Initialize(NameValueCollection param)
        {
            _genParams = param;
        }
       
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="mappedObjectList">The mapped object list.</param>
        /// <remarks> mappedObjectList is already created object</remarks>
        /// <returns></returns>
        public override object GetValue(DataRow row, DataColumn column,
                                        List<MappedObject> mappedObjectList)
        {
            int? retVal = null;

            String referenceTo = _genParams["referenceTo"];

            //Find referenced mapping object in already created mapped set
            MappedObject referencedObject = null;
            foreach (MappedObject mappedObj in mappedObjectList)
            {
                if (referenceTo == mappedObj.MapElColl.TableName)
                {
                    referencedObject = mappedObj;
                    break;
                }
            }

            if (referencedObject == null)
                throw new MappingException("Referenced object not found");

            String foreignColName = referencedObject.MapElColl.PrimaryKeyName;

            if (String.IsNullOrEmpty(foreignColName))
                throw new ArgumentNullException("PK column name must be set");

            foreach (MetaObjectData moData in referencedObject.MetaObjets)
            {
                if (moData.MapColumnData.ContainsKey(foreignColName))
                {
                    String foreignKey = Convert.ToString(moData.MapColumnData[foreignColName]);

                    //Find related referenced object and return his new PrimaryKeyID
                    if (foreignKey.Equals(row[column].ToString(),
                                         StringComparison.InvariantCultureIgnoreCase))
                    {
                        retVal = moData.PrimaryKey;
                        break;
                    }
                }


            }

            return retVal;

        }
    }
}
