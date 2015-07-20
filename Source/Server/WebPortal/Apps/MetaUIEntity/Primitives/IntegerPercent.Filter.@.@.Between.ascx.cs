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
	public partial class IntegerPercent_Filter__Between : System.Web.UI.UserControl, IConditionValue
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
		}

		#region tb_Events
		void tbText2_TextChanged(object sender, EventArgs e)
		{
			GetValue();
			this.RaiseBubbleEvent(this, e);
		}

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
			int[] elem = new int[] { Convert.ToInt32(tbText1.Text), Convert.ToInt32(tbText2.Text) };
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
				tbText2.Text = "0%";
				this.Value = new int[] { 0, 0 };
			}
			else
			{
				int[] elem = (int[])this.Value;
				if (elem.Length >= 1)
				{
					tbText1.Text = elem.GetValue(0).ToString();
				}

				if (elem.Length == 2)
				{
					tbText2.Text = elem.GetValue(1).ToString();
				}
			}

			lblText.Text = tbText1.Text + "%";
			lblText2.Text = tbText2.Text + "%";
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