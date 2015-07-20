using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Ibn.Lists.Mapping;
using System.Data;
using System.IO;
using System.Xml;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Service;
using System.Configuration;
using System.Data.OleDb;
using System.Collections;
using System.Diagnostics;


namespace MappingTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MappingTest
    {
        public MappingTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
         [TestInitialize()]
         public void MyClassInitialize() 
         { 
             string _connectionString = "Data source=S2;Initial catalog=TaskPrototype;User ID=dev;Password=";
             DataContext.Current = new DataContext(_connectionString);
        
         }
        
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

    //    TableName='bookstore'.
    //      Columns count=1
    //      ColumnName='bookstore_Id', type = System.Int32
    //    TableName='book'.
    //      Columns count=5
    //      ColumnName='title', type = System.String
    //      ColumnName='price', type = System.Decimal
    //      ColumnName='genre', type = System.String
    //      ColumnName='book_Id', type = System.Int32
    //      ColumnName='bookstore_Id', type = System.Int32

        [TestMethod]
        [DeploymentItem("MappingTest\\DataSet.xsd")]
        [DeploymentItem("MappingTest\\DataSet.xml")]
        [DeploymentItem("MappingTest\\BookMappingError.xml")]
        public void MappingErrorTest()
        {
            DataSet dataSet = LoadDataSetfromXML("DataSet.xsd", "DataSet.xml");

            MappingEngine mapEngine = new MappingEngine();

            string mapFileName = @"BookMappingError.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(mapFileName);

            MappingDocument mapDoc = MappingDocument.LoadFromXml(xmlDoc.OuterXml);

             //mapEngine.MappingErrorEvent += ErrorHandler;
             mapEngine.ProcessMapping(mapDoc, dataSet);	

            
       

            
        }

        private void SouceValueErrorResolver(MappingEngineErrorEventArgs args)
        {
            args.ResolveError = true;
            args.MappingElement[0].RelationTo = "bookstore";

        }

        private void MetaObjectCreationErrorResolver(MappingEngineErrorEventArgs args)
        {
            args.ResolveError = true;
        }
        
        private void ErrorHandler(object sender, MappingEngineErrorEventArgs args)
        {
            MappingError error = args.Error;
            if (error.ErrorType == MappingErrorType.SourceValueError)
                SouceValueErrorResolver(args);
            else if (error.ErrorType == MappingErrorType.MetaObjectCreation)
                MetaObjectCreationErrorResolver(args);
            else 
                Trace.WriteLine(error.ErrorDescr);
        }

       
        

        [TestMethod]
        [DeploymentItem("MappingTest\\DataSet.xsd")]
        [DeploymentItem("MappingTest\\DataSet.xml")]
        [DeploymentItem("MappingTest\\BookMapping.xml")]
        public void MappingDataTest()
        {
            DataSet dataSet = LoadDataSetfromXML("DataSet.xsd", "DataSet.xml");

            MappingEngine mapEngine = new MappingEngine();

            string mapFileName = @"BookMapping.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(mapFileName);

            MappingDocument mapDoc = MappingDocument.LoadFromXml(xmlDoc.OuterXml);

            mapEngine.ProcessMapping(mapDoc, dataSet);

        }

    
        [TestMethod]
        [DeploymentItem("MappingTest\\price.xls")]
        [DeploymentItem("MappingTest\\priceMapping.xml")]
        public void ExcelMappingTest()
        {
            //IMcOleDbHelper helper = (IMcOleDbHelper)Activator.GetObject(typeof(IMcOleDbHelper),
            //                                                    ConfigurationManager.AppSettings["McOleDbServiceString"]);

            //DataSet dataSet = helper.ConvertExcelToDataSet("price.xls");
            DataSet dataSet = ConvertExcelToDataSet("price.xls");

            MappingEngine mapEngine = new MappingEngine();
            mapEngine.MappingErrorEvent += ErrorHandler;
                        
            MappingDocument mapDoc = MappingDocument.LoadFromFile("priceMapping.xml");

            mapEngine.ProcessMapping(mapDoc, dataSet);
        }

        [TestMethod]
        public void MappingDocSeriliazationTest()
        {
            
            MappingDocument mapDoc = new MappingDocument();
            MappingElement mapeElColl = new MappingElement();
            mapeElColl.ClassName = "Class1";
            mapeElColl.TableName = "Table1";
            mapeElColl.PrimaryKeyName = "Id";

            MappingRule mapEl = new MappingRule();
            mapEl.ColumnName = "column1";
            mapEl.FieldName = "field1";
            mapEl.DefaultValue = "somevalue";

            mapeElColl.Add(mapEl);

            mapDoc.Add(mapeElColl);

            string xml = MappingDocument.GetXml(mapDoc);

            MappingDocument loadedDoc = MappingDocument.LoadFromXml(xml);

            string xml2 = MappingDocument.GetXml(loadedDoc);

            if(xml != xml2)
            {
                throw new AssertFailedException();
            }

        }

        private DataSet LoadDataSetfromXML(string schema, string file)
        {
            
            DataSet retVal = new DataSet();
                           
            // Invoke the ReadXmlSchema method with the file name.
            retVal.ReadXmlSchema(schema);
                       
            retVal.ReadXml(file);


            return retVal;

        }


        private string[] GetSheetNames(OleDbConnection cnn)
        {
            ArrayList list = new ArrayList();

            DataTable tbl = new DataTable();
            tbl = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

            foreach (DataRow row in tbl.Rows)
            {
                list.Add(row["TABLE_NAME"]);
            }
            return (string[])list.ToArray(typeof(string));
        }

        private void RemoveEmptyRows(DataTable table)
        {
            for (int Index = table.Rows.Count - 1; Index >= 0; Index--)
            {
                DataRow row = table.Rows[Index];
                bool empty = true;
                for (int FieldIndex = 0; FieldIndex < table.Columns.Count; FieldIndex++)
                {
                    if (row[FieldIndex] != null && DBNull.Value != row[FieldIndex])
                    {
                        empty = false;
                        break;
                    }
                }
                if (empty)
                    row.Delete();
            }
            table.AcceptChanges();
        }

        private DataSet Parse(string FileName)
        {
            DataSet ds = new DataSet();
            //ds.ReadXmlSchema("price.xsd");

            OleDbConnection cnn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FileName + ";Extended Properties=\"Excel 8.0;HDR=Yes\"");

            cnn.Open();
            try
            {
                string[] SheetNames = GetSheetNames(cnn);
                foreach (string SheetName in SheetNames)
                {
                    /*
                    if (SheetName.EndsWith("$Print_Titles") || SheetName.EndsWith("$Print_Area")
                        || SheetName.EndsWith("$Database") || SheetName.EndsWith("$Criteria")
                        || SheetName.EndsWith("$Data_form") || SheetName.EndsWith("$Sheet_Title"))
                    {
                    }
                    else
                    */
                    if (SheetName.EndsWith("$"))
                    {
                        DataTable table = new DataTable();

                        OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + SheetName + "]", cnn);
                        //da.FillSchema(ds, SchemaType.Mapped);
                        da.Fill(table);
                       
                        RemoveEmptyRows(table);

                        if (table.Rows.Count != 0)
                            ds.Tables.Add(table);
                    }
                }
            }
            finally
            {
                cnn.Close();
            }
            return ds;
        }

    
        private DataSet ConvertExcelToDataSet(string FileName)
        {
            return Parse(FileName);
        }
    

    }
}
