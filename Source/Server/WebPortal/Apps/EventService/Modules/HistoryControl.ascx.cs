using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.EventService
{
	public partial class HistoryControl : MCDataBoundControl
	{
		#region PageSize
		public int PageSize
		{
			get
			{
				if (ViewState["__pageSize"] == null)
					ViewState["__pageSize"] = 10;
				return (int)ViewState["__pageSize"];
			}
			set
			{
				ViewState["__pageSize"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region DataBind
		public override void DataBind()
		{
			BusinessObject bo = (BusinessObject)DataItem;
			if (bo == null)
				return;

			Mediachase.Ibn.Data.Services.EventService es = bo.GetService<Mediachase.Ibn.Data.Services.EventService>();
			if (es == null)
			{
				this.Visible = false;
				return;
			}
			ICollection<EventGroup> mas = es.LoadEvents();

			grdMain.DataSource = mas;
			grdMain.PageSize = PageSize;
			object pIndex = CHelper.GetFromContext(this.ClientID + "_NewPageIndex");
			if (pIndex != null)
			{
				int iIndex = int.Parse(pIndex.ToString());
				CHelper.RemoveFromContext(this.ClientID + "_NewPageIndex");
				grdMain.PageIndex = iIndex;
			}
			int pageIndex = mas.Count / grdMain.PageSize;
			if (pageIndex > 0 && mas.Count % grdMain.PageSize == 0)
				pageIndex = pageIndex - 1;
			if (grdMain.PageIndex > pageIndex)
				grdMain.PageIndex = pageIndex;

			grdMain.DataBind();
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			CHelper.RequireDataBind();
		}
		#endregion

		#region grdMain_PageIndexChanged
		protected void grdMain_PageIndexChanged(object sender, EventArgs e)
		{
			CHelper.RequireDataBind();
		}
		#endregion

		#region grdMain_PageIndexChanging
		protected void grdMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			CHelper.AddToContext(this.ClientID + "_NewPageIndex", e.NewPageIndex.ToString());
			grdMain.PageIndex = e.NewPageIndex;
		}
		#endregion

		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is BusinessObject)
			{
				BusinessObject bo = (BusinessObject)dataItem;
				Mediachase.Ibn.Data.Services.EventService es = bo.GetService<Mediachase.Ibn.Data.Services.EventService>();
				if (es == null || es.LoadEvents().Length == 0)
					return false;
			}
			return base.CheckVisibility(dataItem);
		}
	}
}