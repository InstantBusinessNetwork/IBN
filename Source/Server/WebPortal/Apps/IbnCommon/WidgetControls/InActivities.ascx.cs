using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business.WidgetEngine;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class InActivities : System.Web.UI.UserControl
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

			DataTable dt = Mediachase.IBN.Business.ToDo.GetListToDoAndTasksAssignedToUserDataTable((bool)properties.GetValue(this.ID, keyShowActive), fromDate, toDate);
			DataView dv = dt.DefaultView;

			ctrlGrid.DataSource = dv;

			divNoObjects.Visible = false;
			if (dv.Count == 0)
			{
				ctrlGrid.Visible = false;
				lblNoObjects.Text = GetGlobalResourceObject("IbnFramework.Global", "NoToDos").ToString();
				divNoObjects.Visible = true;
			}

			BindLegend();
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