using System;
using System.IO;
using System.Collections;
using System.Xml;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for SpreadSheetTemplateInfo.
	/// </summary>
	public class SpreadSheetTemplateInfo
	{
		private string _fileName;
		private string _name = string.Empty;
		private string _description = string.Empty;

		public SpreadSheetTemplateInfo(string FileName, string Name, string Description)
		{
			_fileName = FileName;
			_name = Name;
			_description = Description;
		}


		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
		}

		public static SpreadSheetTemplateInfo[] List()
		{
			return List(ProjectSpreadSheet.TemplateDirectory);
		}

		public static SpreadSheetTemplateInfo[] List(string SpreadSheetTemplateFolder)
		{
			ArrayList retVal = new ArrayList();

			foreach(string file in Directory.GetFiles(SpreadSheetTemplateFolder,"*.xml"))
			{
				string fn = Path.GetFileName(file);
				string n = string.Empty;
				string d = string.Empty;

				try
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.Load(file);

					XmlNode nameNode = xmlDoc.SelectSingleNode("SpreadSheet/Template/Name");
					if(nameNode!=null)
						n = nameNode.InnerText;

					XmlNode descNode = xmlDoc.SelectSingleNode("SpreadSheet/Template/Description");
					if(descNode!=null)
						d = descNode.InnerText;

					retVal.Add(new SpreadSheetTemplateInfo(fn, n, d));
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex);
				}
			}

			return (SpreadSheetTemplateInfo[])retVal.ToArray(typeof(SpreadSheetTemplateInfo));
		}
	}
}
