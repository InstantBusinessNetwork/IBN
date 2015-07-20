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
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.DocumentManagement.Primitives
{
	public partial class Text_Edit_DocumentType_Name : System.Web.UI.UserControl, IEditControl
	{
		#region ObjectId
		protected PrimaryKeyId? ObjectId
		{
			get
			{
				PrimaryKeyId value;
				string s = String.Empty;
				if (Request["ObjectId"] != null)
					s = MetaViewGroupUtil.GetIdFromUniqueKey(Request["ObjectId"]);
				if (PrimaryKeyId.TryParse(s, out value))
				{
					if (value != PrimaryKeyId.Empty)
						return value;
					else
						return null;
				}
				else
					return null;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			uniqValue_Required.ErrorMessage = GetGlobalResourceObject("IbnFramework.Global", "MustBeUnique").ToString();
		}

		protected void uniqValue_Required_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;
			if (ViewState[FieldName + "FieldClassName"] != null)
			{
				string realName = FieldName;
				if (FieldName.Contains("."))
					realName = FieldName.Substring(FieldName.IndexOf(".") + 1);
				MetaField field = MetaDataWrapper.GetMetaFieldByName(ViewState[FieldName + "FieldClassName"].ToString(), realName);
				if (!MetaObjectProperty.CheckUniqueValue(field, ObjectId, this.Value))
					args.IsValid = false;
			}
		}

		#region IEditControl Members

		public bool AllowNulls
		{
			set
			{
				vldValue_Required.Visible = !value;
			}
			get
			{
				return !vldValue_Required.Visible;
			}
		}

		public void BindData(MetaField field)
		{
			if (field.Attributes.ContainsKey(McDataTypeAttribute.StringMaxLength))
				txtValue.MaxLength = (int)field.Attributes[McDataTypeAttribute.StringMaxLength];
			if (field.Attributes.GetValue<bool>(McDataTypeAttribute.StringIsUnique, false))
				ViewState[FieldName + "FieldClassName"] = field.Owner.Name;
		}

		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		}

		public string Label
		{
			set { }
			get { return ""; }
		}

		public string LabelWidth
		{
			set { }
			get { return ""; }
		}

		public bool ReadOnly
		{
			set
			{
				txtValue.Enabled = !value;
				vldValue_Required.Enabled = !value;
				if (value)
				{
					txtValue.CssClass = "text-readonly";
				}
			}
			get
			{
				return !txtValue.Enabled;
			}
		}

		public int RowCount
		{
			set { }
			get { return 1; }
		}

		public bool ShowLabel
		{
			set { }
			get { return true; }
		}

		#region TabIndex
		public short TabIndex
		{
			set
			{
				txtValue.TabIndex = value;
			}
		}
		#endregion

		public object Value
		{
			set
			{
				if (value != null)
				{
					string sValue = value.ToString();
					if (sValue.IndexOf("Document_") >= 0)
						sValue = sValue.Substring("Document_".Length);
					txtValue.Text = sValue;
				}
			}
			get
			{
				if (AllowNulls && txtValue.Text.Trim() == String.Empty)
					return null;
				else
					return String.Format("Document_{0}", txtValue.Text.Trim());
			}
		}

		#endregion
	}
}