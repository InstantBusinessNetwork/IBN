using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.Controls.Util.Configuration;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	/// <summary>
	/// 
	/// </summary>
	public partial class MetaFieldEdit : System.Web.UI.UserControl, IAutogenerateSystemNames
	{
		// Fields
		private string _className = "";
		private string _fieldName = "";
		private MetaClass mc = null;
		private MetaField mf = null;

		// Keys
		private readonly string currentControlKey = "CurrentControl";
		private readonly string classLabelTextKey = "ClassLabelText";
		private readonly string returnUrlKey = "ReturnUrl";
		private readonly string placeKey = "Place";
		private readonly string autoGenerateFieldNameKey = "AutoGenerateFieldName";

		// Default Values
		private readonly string returnUrlDefaultValue = "~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx";

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

		#region ClassLabelText
		/// <summary>
		/// Gets or sets the class label text.
		/// </summary>
		/// <value>The class label text.</value>
		public string ClassLabelText
		{
			get 
			{
				string retval;
				if (ViewState[classLabelTextKey] == null)
				{
					if (mc.IsCard)
						retval = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Card").ToString();
					else if (mc.IsBridge)
						retval = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Bridge").ToString();
					else
						retval = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Info").ToString();
				}
				else
				{
					retval = ViewState[classLabelTextKey].ToString();
				}
				return retval; 
			}
			set 
			{
				ViewState[classLabelTextKey] = value;
			}
		}
		#endregion

		#region ReturnUrl
		public string ReturnUrl
		{
			get
			{
				string retval = returnUrlDefaultValue;
				if (ViewState[returnUrlKey] != null)
				{
					retval = ViewState[returnUrlKey].ToString();
				}
				return retval;
			}
			set
			{
				ViewState[returnUrlKey] = value;
			}
		}
		#endregion

		#region Place
		public string Place
		{
			get
			{
				string retval = "";
				if (ViewState[placeKey] != null)
				{
					retval = ViewState[placeKey].ToString();
				}
				return retval;
			}
			set
			{
				ViewState[placeKey] = value;
			}
		}
		#endregion

		#region AutogenerateSystemNames (IAutogenerateSystemNames Member)
		public bool AutogenerateSystemNames
		{
			get
			{
				bool retval = false;
				if (ViewState[autoGenerateFieldNameKey] != null)
				{
					retval = (bool)ViewState[autoGenerateFieldNameKey];
				}
				return retval;
			}
			set
			{
				ViewState[autoGenerateFieldNameKey] = value;
			}
		}
		#endregion

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();

			//CHelper.AddToContext(NavigationBlock.KeyContextMenu, "MetaClassView");
			//CHelper.AddToContext(NavigationBlock.KeyContextMenuTitle, CHelper.GetResFileString(mc.FriendlyName));

			if (!IsPostBack)
			{
				BindData();

				if (mf == null && !AutogenerateSystemNames)
					Page.SetFocus(NameTextBox);
				else
					Page.SetFocus(FriendlyNameTextBox);
			}

			BindInfo();

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
		}
		#endregion

		#region BindInfo
		/// <summary>
		/// Binds the info.
		/// </summary>
		private void BindInfo()
		{
			if (mf != null)
			{
				MainBlockHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EditField").ToString();
			}
			else
			{
				MainBlockHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewField").ToString();
				if (!AutogenerateSystemNames)
				{
					NameTextBox.Attributes.Add("onblur", "SetName('" + NameTextBox.ClientID + "','" + FriendlyNameTextBox.ClientID + "','" + FriendlyNameRequiredValidator.ClientID + "')");
				}
			}

			TableLabel.Text = ClassLabelText;

			MainBlockHeader.AddLink(
				String.Format(CultureInfo.InvariantCulture, "<img src='{0}' border='0' align='absmiddle' width='16px;' height='16px;' />&nbsp;{1}", ResolveClientUrl("~/images/IbnFramework/cancel.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Back").ToString()),
				String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", ReturnUrl, mc.Name));

			CancelButton.Attributes.Add("onclick",
				String.Format(CultureInfo.InvariantCulture, "window.location.href='{0}?class={1}'; return false;", ResolveClientUrl(ReturnUrl), mc.Name));
		}
		#endregion

		#region BindData
		/// <summary>
		/// Binds the data.
		/// </summary>
		private void BindData()
		{
			TableLink.Text = CHelper.GetResFileString(mc.FriendlyName);
			TableLink.NavigateUrl = String.Format("{0}?class={1}", ReturnUrl, mc.Name);

			if (mf == null)
			{
				List<ListItem> listItems = new List<ListItem>();

				// Building the FieldType list
				foreach (string mcDataType in Enum.GetNames(typeof(McDataType)))
				{
					if (!MetaDataBaseSection.CheckMetaFieldTypeVisibility(mc, (McDataType)Enum.Parse(typeof(McDataType), mcDataType)))
						continue;

					if (mcDataType == McDataType.ReferencedField.ToString())
					{
						if (mc.GetReferences().Length <= 0)
							continue;
					}
					else if (mcDataType == McDataType.BackReference.ToString())
					{
						MetaField[] mfList = mc.FindReferencesWithoutBack();
						if (mfList.Length <= 0)
							continue;
					}
					else if (mcDataType == McDataType.Card.ToString())
					{
						continue;
					}
					else if (mcDataType == McDataType.Identifier.ToString())
					{
						if (DataContext.Current.MetaModel.GetRegisteredTypes(McDataType.Identifier).Length <= 0)
							continue;
					}
					else if (mcDataType == McDataType.Link.ToString())
					{
						continue;
					}
					else if (mcDataType == McDataType.Complex.ToString())
					{
						if (MetaDataWrapper.GetFieldFormatsByType(mcDataType).Length <= 0)
							continue;

						// IBN 4.7 fix: check that the list has the TitleField
						if (ListManager.MetaClassIsList(mc))
						{
							if (String.IsNullOrEmpty(mc.TitleFieldName))
								continue;
						}
					}
					else if (mcDataType == McDataType.Reference.ToString())
					{
						if (String.IsNullOrEmpty(mc.TitleFieldName))
							continue;
						// IBN 4.7 fix: check that exists at least one list to which we can make a reference
						//if (ListManager.MetaClassIsList(mc))
						//{
						//    bool exists = false;
						//    foreach (MetaClass metaClass in DataContext.Current.MetaModel.MetaClasses)
						//    {
						//        string name = metaClass.Name;
						//        if (!metaClass.IsBridge && !metaClass.IsCard && !String.IsNullOrEmpty(metaClass.TitleFieldName) && ListManager.MetaClassIsList(name) && name != mc.Name)
						//        {
						//            // Check Security
						//            int listId = ListManager.GetListIdByClassName(name);
						//            if (Mediachase.IBN.Business.ListInfoBus.CanRead(listId))
						//            {
						//                exists = true;
						//                break;
						//            }
						//        }
						//    }

						//    if (!exists)
						//        continue;
						//}
					}

					listItems.Add(new ListItem(CHelper.GetMcDataTypeName(mcDataType), mcDataType));
					//FieldTypeList.Items.Add(new ListItem(CHelper.GetMcDataTypeName(mcDataType), mcDataType));
				}

				listItems.Sort(delegate(ListItem x, ListItem y) { return x.Text.CompareTo(y.Text); });

				FieldTypeList.Items.AddRange(listItems.ToArray());

				CHelper.SafeSelect(FieldTypeList, "String");

				if (ListManager.MetaClassIsList(mc))
				{
					ListInfo li = ListManager.GetListInfoByMetaClassName(mc.Name);
					if (li.IsTemplate)
						trSelector.Visible = false;
					else
						mfs.BindData(mc.Name);
				}
				else
					mfs.BindData(mc.Name);

				if (mfs.Count == 0)
					trSelector.Visible = false;

				if (FieldTypeList.Items.Count > 0)
					BindFormats();
			}
			else  // Edit
			{
				NameTextBox.Text = mf.Name;
				NameTextBox.Enabled = false;

				FriendlyNameTextBox.Text = mf.FriendlyName;

				AllowNullsCheckBox.Checked = mf.IsNullable;
				AllowNullsCheckBox.Enabled = false;

				McDataType mcDataType = mf.GetOriginalMetaType().McDataType;
				FieldTypeList.Items.Add(new ListItem(CHelper.GetMcDataTypeName(mcDataType), mcDataType.ToString()));
				FieldTypeList.Enabled = false;

				FormatList.Items.Add(new ListItem(CHelper.GetResFileString(mf.GetOriginalMetaType().FriendlyName), mf.TypeName));
				FormatList.Enabled = false;

				if (FieldTypeList.SelectedValue != McDataType.Identifier.ToString()
				  && FieldTypeList.SelectedValue != McDataType.Enum.ToString()
				  && FieldTypeList.SelectedValue != McDataType.MultiReference.ToString()
				  && MetaDataWrapper.GetFieldFormatsByType(FieldTypeList.SelectedValue).Length <= 1)
					FormatRow.Visible = false;

				trSelector.Visible = false;

				ShowControl();
			}

			if (AutogenerateSystemNames)
			{
				NameRow.Visible = false;
			}
		}
		#endregion

		#region BindFormats
		/// <summary>
		/// Binds the formats.
		/// </summary>
		private void BindFormats()
		{
			FormatList.Items.Clear();

			MetaFieldType[] metaFieldTypes = MetaDataWrapper.GetFieldFormatsByType(FieldTypeList.SelectedValue);
			Dictionary<string, string> items = new Dictionary<string, string>();
			foreach (MetaFieldType fieldType in metaFieldTypes)
			{
				if (fieldType.Attributes.ContainsKey(McDataTypeAttribute.EnumPrivate) &&
					fieldType.Attributes[McDataTypeAttribute.EnumPrivate].ToString() != mc.Name)
					continue;

				string text = CHelper.GetResFileString(fieldType.FriendlyName);

				//AK 2008-02-13 + 2009-01-28
				if (MetaDataBaseSection.CheckMetaFieldTypeVisibility(mc, fieldType)
					&& !items.ContainsKey(fieldType.Name))
					items.Add(fieldType.Name, text);
			}

			// sort by value
			List<KeyValuePair<string, string>> sortedItems = new List<KeyValuePair<string, string>>(items);
			sortedItems.Sort(
				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
			);

			foreach (KeyValuePair<string, string> kvp in sortedItems)
				FormatList.Items.Add(new ListItem(kvp.Value, kvp.Key));
			//

			if (FieldTypeList.SelectedValue == McDataType.Enum.ToString())
			{
				FormatLabel.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Dictionary").ToString();
				ListItem liSelected = new ListItem(String.Format("[{0}]", GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewSingleValue").ToString()), "Enum");
				liSelected.Selected = true;
				FormatList.Items.Add(liSelected);
				FormatList.Items.Add(new ListItem(String.Format("[{0}]", GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewMultiValue").ToString()), "EnumMultiValue"));
			}
			else
			{
				FormatLabel.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Format").ToString();
			}

			if (FieldTypeList.SelectedValue == McDataType.Identifier.ToString()
			  || FieldTypeList.SelectedValue == McDataType.Enum.ToString()
			  || FieldTypeList.SelectedValue == McDataType.Complex.ToString()
			  || FieldTypeList.SelectedValue == McDataType.MultiReference.ToString()
			  || FormatList.Items.Count > 1)
				FormatRow.Visible = true;
			else
				FormatRow.Visible = false;

			if (!Page.IsPostBack && FieldTypeList.SelectedValue == "String")
			{
				CHelper.SafeSelect(FormatList, "Text");
			}

			if (FormatList.Items.Count > 0)
				ShowControl();
		}
		#endregion

		#region ShowControl
		/// <summary>
		/// Shows the control.
		/// </summary>
		private void ShowControl()
		{
			if (MainPlaceHolder.Controls.Count > 0)
				MainPlaceHolder.Controls.Clear();

			string metaTypeName = FormatList.SelectedValue;
			MetaFieldType fieldType = MetaDataWrapper.GetMetaFieldTypeByName(metaTypeName);

			if (fieldType != null)
			{
				if (fieldType.McDataType == McDataType.Enum)
				{
					if (fieldType.Attributes.GetValue<bool>(McDataTypeAttribute.EnumMultivalue, false))
						metaTypeName = "EnumMultiValue";
					else
						metaTypeName = "Enum";
				}
				if (fieldType.McDataType == McDataType.MultiReference)
					metaTypeName = "MultiReference";
			}

			ResolvedPath resPath = ControlPathResolver.Current.ResolveStrong(metaTypeName, "Manage", "", "", Place);

			// Try to use empty place
			if (resPath == null)
				resPath = ControlPathResolver.Current.ResolveStrong(metaTypeName, "Manage", "", "", "");

			if (resPath == null)
				return;

			string controlPath = resPath.Path; //MetaFieldControlPathResolver.Resolve(metaTypeName, "Manage", "");
			
			if (controlPath.IndexOf("Manage") <= 0)
				return;
			
			if (File.Exists(Server.MapPath(controlPath)))
			{
				Control control = (Control)LoadControl(controlPath);
				control.ID = "ManageControl";
				MainPlaceHolder.Controls.Add(control);

				ViewState[currentControlKey] = controlPath;

				IAutogenerateSystemNames iAutogenerateSystemNames = control as IAutogenerateSystemNames;
				if (iAutogenerateSystemNames != null)
				{
					iAutogenerateSystemNames.AutogenerateSystemNames = AutogenerateSystemNames;
				}

				IManageControl iManageControl = control as IManageControl;
				if (iManageControl != null)
				{
					if (mf == null)
						iManageControl.BindData(mc, FormatList.SelectedValue);
					else
						iManageControl.BindData(mf);
				}
			}
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			if (IsPostBack && ViewState[currentControlKey] != null)
			{
				string controlPath = ViewState[currentControlKey].ToString();
				Control control = (Control)LoadControl(controlPath);
				control.ID = "ManageControl";
				MainPlaceHolder.Controls.Add(control);
			}
			base.CreateChildControls();
		}
		#endregion

		#region FieldTypeList_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the FieldTypeList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void FieldTypeList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindFormats();
		}
		#endregion

		#region FormatList_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the FormatList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void FormatList_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowControl();
		}
		#endregion

		#region SaveButton_ServerClick
		/// <summary>
		/// Handles the ServerClick event of the SaveButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			if (FormatList.Items.Count < 0)
				throw new Exception("Format is not specified");

			Page.Validate();
			if (!Page.IsValid)
				return;

			Mediachase.Ibn.Data.Meta.Management.AttributeCollection attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
			string sDefaultValue = String.Empty;

			if (MainPlaceHolder.Controls.Count > 0)
			{
				IManageControl control = MainPlaceHolder.Controls[0] as IManageControl;

				if (control != null)
				{
					sDefaultValue = control.GetDefaultValue(AllowNullsCheckBox.Checked);
					attr = control.FieldAttributes;
				}
			}

			if (!AllowNullsCheckBox.Checked && sDefaultValue == String.Empty)
			{
				ErrorMessage.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ErrorMessage_AllowNulls").ToString();
				return;
			}

			if (sDefaultValue == null)
				sDefaultValue = String.Empty;

			string sFriendlyName = FriendlyNameTextBox.Text.Trim();

			if (mf == null)
			{
				string sName;
				if (!AutogenerateSystemNames)
				{
					sName = NameTextBox.Text.Trim();
				}
				else
				{
					// Generate the field name as the number of seconds elapsed since 2000-01-01
					sName = String.Format(CultureInfo.InvariantCulture, "Field{0}", CHelper.GetDateDiffInSeconds(DateTime.UtcNow, new DateTime(2000, 1, 1)));
				}

				try
				{
					string typeName = FormatList.SelectedValue;
					//NewEnum
					if (attr.ContainsKey("NewEnum"))
					{
						string name = attr["EnumName"].ToString();
						string friendlyname = attr["EnumFriendlyName"].ToString();
						bool isPrivate = (bool)attr["EnumPrivate"];
						DataTable dt = (DataTable)attr["EnumDataSource"];

						attr.Remove("NewEnum");
						attr.Remove("EnumName");
						attr.Remove("EnumFriendlyName");
						attr.Remove("EnumPrivate");
						attr.Remove("EnumDataSource");

						MetaFieldType type = MetaEnum.Create(name, friendlyname, false);
						if (isPrivate)
							type.Attributes.Add(McDataTypeAttribute.EnumPrivate, mc.Name);

						SortedList sl = new SortedList();
						foreach (DataRow dr in dt.Rows)
							sl.Add((int)dr["OrderId"], dr["Name"].ToString().Trim());
						
						foreach(int i in sl.Keys)
							MetaEnum.AddItem(type, sl[i].ToString(), i);

						typeName = type.Name;
					}
					//NewMultiEnum
					if (attr.ContainsKey("NewMultiEnum"))
					{
						string name = attr["EnumName"].ToString();
						string friendlyname = attr["EnumFriendlyName"].ToString();
						bool isPrivate = (bool)attr["EnumPrivate"];
						DataTable dt = (DataTable)attr["EnumDataSource"];

						attr.Remove("NewMultiEnum");
						attr.Remove("EnumName");
						attr.Remove("EnumFriendlyName");
						attr.Remove("EnumPrivate");
						attr.Remove("EnumDataSource");

						MetaFieldType type = MetaEnum.Create(name, friendlyname, true);
						if (isPrivate)
							type.Attributes.Add(McDataTypeAttribute.EnumPrivate, mc.Name);

						SortedList sl = new SortedList();
						foreach (DataRow dr in dt.Rows)
							sl.Add((int)dr["OrderId"], dr["Name"].ToString().Trim());

						foreach (int i in sl.Keys)
							MetaEnum.AddItem(type, sl[i].ToString(), i);

						typeName = type.Name;
					}

					MetaFieldType mft = DataContext.Current.MetaModel.RegisteredTypes[FormatList.SelectedValue];
					if (mft != null && mft.McDataType == McDataType.Enum &&
						attr.ContainsKey("EnumFriendlyName"))
					{
						mft.FriendlyName = attr["EnumFriendlyName"].ToString();
						attr.Remove("EnumFriendlyName");
					}

					MetaField newField = null;
					if (FieldTypeList.SelectedValue == McDataType.Reference.ToString())
					{
						newField = MetaDataWrapper.CreateReference(mc, attr, sName, sFriendlyName, AllowNullsCheckBox.Checked);
						if (attr.ContainsKey(McDataTypeAttribute.ReferenceUseSecurity))
							Mediachase.Ibn.Data.Services.Security.AddObjectRolesFromReference(newField);
					}
					else if (FieldTypeList.SelectedValue == McDataType.ReferencedField.ToString())
						newField = MetaDataWrapper.CreateReferencedField(mc, attr, sName, sFriendlyName);
					else if (FieldTypeList.SelectedValue == McDataType.BackReference.ToString())
						newField = MetaDataWrapper.CreateBackReference(mc, attr, sName, sFriendlyName);
					else
						newField = MetaDataWrapper.CreateMetaField(mc, attr, sName, sFriendlyName, typeName, AllowNullsCheckBox.Checked, sDefaultValue);
					
					//add to the forms
					if (newField != null)
					{
						List<FormDocument> metaForms = mfs.MetaForms;
						foreach (FormDocument fd in metaForms)
						{
							if (HistoryManager.MetaClassIsHistory(fd.MetaClassName) && !HistoryManager.IsSupportedField(mc.Name, newField.Name))
								continue;
							FormController.AddMetaPrimitive(fd.MetaClassName, fd.Name, newField.Name);
						}

						List<string> profiles = mfs.ListProfiles;
						foreach (string profId in profiles)
						{
							string placeName = "EntityList";
							ListViewProfile profile = ListViewProfile.Load(mc.Name, profId, placeName);
							if (profile != null)
							{
								profile.FieldSet.Add(newField.Name);
								profile.ColumnsUI.Add(new ColumnProperties(newField.Name, "200px", String.Empty));

								ListViewProfile.SaveSystemProfile(mc.Name, placeName, Mediachase.Ibn.Data.Services.Security.CurrentUserId, profile);
							}
						}

						//using (MetaClassManagerEditScope editScope = DataContext.Current.MetaModel.BeginEdit())
						//{
						//    List<string> metaViews = mfs.MetaViews;
						//    foreach (string viewName in metaViews)
						//    {
						//        MetaView metaView = DataContext.Current.MetaModel.MetaViews[viewName];
						//        if (HistoryManager.MetaClassIsHistory(metaView.MetaClassName) && !HistoryManager.IsSupportedField(metaView.MetaClassName, newField.Name))
						//            continue;
						//        McMetaViewPreference pref = UserMetaViewPreference.Load(metaView, (int)DataContext.Current.CurrentUserId);
						//        if (pref == null || pref.Attributes.Count == 0)
						//        {
						//            McMetaViewPreference.CreateDefaultUserPreference(metaView);
						//            pref = UserMetaViewPreference.Load(metaView, (int)DataContext.Current.CurrentUserId);
						//        }
						//        int counter = metaView.AvailableFields.Count;
						//        metaView.AvailableFields.Add(metaView.MetaClass.Fields[newField.Name]);
						//        pref.SetAttribute<int>(newField.Name, McMetaViewPreference.AttrIndex, counter);
						//        pref.SetAttribute<int>(newField.Name, McMetaViewPreference.AttrWidth, 100);
						//    }

						//    editScope.SaveChanges();
						//}
					}

					Response.Redirect(String.Format("{0}?class={1}", ReturnUrl, mc.Name), true);
				}
				catch (MetaFieldAlreadyExistsException)
				{
					ErrorLabel.Text = String.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "FieldExistsErrorMessage").ToString(), "'" + sName + "'");
					ErrorLabel.Visible = true;
				}
				/* 				catch (SqlException sqlException)
								{
									if (sqlException.Number == 1505)	// Duplication key
										ErrorLabel.Text = ex.Message;
									else
										ErrorLabel.Text = ex.Message;
									ErrorLabel.Visible = true;
								}
							 */
			}
			else // Update
			{
				MetaFieldType mft = DataContext.Current.MetaModel.RegisteredTypes[FormatList.SelectedValue];
				if (mft.McDataType == McDataType.Enum && attr.ContainsKey("EnumFriendlyName"))
				{
					mft.FriendlyName = attr["EnumFriendlyName"].ToString();
					attr.Remove("EnumFriendlyName");
				}

				if (FieldTypeList.SelectedValue == McDataType.Reference.ToString()
				  || FieldTypeList.SelectedValue == McDataType.BackReference.ToString()
				  || FieldTypeList.SelectedValue == McDataType.ReferencedField.ToString())
					MetaDataWrapper.UpdateMetaFieldFriendlyName(mf, sFriendlyName);
				else
					MetaDataWrapper.UpdateMetaField(mf, attr, sFriendlyName, sDefaultValue);

				Response.Redirect(String.Format("{0}?class={1}", ReturnUrl, mc.Name), true);
			}
		}
		#endregion
	}
}