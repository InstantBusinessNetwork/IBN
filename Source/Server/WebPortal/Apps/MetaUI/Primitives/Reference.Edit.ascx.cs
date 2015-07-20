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
using Mediachase.Ibn.Clients;
using Mediachase.IBN.Business.Documents;
using Mediachase.Ibn.Lists;
using Mediachase.IbnNext.TimeTracking;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Reference_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
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
			Value = Request["__EVENTARGUMENT"];
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
				if (value != null)
				{
					PrimaryKeyId iObjectId = PrimaryKeyId.Parse(value.ToString());
					string sReferencedClass = ViewState["ReferencedClass"].ToString();
					if (tblEntity.Visible)
					{
						refObjects.ObjectType = sReferencedClass;
						refObjects.ObjectId = iObjectId;
					}
					else
					{
						ViewState["ObjectId"] = iObjectId.ToString();
						MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);

						EntityObject obj = BusinessManager.Load(mc.Name, iObjectId);
						lblReference.Text = CHelper.GetResFileString(obj[mc.TitleFieldName].ToString());
						//        lnkReference.NavigateUrl = String.Format("~/Common/ObjectView.aspx?class={0}&Id={1}", sReferencedClass, iObjectId);
						tdClear.Visible = !ReadOnly;
					}
				}
				else
				{
					if (!tblEntity.Visible)
					{
						ViewState.Remove("ObjectId");
						lblReference.Text = String.Empty;
						//        lnkReference.NavigateUrl = String.Empty;
						tdClear.Visible = false;
					}
				}
			}
			get
			{
				if (tblEntity.Visible)
				{
					if (refObjects.ObjectId != PrimaryKeyId.Empty)
						return refObjects.ObjectId;
					else
						return null;
				}
				else
				{
					if (ViewState["ObjectId"] != null)
						return PrimaryKeyId.Parse(ViewState["ObjectId"].ToString());
					else
						return null;
				}
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
				tdSelect.Visible = !value;
				vldCustom.Enabled = !value;
				tdClear.Visible = !value;
				
				refObjects.ReadOnly = value;
			}
			get
			{
				if (tblEntity.Visible)
					return refObjects.ReadOnly;
				else
					return !tdSelect.Visible;
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
			if (sReferencedClass == TimeTrackingEntry.GetAssignedMetaClass().Name ||
				sReferencedClass == TimeTrackingBlock.GetAssignedMetaClass().Name ||
				sReferencedClass == TimeTrackingBlockType.GetAssignedMetaClass().Name ||
				sReferencedClass == TimeTrackingBlockTypeInstance.GetAssignedMetaClass().Name ||
				sReferencedClass == Principal.GetAssignedMetaClass().Name ||
				Mediachase.IBN.Business.Security.CurrentUser == null)
			{
				tblEntity.Visible = false;
				string url = ResolveClientUrl(String.Format("~/Apps/MetaUI/Pages/Public/SelectItem.aspx?class={0}&btn={1}", sReferencedClass, Page.ClientScript.GetPostBackEventReference(btnRefresh, "xxx")));

				ibSelect.OnClientClick = String.Format("OpenPopUpWindow(\"{0}\", 640, 480, 1); return false;", url);
			}
			else
			{
				ReferenceUpdatePanel.Visible = false;
				refObjects.ObjectTypes = sReferencedClass;
				if (Request["ContainerFieldName"] != null
					&& field.Name == Request["ContainerFieldName"]
					&& Request["ContainerId"] != null)
					this.Value = PrimaryKeyId.Parse(Request["ContainerId"]);
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

		#region TabIndex
		public short TabIndex
		{
			set
			{
				lblReference.TabIndex = value;
				refObjects.TabIndex = value;
			}
		}
		#endregion
		#endregion
	}
}