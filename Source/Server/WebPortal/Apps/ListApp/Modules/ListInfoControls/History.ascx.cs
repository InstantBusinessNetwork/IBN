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
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls
{
	public partial class History : MCDataBoundControl
	{
		private MetaClass _mc = null;

		#region DataItem
		public override object DataItem
		{
			get
			{
				return base.DataItem;
			}
			set
			{
				if (value is MetaClass)
				{
					mc = value as MetaClass;
				}

				base.DataItem = value;
			}
		}
		#endregion

		#region MetaClass mc
		protected MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState["ClassName"] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState["ClassName"].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState["ClassName"] = value.Name;
				_mc = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ShowHideBlocks();
			}
		}

		#region DataBind
		public override void DataBind()
		{
			HistoryFieldsControl.ClassName = mc.Name;
			base.DataBind();
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				MetaClass mc = (MetaClass)dataItem;
				ListInfo li = ListManager.GetListInfoByMetaClassName(mc.Name);
				if (li != null && li.IsTemplate)
					return false;
			}
			return base.CheckVisibility(dataItem);
		}
		#endregion

		#region ShowHideBlocks
		private void ShowHideBlocks()
		{
			if (ListManager.IsHistoryActivated(mc))
			{
				ServiceIsNotInstalledBlock.Visible = false;
				ServiceIsInstalledBlock.Visible = true;
			}
			else
			{
				ServiceIsNotInstalledBlock.Visible = true;
				ServiceIsInstalledBlock.Visible = false;
			}
		}
		#endregion

		#region InstallHistory_Click
		protected void InstallHistory_Click(object sender, EventArgs e)
		{
			ListManager.ActivateHistory(mc);
			CHelper.RequireDataBind();
		}
		#endregion

		#region UnistallHistory_Click
		protected void UnistallHistory_Click(object sender, EventArgs e)
		{
			ListManager.DeactivateHistory(mc);
			CHelper.RequireDataBind();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (CHelper.NeedToDataBind())
				ShowHideBlocks();
		}
		#endregion
	}
}