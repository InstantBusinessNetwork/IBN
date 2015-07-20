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
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class MultiReference_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ibClear.ImageUrl = CHelper.GetAbsolutePath("/Images/IbnFramework/delete.gif");
			ibSelect.ImageUrl = CHelper.GetAbsolutePath("/Images/IbnFramework/search.gif");
		}
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
		}

		#region vldCustom_ServerValidate
		protected void vldCustom_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (!AllowNulls && Value == null)
				args.IsValid = false;
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			string param = Request["__EVENTARGUMENT"];

			string[] mas = param.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
			if (mas.Length != 2)
				return;
			try
			{
				MultiReferenceObject mro = new MultiReferenceObject(mas[0], PrimaryKeyId.Parse(mas[1]));
				Value = mro;
			}
			catch { }
		}
		#endregion

		#region ibClear_Click
		protected void ibClear_Click(object sender, ImageClickEventArgs e)
		{
			Value = null;
		}
		#endregion


		#region IEditControl Members

		#region Value
		public object Value
		{
			set
			{
				if (value != null && value is MultiReferenceObject && ((MultiReferenceObject)value).HasValue)
				{
					MultiReferenceObject mro = (MultiReferenceObject)value;

					ViewState["ObjectId"] = mro.ObjectId.Value.ToString();
					ViewState["Class"] = mro.ActiveReference.Name;

					lblReference.Text = mro.ObjectTitle;
					tdClear.Visible = !ReadOnly;
				}
				else
				{
					ViewState.Remove("ObjectId");
					ViewState.Remove("Class");
					lblReference.Text = String.Empty;
					tdClear.Visible = false;
				}
			}
			get
			{
				if (ViewState["ObjectId"] != null && ViewState["Class"] != null)
					return new MultiReferenceObject(ViewState["Class"].ToString(), PrimaryKeyId.Parse(ViewState["ObjectId"].ToString()));
				else
					return null;
			}
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

		#region ShowLabel
		public bool ShowLabel
		{
			set {}
			get { return true; }
		}
		#endregion

		#region Label
		public string Label
		{
			set { }
			get { return String.Empty; }
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
				tdSelect.Visible = !value;
				vldCustom.Enabled = !value;
				tdClear.Visible = !value;
			}
			get
			{
				return !tdSelect.Visible;
			}
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			set { }
			get { return String.Empty; }
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
			string url = CHelper.GetAbsolutePath(String.Format("/Apps/MetaUI/Pages/Public/SelectMultiReference.aspx?type={0}&btn={1}", field.GetMetaType().Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, "xxx")));

			ibSelect.OnClientClick = String.Format("OpenPopUpWindow(\"{0}\", 640, 480, 1); return false;", url);
		}
		#endregion

		#region TabIndex
		public short TabIndex
		{
			set
			{
				lblReference.TabIndex = value;
			}
		}
		#endregion
		#endregion
	}
}