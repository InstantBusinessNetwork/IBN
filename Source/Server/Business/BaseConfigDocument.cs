using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for BaseConfigDocument.
	/// </summary>
	public class BaseConfigDocument
	{
		private Hashtable _blockHash = new Hashtable();

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseConfigDocument"/> class.
		/// </summary>
		public BaseConfigDocument()
		{
		}

		/// <summary>
		/// Determines whether the specified block type contains block.
		/// </summary>
		/// <param name="BlockType">Type of the block.</param>
		/// <returns>
		/// 	<c>true</c> if the specified block type contains block; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsBlock(Type BlockType)
		{
			return GetBlock(BlockType)!=null;
		}

		/// <summary>
		/// Gets the blocks.
		/// </summary>
		/// <value>The blocks.</value>
		public BaseConfigBlock[] Blocks
		{
			get 
			{
				ArrayList retVal = new ArrayList(_blockHash.Values);

				return (BaseConfigBlock[])retVal.ToArray(typeof(BaseConfigBlock));
			}
		}

		/// <summary>
		/// Gets the block.
		/// </summary>
		/// <param name="BlockType">Type of the block.</param>
		/// <returns></returns>
		public BaseConfigBlock GetBlock(Type BlockType)
		{
			if(_blockHash.ContainsKey(BlockType))
				return (BaseConfigBlock)_blockHash[BlockType];

			return null;
		}

		/// <summary>
		/// Adds the block.
		/// </summary>
		/// <param name="block">The block.</param>
		public BaseConfigBlock AddBlock(BaseConfigBlock block)
		{
			if(block==null)
				throw new ArgumentNullException("block");

			if(this.ContainsBlock(block.GetType()))
				throw new ArgumentException("Block alredy added.", "block");

			_blockHash.Add(block.GetType(), block);

			return block;
		}

		#region Load
		private static Type LoadType(string Type) 
		{
			string[] typeInformation = Type.Split(',');

			if( typeInformation.Length < 2 )
				throw new ArgumentException("Unable to parse type name.  Use 'type,assembly'");

			Assembly asm = Assembly.Load(typeInformation[1].Trim());
			if(asm == null)
				throw new ArgumentException("Unable to load assembly " + typeInformation[1]);

			string TypeName = typeInformation[0].Trim();

//			switch(TypeName)
//			{
//				case "Mediachase.IBN.Business.EMail.ExternalEMailActionType":
//					TypeName = "Mediachase.IBN.Business.EMail.IncomingEMailActionType";
//					break;
//				case "Mediachase.IBN.Business.EMail.InternalEMailActionType":
//					TypeName = "Mediachase.IBN.Business.EMail.OutgoingEMailActionType";
//					break;
//			}

			Type networkServerType = asm.GetType(TypeName);
			if( networkServerType == null)
				throw new ArgumentException("Unable to load type " + TypeName);

			return networkServerType;
		}

		private static object GetValueFromString(string Type, string Value) 
		{
			if(Type==null)
				return Value;

			if(Value==null)
				return Value;

			Type realType = LoadType(Type);

			XmlSerializer xmlsz = new XmlSerializer(realType);

			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Value)))
			{
				return xmlsz.Deserialize(ms);
			}
		}

		public void Load(string xml)
		{
			if(xml==null || xml==string.Empty)
				return;

			/*
			 <IncidentBoxDocument>
				<Block Type="">
					<Param Name="" Type="" Value="" />
				</Block>
			 </IncidentBoxDocument>
			 */
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);

			// List and load block
			foreach(XmlNode xmlBlockNode in xmlDoc.SelectNodes("IncidentBoxDocument/Block"))
			{
				BaseConfigBlock CurrentBlock = null;

				string TypeName = xmlBlockNode.Attributes["Type"]!=null?xmlBlockNode.Attributes["Type"].Value:typeof(BaseConfigBlock).AssemblyQualifiedName;

				Type BoxType = LoadType(TypeName);

				CurrentBlock = this.GetBlock(BoxType);

				if(CurrentBlock==null)
				{
					// Register a new block
					CurrentBlock =  this.AddBlock((BaseConfigBlock)Activator.CreateInstance(BoxType));
				}

				// List And Load Params
				foreach(XmlNode xmlParam in xmlBlockNode.SelectNodes("Param"))
				{
					string Name = xmlParam.Attributes["Name"].Value;
					string Value = xmlParam.Attributes["Value"]!=null?xmlParam.Attributes["Value"].Value:null;
					string Type = xmlParam.Attributes["Type"]!=null?xmlParam.Attributes["Type"].Value:null;

					CurrentBlock.Params.Add(Name, GetValueFromString(Type, Value));
				}
			}
		}

		#endregion

		#region Save
		private static string GetValueString(object Value) 
		{
			if(Value is string)
				return (string)Value;

			XmlSerializer xmlsz = new XmlSerializer(Value.GetType());

			using (MemoryStream ms = new MemoryStream())
			{
				xmlsz.Serialize(ms, Value);
				ms.Capacity = (int)ms.Length;

				return System.Text.Encoding.UTF8.GetString(ms.GetBuffer());
			}
		}

		public string GetDocumentString()
		{
			/*
			 <IncidentBoxDocument>
				<Block Type="">
					<Param Name="" Type="" Value="" />
				</Block>
			 </IncidentBoxDocument>
			 */

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<IncidentBoxDocument/>");

			XmlNode xmlIncBoxDoc = xmlDoc.SelectSingleNode("IncidentBoxDocument");

			foreach(BaseConfigBlock block in this.Blocks)
			{
				XmlElement xmlBlock = xmlDoc.CreateElement("Block");

				if(block.GetType()!=typeof(BaseConfigBlock))
					xmlBlock.SetAttribute("Type", block.GetType().AssemblyQualifiedName);

				foreach(string PrmName in block.Params.AllKeys)
				{
					object PrmValue = block.Params[PrmName];

					XmlElement xmlParam = xmlDoc.CreateElement("Param");

					xmlParam.SetAttribute("Name", PrmName);

					if(PrmValue!=null)
					{
						if(PrmValue.GetType()!=typeof(string))
							xmlParam.SetAttribute("Type", PrmValue.GetType().AssemblyQualifiedName);
						xmlParam.SetAttribute("Value", GetValueString(PrmValue));
					}

					xmlBlock.AppendChild(xmlParam);
				}

				xmlIncBoxDoc.AppendChild(xmlBlock);
			}

			return xmlDoc.InnerXml;
		}
		#endregion
	}
}
