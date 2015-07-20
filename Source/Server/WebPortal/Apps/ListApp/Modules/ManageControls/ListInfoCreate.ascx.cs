using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class ListInfoCreate : System.Web.UI.UserControl
	{
		#region ListFolderId
		private int _folderId = -1;
		public int ListFolderId
		{
			get 
			{
				if (_folderId < 0)
				{
					_folderId = 0;
					if (Request.QueryString["ListFolderId"] != null)
					{
						_folderId = int.Parse(Request.QueryString["ListFolderId"]);
					}
					else
					{
						ListFolder folder = ListManager.GetPrivateRoot(Mediachase.Ibn.Data.Services.Security.CurrentUserId);
						_folderId = folder.PrimaryKeyId.Value;
					}
				}
				return _folderId; 
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack && ListFolderId > 0 && !ListManager.CanReadFolder(ListFolderId))
			{
				throw new AccessDeniedException();
			}

			if (!IsPostBack)
			{
				BindLists();
				Page.SetFocus(ListNameTextBox);

				MainBlockHeader.AddLink("~/images/IbnFramework/cancel.gif", 
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Back").ToString(),
					String.Format(CultureInfo.InvariantCulture, "~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId={0}", ListFolderId));
			}

			cbWithData.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tWithData}");
			CancelButton.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "window.location.href='ListInfoList.aspx?ListFolderId={0}';return false;", ListFolderId));
		}

		#region ddTemplates_SelectedIndexChanged
		protected void ddTemplates_SelectedIndexChanged(object sender, EventArgs e)
		{
			int value = int.Parse(ddTemplates.SelectedValue);
			if (value > -1)
			{
				ListInfo li = new ListInfo(value);
				trWithData.Visible = li.IsTemplate && MetaObject.GetTotalCount(ListManager.GetListMetaClass(li.PrimaryKeyId.Value)) > 0;
			}
			else
				trWithData.Visible = false;
		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			trWithData.Visible = false;
			ddTemplates.Items.Clear();
			ddTemplates.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.ListInfo:tNotSet}"), "-1"));
			foreach (ListInfo li in ListManager.GetTemplates())
			{
				ddTemplates.Items.Add(new ListItem(li.Title, li.PrimaryKeyId.Value.ToString()));
			}
			if (ddTemplates.Items.Count == 1)
				upTemplate.Visible = false;

			MetaFieldType enumListType = DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListTypeEnumName];
			MetaFieldType enumListStatus = DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListStatusEnumName];

			foreach (MetaEnumItem mei in enumListType.EnumItems)
				ddType.Items.Add(new ListItem(CHelper.GetResFileString(mei.Name), mei.Handle.ToString()));

			foreach (MetaEnumItem mei in enumListStatus.EnumItems)
				ddStatus.Items.Add(new ListItem(CHelper.GetResFileString(mei.Name), mei.Handle.ToString()));
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			int fid = ListFolderId;
			ListInfo list;

			using (TransactionScope scope = DataContext.Current.BeginTransaction())
			{
				int templateId = int.Parse(ddTemplates.SelectedValue);
				if (templateId > 0)
					list = ListManager.CreateListFromTemplate(templateId, fid, ListNameTextBox.Text.Trim(), cbWithData.Checked);
				else
					list = ListManager.CreateList(fid, ListNameTextBox.Text.Trim());
				
				list.Description = txtDescription.Text;
				list.Properties["ListType"].Value = int.Parse(ddType.SelectedValue);
				list.Properties["Status"].Value = int.Parse(ddStatus.SelectedValue);
				list.Save();

				scope.Commit();
			}
			
			MetaClass mc = ListManager.GetListMetaClass(list);

			string url = String.Format(CultureInfo.InvariantCulture,
				"{0}?class={1}", 
				CHelper.ListAdminPage,
				mc.Name);
			Response.Redirect(url, true);
		}
		#endregion
	}
}