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
	public partial class Currency_Filter__Between : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			textContainer.Style.Add(HtmlTextWriterStyle.Display, "inline");

			textContainer.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			tbText1.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbText1, "")));
			tbText2.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbText2, "")));

			tbText1.TextChanged += new EventHandler(tbText1_TextChanged);
			tbText2.TextChanged += new EventHandler(tbText2_TextChanged);

			lblAnd.Text = CHelper.GetResFileString("{IbnFramework.Common:tFilterAnd}");
			lblError.Text = string.Empty;
		}

		#region tbText2_TextChanged
		/// <summary>
		/// Handles the TextChanged event of the tbText2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void tbText2_TextChanged(object sender, EventArgs e)
		{
			decimal val = 0;
			if (decimal.TryParse(tbText1.Text, out val) && decimal.TryParse(tbText2.Text, out val))
			{
				lblError.Text = string.Empty;
				this.Value = new decimal[] { Convert.ToDecimal(tbText1.Text), Convert.ToDecimal(tbText2.Text) }; ;
				this.RaiseBubbleEvent(this, e);
			}
			else
			{
				lblError.Text = "*";
			}

			BindFromValue();
		} 
		#endregion

		#region tbText1_TextChanged
		/// <summary>
		/// Handles the TextChanged event of the tbText1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void tbText1_TextChanged(object sender, EventArgs e)
		{
			decimal val = 0;
			if (decimal.TryParse(tbText1.Text, out val) && decimal.TryParse(tbText2.Text, out val))
			{
				lblError.Text = string.Empty;
				this.Value = new decimal[] { Convert.ToDecimal(tbText1.Text), Convert.ToDecimal(tbText2.Text) };
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
				lblText2.Text = "0.0";
				this.Value = new decimal[] { 0, 0 };
			}
			else
			{
				decimal[] val = (decimal[])this.Value;
				
				lblText.Text = val.GetValue(0).ToString();
				tbText1.Text = val.GetValue(0).ToString();

				lblText2.Text = val.GetValue(1).ToString();
				tbText2.Text = val.GetValue(1).ToString();
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