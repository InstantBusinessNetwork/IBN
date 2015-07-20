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
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class Date_Filter__Between : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			textContainer.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");
			textContainer.Style.Add(HtmlTextWriterStyle.Display, "inline");
			container.Style.Add(HtmlTextWriterStyle.Display, "none");

			ctrlPicker.ValueChange += new PickerControl.ValueChanged(ctrlPicker_ValueChange);
			ctrlPicker2.ValueChange += new PickerControl.ValueChanged(ctrlPicker2_ValueChange);

			lblAnd.Text = CHelper.GetResFileString("{IbnFramework.Common:tFilterAnd}");
		}

		#region ctrlPicker2_ValueChange
		/// <summary>
		/// Handles the ValueChange event of the ctrlPicker2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ctrlPicker2_ValueChange(object sender, EventArgs e)
		{
			GetValue();
			this.RaiseBubbleEvent(this, e);
		} 
		#endregion

		#region ctrlPicker_ValueChange
		/// <summary>
		/// Handles the ValueChange event of the ctrlPicker control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ctrlPicker_ValueChange(object sender, EventArgs e)
		{
			GetValue();
			this.RaiseBubbleEvent(this, e);
		} 
		#endregion

		#region GetValue
		/// <summary>
		/// Gets the value.
		/// </summary>
		private void GetValue()
		{
			DateTime[] arr = new DateTime[] { ctrlPicker.SelectedDate, ctrlPicker2.SelectedDate };
			this.Value = arr;
			BindValue();
		} 
		#endregion

		#region BindValue
		/// <summary>
		/// Binds the value.
		/// </summary>
		private void BindValue()
		{
			if (this.Value == null)
			{
				ctrlPicker.SelectedDate = DateTime.Now;
				ctrlPicker2.SelectedDate = DateTime.Now;
				lblText.Text = DateTime.Now.ToShortDateString();
				lblText2.Text = DateTime.Now.ToShortDateString();
				this.Value = new DateTime[] { DateTime.Now, DateTime.Now };
			}
			else
			{
				DateTime dt = ((DateTime[])this.Value)[0];
				DateTime dt2 = ((DateTime[])this.Value)[1];

				ctrlPicker.SelectedDate = dt;
				ctrlPicker2.SelectedDate = dt2;

				lblText.Text = dt.ToShortDateString();
				lblText2.Text = dt2.ToShortDateString();
			}
		}
		#endregion

		#region IConditionValue Members

		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node, ConditionElement condition)
		{
			BindValue();
		}

		#region prop: Value
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value
		{
			get
			{
				return ViewState["_Value"];
			}
			set
			{
				ViewState["_Value"] = value;
			}
		}
		#endregion

		#endregion
	}
}