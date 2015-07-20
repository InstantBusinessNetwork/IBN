using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Ibn.Business.Customization
{
	public class NavigationFileProvider : FileProvider
	{
		#region public override FileDescriptor[] GetFiles(string structureName, string searchPattern, Selector[] selectors)
		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="structureName">Name of the structure.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="selectors">The selectors (4st argument - profileId, 5nd argument - principalId).</param>
		/// <returns></returns>
		public override FileDescriptor[] GetFiles(string structureName, string searchPattern, Selector[] selectors)
		{
			List<FileDescriptor> files = new List<FileDescriptor>();

			if (string.Compare(structureName, "Navigation", StringComparison.OrdinalIgnoreCase) == 0
				&& string.Compare(searchPattern, "*.xml", StringComparison.OrdinalIgnoreCase) == 0)
			{
				FilterElementCollection filters;
				EntityObject[] items;
				PrimaryKeyId profileId = PrimaryKeyId.Empty;
				PrimaryKeyId principalId = PrimaryKeyId.Empty;

				// 1. Global Layer
				filters = new FilterElementCollection();
				filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldStructureType, (int)CustomizationStructureType.NavigationMenu));
				filters.Add(FilterElement.IsNullElement(CustomizationItemEntity.FieldProfileId));
				filters.Add(FilterElement.IsNullElement(CustomizationItemEntity.FieldPrincipalId));

				items = BusinessManager.List(CustomizationItemEntity.ClassName, filters.ToArray());
				ProcessItems(items, files, NavigationManager.CustomizationLayerGlobal);
				
				// 2. Profile Layer
				if (selectors != null && selectors.Length != 0 && selectors[0].Items.Count > 3 && !string.IsNullOrEmpty(selectors[0].Items[3]))
				{
					string profileSelector = selectors[0].Items[3];
					if (PrimaryKeyId.TryParse(profileSelector, out profileId))
					{
						filters = new FilterElementCollection();
						filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldStructureType, (int)CustomizationStructureType.NavigationMenu));
						filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldProfileId, profileId));
						filters.Add(FilterElement.IsNullElement(CustomizationItemEntity.FieldPrincipalId));

						items = BusinessManager.List(CustomizationItemEntity.ClassName, filters.ToArray());
						ProcessItems(items, files, NavigationManager.CustomizationLayerProfile);
					}
				}

				// 3. User Layer
				if (selectors != null && selectors.Length != 0 && selectors[0].Items.Count > 4 && !string.IsNullOrEmpty(selectors[0].Items[4]))
				{
					string principalSelector = selectors[0].Items[4];
					if (PrimaryKeyId.TryParse(principalSelector, out principalId))
					{
						filters = new FilterElementCollection();
						filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldStructureType, (int)CustomizationStructureType.NavigationMenu));
						filters.Add(FilterElement.IsNullElement(CustomizationItemEntity.FieldProfileId));
						filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldPrincipalId, principalId));

						items = BusinessManager.List(CustomizationItemEntity.ClassName, filters.ToArray());
						ProcessItems(items, files, NavigationManager.CustomizationLayerUser);
					}
				}
			}

			return files.ToArray();
		}
		#endregion

		#region ProcessItems
		/// <summary>
		/// Processes the items.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="files">The files.</param>
		private void ProcessItems(EntityObject[] items, List<FileDescriptor> files, string customizationLayer)
		{
			foreach (CustomizationItemEntity item in items)
			{
				string xml = null;
				FileDescriptor file = new FileDescriptor(this);
				file.FileNameWithoutExtension = "@";
				file.CustomizationLayer = customizationLayer;

				Dictionary<string, CustomizationItemArgumentEntity> arguments = NavigationManager.GetCustomizationItemArguments(item.PrimaryKeyId.Value);
				if (item.CommandType == (int)ItemCommandType.Add)
				{
					xml = BuildXmlForAdd(item.XmlFullId, arguments);
				}
				else if (item.CommandType == (int)ItemCommandType.Update)
				{
					xml = BuildXmlForUpdate(item.XmlFullId, arguments);
				}
				else if (item.CommandType == (int)ItemCommandType.Remove)
				{
					xml = BuildXmlForRemove(item.XmlFullId);
				}

				if (!String.IsNullOrEmpty(xml))
				{
					file.Content = new MemoryStream(Encoding.UTF8.GetBytes(xml));
					files.Add(file);
				}
			}
		}
		#endregion

		#region private static string BuildXmlForAdd(string fullId, Dictionary<string, CustomizationItemArgumentEntity> args)
		/// <summary>
		/// Builds the XML for add.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns></returns>
		private static string BuildXmlForAdd(string fullId, Dictionary<string, CustomizationItemArgumentEntity> arguments)
		{
			string result = null;

			if (!string.IsNullOrEmpty(fullId))
			{
				string[] ids = fullId.Split('/');
				if (ids.Length > 1)
				{
					StringBuilder builder = new StringBuilder();

					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Encoding = Encoding.UTF8;
					settings.OmitXmlDeclaration = true;

					using (XmlWriter writer = XmlWriter.Create(builder, settings))
					{
						writer.WriteStartElement(NavigationWriterTag.Navigation);

						// Tabs
						writer.WriteStartElement(NavigationWriterTag.Tabs);
						writer.WriteStartElement(NavigationWriterTag.Tab);
						writer.WriteAttributeString(NavigationWriterAttribute.Id, ids[0]);

						int lastIndex = ids.Length - 1;
						for (int i = 1; i < lastIndex; i++)
						{
							writer.WriteStartElement(NavigationWriterTag.Link);
							writer.WriteAttributeString(NavigationWriterAttribute.Id, ids[i]);
						}

						writer.WriteStartElement(NavigationWriterTag.Add);
						writer.WriteStartElement(NavigationWriterTag.Link);

						writer.WriteAttributeString(NavigationWriterAttribute.Id, ids[lastIndex]);
						if (arguments.ContainsKey(NavigationManager.ItemArgumentOrder))
							writer.WriteAttributeString(NavigationWriterAttribute.Order, arguments[NavigationManager.ItemArgumentOrder].Value);
						if (arguments.ContainsKey(NavigationManager.ItemArgumentText))
							writer.WriteAttributeString(NavigationWriterAttribute.Text, arguments[NavigationManager.ItemArgumentText].Value);
						if (arguments.ContainsKey(NavigationManager.ItemArgumentCommand))
							writer.WriteAttributeString(NavigationWriterAttribute.Command, arguments[NavigationManager.ItemArgumentCommand].Value);
						writer.WriteEndElement(); // Link
						writer.WriteEndElement(); // add

						for (int i = 1; i < lastIndex; i++)
						{
							writer.WriteEndElement(); // Link
						}

						writer.WriteEndElement(); // Tab
						writer.WriteEndElement(); // Tabs

						// Commands
						if (arguments.ContainsKey(NavigationManager.ItemArgumentCommand))
						{
							writer.WriteStartElement(NavigationWriterTag.Commands);
							writer.WriteStartElement(NavigationWriterTag.Add);
							writer.WriteStartElement(NavigationWriterTag.Command);
							writer.WriteAttributeString(NavigationWriterAttribute.Id, arguments[NavigationManager.ItemArgumentCommand].Value);
							writer.WriteElementString(NavigationWriterTag.CommandType, "Navigate");
							writer.WriteElementString(NavigationWriterTag.Target, "right");
							if (arguments.ContainsKey(NavigationManager.ItemArgumentUrl))
								writer.WriteElementString(NavigationWriterTag.Url, arguments[NavigationManager.ItemArgumentUrl].Value);
							if (arguments.ContainsKey(NavigationManager.ItemArgumentEnableHandler))
								writer.WriteElementString(NavigationWriterTag.EnableHandler, arguments[NavigationManager.ItemArgumentEnableHandler].Value);
							if (arguments.ContainsKey(NavigationManager.ItemArgumentParams))
								writer.WriteElementString(NavigationWriterTag.Params, arguments[NavigationManager.ItemArgumentParams].Value);
							writer.WriteEndElement(); // Command
							writer.WriteEndElement(); // add
							writer.WriteEndElement(); // Commands
						}

						writer.WriteEndElement(); // Navigation
					}

					result = builder.ToString();
				}
			}

			return result;
		}
		#endregion

		#region private static string BuildXmlForUpdate(string fullId, Dictionary<string, CustomizationItemArgumentEntity> arguments)
		/// <summary>
		/// Builds the XML for update.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns></returns>
		private static string BuildXmlForUpdate(string fullId, Dictionary<string, CustomizationItemArgumentEntity> arguments)
		{
			string result = null;

			if (!string.IsNullOrEmpty(fullId))
			{
				string[] ids = fullId.Split('/');
				if (ids.Length > 0)
				{
					StringBuilder builder = new StringBuilder();

					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Encoding = Encoding.UTF8;
					settings.OmitXmlDeclaration = true;

					using (XmlWriter writer = XmlWriter.Create(builder, settings))
					{
						writer.WriteStartElement(NavigationWriterTag.Navigation);
						writer.WriteStartElement(NavigationWriterTag.Tabs);
						int lastIndex = ids.Length - 1;
						for (int i = 0; i < lastIndex; i++)
						{
							writer.WriteStartElement(i == 0 ? NavigationWriterTag.Tab : NavigationWriterTag.Link);
							writer.WriteAttributeString(NavigationWriterAttribute.Id, ids[i]);
						}

						if (arguments.ContainsKey(NavigationManager.ItemArgumentOrder))
							WriteSetCommand(writer, lastIndex == 0 ? NavigationWriterTag.Tab : NavigationWriterTag.Link, ids[lastIndex], NavigationWriterAttribute.Order, arguments[NavigationManager.ItemArgumentOrder].Value);
						if (arguments.ContainsKey(NavigationManager.ItemArgumentText))
							WriteSetCommand(writer, lastIndex == 0 ? NavigationWriterTag.Tab : NavigationWriterTag.Link, ids[lastIndex], NavigationWriterAttribute.Text, arguments[NavigationManager.ItemArgumentText].Value);

						for (int i = 0; i < lastIndex; i++)
						{
							writer.WriteEndElement(); // Tab or Link
						}
						writer.WriteEndElement(); // Tabs
						writer.WriteEndElement(); // Navigation
					}

					result = builder.ToString();
				}
			}

			return result;
		}
		#endregion

		#region BuildXmlForRemove
		/// <summary>
		/// Builds the XML for remove.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <returns></returns>
		private string BuildXmlForRemove(string fullId)
		{
			string result = null;

			if (!string.IsNullOrEmpty(fullId))
			{
				string[] ids = fullId.Split('/');

				StringBuilder builder = new StringBuilder();

				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Encoding = Encoding.UTF8;
				settings.OmitXmlDeclaration = true;

				using (XmlWriter writer = XmlWriter.Create(builder, settings))
				{
					writer.WriteStartElement(NavigationWriterTag.Navigation);
					writer.WriteStartElement(NavigationWriterTag.Tabs);
					int lastIndex = ids.Length - 1;
					for (int i = 0; i < lastIndex; i++)
					{
						writer.WriteStartElement(i == 0 ? NavigationWriterTag.Tab : NavigationWriterTag.Link);
						writer.WriteAttributeString(NavigationWriterAttribute.Id, ids[i]);
					}

					writer.WriteStartElement(NavigationWriterTag.Remove);
					writer.WriteAttributeString(NavigationWriterAttribute.NodeId, ids[lastIndex]);
					writer.WriteEndElement(); // remove

					for (int i = 0; i < lastIndex; i++)
					{
						writer.WriteEndElement(); // Link and Tab
					}

					writer.WriteEndElement(); // Tabs
					writer.WriteEndElement(); // Navigation
				}

				result = builder.ToString();
			}

			return result;
		}
		#endregion

		#region WriteSetCommand
		/// <summary>
		/// Writes the set command.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="elementName">Name of the element.</param>
		/// <param name="elementId">The element id.</param>
		/// <param name="attributeName">Name of the attribute.</param>
		/// <param name="attributeValue">The attribute value.</param>
		private static void WriteSetCommand(XmlWriter writer, string elementName, string elementId, string attributeName, string attributeValue)
		{
			writer.WriteStartElement(elementName);
			writer.WriteAttributeString(NavigationWriterAttribute.Id, elementId);
			writer.WriteStartElement(NavigationWriterTag.Set);
			writer.WriteAttributeString(NavigationWriterAttribute.Name, attributeName);
			writer.WriteAttributeString(NavigationWriterAttribute.Value, attributeValue);
			writer.WriteEndElement(); // set
			writer.WriteEndElement(); // elementName
		}
		#endregion
	}
}
