using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Lists.Mapping
{
    public delegate void MappingErrorEventHandler(MappingEngine sender, MappingEngineErrorEventArgs e);

    public class MappingEngine
    {

        private List<MappingError> _errorInfoList = new List<MappingError>();
        private int _errResolveAtteptNum = 0;
        private int _errResolveAtteptMax = 1;
 
        public event MappingErrorEventHandler Error;

        /// <summary>
        /// Raises the error event.
        /// </summary>
        protected void RaiseErrorEvent(MappingEngineErrorEventArgs args)
        {
            MappingErrorEventHandler dummy = Error;
            if(dummy != null)
            {
                Error(this, args);
            }
        }
     
	
        /// <summary>
        /// Gets the get last error.
        /// </summary>
        /// <value>The get last error.</value>
        public List<MappingError> Errors
        {
            get {  return _errorInfoList;    }
        }
	
        /// <summary>
        /// Processes the mapping.
        /// </summary>
        /// <param name="xmlMapRules">The XML map rules.</param>
        /// <param name="mapDataSet">The map data set.</param>
        public void ProcessMapping(MappingDocument mappingDoc, DataSet dataSet)
        {
            List<MappedObject> mappedObjectList = new List<MappedObject>();
         
            //Process retrieve mapping values and fill meta creation structure
            foreach (MappingElement mapElColl in mappingDoc)
            {
                MappedObject mappedObject = null;

                //try
                {
                   mappedObject = PrepareMappingSet(mapElColl, dataSet, mappedObjectList);

                }
                //catch (Exception e)
                {
                    //MappingError errorInfo = new MappingError(MappingErrorType.MappingDeclarationError,
                    //                      e.Message);

                    //MappingEngineErrorEventArgs args = new MappingEngineErrorEventArgs(errorInfo);
                    //args.MappingElement = mapElColl;
                    //args.Exception = e;
                  
                    //RaiseErrorEvent(args);

                    //if ((args.ResolveError) && (_errResolveAtteptNum < _errResolveAtteptMax))
                    //{
                    //    _errResolveAtteptNum++;
                    //    //Try resolve error
                    //    mappedObject = PrepareMappingSet(args.MappingElement, dataSet, 
                    //                                     mappedObjectList);
                    //}
                    //else
                    //{
                    //  _errorInfoList.Add(errorInfo);
                    //   throw new MappingException(e.Message);
                    // }
                    //throw;
                }
             
                //Creation meta object from mapping data
               if (mappedObject != null)
               {
                   using (TransactionScope tran = DataContext.Current.BeginTransaction())
                   {
                       //Process create meta object from mapping meta structure
                       CreateMetaObjects(mappedObject, dataSet);
                       tran.Commit();
                   }

                   mappedObjectList.Add(mappedObject);
               }
            }
            
        }


        /// <summary>
        /// Prepares the mapping set.
        /// </summary>
        /// <param name="mapElColl">The map el coll.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="mappedObjectList">The mapped object list.</param>
        /// <returns></returns>
        private MappedObject PrepareMappingSet(MappingElement mapElColl, DataSet dataSet,
                                                List<MappedObject> mappedObjectList)
        {
            DataTable dataTable = null;

            dataTable = dataSet.Tables[mapElColl.TableName];
            //Not found dataTable specified in MappingElement Table attribute
            if (dataTable == null)
               throw new ObjectNotFoundException("Not found data table described in mapping declaration");
            
            return PrepareMappingSet(mapElColl, dataTable, mappedObjectList);
        }

        /// <summary>
        /// Prepares the mapping set.
        /// </summary>
        /// <param name="mapElColl">The map el coll.</param>
        /// <param name="dataSet">The data set.</param>
        /// <returns></returns>
        private MappedObject PrepareMappingSet(MappingElement mapElColl, 
                                               DataTable dataTable, List<MappedObject> mappedObjectList)
        {
            DataColumn dataCol = null;
            MetaClass metaType = DataContext.Current.GetMetaClass(mapElColl.ClassName);
            MappedObject retVal = new MappedObject(mapElColl.ClassName, mapElColl);

            //Process each row
            foreach (DataRow dataRow in dataTable.Rows)
            {
                MetaObjectData moData = new MetaObjectData();

                //Always add primary key in MappingData
                if (!String.IsNullOrEmpty(mapElColl.PrimaryKeyName))
                {
                    moData.MapColumnData.Add(mapElColl.PrimaryKeyName,
                                             dataRow[mapElColl.PrimaryKeyName]);
                }
                //Prepare mapping values
                foreach (MappingRule mapEl in mapElColl)
                {
                    object value = null;

                    try
                    {
                        if (String.IsNullOrEmpty(mapEl.ColumnName))
                            continue;

                        dataCol = dataTable.Columns[mapEl.ColumnName];

                        //try find record in mapping definition document
                        BaseSourceValue valueGenerator =
                                        BaseSourceValue.CreateValueSourceInstance(mapEl);

                        value = valueGenerator.GetValue(dataRow, dataCol,
                                                               mappedObjectList);

                        value = Value2MetaPropertyType(value, metaType.Fields[mapEl.FieldName]);
                      
                    }
                    catch (Exception e)
                    {
                        MappingError errorInfo = new MappingError(MappingErrorType.SourceValueError,
                                                       e.Message);
                        errorInfo.Row = dataRow;
                        errorInfo.Column = dataCol;
                        errorInfo.Table = dataTable;
                        errorInfo.Exception = e;
                     
                        MappingEngineErrorEventArgs args = new MappingEngineErrorEventArgs(errorInfo);
                        args.MappingRule = mapEl;
                        args.MappingElement = mapElColl;
                        
                        RaiseErrorEvent(args);

                        if ((args.ResolveError) && (_errResolveAtteptNum < _errResolveAtteptMax))
                        {
                            _errResolveAtteptNum++;
                            //Try resolve error
                            return PrepareMappingSet(args.MappingElement, args.Error.Table, 
                                                     mappedObjectList);
                        }
                        else
                        {
                            _errorInfoList.Add(errorInfo);
                            continue;
                        }
                        
                    }

                    if (!moData.MapMetaFieldData.ContainsKey(mapEl.FieldName))
                    {
                        //Save mapped value
                        moData.MapMetaFieldData.Add(mapEl.FieldName, value);
                    }
                    if (!moData.MapColumnData.ContainsKey(dataCol.ColumnName))
                    {
                        //save previous row column value
                        moData.MapColumnData.Add(dataCol.ColumnName, dataRow[dataCol]);
                    }
                    //reset error resolve attempt
                    _errResolveAtteptNum = 0;

                }

                retVal.MetaObjets.Add(moData);
            }

            return retVal;
   
        }

        /// <summary>
        /// Value2s the type of the meta property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="mf">The mf.</param>
        /// <returns></returns>
        private object Value2MetaPropertyType(object value, MetaField mf)
        {
                return Type2McDataType.ConvertType2McDataType(mf.GetMetaType().McDataType,
                                                              value);
         }
        /// <summary>
        /// Creates the meta objects.
        /// </summary>
        /// <param name="mcData">The mc data.</param>
        private void CreateMetaObjects(MappedObject mcData, DataSet dataSet)
        {

            MetaClass metaType = DataContext.Current.GetMetaClass(mcData.ClassName);
            MetaObject mo = null;
            DataTable table = null;
            DataRow row = null;
            int rowIndex = 0;
           
            if (metaType == null)
                throw new ObjectNotFoundException("Not found");

            table = dataSet.Tables[mcData.MapElColl.TableName];

            //Create meta object from mapping data
            foreach (MetaObjectData moData in mcData.MetaObjets)
            {
                row = table.Rows[rowIndex++];

                try
                {
                    mo = MetaObjectActivator.CreateInstance(metaType);

                    foreach (KeyValuePair<String, object> pair in moData.MapMetaFieldData)
                    {
                        MetaField metaField = metaType.Fields[pair.Key];
                        mo.Properties[pair.Key].Value = pair.Value;
                    }
                    //Save changes
                    mo.Save();
                }
                catch (Exception e)
                {
                    MappingError errorInfo = new MappingError(MappingErrorType.MetaObjectCreation,
                                                          e.Message);
                    errorInfo.Exception = e;
                    errorInfo.Table = table;
                    errorInfo.Row = row;
                 

                    MappingEngineErrorEventArgs args = new MappingEngineErrorEventArgs(errorInfo);
                    args.MappingElement = mcData.MapElColl;
                    args.MetaObject = mo;
                    
                    RaiseErrorEvent(args);

                    if (args.ResolveError)
                    {
                        //Try resolve error
                        try
                        {
                            args.MetaObject.Save();
                        }
                        catch (System.Exception)
                        {
                            _errorInfoList.Add(errorInfo);
                            continue;
                        }

                    }
                    else
                    {
                        _errorInfoList.Add(errorInfo);
                        continue;
                    }
                }

                moData.PrimaryKey = mo.PrimaryKeyId.Value;
            }
            
        }
 

    }
}
