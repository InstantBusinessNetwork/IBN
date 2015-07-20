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

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Reference_Edit_Organization_PrimaryContactId : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region vldCustom_ServerValidate
		protected void vldCustom_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (!AllowNulls && Value == null)
				args.IsValid = false;
		}
		#endregion

		#region IEditControl Members

		#region Value
		public object Value
		{
			set
			{
				if (value != null)
				{
					PrimaryKeyId iObjectId = PrimaryKeyId.Parse(value.ToString());
					string sReferencedClass = ViewState["ReferencedClass"].ToString();
					refObjects.ObjectType = sReferencedClass;
					refObjects.ObjectId = iObjectId;
				}
			}
			get
			{
				if (refObjects.ObjectId != PrimaryKeyId.Empty)
					return refObjects.ObjectId;
				else
					return null;
			}
		}
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region Label
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region AllowNulls
		public bool AllowNulls
		{
			set
			{
				ViewState["AllowNulls"] = value;
			}
			get
			{
				if (ViewState["AllowNulls"] != null)
					return (bool)ViewState["AllowNulls"];
				else
					return false;
			}
		}
		#endregion

		#region RowCount
		public int RowCount
		{
			set { }
			get { return 1; }
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			set
			{
				refObjects.ReadOnly = value;
			}
			get
			{
				return refObjects.ReadOnly;
			}
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
			string sReferencedClass = field.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
			ViewState["ReferencedClass"] = sReferencedClass;

			refObjects.ObjectTypes = sReferencedClass;
			refObjects.FilterName = "OrganizationId";
			refObjects.FilterValue = (Request["ObjectId"] == null) ? String.Empty : Request["ObjectId"];
		}
		#endregion

		#region FieldName
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
		#endregion

		#region TabIndex
		public short TabIndex
		{
			set
			{
				refObjects.TabIndex = value;
			}
		}
		#endregion
		#endregion
	}
}