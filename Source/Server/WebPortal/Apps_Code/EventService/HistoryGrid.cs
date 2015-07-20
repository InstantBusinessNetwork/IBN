using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.EventService
{
	public class HistoryGrid : GridView
	{
		public HistoryGrid()
		{
			this.Columns.Add(new TemplateField());
			this.AutoGenerateColumns = false;
			this.GridLines = GridLines.None;
			this.Width = Unit.Percentage(100);
		}

		#region OnBubbleEvent
		protected override bool OnBubbleEvent(object sender, EventArgs e)
		{
			bool handled = false;

			return handled;
		}
		#endregion

		#region OnRowCreated
		protected override void OnRowCreated(GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				string sControl;
				sControl = "~/Apps/EventService/Modules/row_default.ascx";

				e.Row.Cells.Clear();

				DataControlField field = this.Columns[0];
				((TemplateField)field).ItemTemplate = this.Page.LoadTemplate(sControl);

				DataControlField[] fields = new DataControlField[1] { field };
				this.InitializeRow(e.Row, fields);
			}
			base.OnRowCreated(e);
		}
		#endregion
	}
}
