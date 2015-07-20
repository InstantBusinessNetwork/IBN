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
	public partial class RelationEdit : System.Web.UI.UserControl
	{
		private readonly string modeN1 = "N1";
		private readonly string returnUrlDefaultValue = "~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx";
		private readonly string businessObjectsBlock = "[BusinessObjects]";
		private readonly string listsBlock = "[Lists]";
		private readonly string notSetValue = "[NotSet]";

		// Fields
		private string _className = "";
		private string _fieldName = "";
		private string _refClassName = "";
		private string _refFieldName = "";
		private string _mode = "N1";
		private MetaClass mc = null;
		private MetaField mf = null;
		private MetaClass mcRef = null;
		private MetaField mfRef = null;

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
		public string RefClassName
		{
			get { return _refClassName; }
			set { _refClassName = value; }
		}
		#endregion

		#region RefFieldName
		public string RefFieldName
		{
			get { return _refFieldName; }
			set { _refFieldName = value; }
		}
		#endregion

		#region Mode
		public string Mode
		{
			get { return _mode; }
			set { _mode = value; }
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
				if (Mode == modeN1)
					BindDataN1();
				else
					BindData1N();

				if (mf == null)
					Page.SetFocus(RelatedObjectList);
				else
					Page.SetFocus(FriendlyNameTextBox);
			}

			ErrorMessage.Text = "";
		}

		#region LoadRequestVariables
		/// <summary>
		/// Loads the request variables.
		/// </summary>
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}

			if (mc != null && Request.QueryString["field"] != null)
			{
				FieldName = Request.QueryString["field"];
				mf = mc.Fields[FieldName];
			}

			if (Request.QueryString["refclass"] != null)
			{
				RefClassName = Request.QueryString["refclass"];
				mcRef = MetaDataWrapper.GetMetaClassByName(RefClassName);
			}

			if (mcRef != null && Request.QueryString["reffield"] != null)
			{
				RefFieldName = Request.QueryString["reffield"];
				mfRef = mcRef.Fields[RefFieldName];
			}

			if (Request.QueryString["mode"] != null)
			{
				Mode = Request.QueryString["mode"].ToUpperInvariant();
			}
		}
		#endregion

		#region BindDataN1
		private void BindDataN1()
		{
			PrimaryObjectLink.Visible = false;
			PrimaryObjectList.Visible = true;
			RelatedObjectLink.Visible = true;
			RelatedObjectList.Visible = false;

			RelatedObjectLink.Text = CHelper.GetResFileString(mc.FriendlyName);
			RelatedObjectLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
				"{0}?class={1}&Tab=RelN1", ReturnUrl, ClassName);

			if (mf == null)	// new N:1
			{
				FillObjectList(PrimaryObjectList);
				RebindFieldInfoByPrimaryObject();
				RebindDisplayInfo(PrimaryObjectList.SelectedValue);

				mfs.BindData(ClassName);
				if (mfs.Count == 0)
					FormsRow.Visible = false;

				// Display Block
				DisplayText.Text = CHelper.GetResFileString(mc.PluralName);
			}
			else // edit N:1
			{
				string primaryClassName = mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty);
				MetaClass primaryClass = MetaDataWrapper.GetMetaClassByName(primaryClassName);
				PrimaryObjectList.Items.Add(new ListItem(CHelper.GetResFileString(primaryClass.FriendlyName), primaryClassName));
				PrimaryObjectList.Enabled = false;

				// Field Block
				NameTextBox.Text = mf.Name;
				NameTextBox.Enabled = false;

				FriendlyNameTextBox.Text = mf.FriendlyName;

				AllowNullsCheckBox.Checked = mf.IsNullable;
				AllowNullsCheckBox.Enabled = false;

				FormsRow.Visible = false;

				// Display Block
				RebindDisplayInfo(primaryClassName);

				CHelper.SafeSelect(DisplayRegion, mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock, notSetValue));
				DisplayText.Text = mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText, CHelper.GetResFileString(mc.PluralName));
				DisplayOrderText.Text = mf.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayOrder, "10000");
				if (DisplayRegion.SelectedValue == notSetValue)
				{
					DisplayTextRow.Visible = false;
					DisplayOrderRow.Visible = false;
				}
				else
				{
					DisplayTextRow.Visible = true;
					DisplayOrderRow.Visible = true;
				}
			}

			FieldBlockHeader.Title = String.Format(CultureInfo.InvariantCulture,
				"{0} \"{1}\"",
				GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Object"),
				CHelper.GetResFileString(mc.FriendlyName));

			MainBlockHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "RelationN1").ToString();

			CancelButton.Attributes.Add("onclick",
				String.Format(CultureInfo.InvariantCulture,
					"window.location.href='{0}?class={1}&Tab=RelN1'; return false;",
					ResolveClientUrl(ReturnUrl),
					ClassName));
		}
		#endregion

		#region BindData1N
		private void BindData1N()
		{
			PrimaryObjectLink.Visible = true;
			PrimaryObjectList.Visible = false;
			RelatedObjectLink.Visible = false;
			RelatedObjectList.Visible = true;

			PrimaryObjectLink.Text = CHelper.GetResFileString(mc.FriendlyName);
			PrimaryObjectLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
				"{0}?class={1}&Tab=Rel1N", ReturnUrl, ClassName);

			if (mfRef == null)	// new 1:N
			{
				FillObjectList(RelatedObjectList);
				RebindFieldInfoByRelatedObject();

				NameTextBox.Text = ClassName;
				FriendlyNameTextBox.Text = CHelper.GetResFileString(mc.FriendlyName);

				// Display Block
				RebindDisplayInfo(ClassName);

				DisplayText.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(RelatedObjectList.SelectedValue).PluralName);
			}
			else  // edit 1:N
			{
				RelatedObjectList.Items.Add(new ListItem(CHelper.GetResFileString(mcRef.FriendlyName), RefClassName));
				RelatedObjectList.Enabled = false;

				FieldBlockHeader.Title = String.Format(CultureInfo.InvariantCulture,
					"{0} \"{1}\"",
					ListManager.MetaClassIsList(RefClassName)
						? GetGlobalResourceObject("IbnFramework.ListInfo", "List")
						: GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Object"),
					CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(RefClassName).FriendlyName));

				NameTextBox.Text = mfRef.Name;
				NameTextBox.Enabled = false;

				FriendlyNameTextBox.Text = mfRef.FriendlyName;

				AllowNullsCheckBox.Checked = mfRef.IsNullable;
				AllowNullsCheckBox.Enabled = false;

				FormsRow.Visible = false;

				// Display Block
				RebindDisplayInfo(ClassName);

				CHelper.SafeSelect(DisplayRegion, mfRef.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayBlock, notSetValue));

				DisplayText.Text = mfRef.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayText, CHelper.GetResFileString(mcRef.PluralName));
				DisplayOrderText.Text = mfRef.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceDisplayOrder, "10000");

				if (DisplayRegion.SelectedValue == notSetValue)
				{
					DisplayTextRow.Visible = false;
					DisplayOrderRow.Visible = false;
				}
				else
				{
					DisplayTextRow.Visible = true;
					DisplayOrderRow.Visible = true;
				}
			}

			MainBlockHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Relation1N").ToString();

			CancelButton.Attributes.Add("onclick",
				String.Format(CultureInfo.InvariantCulture,
					"window.location.href='{0}?class={1}&Tab=Rel1N'; return false;",
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
						items.Add(name, CHelper.GetResFileString(metaClass.FriendlyName));
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

		#region PrimaryObjectList_SelectedIndexChanged
		protected void PrimaryObjectList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (PrimaryObjectList.SelectedValue == businessObjectsBlock || PrimaryObjectList.SelectedValue == listsBlock)
			{
				PrimaryObjectList.SelectedIndex = PrimaryObjectList.SelectedIndex + 1;
			}
			RebindFieldInfoByPrimaryObject();
			RebindDisplayInfo(PrimaryObjectList.SelectedValue);
		}
		#endregion

		#region RelatedObjectList_SelectedIndexChanged
		protected void RelatedObjectList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RelatedObjectList.SelectedValue == businessObjectsBlock || RelatedObjectList.SelectedValue == listsBlock)
			{
				RelatedObjectList.SelectedIndex = RelatedObjectList.SelectedIndex + 1;
			}

			if (mfRef == null)	// new
			{
				RebindFieldInfoByRelatedObject();
			}
			else // Edit
			{
				FormsRow.Visible = false;
			}

			DisplayText.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(RelatedObjectList.SelectedValue).PluralName);
		}
		#endregion

		#region RebindFieldInfoByPrimaryObject
		private void RebindFieldInfoByPrimaryObject()
		{
			string selectedClassName = PrimaryObjectList.SelectedValue;
			NameTextBox.Text = selectedClassName;
			FriendlyNameTextBox.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(selectedClassName).FriendlyName);
		}
		#endregion

		#region RebindFieldInfoByRelatedObject
		private void RebindFieldInfoByRelatedObject()
		{
			string selectedClassName = RelatedObjectList.SelectedValue;

			// Forms
			FieldBlockHeader.Title = String.Format(CultureInfo.InvariantCulture,
				"{0} \"{1}\"",
				ListManager.MetaClassIsList(selectedClassName)
					? GetGlobalResourceObject("IbnFramework.ListInfo", "List")
					: GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Object"),
				CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(selectedClassName).FriendlyName));

			mfs.BindData(selectedClassName);

			if (mfs.Count == 0)
				FormsRow.Visible = false;
			else
				FormsRow.Visible = true;
		}
		#endregion

		#region RebindDisplayInfo
		private void RebindDisplayInfo(string className)
		{
			if (ListManager.MetaClassIsList(className))
			{
				DisplayRow.Visible = false;
			}
			else
			{
				DisplayRow.Visible = true;

				DisplayBlockHeader.Title = String.Format(CultureInfo.InvariantCulture,
					"{0} \"{1}\"",
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Object"),
					CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(className).FriendlyName));

				FillRegions(className);
			}
		}
		#endregion

		#region FillRegions
		private void FillRegions(string className)
		{
			DisplayRegion.Items.Clear();
			DisplayRegion.Items.Add(new ListItem("[ " + GetGlobalResourceObject("IbnFramework.Global", "_mc_NotSet").ToString() + " ]", notSetValue));

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
					
					DisplayRegion.Items.Add(new ListItem(CHelper.GetResFileString(blockName), blockId));
				}
			}

			DisplayTextRow.Visible = false;
			DisplayOrderRow.Visible = false;
		}
		#endregion

		#region DisplayRegion_SelectedIndexChanged
		protected void DisplayRegion_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (DisplayRegion.SelectedValue == notSetValue)
			{
				DisplayTextRow.Visible = false;
				DisplayOrderRow.Visible = false;
			}
			else
			{
				DisplayTextRow.Visible = true;
				DisplayOrderRow.Visible = true;
			}
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			string fieldName = NameTextBox.Text.Trim();
			string friendlyName = FriendlyNameTextBox.Text.Trim();
			bool allowNulls = AllowNullsCheckBox.Checked;

			string sectionName = DisplayRegion.SelectedValue; ;
			string displayText = DisplayText.Text.Trim();
			string displayOrder = DisplayOrderText.Text.Trim();

			string url;
			if (Mode == modeN1)
			{
				if (mf == null)	// new N:1
				{
					try
					{
						CreateRelation(PrimaryObjectList.SelectedValue, ClassName, fieldName, friendlyName, allowNulls, sectionName, displayText, displayOrder, mfs);
					}
					catch (MetaFieldAlreadyExistsException)
					{
						ErrorMessage.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "FieldExistsErrorMessage").ToString(), "'" + NameTextBox.Text.Trim() + "'"); ;
						Page.SetFocus(NameTextBox);
						return;
					}
				}
				else
				{
					UpdateRelation(mf, friendlyName, sectionName, displayText, displayOrder);
				}

				url = String.Format(CultureInfo.InvariantCulture, "{0}?class={1}&Tab=RelN1", ReturnUrl, ClassName);
			}
			else
			{
				if (mfRef == null)	// new 1:N
				{
					try
					{
						CreateRelation(ClassName, RelatedObjectList.SelectedValue, fieldName, friendlyName, allowNulls, sectionName, displayText, displayOrder, mfs);
					}
					catch (MetaFieldAlreadyExistsException)
					{
						ErrorMessage.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "FieldExistsErrorMessage").ToString(), "'" + NameTextBox.Text.Trim() + "'"); ;
						Page.SetFocus(NameTextBox);
						return;
					}
				}
				else
				{
					UpdateRelation(mfRef, friendlyName, sectionName, displayText, displayOrder);
				}

				url = String.Format(CultureInfo.InvariantCulture, "{0}?class={1}&Tab=Rel1N", ReturnUrl, ClassName);
			}

			Response.Redirect(url, true);
		}
		#endregion

		#region CreateRelation
		private void CreateRelation(string primaryClassName, string relatedClassName, string fieldName, string fieldFriendlyName, bool allowNulls, string sectionName, string displayText, string displayOrder, MetaFormSelector mfs)
		{
			if (sectionName == notSetValue || ListManager.MetaClassIsList(primaryClassName))
			{
				sectionName = string.Empty;
				displayText = string.Empty;
				displayOrder = string.Empty;
			}

			MetaField newField = MetaDataWrapper.CreateReference(primaryClassName, relatedClassName, fieldName, fieldFriendlyName, allowNulls, sectionName, displayText, displayOrder);

			//add to the forms
			if (newField != null)
			{
				List<FormDocument> metaForms = mfs.MetaForms;
				foreach (FormDocument fd in metaForms)
				{
					if (HistoryManager.MetaClassIsHistory(fd.MetaClassName) && !HistoryManager.IsSupportedField(fd.MetaClassName, newField.Name))
						continue;
					FormController.AddMetaPrimitive(fd.MetaClassName, fd.Name, newField.Name);
				}

				List<string> profiles = mfs.ListProfiles;
				foreach (string profId in profiles)
				{
					string placeName = "EntityList";
					ListViewProfile profile = ListViewProfile.Load(primaryClassName, profId, placeName);
					if (profile != null)
					{
						profile.FieldSet.Add(newField.Name);
						profile.ColumnsUI.Add(new ColumnProperties(newField.Name, "200px", String.Empty));

						ListViewProfile.SaveSystemProfile(primaryClassName, placeName, Mediachase.Ibn.Data.Services.Security.CurrentUserId, profile);
					}
				}
			}
		}
		#endregion

		#region UpdateRelation
		private void UpdateRelation(MetaField metaField, string friendlyName, string sectionName, string displayText, string displayOrder)
		{
			string primaryClassName = metaField.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName, string.Empty);

			if (sectionName == notSetValue || ListManager.MetaClassIsList(primaryClassName))
			{
				sectionName = string.Empty;
				displayText = string.Empty;
				displayOrder = string.Empty;
			}

			MetaDataWrapper.UpdateReference(metaField, friendlyName, sectionName, displayText, displayOrder);
		}
		#endregion
	}
}