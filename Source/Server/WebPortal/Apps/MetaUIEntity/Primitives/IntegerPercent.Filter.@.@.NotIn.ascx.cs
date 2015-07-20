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
using System.Collections.Generic;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class IntegerPercent_Filter__NotIn : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			lblText.Style.Add(HtmlTextWriterStyle.Display, "inline");

			lblText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			taText.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(imgOk, "")));
			imgOk.Click += new ImageClickEventHandler(imgOk_Click);

			lblError.Text = string.Empty;
		}

		#region GetValue
		/// <summary>
		/// Gets the value.
		/// </summary>
		private void GetValue()
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			string[] elem = taText.Value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries); //tbText.Text.Split(new string[] { divider }, StringSplitOptions.RemoveEmptyEntries);
			List<int> listInt = new List<int>();
			int tmpVal = 0;

			foreach (string s in elem)
			{
				if (int.TryParse(s, out tmpVal))
				{
					listInt.Add(tmpVal);
				}
				else
				{
					lblError.Text = "*";
				}
			}


			if (lblError.Text == string.Empty)
			{
				this.Value = listInt.ToArray();
				BindFromValue();
			}
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
				lblText.Text = "0";
				this.Value = new int[] { 0 };
			}
			else
			{
				int[] elem = (int[])this.Value;

				if (elem.Length == 0)
				{
					lblText.Text = "0";
					//tbText.Text = string.Empty;
					taText.Value = string.Empty;
				}
				else if (elem.Length >= 1)
				{
					lblText.Text = ((int[])elem).GetValue(0).ToString();
					//tbText.Text = lblText.Text;
					taText.Value = lblText.Text;
				}

				if (elem.Length > 1)
				{
					string tMore = string.Format(" ({0})", CHelper.GetResFileString("{IbnFramework.Common:tFilterMore}"));
					lblText.Text += String.Format(tMore, elem.Length - 1);

					taText.Value = string.Empty;
					foreach (int s in elem)
					{
						taText.Value += s.ToString() + "\r\n";
					}
				}

			}
			//tbText.Text = taText.Value;
		}
		#endregion

		#region imgOk_Click
		/// <summary>
		/// Handles the Click event of the imgOk control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
		void imgOk_Click(object sender, ImageClickEventArgs e)
		{
			GetValue();
			////lblText.Text = this.Value;
			this.RaiseBubbleEvent(this, e);
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