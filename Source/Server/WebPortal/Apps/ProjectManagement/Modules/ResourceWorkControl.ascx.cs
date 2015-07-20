using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class ResourceWorkControl : System.Web.UI.UserControl
	{
		#region UserId
		public int UserId
		{
			set
			{
				ViewState["UserId"] = value;
			}
			get
			{
				int retval = -1;
				if (ViewState["UserId"] != null)
					retval = (int)ViewState["UserId"];
				return retval;
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strListView", Assembly.GetExecutingAssembly());
		private int lastDay = 0;
		private bool isWhite = false;

		protected void Page_Load(object sender, EventArgs e)
		{
			ctrlGrid.InnerGrid.RowDataBound += new GridViewRowEventHandler(InnerGrid_RowDataBound);

			if (PortalConfig.GeneralAllowGeneralCategoriesField)
			{
				ctrlGrid.ViewName = "AllFields";
			}
			else
			{
				ctrlGrid.ViewName = "NoCategories";
			}

			string scriptKey = "MyWorkRefresh";
			if (!Page.ClientScript.IsClientScriptBlockRegistered(scriptKey))
			{
				StringBuilder builder = new StringBuilder(152);
				builder.Append("<script type=\"text/javascript\">\r\n");
				builder.Append("\tfunction MyWorkRefresh(params) {\r\n");
				builder.Append("\t\t");
				builder.Append(Page.ClientScript.GetPostBackEventReference(RefreshGridButton, ""));
				builder.Append("\r\n");
				builder.Append("\t}\r\n");
				builder.Append("</script>");

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), scriptKey, builder.ToString());
			}

			BindData(!IsPostBack);
		}

		#region BindData
		public void BindData(bool dataBind)
		{
			if (UserId <= 0)
			{
				ctrlGrid.Visible = false;
				InfoLabel.Visible = true;
				InfoLabel.Text = GetGlobalResourceObject("IbnFramework.Calendar", "NoUsersInGroup").ToString();
			}
			else if (Mediachase.IBN.Business.Calendar.CheckUserCalendar(UserId))
			{
				ArrayList objectTypes = new ArrayList();
				objectTypes.Add(ObjectTypes.Task);
				objectTypes.Add(ObjectTypes.ToDo);
				objectTypes.Add(ObjectTypes.Issue);
				objectTypes.Add(ObjectTypes.Document);
				objectTypes.Add(ObjectTypes.CalendarEntry);

				DateTime userNow = Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
				DataTable dtSource = Mediachase.IBN.Business.Calendar.GetResourceUtilization(
					UserId,
					userNow,
					userNow,
					userNow.AddMonths(1),
					objectTypes,
					null,
					false,
					false,
					true,
					false);

				// Remove Events and merge repeated objects
				DataTable dt = dtSource.Clone();
				int lastObjectId = -1;
				int lastObjectTypeId = (int)ObjectTypes.UNDEFINED;
				string lastAssignmentId = string.Empty;
				foreach (DataRow row in dtSource.Rows)
				{
					if ((int)row["ObjectTypeId"] != (int)ObjectTypes.CalendarEntry)
					{
						if (lastObjectId != (int)row["ObjectId"] || lastObjectTypeId != (int)row["ObjectTypeId"] || lastAssignmentId != row["AssignmentId"].ToString())
						{
							dt.ImportRow(row);
							lastObjectId = (int)row["ObjectId"];
							lastObjectTypeId = (int)row["ObjectTypeId"];
							lastAssignmentId = row["AssignmentId"].ToString();
						}
						else // merging
						{
							dt.Rows[dt.Rows.Count - 1]["Finish"] = row["Finish"];
						}
					}
				}
				//

				// Specify PrimaryKeyId = ObjectTypeId:ObjectIds
				dt.Columns.Add(new DataColumn("PrimaryKeyId", typeof(string)));
				foreach (DataRow row in dt.Rows)
				{
					if (row["ObjectTypeId"] != DBNull.Value && row["ObjectId"] != DBNull.Value)
						row["PrimaryKeyId"] = String.Concat(row["ObjectTypeId"].ToString(), ":", (string)row["ObjectId"].ToString());
				}
				
				DataView dv = dt.DefaultView;

				if (dv.Count == 0)
				{
					ctrlGrid.Visible = false;
					InfoLabel.Visible = true;

					InfoLabel.Text = LocRM.GetString("tNoItems");
				}
				else
				{
					ctrlGrid.Visible = true;
					InfoLabel.Visible = false;

					ctrlGrid.DataSource = dv;
					if (dataBind)
						ctrlGrid.DataBind();
				}
			}
			else
			{
				ctrlGrid.Visible = false;
				InfoLabel.Visible = true;

				if (Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Administrator) || Mediachase.IBN.Business.Security.CurrentUser.UserID == UserId)
				{
					InfoLabel.Text = String.Format(CultureInfo.InvariantCulture,
						"{0} <a href='{1}?UserID={2}&Tab=5'>{3}</a>",
						GetGlobalResourceObject("IbnFramework.Calendar", "NoUserCalendar"),
						ResolveClientUrl("~/Directory/UserView.aspx"),
						UserId,
						GetGlobalResourceObject("IbnFramework.Calendar", "CreateCalendar"));
				}
				else
				{
					InfoLabel.Text = GetGlobalResourceObject("IbnFramework.Calendar", "NoUserCalendar").ToString();
				}
			}
		}
		#endregion

		#region InnerGrid_RowDataBound
		void InnerGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow && ((System.Data.DataRowView)(e.Row.DataItem)).Row["Start"] != DBNull.Value)
			{
				DateTime start = (DateTime)((System.Data.DataRowView)(e.Row.DataItem)).Row["Start"];
				if (start.Day != lastDay)
				{
					isWhite = !isWhite;
					lastDay = start.Day;
				}

				if (!isWhite)
					e.Row.CssClass = "ibn-alternating";
			}
		}
		#endregion

		#region RefreshGridButton_Click
		protected void RefreshGridButton_Click(object sender, EventArgs e)
		{
			BindData(true);
		}
		#endregion
	}
}