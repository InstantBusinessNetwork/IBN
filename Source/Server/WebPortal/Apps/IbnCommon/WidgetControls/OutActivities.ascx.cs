using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.WidgetEngine;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class OutActivities : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strListView", Assembly.GetExecutingAssembly());

		private const string keyShowActive = "showActive";
		private const string keyType = "type";
		private const string keyFrom = "from";
		private const string keyTo = "to";

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");
			BindDataGrid(!IsPostBack);
		}

		#region BindDataGrid
		/// <summary>
		/// Binds the data grid.
		/// </summary>
		/// <param name="dataBind">if set to <c>true</c> [data bind].</param>
		void BindDataGrid(bool dataBind)
		{
			ControlPropertiesBase properties = ControlProperties.Provider;

			if (properties.GetValue(this.ID, keyShowActive) == null)
				properties.SaveValue(this.ID, keyShowActive, true);

			if (properties.GetValue(this.ID, keyType) == null)
				properties.SaveValue(this.ID, keyType, "365");	// week

			if ((string)properties.GetValue(this.ID, keyType) == "0" && (properties.GetValue(this.ID, keyFrom) == null || properties.GetValue(this.ID, keyTo) == null))
				properties.SaveValue(this.ID, keyType, "365");	// week

			DateTime fromDate = DateTime.Now;
			DateTime toDate = DateTime.Now;

			if ((string)properties.GetValue(this.ID, keyType) != "0")	// period
			{
				int days = int.Parse((string)properties.GetValue(this.ID, keyType), CultureInfo.InvariantCulture);
				fromDate = fromDate.AddDays(-days);
			}
			else
			{
				fromDate = (DateTime)properties.GetValue(this.ID, keyFrom);
				toDate = (DateTime)properties.GetValue(this.ID, keyTo);
			}

			DataTable dt = Mediachase.IBN.Business.ToDo.GetListToDoAndTasksAssignedByUserDataTable((bool)properties.GetValue(this.ID, keyShowActive), fromDate, toDate);

			DataView dv = dt.DefaultView;

			ctrlGrid.DataSource = dv;

			divNoObjects.Visible = false;
			if (dv.Count == 0)
			{
				ctrlGrid.Visible = false;
				lblNoObjects.Text = String.Format("{0} <a href='{1}'>{2}</a>",
					GetGlobalResourceObject("IbnFramework.Global", "NoToDos").ToString(),
					this.Page.ResolveUrl("~/ToDo/ToDoEdit.aspx"),
						GetGlobalResourceObject("IbnFramework.Global", "CreateToDo").ToString());
				divNoObjects.Visible = true;
			}

			BindLegend();
		}
		#endregion

		#region GetResources
		public static string GetResources(int completionType, int isToDo, int itemId, int overallPercentCompleted)
		{
			StringBuilder sb = new StringBuilder();

			int count = 0;
			using (IDataReader reader = ((isToDo == 0) ? Task.GetListResources(itemId) : Mediachase.IBN.Business.ToDo.GetListResources(itemId)))
			{
				while (reader.Read())
				{
					if ((bool)reader["MustBeConfirmed"] && !(bool)reader["ResponsePending"] && !(bool)reader["IsConfirmed"])
						continue;

					string percents = String.Empty;
					if (completionType == (int)CompletionType.All)
						percents = String.Format(CultureInfo.InvariantCulture, "<td style='padding-left:5px; width:45px'>{0}%</td>", reader["PercentCompleted"].ToString());
					else
						percents = String.Format(CultureInfo.InvariantCulture, "<td style='padding-left:5px; width:45px'>{0}%</td>", overallPercentCompleted.ToString());

					if (count == 0)
						sb.AppendLine("<table class='text' cellspacing='0' cellpadding='1' border='0' width='100%'>");

					sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), percents));

					count++;
				}
			}

			if (count > 0)
				sb.AppendLine("</table>");

			return sb.ToString();
		}
		#endregion

		#region BindLegend
		private void BindLegend()
		{
			ControlPropertiesBase properties = ControlProperties.Provider;

			imageFilter.ToolTip = LocRM.GetString("Filter");
			imageFilter.AlternateText = imageFilter.ToolTip;

			if ((bool)properties.GetValue(this.ID, keyShowActive))
				labelLegend.Text = LocRM.GetString("ShowActive");
			else
				labelLegend.Text = LocRM.GetString("ShowAll");

			labelLegend.Text += ", ";

			if ((string)properties.GetValue(this.ID, keyType) == "7")
			{
				labelLegend.Text += LocRM.GetString("ShowLastWeek");
			}
			else if ((string)properties.GetValue(this.ID, keyType) == "30")
			{
				labelLegend.Text += LocRM.GetString("ShowLastMonth");
			}
			else if ((string)properties.GetValue(this.ID, keyType) == "365")
			{
				labelLegend.Text += LocRM.GetString("ShowLastYear");
			}
			else
			{
				labelLegend.Text += String.Format(CultureInfo.InvariantCulture,
					"{0} {1} {2} {3}",
					LocRM.GetString("ShowFrom"),
					((DateTime)properties.GetValue(this.ID, keyFrom)).ToShortDateString(),
					LocRM.GetString("ShowTo"),
					((DateTime)properties.GetValue(this.ID, keyTo)).ToShortDateString());
			}

			labelLegend.ToolTip = LocRM.GetString("Filter");
		}
		#endregion
	}
}