using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.IbnNext.TimeTracking;
using IBN45Business = Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class Clone : System.Web.UI.UserControl
	{
		private IBN45Business.UserLightPropertyCollection pc = IBN45Business.Security.CurrentUser.Properties;
		protected int ProjectObjectType = (int)IBN45Business.ObjectTypes.Project;
		private string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindGrid();
			}
		}

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("EntryId", typeof(int)));
			dt.Columns.Add(new DataColumn("ObjectId", typeof(int)));
			dt.Columns.Add(new DataColumn("ObjectTypeId", typeof(int)));
			dt.Columns.Add(new DataColumn("ObjectName", typeof(string)));
			dt.Columns.Add(new DataColumn("BlockTypeInstanceId", typeof(int)));
			DataRow dr;

			DateTime localDate = IBN45Business.User.GetLocalDate(DateTime.UtcNow);
			DateTime srcStartDate = IBN45Business.TimeTracking.GetWeekStart(localDate.AddDays(-7));
			DateTime destStartDate = IBN45Business.TimeTracking.GetWeekStart(localDate);

			TimeTrackingEntry[] srcEntries = TimeTrackingManager.GetEntriesForClone(Mediachase.Ibn.Data.Services.Security.CurrentUserId, srcStartDate, destStartDate);

			int blockTypeInstanceId = -1;
			foreach (TimeTrackingEntry srcEntry in srcEntries)
			{
				if (blockTypeInstanceId != srcEntry.BlockTypeInstanceId)
				{
					blockTypeInstanceId = srcEntry.BlockTypeInstanceId;
					dr = dt.NewRow();
					dr["EntryId"] = -1;
					dr["ObjectId"] = blockTypeInstanceId;
					dr["ObjectTypeId"] = ProjectObjectType;
					dr["ObjectName"] = srcEntry.ParentBlock;
					dr["BlockTypeInstanceId"] = blockTypeInstanceId;
					dt.Rows.Add(dr);
				}

				dr = dt.NewRow();
				dr["EntryId"] = srcEntry.PrimaryKeyId;
				if (srcEntry.Properties["ObjectId"].Value != null && srcEntry.Properties["ObjectTypeId"].Value != null)
				{
					dr["ObjectId"] = srcEntry.Properties["ObjectId"].Value;
					dr["ObjectTypeId"] = srcEntry.Properties["ObjectTypeId"].Value;
				}
				else
				{
					dr["ObjectId"] = -1;
					dr["ObjectTypeId"] = -1;
				}
				dr["ObjectName"] = srcEntry.Title;
				dr["BlockTypeInstanceId"] = blockTypeInstanceId;
				dt.Rows.Add(dr);
			}

			MainGrid.DataSource = dt.DefaultView;
			MainGrid.DataBind();

			foreach (DataGridItem dgi in MainGrid.Items)
			{
				CheckBox cb;
				if (dgi.Cells[1].Text == ProjectObjectType.ToString(CultureInfo.InvariantCulture))
				{
					dgi.BackColor = Color.FromArgb(240, 240, 240);
					dgi.Font.Bold = true;

					// Add the OnClick handler
					cb = (CheckBox)dgi.FindControl("chkElement");
					if (cb != null)
					{
						cb.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "CheckChildren(this, {0})", dgi.Cells[2].Text));
					}
				}
			}
		}
		#endregion

		#region AddButton_Click
		protected void AddButton_Click(object sender, EventArgs e)
		{
			DateTime localDate = IBN45Business.User.GetLocalDate(DateTime.UtcNow);
			DateTime destStartDate = IBN45Business.TimeTracking.GetWeekStart(localDate);

			List<int> entryIdList = new List<int>();
			foreach (DataGridItem dgi in MainGrid.Items)
			{
				CheckBox cb = (CheckBox)dgi.FindControl("chkElement");
				if (cb != null && cb.Checked && dgi.Cells[1].Text != ProjectObjectType.ToString(CultureInfo.InvariantCulture))
				{
					entryIdList.Add(int.Parse(dgi.Cells[0].Text, CultureInfo.InvariantCulture));
				}
			}
			TimeTrackingManager.AddEntries(destStartDate, Mediachase.Ibn.Data.Services.Security.CurrentUserId, entryIdList);

			// After rebind the selected items will be disappeared.
			BindGrid();

			// Refresh parent window
			CommandParameters cp = new CommandParameters("MC_TimeTracking_CopyPrevWeekFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentFromFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (MainGrid.Items.Count > 0)
			{
				MainGrid.Visible = true;
				NoItemsDiv.Visible = false;
				AddButton.Visible = true;
			}
			else
			{
				MainGrid.Visible = false;
				NoItemsDiv.Visible = true;
				AddButton.Visible = false;
			}
		}
		#endregion

		#region CloseButton_Click
		protected void CloseButton_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters("MC_TimeTracking_CopyPrevWeekFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}