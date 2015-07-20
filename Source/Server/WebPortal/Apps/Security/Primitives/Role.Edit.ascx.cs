using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.Security.Primitives
{
	public partial class Role_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl Members
		public object Value
		{
			get
			{
				if (MainSelectControl.ObjectId > 0)
					return MainSelectControl.ObjectId;
				else
					return null;
			}
			set
			{
				if (value != null && value is RolePrincipal)
				{
					RolePrincipal srp = (RolePrincipal)value;
					MainSelectControl.ObjectId = srp.PrincipalId.Value;
				}
			}
		}

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
				ViewState["ReadOnly"] = value;
			}
			get
			{
				bool retval = false;
				if (ViewState["ReadOnly"] != null)
					retval = (bool)ViewState["ReadOnly"];
				return retval;
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

		#region TabIndex
		public short TabIndex
		{
			set
			{
			}
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
			string cards = String.Empty;
			int[] permittedTypes = (int[])field.Attributes[RoleTypeAttribute.PermittedPrincipalTypes];
			foreach (int i in permittedTypes)
			{
				if (!String.IsNullOrEmpty(cards))
					cards += ",";

				//TODO DELEGATE
				//PrincipalTypes type = (PrincipalTypes)i;
				//cards += type.ToString();
			}

			MainPopupControl.CardFilter = cards;
		}
		#endregion
		#endregion

		#region MainCustomValidator_ServerValidate
		protected void MainCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (!AllowNulls && Value == null)
				args.IsValid = false;
		}
		#endregion
	}
}