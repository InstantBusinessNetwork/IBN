using System;
using System.Xml;
using System.Text;
using System.Web;
using System.Resources;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for TestSpreadSheetDocumentFactory.
	/// </summary>
	public class TestSpreadSheetDocumentFactory
	{
		
		private TestSpreadSheetDocumentFactory()
		{
		}

		

		private static SpreadSheetView _testView = null;
		public static SpreadSheetView TestView
		{
			get 
			{
				if(_testView==null)
					_testView = CreateView(CreateDocument2());
				return _testView;
			}
		}

		public static SpreadSheetDocument CreateDocument1()
		{
			SpreadSheetDocument retVal = new SpreadSheetDocument();

			retVal.Template.Rows.Add(new Row("R1","Затрата 1"));
			retVal.Template.Rows.Add(new Row("R2","Затрата 2"));
			retVal.Template.Rows.Add(new Row("R3","Затрата 3"));

			retVal.AddCell("2006-Q1-01", "R1", 1);
			retVal.AddCell("2006-Q1-02", "R1", 2);
			retVal.AddCell("2006-Q1-03", "R1", 3);

			retVal.AddCell("2006-Q2-04", "R1", 4);
			retVal.AddCell("2006-Q2-05", "R1", 5);
			retVal.AddCell("2006-Q2-06", "R1", 6);

			retVal.AddCell("2006-Q3-07", "R1", 7);
			retVal.AddCell("2006-Q3-08", "R1", 8);
			retVal.AddCell("2006-Q3-09", "R1", 9);

			retVal.AddCell("2006-Q4-10", "R1", 10);
			retVal.AddCell("2006-Q4-11", "R1", 11);
			retVal.AddCell("2006-Q4-12", "R1", 12);

			retVal.AddCell("2006-Q1-01", "R3", 1.01);
			retVal.AddCell("2006-Q1-02", "R3", 2.01);
			retVal.AddCell("2006-Q1-03", "R3", 3.01);

			retVal.AddCell("2006-Q4-10", "R3", 10.01);
			retVal.AddCell("2006-Q4-11", "R3", 11.01);
			retVal.AddCell("2006-Q4-12", "R3", 12.01);

			return retVal;
		}

		public static SpreadSheetDocument CreateDocument2()
		{
			SpreadSheetDocument retVal = new SpreadSheetDocument();

			retVal.Template.Rows.Add(new Row("R1","Затрата 1"));
			retVal.Template.Rows.Add(new Row("R2","Затрата 2"));

			Block block = new Block("B1", "Блок Затрат");

			block.ChildRows.Add(new Row("B1-1","Item 1"));
			block.ChildRows.Add(new Row("B1-2","Item 2"));
			block.ChildRows.Add(new Row("B1-3","Item 3"));

			retVal.Template.Rows.Add(block);

			retVal.Template.Rows.Add(new Row("R3","Затрата 3"));

//			retVal.AddCell("2005-Q2-06", "R1", 100);
//			retVal.AddCell("2007-Q2-06", "R1", -100);

			retVal.AddCell("2006-Q1-01", "R1", 1);
			retVal.AddCell("2006-Q1-02", "R1", 2);
			retVal.AddCell("2006-Q1-03", "R1", 3);

			retVal.AddCell("2006-Q2-04", "R1", 4);
			retVal.AddCell("2006-Q2-05", "R1", 5);
			retVal.AddCell("2006-Q2-06", "R1", 6);

			retVal.AddCell("2006-Q3-07", "R1", 7);
			retVal.AddCell("2006-Q3-08", "R1", 8);
			retVal.AddCell("2006-Q3-09", "R1", 9);

			retVal.AddCell("2006-Q4-10", "R1", 10);
			retVal.AddCell("2006-Q4-11", "R1", 11);
			retVal.AddCell("2006-Q4-12", "R1", 12);

			retVal.AddCell("2006-Q1-01", "R3", 1.01);
			retVal.AddCell("2006-Q1-02", "R3", 2.01);
			retVal.AddCell("2006-Q1-03", "R3", 3.01);

			retVal.AddCell("2006-Q4-10", "R3", 10.01);
			retVal.AddCell("2006-Q4-11", "R3", 11.01);
			retVal.AddCell("2006-Q4-12", "R3", 12.01);

			return retVal;
		}

		public static SpreadSheetView CreateView(SpreadSheetDocument doc)
		{
			SpreadSheetView view = new SpreadSheetView(doc, 2006, 2006);
			
			//
//			for (int i = 0; i <  12; i++)
//			{
//				Column col = new Column();
//				col.AllowUserValue = true;
//				col.Id = i.ToString();//String.Format("2006-Q{0}-{1}", (int)((i / 3) + 1), ((i % 12) + 1));
//				col.Name = String.Format("2006-Q{0}-{1}", (int)((i / 3) + 1), ((i % 12) + 1));
//				view.Columns.Add(col);
//			}

//
//			if (_testView == null)
//				_testView = view;

			return view;
		}



	}
}
