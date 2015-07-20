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
using System.Globalization;

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class DateTime_Filter___ : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			textContainer.Style.Add(HtmlTextWriterStyle.Display, "inline");

			textContainer.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none'; ");

			ctrlPicker.ValueChange += new Mediachase.Ibn.Web.UI.PickerControl.ValueChanged(ctrlPicker_ValueChange);
			ctrlPicker.InnerTimeControl.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(imgOk, "")));
			imgOk.Click += new EventHandler(imgOk_Click);
		}

		void imgOk_Click(object sender, EventArgs e)
		{
			this.Value = ctrlPicker.SelectedDate;
			lblText.Text = ctrlPicker.SelectedDate.ToString("g");
			this.RaiseBubbleEvent(this, e);			
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (this.Value != null)
				lblText.Text = Convert.ToDateTime(this.Value, CultureInfo.InvariantCulture).ToString("g");
		}

		#region ctrlPicker_ValueChange
		/// <summary>
		/// Handles the ValueChange event of the ctrlPicker control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ctrlPicker_ValueChange(object sender, EventArgs e)
		{
			this.Value = ctrlPicker.SelectedDate;
			lblText.Text = ctrlPicker.SelectedDate.ToString("g");
			this.RaiseBubbleEvent(this, e);
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
				lblText.Text = DateTime.Now.ToShortDateString();
				this.Value = DateTime.Now;
			}
			else
			{
				DateTime dt = Convert.ToDateTime(this.Value, CultureInfo.InvariantCulture);

				ctrlPicker.SelectedDate = dt;
				lblText.Text = dt.ToString("g");
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