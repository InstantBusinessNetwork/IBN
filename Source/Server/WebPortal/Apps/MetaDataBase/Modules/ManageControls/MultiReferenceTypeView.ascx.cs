using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MultiReferenceTypeView : System.Web.UI.UserControl
	{

		protected MetaFieldType mft = null;

		#region TypeName
		private string _type = "";
		public string TypeName
		{
			get { return _type; }
			set { _type = value; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();
			if (!IsPostBack)
				BindData();

			BindToolbar();
		}
		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["type"] != null)
			{
				TypeName = Request.QueryString["type"];
				mft = MetaDataWrapper.GetTypeByName(TypeName);
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "MReferenceTypeInfo").ToString();
			secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/edit.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "MReferenceTypeEdit").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeEdit.aspx?type=" + TypeName + "&back=view");
			secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToList").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeList.aspx");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			lblSystemName.Text = TypeName;
			lblFriendlyName.Text = CHelper.GetResFileString(mft.FriendlyName);

			foreach (MultiReferenceItem mri in MultiReferenceType.GetMultiReferenceItems(mft))
				lbClasses.Text += mri.MetaClassName + "<br />";
		}
		#endregion

	}
}