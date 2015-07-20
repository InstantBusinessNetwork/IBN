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
	public partial class IntegerPercent_Filter : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			textContainer.Style.Add(HtmlTextWriterStyle.Display, "inline");

			textContainer.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			tbText1.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbText1, "")));

			tbText1.TextChanged += new EventHandler(tbText1_TextChanged);
		}

		#region tb_Events

		void tbText1_TextChanged(object sender, EventArgs e)
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
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			int elem = Convert.ToInt32(tbText1.Text);
			this.Value = elem;
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
				tbText1.Text = "0%";
				this.Value = 0;
			}
			else
			{
				int elem = int.Parse(this.Value.ToString());
				tbText1.Text = elem.ToString();
			}

			lblText.Text = tbText1.Text + "%";
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