using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for SpreadSheetTemplate.
	/// </summary>
	public class SpreadSheetTemplate
	{
		ArrayList	_rows = new ArrayList();

		List<string>	_deletedUserRows = new List<string>();

		public SpreadSheetTemplate()
		{
		}

		/// <summary>
		/// Gets the rows.
		/// </summary>
		/// <value>The rows.</value>
		public ArrayList Rows
		{
			get 
			{
				return _rows;
			}
		}

		/// <summary>
		/// Loads the specified file by name.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		public void Load(string FileName)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(FileName);

			XmlNode templateNode = xmlDoc.SelectSingleNode("SpreadSheet/Template");
			this.LoadFromXmlNode(templateNode);
		}

		/// <summary>
		/// Loads the XML string.
		/// </summary>
		/// <param name="Xml">The XML string.</param>
		public void LoadXml(string Xml)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(Xml);

			XmlNode templateNode = xmlDoc.SelectSingleNode("SpreadSheet/Template");
			this.LoadFromXmlNode(templateNode);
		}

		/// <summary>
		/// Loads from XML.
		/// </summary>
		/// <param name="templateNode">The template node.</param>
		public void LoadFromXmlNode(XmlNode templateNode)
		{
			XmlNode rowsNode = templateNode.SelectSingleNode("Rows");
			if(rowsNode!=null)
				LoadRowsFromXmlNode(rowsNode, this._rows);
		}

		
		/// <summary>
		/// Loads the rows from XML.
		/// </summary>
		/// <param name="rowsNode">The rows node.</param>
		protected static void LoadRowsFromXmlNode(XmlNode rowsNode, ArrayList rows)
		{
			foreach(XmlNode rowNode in rowsNode.ChildNodes)
			{
				switch(rowNode.Name)
				{	
					case "Row":
						XmlElement rowElement = (XmlElement)rowNode;

						string RowName = rowElement.GetAttribute("Name");
						string RowId = rowElement.GetAttribute("Id");
						string RowReadOnly = rowElement.GetAttribute("ReadOnly");
						string RowExpression = rowElement.GetAttribute("Expression");
						string RowFormat = rowElement.GetAttribute("Format");

						string RowVisibility = rowElement.GetAttribute("Visibility");

						RowReadOnly = RowReadOnly.Trim().ToUpper();

						if(FindRowById(rows, RowId)==null)
						{
							Row newRow = new Row(RowId, 
								RowName, 
								(RowReadOnly=="1" || RowReadOnly=="TRUE"),
								RowExpression, 
								RowFormat);

							if(RowVisibility!=string.Empty)
								newRow.Visibility = (RowVisibility)Enum.Parse(typeof(RowVisibility), RowVisibility, true);

							rows.Add(newRow);
						}
						break;
					case "Block":
						XmlElement blockElement = (XmlElement)rowNode;

						string BlockName = blockElement.GetAttribute("Name");
						string BlockId = blockElement.GetAttribute("Id");
						string BlockCanAddRow = blockElement.GetAttribute("CanAddRow");
						string BlockReadOnly = blockElement.GetAttribute("ReadOnly");
						string BlockExpression = blockElement.GetAttribute("Expression");
						string BlockFormat = blockElement.GetAttribute("Format");
						string BlockNewRowDefaultName = blockElement.GetAttribute("NewRowDefaultName");

						BlockCanAddRow = BlockCanAddRow.Trim().ToUpper();
						BlockReadOnly = BlockReadOnly.Trim().ToUpper();

						Block currentBlock = (Block)FindRowById(rows, BlockId);

						if(currentBlock==null)
						{
							currentBlock = new Block(BlockId, 
								BlockName, 
								(BlockCanAddRow=="1" || BlockCanAddRow=="TRUE"),
								(BlockReadOnly=="1" || BlockReadOnly=="TRUE"),
								BlockExpression,
								BlockFormat);

							currentBlock.NewRowDefaultName = BlockNewRowDefaultName;

							rows.Add(currentBlock);
						}

						LoadRowsFromXmlNode(rowNode, currentBlock.ChildRows);
						break;
				}
			}
		}

		/// <summary>
		/// Finds the row by id.
		/// </summary>
		/// <param name="rows">The rows.</param>
		/// <param name="RowId">The row id.</param>
		/// <returns></returns>
		protected static Row FindRowById(ArrayList rows, string RowId)
		{
			foreach(Row row in rows)
			{
				if(string.Compare(row.Id, RowId, true)==0)
					return row;
			}
			return null;
		}

		/// <summary>
		/// Gets the user row XML.
		/// </summary>
		/// <returns></returns>
		public string GetUserRowXml()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<SpreadSheet><Template></Template></SpreadSheet>");

			XmlNode templateNode = xmlDoc.SelectSingleNode("SpreadSheet/Template");

			XmlNode rowsNode = xmlDoc.CreateElement("Rows");

			if(SaveUserRows(rowsNode ,_rows))
			{
				templateNode.AppendChild(rowsNode);

				return xmlDoc.InnerXml;
			}

			return string.Empty;
		}

		/// <summary>
		/// Deletes the user row.
		/// </summary>
		/// <param name="RowId">The row id.</param>
		internal void DeleteUserRow(string RowId)
		{
			_deletedUserRows.Add(RowId);
		}

		/// <summary>
		/// Gets the deleted user rows.
		/// </summary>
		/// <value>The deleted user rows.</value>
		internal List<string> DeletedUserRows
		{
			get 
			{
				return _deletedUserRows;
			}
		}

		/// <summary>
		/// Saves the user rows.
		/// </summary>
		/// <param name="parentNode">The parent node.</param>
		/// <param name="list">The list.</param>
		/// <returns></returns>
		private bool SaveUserRows(XmlNode parentNode, IEnumerable list)
		{
			bool bRetVal = false;

			foreach(Row row in list)
			{
				if(row is Block)
				{
					XmlElement blockElement = parentNode.OwnerDocument.CreateElement("Block");
					blockElement.SetAttribute("Id", row.Id);

					if(SaveUserRows(blockElement, ((Block)row).ChildRows))
					{
						parentNode.AppendChild(blockElement);
						bRetVal = true;
					}
				}
				else
				if(row.Visibility == RowVisibility.User)
				{
					XmlElement rowElement = parentNode.OwnerDocument.CreateElement("Row");

					rowElement.SetAttribute("Id", row.Id);
					rowElement.SetAttribute("Name", row.Name);
					rowElement.SetAttribute("Visibility", row.Visibility.ToString());
					rowElement.SetAttribute("Expression", row.Expression);
					rowElement.SetAttribute("Format", row.Format);
					rowElement.SetAttribute("ReadOnly", row.ReadOnly.ToString());

					parentNode.AppendChild(rowElement);

					bRetVal = true;
				}
			}

			return bRetVal; 
		}
	}
}
