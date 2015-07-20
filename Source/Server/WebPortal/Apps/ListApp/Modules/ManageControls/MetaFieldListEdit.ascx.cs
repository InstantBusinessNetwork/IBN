using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util.Configuration;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;
using System.IO;
using Mediachase.Ibn.Lists.Mapping;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class MetaFieldListEdit : System.Web.UI.UserControl, IAutogenerateSystemNames
	{
		private readonly string currentControlKey = "CurrentControl";
		ListImportParameters lip = null;

		#region _commandName
		private string _commandName
		{
			get
			{
				if (Request["CommandName"] != null)
					return Request["CommandName"];
				return String.Empty;
			}
		} 
		#endregion

		#region AutogenerateSystemNames (IAutogenerateSystemNames Member)
		private readonly string autoGenerateFieldNameKey = "AutoGenerateFieldName";
		public bool AutogenerateSystemNames
		{
			get
			{
				bool retval = true;
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

		protected void Page_Load(object sender, EventArgs e)
		{
			AllowNullsCheckBox.Text =  " " + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "AllowNulls").ToString();
			ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(btnUpdate);
			lblNotEmptyName.Style.Add("display", "none");
			if (!Page.IsPostBack)
			{
				ViewState["_keyLIP"] = Request["key"];
				if (Request["field"] != null)
					ViewState["_field"] = Request["field"];
				else
					ViewState["_field"] = null;

				ViewState[currentControlKey] = null;

				//lip = (ListImportParameters)CHelper.GetFromContext(ViewState["_keyLIP"].ToString());
				//if(lip == null)
				lip = (ListImportParameters)Session[ViewState["_keyLIP"].ToString()];
				if (lip != null)
				{
					BindData();

					if (!AutogenerateSystemNames)
						Page.SetFocus(NameTextBox);
					else
						Page.SetFocus(FriendlyNameTextBox);
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			BindInfo();
			base.OnPreRender(e);
		}

		#region BindInfo
		/// <summary>
		/// Binds the info.
		/// </summary>
		private void BindInfo()
		{
			NameTextBox.Attributes.Add("onblur", "SetName('" + NameTextBox.ClientID + "','" + FriendlyNameTextBox.ClientID + "')");

			TableLabel.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "List").ToString();

			CancelButton.Attributes.Add("onclick", String.Format("javascript:{{{0}}}", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true)));
			SaveButton.Attributes.Add("onclick", String.Format("javascript:{{MFListAdd_CheckName()}}"));
		}
		#endregion

		#region BindData
		/// <summary>
		/// Binds the data.
		/// </summary>
		private void BindData()
		{
			tblName.Text = lip.ListName;
			MetaField mf = null;
			if (ViewState["_field"] != null)
				mf = lip.GetDestinationMetaField(ViewState["_field"].ToString());

			if (mf != null)
			{
				NameTextBox.Text = mf.Name;
				FriendlyNameTextBox.Text = mf.FriendlyName;
				AllowNullsCheckBox.Checked = mf.IsNullable;
			}
			else
			{
				NameTextBox.Text = "";
				FriendlyNameTextBox.Text = "";
				AllowNullsCheckBox.Checked = true;
			}

			FieldTypeList.Items.Clear();
			// Building the FieldType list
			foreach (string mcDataType in Enum.GetNames(typeof(McDataType)))
			{
				if (!MetaDataBaseSection.CheckMetaFieldTypeVisibility("List_XXX", (McDataType)Enum.Parse(typeof(McDataType), mcDataType)))
					continue;

				if ((mcDataType == McDataType.ReferencedField.ToString()) ||
					(mcDataType == McDataType.Reference.ToString()) ||
					(mcDataType == McDataType.BackReference.ToString()) ||
					(mcDataType == McDataType.Card.ToString()) ||
					(mcDataType == McDataType.Link.ToString()) ||
					(mcDataType == McDataType.Complex.ToString()) ||
					(mcDataType == McDataType.MultiReference.ToString()))
					continue;
				else if (mcDataType == McDataType.Identifier.ToString())
					if (DataContext.Current.MetaModel.GetRegisteredTypes(McDataType.Identifier).Length <= 0)
						continue;

				FieldTypeList.Items.Add(new ListItem(CHelper.GetMcDataTypeName(mcDataType), mcDataType));
			}
			if (mf != null)
				CHelper.SafeSelect(FieldTypeList, mf.GetOriginalMetaType().McDataType.ToString());
			else
				CHelper.SafeSelect(FieldTypeList, "String");

			if (FieldTypeList.Items.Count > 0)
			{
				string typeName = String.Empty;
				if (mf != null)
					typeName = mf.TypeName;
				BindFormats(typeName);
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
		/// 
		private void BindFormats()
		{
			BindFormats(String.Empty);
		}

		private void BindFormats(string selectedType)
		{
			FormatList.Items.Clear();

			MetaFieldType[] metaFieldTypes = MetaDataWrapper.GetFieldFormatsByType(FieldTypeList.SelectedValue);
			foreach (MetaFieldType fieldType in metaFieldTypes)
			{
				string text = CHelper.GetResFileString(fieldType.FriendlyName);
				//AK 2008-02-13
				//if (MetaDataBaseSection.CheckMetaFieldTypeVisibility(mc, fieldType))
					FormatList.Items.Add(new ListItem(text, fieldType.Name));
			}

			if (FieldTypeList.SelectedValue == McDataType.Enum.ToString())
			{
				FormatList.Items.Add(new ListItem(String.Format("[{0}]", GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewSingleValue").ToString()), "Enum"));
				FormatList.Items.Add(new ListItem(String.Format("[{0}]", GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewMultiValue").ToString()), "EnumMultiValue"));
			}

			if (!String.IsNullOrEmpty(selectedType))
				CHelper.SafeSelect(FormatList, selectedType);

			if (FieldTypeList.SelectedValue == McDataType.Identifier.ToString()
			  || FieldTypeList.SelectedValue == McDataType.Enum.ToString()
			  || FieldTypeList.SelectedValue == McDataType.Complex.ToString()
			  || FieldTypeList.SelectedValue == McDataType.MultiReference.ToString()
			  || FormatList.Items.Count > 1)
				FormatRow.Visible = true;
			else
				FormatRow.Visible = false;

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

			ResolvedPath resPath = ControlPathResolver.Current.ResolveStrong(metaTypeName, "Manage", "", "", "ListInfoImport");

			// Try to use empty place
			if (resPath == null)
				resPath = ControlPathResolver.Current.Resolve(metaTypeName, "Manage", "", "", "ListInfoImport");

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
					iManageControl.BindData(null, FormatList.SelectedValue);
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
			if (ViewState[currentControlKey] != null)
			{
				Control c = MainPlaceHolder.FindControl("ManageControl");
				if (c == null)
				{
					string controlPath = ViewState[currentControlKey].ToString();
					Control control = (Control)LoadControl(controlPath);
					control.ID = "ManageControl";
					MainPlaceHolder.Controls.Add(control);
				}
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

			if (ViewState["_keyLIP"] != null)
			{
				//lip = (ListImportParameters)CHelper.GetFromContext(ViewState["_keyLIP"].ToString());
				//if (lip == null)
				lip = (ListImportParameters)Session[ViewState["_keyLIP"].ToString()];
				if (lip != null)
				{

					Mediachase.Ibn.Data.Meta.Management.AttributeCollection attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
					string sDefaultValue = String.Empty;

					IManageControl control = MainPlaceHolder.Controls[0] as IManageControl;

					if (control != null)
					{
						sDefaultValue = control.GetDefaultValue(AllowNullsCheckBox.Checked);
						attr = control.FieldAttributes;
					}

					if (!AllowNullsCheckBox.Checked && sDefaultValue == String.Empty)
					{
						if (attr.ContainsKey("NewEnum") || attr.ContainsKey("NewMultiEnum"))
							sDefaultValue = "1";

						ErrorMessage.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ErrorMessage_AllowNulls").ToString();
						return;
					}

					if (sDefaultValue == null)
						sDefaultValue = String.Empty;

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

					string sFriendlyName = FriendlyNameTextBox.Text.Trim();
					if (String.IsNullOrEmpty(sFriendlyName))
						sFriendlyName = sName;

					try
					{
						string typeName = FormatList.SelectedValue;
						if (attr.ContainsKey("NewEnum"))
						{
							string name = attr["EnumName"].ToString();
							string friendlyname = attr["EnumFriendlyName"].ToString();
							bool isPrivate = (bool)attr["EnumPrivate"];
							
							attr.Remove("NewEnum");
							attr.Remove("EnumName");
							attr.Remove("EnumFriendlyName");
							attr.Remove("EnumPrivate");
							
							NewEnumInfo nei = new NewEnumInfo();
							nei.Name = name;
							nei.FriendlyName = friendlyname;
							nei.MultipleValues = false;
							nei.IsPrivate = isPrivate;
							lip.NewEnumTypes.Add(nei);

							typeName = name;
						}
						//NewMultiEnum
						if (attr.ContainsKey("NewMultiEnum"))
						{
							string name = attr["EnumName"].ToString();
							string friendlyname = attr["EnumFriendlyName"].ToString();
							bool isPrivate = (bool)attr["EnumPrivate"];
							
							attr.Remove("NewMultiEnum");
							attr.Remove("EnumName");
							attr.Remove("EnumFriendlyName");
							attr.Remove("EnumPrivate");

							NewEnumInfo nei = new NewEnumInfo();
							nei.Name = name;
							nei.FriendlyName = friendlyname;
							nei.MultipleValues = true;
							nei.IsPrivate = isPrivate;
							lip.NewEnumTypes.Add(nei);

							typeName = name;
						}

						MetaFieldType mft = DataContext.Current.MetaModel.RegisteredTypes[FormatList.SelectedValue];
						if (mft != null && mft.McDataType == McDataType.Enum &&
							attr.ContainsKey("EnumFriendlyName"))
						{
							mft.FriendlyName = attr["EnumFriendlyName"].ToString();
							attr.Remove("EnumFriendlyName");
						}

						MappingRule mr = null;
						if (ViewState["_field"] != null)
						{
							mr = lip.GetRuleByMetaField(ViewState["_field"].ToString());
							lip.RemoveNewMetaField(ViewState["_field"].ToString());
						}

						lip.AddNewMetaField(sName, sFriendlyName, typeName, AllowNullsCheckBox.Checked, sDefaultValue, attr);
						if (mr != null)
							mr.FieldName = sName;
						Session[ViewState["_keyLIP"].ToString()] = lip;
						CommandParameters cp = new CommandParameters(_commandName);
						Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
						//CHelper.AddToContext("NeedToChangeDataSource", true);
					}
					catch (MetaFieldAlreadyExistsException)
					{
					}

					//NameTextBox.Text = String.Empty;
					//FriendlyNameTextBox.Text = String.Empty;
				}
			}
		}
		#endregion
	}
}