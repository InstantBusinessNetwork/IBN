using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.Security.Primitives
{
	public partial class RoleMultiValue_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl Members
		public object Value
		{
			get
			{
				return MainSelectControl.ObjectIds.ToArray();
			}
			set
			{
				if (value != null && value is RolePrincipalCollection)
				{
					RolePrincipalCollection coll = (RolePrincipalCollection)value;
					List<int> principalIdList = new List<int>();

					foreach (RolePrincipal srp in coll)
					{
						principalIdList.Add(srp.PrincipalId.Value);
					}
					MainSelectControl.ObjectIds = principalIdList;
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
			set { LabelCell.Visible = value; }
			get { return LabelCell.Visible; }
		}
		#endregion

		#region Label
		public string Label
		{
			set { MainLabel.Text = value; }
			get { return MainLabel.Text; }
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
			set { LabelCell.Width = value; }
			get { return LabelCell.Width; }
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

				//TODO DELEGATES
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
			if (!AllowNulls && (Value == null || ((int[])Value).Length <= 0))
				args.IsValid = false;
		}
		#endregion
	}
}