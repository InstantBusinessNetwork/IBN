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
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;
using System.Collections.Generic;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class FloatPercent_Filter__In : System.Web.UI.UserControl, IConditionValue
	{
		#region prop: Scale
		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		/// <value>The scale.</value>
		private int Scale
		{
			get
			{
				if (ViewState["_Scale"] == null)
					return 2;
				return Convert.ToInt32(ViewState["_Scale"].ToString(), CultureInfo.InvariantCulture);
			}
			set { ViewState["_Scale"] = value; }
		}
		#endregion

		#region prop: Precision
		/// <summary>
		/// Gets or sets the precision.
		/// </summary>
		/// <value>The scale.</value>
		private int Precision
		{
			get
			{
				if (ViewState["Precision"] == null)
					return 6;
				return Convert.ToInt32(ViewState["Precision"].ToString(), CultureInfo.InvariantCulture);
			}
			set { ViewState["Precision"] = value; }
		}
		#endregion

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
			CultureInfo ci = CultureInfo.CurrentCulture;
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			string[] elem = taText.Value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries); //tbText.Text.Split(new string[] { divider }, StringSplitOptions.RemoveEmptyEntries);
			List<double> listDouble = new List<double>();
			double tmpVal = 0;

			foreach (string s in elem)
			{
				int startPos = s.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
				string val = s;
				if (startPos >= 0)
				{
					val = val.Substring(0, startPos + this.Scale + 1);
				}
				if (double.TryParse(val, out tmpVal))
				{
					listDouble.Add(tmpVal);
				}
				else
				{
					lblError.Text = "*";
				}
			}


			if (lblError.Text == string.Empty)
			{
				this.Value = listDouble.ToArray();
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
				lblText.Text = "0.0%";
				this.Value = new double[] { 0 };
			}
			else
			{
				double[] elem = (double[])this.Value;

				if (elem.Length == 0)
				{
					lblText.Text = "0.0%";
					//tbText.Text = string.Empty;
					taText.Value = string.Empty;
				}
				else if (elem.Length >= 1)
				{
					double val = (double)((double[])elem).GetValue(0);
					lblText.Text = val.ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture) + "%";
					//tbText.Text = lblText.Text;
					taText.Value = lblText.Text;
				}

				if (elem.Length > 1)
				{
					string tMore = string.Format(" ({0})", CHelper.GetResFileString("{IbnFramework.Common:tFilterMore}"));
					lblText.Text += String.Format(tMore, elem.Length - 1);

					taText.Value = string.Empty;
					foreach (double s in elem)
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
			MetaField mf = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaFieldByName(Mediachase.Ibn.Core.MetaViewGroupUtil.GetIdFromUniqueKey(expressionPlace), node.Key.Split('-')[0]);
			if (mf.Attributes[McDataTypeAttribute.DecimalPrecision] != null && mf.Attributes[McDataTypeAttribute.DecimalScale] != null)
			{
				this.Scale = Convert.ToInt32(mf.Attributes[McDataTypeAttribute.DecimalScale].ToString(), CultureInfo.InvariantCulture);
				this.Precision = Convert.ToInt32(mf.Attributes[McDataTypeAttribute.DecimalPrecision].ToString(), CultureInfo.InvariantCulture);
			}
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