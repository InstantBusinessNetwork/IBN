using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls
{
	public partial class HistoryFieldAdd : System.Web.UI.UserControl//, IModalPopupControl
	{
		#region ClassName
		protected string ClassName
		{
			get
			{
				return Request["ClassName"];
			}
		}
		#endregion

		#region CommandName
		protected string CommandName
		{
			get
			{
				if(Request["CommandName"] != null)
					return Request["CommandName"];
				return String.Empty;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if(!Page.IsPostBack)
				BindList();
		}

		#region BindList
		private void BindList()
		{
			FieldList.Items.Clear();

			if (!String.IsNullOrEmpty(ClassName))
			{
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(ClassName);

				HistoryMetaClassInfo historyInfo = HistoryManager.GetInfo(mc);
				Collection<string> selectedFields = historyInfo.SelectedFields;

				foreach (MetaField mf in mc.Fields)
				{
					if (HistoryManager.IsSupportedField(mf) && !selectedFields.Contains(mf.Name))
						FieldList.Items.Add(new ListItem(CHelper.GetMetaFieldName(mf), mf.Name));
				}
			}
		}
		#endregion

		#region SaveButton_Click
		protected void SaveButton_Click(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(ClassName))
			{
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(ClassName);

				HistoryMetaClassInfo historyInfo = HistoryManager.GetInfo(mc);
				historyInfo.SelectedFields.Add(FieldList.SelectedValue);
				HistoryManager.SetInfo(mc, historyInfo);

				ListViewProfile[] mas = ListViewProfile.GetSystemProfiles(HistoryManager.GetHistoryMetaClassName(ClassName), "ItemHistoryList");
				if (mas.Length == 0)
				{
					CHelper.GetHistorySystemListViewProfile(HistoryManager.GetHistoryMetaClassName(ClassName), "ItemHistoryList");
					mas = ListViewProfile.GetSystemProfiles(HistoryManager.GetHistoryMetaClassName(ClassName), "ItemHistoryList");
				}
				if (!mas[0].FieldSet.Contains(FieldList.SelectedValue))
				{
					mas[0].FieldSet.Add(FieldList.SelectedValue);
					mas[0].ColumnsUI.Add(new ColumnProperties(FieldList.SelectedValue, "150px", String.Empty));
					ListViewProfile.SaveSystemProfile(HistoryManager.GetHistoryMetaClassName(ClassName), "ItemHistoryList", Mediachase.IBN.Business.Security.CurrentUser.UserID, mas[0]);
				}

				CommandParameters cp = new CommandParameters(CommandName);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
				//CHelper.UpdateModalPopupContainer(this, ContainerId);
				//CHelper.RequireDataBind();
			}
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			//if (CHelper.NeedToDataBind())
			//    BindList();

			CancelButton.OnClientClick = String.Format("javascript:{{{0};}}", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			//SaveButton.OnClientClick = String.Format("javascript:{{{0};}}", ScriptHidePopup);
		}
		#endregion
	}
}