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
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class PendingMessages : System.Web.UI.UserControl
	{
		private const string _className = "PendingMessage";
		private string _viewName = "";
		private const string _placeName = "PendingMessageList";
		protected UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			grdMain.ClassName = _className;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;

			MainMetaToolbar.ClassName = _className;
			MainMetaToolbar.ViewName = _viewName;
			MainMetaToolbar.PlaceName = _placeName;

			BindDataGrid(!Page.IsPostBack);

			if (!Page.IsPostBack)
				BindBlockHeader();

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand(_className, _viewName, _placeName, "MC_Pend_Selected_AddToExistServer");
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (CHelper.NeedToBindGrid())
				BindDataGrid(true);

			base.OnPreRender(e);
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.Incident", "PendingMessages").ToString();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			DataView dv = EMailMessage.GetPendingMessages().DefaultView;
			grdMain.DataSource = dv;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion
	}
}