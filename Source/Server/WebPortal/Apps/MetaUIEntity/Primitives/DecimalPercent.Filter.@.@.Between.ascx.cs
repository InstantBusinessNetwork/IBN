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
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class DecimalPercent_Filter__Between : System.Web.UI.UserControl, IConditionValue
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
			double val = 0;
			if (double.TryParse(tbText1.Text, out val) && double.TryParse(tbText2.Text, out val))
			{
				lblError.Text = string.Empty;
				this.Value = new double[] { Convert.ToDouble(tbText1.Text), Convert.ToDouble(tbText2.Text) }; ;
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
			CultureInfo ci = CultureInfo.CurrentCulture;
			double val = 0;

			int startPos = tbText1.Text.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
			string valStr1 = tbText1.Text;
			string valStr2 = tbText2.Text;
			if (startPos >= 0 && startPos + this.Scale + 1 <= valStr1.Length)
			{
				valStr1 = valStr1.Substring(0, startPos + this.Scale + 1);
			}

			startPos = tbText2.Text.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
			if (startPos >= 0 && startPos + this.Scale + 1 <= valStr2.Length)
			{
				valStr2 = valStr2.Substring(0, startPos + this.Scale + 1);
			}

			if (double.TryParse(valStr1, out val) && double.TryParse(valStr2, out val))
			{
				lblError.Text = string.Empty;
				this.Value = new double[] { Convert.ToDouble(valStr1), Convert.ToDouble(valStr2) };
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
				lblText.Text = "0.0%";
				lblText2.Text = "0.0%";
				this.Value = new double[] { 0, 0 };
			}
			else
			{
				double[] val = (double[])this.Value;

				lblText.Text = ((double)val.GetValue(0)).ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture) + "%";
				tbText1.Text = ((double)val.GetValue(0)).ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture);

				lblText2.Text = ((double)val.GetValue(1)).ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture) + "%";
				tbText2.Text = ((double)val.GetValue(1)).ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture);
			}
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