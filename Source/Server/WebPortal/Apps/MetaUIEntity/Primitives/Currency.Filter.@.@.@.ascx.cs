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

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class Currency_Filter___ : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			lblText.Style.Add(HtmlTextWriterStyle.Display, "inline");

			lblText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			tbText1.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbText1, "")));
			tbText1.TextChanged += new EventHandler(tbText1_TextChanged);
			lblError.Text = string.Empty;
		}

		#region tbText1_TextChanged
		/// <summary>
		/// Handles the TextChanged event of the tbText1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void tbText1_TextChanged(object sender, EventArgs e)
		{
			decimal val = 0;
			if (decimal.TryParse(tbText1.Text, out val))
			{
				lblError.Text = string.Empty;
				this.Value = val;
				//BindFromValue();
				this.RaiseBubbleEvent(this, e);
			}
			else
			{
				lblError.Text = "*";				
			}

			BindFromValue();
		} 
		#endregion

		#region BindFromValue
		/// <summary>
		/// Binds from value.
		/// </summary>
		private void BindFromValue()
		{
			if (this.Value == null)
			{
				lblText.Text = "0.0";
				this.Value = 0;
			}
			else
			{
				decimal val = decimal.Parse(this.Value.ToString());
				lblText.Text = val.ToString("#.##");
				tbText1.Text = val.ToString("#.##"); 
			}
		}
		#endregion

		#region IConditionValue Members

		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node, ConditionElement condition)
		{
			BindFromValue();
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