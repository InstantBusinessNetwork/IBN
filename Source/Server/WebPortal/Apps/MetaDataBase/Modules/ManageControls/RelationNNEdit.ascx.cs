using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.XPath;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta.Schema;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.MetaDataBase.Modules.ManageControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class RelationNNEdit : System.Web.UI.UserControl
	{
		private readonly string returnUrlDefaultValue = "~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx";
		private readonly string businessObjectsBlock = "[BusinessObjects]";
		private readonly string listsBlock = "[Lists]";
		private readonly string notSetValue = "[NotSet]";

		// Fields
		private string _className = "";
		private string _bridgeName = "";
		private string _fieldName = "";
		private string _refClassName = "";
		private MetaClass mc = null;	// current class
		private MetaClass mcBridge = null;	// bridge class
		private MetaField mf = null;	// meta field in bridge referenced to current class
		private MetaClass mcRef = null;	// another meta class
		private MetaField mfRef = null;	// meta field in bridge referenced to another class

		#region ClassName
		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}
		#endregion

		#region BridgeName
		/// <summary>
		/// Gets or sets the name of the bridge.
		/// </summary>
		/// <value>The name of the bridge.</value>
		public string BridgeName
		{
			get { return _bridgeName; }
			set { _bridgeName = value; }
		}
		#endregion

		#region FieldName
		/// <summary>
		/// Gets or sets the name of the field.
		/// </summary>
		/// <value>The name of the field.</value>
		public string FieldName
		{
			get { return _fieldName; }
			set { _fieldName = value; }
		}
		#endregion

		#region RefClassName
		/// <summary>
		/// Gets or sets the name of the ref class.
		/// </summary>
		/// <value>The name of the ref class.</value>
		public string RefClassName
		{
			get { return _refClassName; }
			set { _refClassName = value; }
		}
		#endregion

		#region ReturnUrl
		public string ReturnUrl
		{
			get
			{
				string retval = returnUrlDefaultValue;
				if (ViewState["ReturnUrl"] != null)
				{
					retval = ViewState["ReturnUrl"].ToString();
				}
				return retval;
			}
			set
			{
				ViewState["ReturnUrl"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();

			if (!IsPostBack)
			{
				BindData();

				if (mf == null)
					Page.SetFocus(RelatedObjectList);
				else
					Page.SetFocus(CurrentDisplayRegion);
			}

			ErrorMessage.Text = "";
		}

		#region LoadRequestVariables
		/// <summary>
		/// Loads the request variables.
		/// </summary>
		private void LoadRequestVariables()
		{
			// Class
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}

			// Bridge
			if (Request.QueryString["bridge"] != null)
			{
				BridgeName = Request.QueryString["bridge"];
				mcBridge = MetaDataWrapper.GetMetaClassByName(BridgeName);
			}

			// Current Field
			if (mcBridge != null && Request.QueryString["field"] != null)
			{
				FieldName = Request.QueryString["field"];
				mf = mcBridge.Fields[FieldName];
			}

			// Another field
			if (mf != null)
			{
				if (mf.Name == mcBridge.Attributes.GetValue<string>(MetaClassAttribute.BridgeRef1Name, string.Empty))
					mfRef = mcBridge.Fields[mcBridge.Attributes[MetaClassAttribute.BridgeRef2Name].ToString()];
				else
					mfRef = mcBridge.Fields[mcBridge.Attributes[MetaClassAttribute.BridgeRef1Name].ToString()];
			}

			// Another Class
			if (mfRef != null)
			{
				RefClassName = mfRef.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty);
				if (!String.IsNullOrEmpty(RefClassName))
					mcRef = MetaDataWrapper.GetMetaClassByName(RefClassName);
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			CurrentObjectLink.Text = CHelper.GetResFileString(mc.FriendlyName);
			CurrentObjectLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
				"{0}?class={1}&Tab=RelNN", ReturnUrl, ClassName);

			if (mf == null)	// new bridge
			{
				FillObjectList(RelatedObjectList);

				RebindFieldInfo(RelatedObjectList.SelectedValue);

				// Current Block
				RebindDisplayInfo(
					ClassName, 
					CurrentRow, 
					CurrentBlockHeader, 
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "CurrentObject").ToString(), 
					CurrentDisplayRegion,
					CurrentDisplayTextRow,
					CurrentDisplayOrderRow);

				CurrentDisplayText.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(RelatedObjectList.SelectedValue).PluralName);

				// Related Block
				RebindDisplayInfo(
					RelatedObjectList.SelectedValue,
					RelatedRow,
					RelatedBlockHeader,
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "RelatedObject").ToString(),
					RelatedDisplayRegion,
					RelatedDisplayTextRow,
					RelatedDisplayOrderRow);

				RelatedDisplayText.Text = CHelper.GetResFileString(mc.PluralName);
			}
			else  // edit bridge
			{
				RelatedObjectList.Items.Add(new ListItem(CHelper.GetResFileString(mcRef.FriendlyName), RefClassName));
				RelatedObjectList.Enabled = false;

				NameTextBox.Text = mcBridge.Name;
				NameTextBox.Enabled = false;

				FriendlyNameTextBox.Text = mcBridge.FriendlyName;

				// Current Block
				RebindDisplayInfo(
					ClassName,
					CurrentRow,
					CurrentBlockHeader,
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "CurrentObject").ToString(),
					CurrentDisplayRegion,
					CurrentDisplayTextRow,
					CurrentDisplayOrderRow);

				CHelper.SafeSelect(CurrentDisplayRegion, mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock, notSetValue));

				CurrentDisplayText.Text = mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText, CHelper.GetResFileString(mcRef.PluralName));
				CurrentDisplayOrderText.Text = mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayOrder, "10000");

				if (CurrentDisplayRegion.SelectedValue == notSetValue)
				{
					CurrentDisplayTextRow.Visible = false;
					CurrentDisplayOrderRow.Visible = false;
				}
				else
				{
					CurrentDisplayTextRow.Visible = true;
					CurrentDisplayOrderRow.Visible = true;
				}

				// Related Block
				RebindDisplayInfo(
					RefClassName,
					RelatedRow,
					RelatedBlockHeader,
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "RelatedObject").ToString(),
					RelatedDisplayRegion,
					RelatedDisplayTextRow,
					RelatedDisplayOrderRow);

				CHelper.SafeSelect(RelatedDisplayRegion, mfRef.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock, notSetValue));

				RelatedDisplayText.Text = mfRef.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText, CHelper.GetResFileString(mc.PluralName));
				RelatedDisplayOrderText.Text = mfRef.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayOrder, "10000");

				if (RelatedDisplayRegion.SelectedValue == notSetValue)
				{
					RelatedDisplayTextRow.Visible = false;
					RelatedDisplayOrderRow.Visible = false;
				}
				else
				{
					RelatedDisplayTextRow.Visible = true;
					RelatedDisplayOrderRow.Visible = true;
				}
			}

			MainBlockHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "RelationNN").ToString();

			CancelButton.Attributes.Add("onclick",
				String.Format(CultureInfo.InvariantCulture,
					"window.location.href='{0}?class={1}&Tab=RelNN'; return false;",
					ResolveClientUrl(ReturnUrl),
					ClassName));
		}
		#endregion

		#region FillObjectList
		private void FillObjectList(DropDownList lst)
		{
			Dictionary<string, string> items;
			List<KeyValuePair<string, string>> sortedItems;

			// Meta Classes
			items = new Dictionary<string, string>();
			foreach (MetaClass metaClass in DataContext.Current.MetaModel.MetaClasses)
			{
				if (!metaClass.IsBridge
					&& !metaClass.IsCard
					&& !String.IsNullOrEmpty(metaClass.TitleFieldName)
					&& CHelper.CheckMetaClassIsPublic(metaClass)
					&& !ListManager.MetaClassIsList(metaClass))
				{
					items.Add(metaClass.Name, CHelper.GetResFileString(metaClass.FriendlyName));
				}
			}

			// [BusinessObjects] Block
			if (items.Count > 0)
			{
				ListItem li = new ListItem(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BusinessObjectsBlock").ToString(), businessObjectsBlock);
				lst.Items.Add(li);
			}

			// SortByValue
			sortedItems = new List<KeyValuePair<string, string>>(items);
			sortedItems.Sort(
				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
			);

			// add metaclasses
			foreach (KeyValuePair<string, string> kvp in sortedItems)
			{
				string text = String.Format(CultureInfo.InvariantCulture, "   {0}", kvp.Value);
				lst.Items.Add(new ListItem(text, kvp.Key));
			}

			// Lists
			items = new Dictionary<string, string>();
			foreach (MetaClass metaClass in DataContext.Current.MetaModel.MetaClasses)
			{
				string name = metaClass.Name;
				if (!metaClass.IsBridge
					&& !metaClass.IsCard &&
					!String.IsNullOrEmpty(metaClass.TitleFieldName)
					&& ListManager.MetaClassIsList(name))
				{
					// Check Security
					int listId = ListManager.GetListIdByClassName(name);
					if (Mediachase.IBN.Business.ListInfoBus.CanRead(listId))
					{
						items.Add(name, CHelper.GetResFileString(metaClass.FriendlyName));
					}
				}
			}

			// [Lists] Block
			if (items.Count > 0)
			{
				ListItem li = new ListItem(GetGlobalResourceObject("IbnFramework.ListInfo", "ListsBlock").ToString(), listsBlock);
				lst.Items.Add(li);
			}

			// SortByValue
			sortedItems = new List<KeyValuePair<string, string>>(items);
			sortedItems.Sort(
				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
			);

			// add lists
			foreach (KeyValuePair<string, string> kvp in sortedItems)
			{
				string text = String.Format(CultureInfo.InvariantCulture, "   {0}", kvp.Value);
				lst.Items.Add(new ListItem(text, kvp.Key));
			}

			lst.SelectedIndex = 1;
		}
		#endregion

		#region RebindDisplayInfo
		private void RebindDisplayInfo(
			string className, 
			HtmlTableRow displayRow, 
			BlockHeaderLight2 displayBlockHeader, 
			string title,
			DropDownList displayRegion, 
			HtmlTableRow displayTextRow, 
			HtmlTableRow displayOrderRow)
		{
			if (ListManager.MetaClassIsList(className))
			{
				displayRow.Visible = false;
			}
			else
			{
				displayRow.Visible = true;

				displayBlockHeader.Title = String.Format(CultureInfo.InvariantCulture,
					"{0} ({1})",
					title,
					CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(className).FriendlyName));

				FillRegions(className, displayRegion, displayTextRow, displayOrderRow);
			}
		}
		#endregion

		#region FillRegions
		private void FillRegions(string className, DropDownList displayRegion, HtmlTableRow displayTextRow, HtmlTableRow displayOrderRow)
		{
			displayRegion.Items.Clear();
			displayRegion.Items.Add(new ListItem("[ " + GetGlobalResourceObject("IbnFramework.Global", "_mc_NotSet").ToString() + " ]", notSetValue));

			// Find blocks
			Selector selector = new Selector(LayoutType.EntityView.ToString(), className, string.Empty);
			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Layout, selector);
			XPathNavigator layoutNode = navigable.CreateNavigator().SelectSingleNode("Layout");
			XPathNodeIterator blocks = layoutNode.SelectChildren("Block", string.Empty);
			foreach (XPathNavigator block in blocks)
			{
				if (block.SelectChildren("Block", string.Empty).Count > 0)	// there are children
				{
					string blockId = block.GetAttribute("id", string.Empty);
					string blockName = block.GetAttribute("name", string.Empty);

					displayRegion.Items.Add(new ListItem(CHelper.GetResFileString(blockName), blockId));
				}
			}

			displayTextRow.Visible = false;
			displayOrderRow.Visible = false;
		}
		#endregion

		#region RebindFieldInfo
		private void RebindFieldInfo(string selectedClassName)
		{
			NameTextBox.Text = String.Concat(ClassName, "_", selectedClassName);
			FriendlyNameTextBox.Text = String.Concat(
				CHelper.GetResFileString(mc.PluralName),
				CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(selectedClassName).PluralName));
		}
		#endregion

		#region RelatedObjectList_SelectedIndexChanged
		protected void RelatedObjectList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RelatedObjectList.SelectedValue == businessObjectsBlock || RelatedObjectList.SelectedValue == listsBlock)
			{
				RelatedObjectList.SelectedIndex = RelatedObjectList.SelectedIndex + 1;
			}

			string selectedClassName = RelatedObjectList.SelectedValue;

			RebindFieldInfo(selectedClassName);

			CurrentDisplayText.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(selectedClassName).PluralName);

			RebindDisplayInfo(
					selectedClassName,
					RelatedRow,
					RelatedBlockHeader,
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "RelatedObject").ToString(),
					RelatedDisplayRegion,
					RelatedDisplayTextRow,
					RelatedDisplayOrderRow);
		}
		#endregion

		#region CurrentDisplayRegion_SelectedIndexChanged
		protected void CurrentDisplayRegion_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CurrentDisplayRegion.SelectedValue == notSetValue)
			{
				CurrentDisplayTextRow.Visible = false;
				CurrentDisplayOrderRow.Visible = false;
			}
			else
			{
				CurrentDisplayTextRow.Visible = true;
				CurrentDisplayOrderRow.Visible = true;
			}
		}
		#endregion

		#region RelatedDisplayRegion_SelectedIndexChanged
		protected void RelatedDisplayRegion_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RelatedDisplayRegion.SelectedValue == notSetValue)
			{
				RelatedDisplayTextRow.Visible = false;
				RelatedDisplayOrderRow.Visible = false;
			}
			else
			{
				RelatedDisplayTextRow.Visible = true;
				RelatedDisplayOrderRow.Visible = true;
			}
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			string name = NameTextBox.Text.Trim();
			string friendlyName = FriendlyNameTextBox.Text.Trim();

			string currentSectionName = CurrentDisplayRegion.SelectedValue; ;
			string currentDisplayText = CurrentDisplayText.Text.Trim();
			string currentDisplayOrder = CurrentDisplayOrderText.Text.Trim();

			string relatedSectionName = RelatedDisplayRegion.SelectedValue; ;
			string relatedDisplayText = RelatedDisplayText.Text.Trim();
			string relatedDisplayOrder = RelatedDisplayOrderText.Text.Trim();

			if (mf == null)	// new 
			{
				try
				{
					CreateRelation(name, friendlyName, ClassName, currentSectionName, currentDisplayText, currentDisplayOrder,
						RelatedObjectList.SelectedValue, relatedSectionName, relatedDisplayText, relatedDisplayOrder);
				}
				catch (MetaClassAlreadyExistsException)
				{
					ErrorMessage.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BridgeExistsErrorMessage").ToString(), "'" + NameTextBox.Text.Trim() + "'"); ;
					Page.SetFocus(NameTextBox);
					return;
				}
			}
			else  // edit
			{
				UpdateRelation(mcBridge, friendlyName, mf.Name, currentSectionName, currentDisplayText, currentDisplayOrder, mfRef.Name, relatedSectionName, relatedDisplayText, relatedDisplayOrder);
			}

			string url = String.Format(CultureInfo.InvariantCulture, "{0}?class={1}&Tab=RelNN", ReturnUrl, ClassName);
			Response.Redirect(url, true);
		}
		#endregion

		#region CreateRelation
		private void CreateRelation(
			string bridgeName,
			string bridgeFriendlyName,
			string currentClassName, 
			string currentSectionName,
			string currentDisplayText,
			string currentDisplayOrder,
			string relatedClassName,
			string relatedSectionName,
			string relatedDisplayText,
			string relatedDisplayOrder)
		{
			string currentFieldFiendlyName = currentDisplayText;
			string relatedFieldFiendlyName = relatedDisplayText;

			if (currentSectionName == notSetValue || ListManager.MetaClassIsList(currentClassName))
			{
				currentSectionName = string.Empty;
				currentDisplayText = string.Empty;
				currentDisplayOrder = string.Empty;
			}

			if (relatedSectionName == notSetValue || ListManager.MetaClassIsList(relatedClassName))
			{
				relatedSectionName = string.Empty;
				relatedDisplayText = string.Empty;
				relatedDisplayOrder = string.Empty;
			}

			MetaDataWrapper.CreateBridgeClass(
				bridgeName,
				bridgeFriendlyName,
				currentClassName,
				MetaDataWrapper.BridgeField1Name,
				currentFieldFiendlyName,
				currentSectionName,
				currentDisplayText,
				currentDisplayOrder,
				relatedClassName,
				MetaDataWrapper.BridgeField2Name,
				relatedFieldFiendlyName,
				relatedSectionName,
				relatedDisplayText,
				relatedDisplayOrder);
 		}
		#endregion

		#region UpdateRelation
		private void UpdateRelation(
			MetaClass bridgeClass,
			string friendlyName,
			string currentFieldName,
			string currentSectionName, 
			string currentDisplayText, 
			string currentDisplayOrder,
			string relatedFieldName,
			string relatedSectionName,
			string relatedDisplayText,
			string relatedDisplayOrder)
		{
			string currentFieldFiendlyName = currentDisplayText;
			string relatedFieldFiendlyName = relatedDisplayText;

			if (currentSectionName == notSetValue || ListManager.MetaClassIsList(ClassName))
			{
				currentSectionName = string.Empty;
				currentDisplayText = string.Empty;
				currentDisplayOrder = string.Empty;
			}

			if (relatedSectionName == notSetValue || ListManager.MetaClassIsList(RefClassName))
			{
				relatedSectionName = string.Empty;
				relatedDisplayText = string.Empty;
				relatedDisplayOrder = string.Empty;
			}

			MetaDataWrapper.UpdateBridge(
				bridgeClass,
				friendlyName,
				currentFieldName,
				currentFieldFiendlyName,
				currentSectionName,
				currentDisplayText,
				currentDisplayOrder,
				relatedFieldName,
				relatedFieldFiendlyName,
				relatedSectionName,
				relatedDisplayText,
				relatedDisplayOrder);
		}
		#endregion
	}
}