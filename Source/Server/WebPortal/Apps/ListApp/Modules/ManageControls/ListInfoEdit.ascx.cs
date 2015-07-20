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

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class ListInfoEdit : System.Web.UI.UserControl
	{
		ListInfo li;

		#region ClassName
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		protected string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request["class"] != null)
					retval = Request["class"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			li = ListManager.GetListInfoByMetaClassName(ClassName);

			if (!IsPostBack)
			{
				if (!li.IsTemplate && !ListInfoBus.CanAdmin(li.PrimaryKeyId.Value))
					throw new Mediachase.Ibn.AccessDeniedException();

				BindLists();
				BindSavedData();
				Page.SetFocus(ListNameTextBox);

				MainBlockHeader.AddLink("~/images/IbnFramework/cancel.gif",
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Back").ToString(),
					String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.ListAdminPage, ClassName));
			}

			CancelButton.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "window.location.href='ListInfoView.aspx?class={0}';return false;", ClassName));
		}

		#region BindLists
		private void BindLists()
		{
			MetaFieldType enumListType = DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListTypeEnumName];
			MetaFieldType enumListStatus = DataContext.Current.MetaModel.RegisteredTypes[ListManager.ListStatusEnumName];

			foreach (MetaEnumItem mei in enumListType.EnumItems)
				ddType.Items.Add(new ListItem(CHelper.GetResFileString(mei.Name), mei.Handle.ToString()));

			foreach (MetaEnumItem mei in enumListStatus.EnumItems)
				ddStatus.Items.Add(new ListItem(CHelper.GetResFileString(mei.Name), mei.Handle.ToString()));
		}
		#endregion

		#region BindSavedData
		private void BindSavedData()
		{
			ListNameTextBox.Text = li.Title;
			txtDescription.Text = li.Description;
			if (li.ListType.HasValue)
				CHelper.SafeSelect(ddType, li.ListType.ToString());
			CHelper.SafeSelect(ddStatus, li.Status.ToString());
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			ListManager.UpdateList(li, ListNameTextBox.Text, txtDescription.Text, int.Parse(ddType.SelectedValue), int.Parse(ddStatus.SelectedValue));

			string url = String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.ListAdminPage, ClassName);
			Response.Redirect(url, true);			
		}
		#endregion
	}
}