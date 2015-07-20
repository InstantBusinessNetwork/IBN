using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Ibn.Web.UI
{
	public class DynamicFileProvider : FileProvider
	{
		public override FileDescriptor[] GetFiles(string structureName, string searchPattern, Selector[] selectors)
		{
			List<FileDescriptor> files = new List<FileDescriptor>();

			if (string.Compare(structureName, "Layout", StringComparison.OrdinalIgnoreCase) == 0
				&& string.Compare(searchPattern, "*.xml", StringComparison.OrdinalIgnoreCase) == 0
				&& selectors.Length > 0
				&& selectors[0].Items.Count == 3
				&& string.Compare(selectors[0].Items[0], "EntityView", StringComparison.OrdinalIgnoreCase) == 0
				)
			{
				string className = selectors[0].Items[1];

				MetaClass mc = null;
				try
				{
					mc = MetaDataWrapper.GetMetaClassByName(className);
				}
				catch { }

				if (mc != null)
				{
					#region References 1N
					foreach (MetaField field in mc.FindReferencesTo(true))
					{
						if (field.Owner.IsBridge)
							continue;

						string superBlockId = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock);

						if (String.IsNullOrEmpty(superBlockId))
							continue;

						//AK 2009-06-05 check for canread referenced list
						MetaClass own = field.Owner;
						if (ListManager.MetaClassIsList(own))
						{
							ListInfo li = ListManager.GetListInfoByMetaClass(own);
							if (li == null || !Mediachase.IBN.Business.ListInfoBus.CanRead(li.PrimaryKeyId.Value))
								continue;
						}

						string blockFriendlyName = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText);
						string blockOrder = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayOrder);

						//string xmlContent = String.Format(
						//    "<Layout><Block id=\"{0}\"><add><Block id=\"{1}\" name=\"{2}\" order=\"{3}\"><Placeholder id=\"Placeholder_1\"><Control id=\"{4}\" path=\"~/Apps/MetaUIEntity/Modules/EntityListReference1N.ascx\"><Property name=\"ClassName\" value=\"{5}\" /><Property name=\"FilterFieldName\" value=\"{6}\" /></Control></Placeholder><Placeholder id=\"Placeholder_2\" /><Placeholder id=\"Placeholder_3\" /><Placeholder id=\"Placeholder_4\" /></Block></add></Block></Layout>",
						//        superBlockId, 
						//        String.Format("block_{0}_{1}", field.Owner.Name, field.Name), 
						//        blockFriendlyName, blockOrder,
						//        String.Format("ctrl_{0}_{1}", field.Owner.Name, field.Name), 
						//        field.Owner.Name, field.Name);
						//MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
						//file.Content = memoryStream;

						// O.R. [2009-07-27] using MemoryStream instead of string
						FileDescriptor file = CreateFileDescriptor(className);
						using (XmlWriter writer = XmlWriter.Create(file.Content))
						{
							writer.WriteStartDocument();

							writer.WriteStartElement("Layout");

							// Block
							writer.WriteStartElement("Block");
							writer.WriteAttributeString("id", superBlockId);

							// add
							writer.WriteStartElement("add");

							// Block
							writer.WriteStartElement("Block");
							writer.WriteAttributeString("id", String.Format(CultureInfo.InvariantCulture, "block_{0}_{1}", field.Owner.Name, field.Name));
							writer.WriteAttributeString("name", blockFriendlyName);
							writer.WriteAttributeString("order", blockOrder);

							#region Placeholder 1
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_1");

							// Control
							writer.WriteStartElement("Control");
							writer.WriteAttributeString("id", String.Format(CultureInfo.InvariantCulture, "ctrl_{0}_{1}", field.Owner.Name, field.Name));
							writer.WriteAttributeString("path", "~/Apps/MetaUIEntity/Modules/EntityListReference1N.ascx");

							// Property
							writer.WriteStartElement("Property");
							writer.WriteAttributeString("name", "ClassName");
							writer.WriteAttributeString("value", field.Owner.Name);
							writer.WriteEndElement();

							// Property
							writer.WriteStartElement("Property");
							writer.WriteAttributeString("name", "FilterFieldName");
							writer.WriteAttributeString("value", field.Name);
							writer.WriteEndElement();

							writer.WriteEndElement(); // Control
							writer.WriteEndElement(); // Placeholder 
							#endregion

							#region Placeholder 2
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_2");
							writer.WriteEndElement();
							#endregion

							#region Placeholder 3
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_3");
							writer.WriteEndElement();
							#endregion

							#region Placeholder 4
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_4");
							writer.WriteEndElement();
							#endregion

							writer.WriteEndElement(); // Block
							writer.WriteEndElement(); // add
							writer.WriteEndElement(); // Block
							writer.WriteEndElement(); // Layout

							writer.Flush();
						}
						file.Content.Seek(0, SeekOrigin.Begin);
						files.Add(file);
					}
					#endregion

					#region References NN
					foreach (MetaClass bridgeClass in DataContext.Current.MetaModel.GetBridgesReferencedToClass(mc))
					{
						if (!bridgeClass.Attributes.ContainsKey(MetaClassAttribute.BridgeRef1Name)
						 || !bridgeClass.Attributes.ContainsKey(MetaClassAttribute.BridgeRef2Name))
							continue;

						string field1Name = bridgeClass.Attributes.GetValue<string>(MetaClassAttribute.BridgeRef1Name);
						string field2Name = bridgeClass.Attributes.GetValue<string>(MetaClassAttribute.BridgeRef2Name);
						MetaField bridgeField1; // reference to current class
						MetaField bridgeField2; // reference to another class
						if (bridgeClass.Fields[field1Name].Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty) == mc.Name)
						{
							bridgeField1 = bridgeClass.Fields[field1Name];
							bridgeField2 = bridgeClass.Fields[field2Name];
						}
						else
						{
							bridgeField1 = bridgeClass.Fields[field2Name];
							bridgeField2 = bridgeClass.Fields[field1Name];
						}
						if (bridgeField2 == null)
							continue;
						string relatedClassName = bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty);
						if (relatedClassName == String.Empty)
							continue;
						MetaClass relatedClass = MetaDataWrapper.GetMetaClassByName(relatedClassName);

						string superBlockId = bridgeField1.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock);

						if (String.IsNullOrEmpty(superBlockId))
							continue;

						string blockFriendlyName = bridgeField1.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText);
						string blockOrder = bridgeField1.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayOrder);

						//string xmlContent = String.Format(
						//    "<Layout><Block id=\"{0}\"><add><Block id=\"{1}\" name=\"{2}\" order=\"{3}\"><Placeholder id=\"Placeholder_1\"><Control id=\"{4}\" path=\"~/Apps/MetaUIEntity/Modules/EntityListReferenceNN.ascx\"><Property name=\"BridgeClassName\" value=\"{5}\" /><Property name=\"ClassName\" value=\"{6}\" /><Property name=\"Filter1FieldName\" value=\"{7}\" /><Property name=\"Filter2FieldName\" value=\"{8}\" /></Control></Placeholder><Placeholder id=\"Placeholder_2\" /><Placeholder id=\"Placeholder_3\" /><Placeholder id=\"Placeholder_4\" /></Block></add></Block></Layout>",
						//        superBlockId,
						//        String.Format("block_{0}_{1}", bridgeClass.Name, bridgeField1.Name),
						//        blockFriendlyName, blockOrder,
						//        String.Format("ctrl_{0}_{1}", bridgeClass.Name, bridgeField1.Name),
						//        bridgeClass.Name,
						//        relatedClassName, field1Name, field2Name);
						//MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
						//file.Content = memoryStream;

						// O.R. [2009-07-27] using MemoryStream instead of String
						FileDescriptor file = CreateFileDescriptor(className);
						using (XmlWriter writer = XmlWriter.Create(file.Content))
						{
							writer.WriteStartDocument();

							writer.WriteStartElement("Layout");

							// Block
							writer.WriteStartElement("Block");
							writer.WriteAttributeString("id", superBlockId);

							// add
							writer.WriteStartElement("add");

							// Block
							writer.WriteStartElement("Block");
							writer.WriteAttributeString("id", String.Format(CultureInfo.InvariantCulture, "block_{0}_{1}", bridgeClass.Name, bridgeField1.Name));
							writer.WriteAttributeString("name", blockFriendlyName);
							writer.WriteAttributeString("order", blockOrder);

							#region Placeholder 1
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_1");

							// Control
							writer.WriteStartElement("Control");
							writer.WriteAttributeString("id", String.Format(CultureInfo.InvariantCulture, "ctrl_{0}_{1}", bridgeClass.Name, bridgeField1.Name));
							writer.WriteAttributeString("path", "~/Apps/MetaUIEntity/Modules/EntityListReferenceNN.ascx");

							// Property BridgeClassName
							writer.WriteStartElement("Property");
							writer.WriteAttributeString("name", "BridgeClassName");
							writer.WriteAttributeString("value", bridgeClass.Name);
							writer.WriteEndElement();

							// Property ClassName
							writer.WriteStartElement("Property");
							writer.WriteAttributeString("name", "ClassName");
							writer.WriteAttributeString("value", relatedClassName);
							writer.WriteEndElement();

							// Property Filter1FieldName
							writer.WriteStartElement("Property");
							writer.WriteAttributeString("name", "Filter1FieldName");
							writer.WriteAttributeString("value", bridgeField1.Name);
							writer.WriteEndElement();

							// Property Filter2FieldName
							writer.WriteStartElement("Property");
							writer.WriteAttributeString("name", "Filter2FieldName");
							writer.WriteAttributeString("value", bridgeField2.Name);
							writer.WriteEndElement();

							writer.WriteEndElement(); // Control
							writer.WriteEndElement(); // Placeholder 
							#endregion

							#region Placeholder 2
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_2");
							writer.WriteEndElement();
							#endregion

							#region Placeholder 3
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_3");
							writer.WriteEndElement();
							#endregion

							#region Placeholder 4
							writer.WriteStartElement("Placeholder");
							writer.WriteAttributeString("id", "Placeholder_4");
							writer.WriteEndElement();
							#endregion

							writer.WriteEndElement(); // Block
							writer.WriteEndElement(); // add
							writer.WriteEndElement(); // Block
							writer.WriteEndElement(); // Layout

							writer.Flush();
						}
						file.Content.Seek(0, SeekOrigin.Begin);
						files.Add(file);

						if (bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty) == mc.Name)
						{
							superBlockId = bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock);

							if (!String.IsNullOrEmpty(superBlockId))
							{
								blockFriendlyName = bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText);
								blockOrder = bridgeField2.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayOrder);

								//xmlContent = String.Format(
								//    "<Layout><Block id=\"{0}\"><add><Block id=\"{1}\" name=\"{2}\" order=\"{3}\"><Placeholder id=\"Placeholder_1\"><Control id=\"{4}\" path=\"~/Apps/MetaUIEntity/Modules/EntityListReferenceNN.ascx\"><Property name=\"BridgeClassName\" value=\"{5}\" /><Property name=\"ClassName\" value=\"{6}\" /><Property name=\"Filter1FieldName\" value=\"{7}\" /><Property name=\"Filter2FieldName\" value=\"{8}\" /></Control></Placeholder><Placeholder id=\"Placeholder_2\" /><Placeholder id=\"Placeholder_3\" /><Placeholder id=\"Placeholder_4\" /></Block></add></Block></Layout>",
								//        superBlockId,
								//        String.Format("block_{0}_{1}", bridgeClass.Name, bridgeField2.Name),
								//        blockFriendlyName, blockOrder,
								//        String.Format("ctrl_{0}_{1}", bridgeClass.Name, bridgeField2.Name),
								//        bridgeClass.Name,
								//        relatedClassName, field2Name, field1Name);
								//memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
								//file.Content = memoryStream;

								// O.R. [2009-07-27] using MemoryStream instead of String
								file = CreateFileDescriptor(className);
								using (XmlWriter writer = XmlWriter.Create(file.Content))
								{
									writer.WriteStartDocument();

									writer.WriteStartElement("Layout");

									// Block
									writer.WriteStartElement("Block");
									writer.WriteAttributeString("id", superBlockId);

									// add
									writer.WriteStartElement("add");

									// Block
									writer.WriteStartElement("Block");
									writer.WriteAttributeString("id", String.Format(CultureInfo.InvariantCulture, "block_{0}_{1}", bridgeClass.Name, bridgeField2.Name));
									writer.WriteAttributeString("name", blockFriendlyName);
									writer.WriteAttributeString("order", blockOrder);

									#region Placeholder 1
									writer.WriteStartElement("Placeholder");
									writer.WriteAttributeString("id", "Placeholder_1");

									// Control
									writer.WriteStartElement("Control");
									writer.WriteAttributeString("id", String.Format(CultureInfo.InvariantCulture, "ctrl_{0}_{1}", bridgeClass.Name, bridgeField2.Name));
									writer.WriteAttributeString("path", "~/Apps/MetaUIEntity/Modules/EntityListReferenceNN.ascx");

									// Property BridgeClassName
									writer.WriteStartElement("Property");
									writer.WriteAttributeString("name", "BridgeClassName");
									writer.WriteAttributeString("value", bridgeClass.Name);
									writer.WriteEndElement();

									// Property ClassName
									writer.WriteStartElement("Property");
									writer.WriteAttributeString("name", "ClassName");
									writer.WriteAttributeString("value", relatedClassName);
									writer.WriteEndElement();

									// Property Filter1FieldName
									writer.WriteStartElement("Property");
									writer.WriteAttributeString("name", "Filter1FieldName");
									writer.WriteAttributeString("value", bridgeField2.Name);
									writer.WriteEndElement();

									// Property Filter2FieldName
									writer.WriteStartElement("Property");
									writer.WriteAttributeString("name", "Filter2FieldName");
									writer.WriteAttributeString("value", bridgeField1.Name);
									writer.WriteEndElement();

									writer.WriteEndElement(); // Control
									writer.WriteEndElement(); // Placeholder 
									#endregion

									#region Placeholder 2
									writer.WriteStartElement("Placeholder");
									writer.WriteAttributeString("id", "Placeholder_2");
									writer.WriteEndElement();
									#endregion

									#region Placeholder 3
									writer.WriteStartElement("Placeholder");
									writer.WriteAttributeString("id", "Placeholder_3");
									writer.WriteEndElement();
									#endregion

									#region Placeholder 4
									writer.WriteStartElement("Placeholder");
									writer.WriteAttributeString("id", "Placeholder_4");
									writer.WriteEndElement();
									#endregion

									writer.WriteEndElement(); // Block
									writer.WriteEndElement(); // add
									writer.WriteEndElement(); // Block
									writer.WriteEndElement(); // Layout

									writer.Flush();
								}
								file.Content.Seek(0, SeekOrigin.Begin);
								files.Add(file);
							}
						}
					}
					#endregion
				}
			}

			return files.ToArray();
		}

		private FileDescriptor CreateFileDescriptor(string className)
		{
			FileDescriptor file = new FileDescriptor(this);

			file.ModuleName = "DynamicFileProvider";
			file.FileNameWithoutExtension = String.Format("EntityView.{0}", className);
			file.Content = new MemoryStream();

			return file;
		}
	}
}
