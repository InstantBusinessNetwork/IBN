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

namespace Mediachase.UI.Web.Apps.MetaUIEntity.Primitives
{
	public partial class Decimal_Filter___ : System.Web.UI.UserControl, IConditionValue
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
			CultureInfo ci = CultureInfo.CurrentCulture;
			double val = 0;

			int startPos = tbText1.Text.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
			string valStr = tbText1.Text;
			if (startPos >= 0 && startPos + this.Scale + 1 <= valStr.Length)
			{
				valStr = valStr.Substring(0, startPos + this.Scale + 1);
			}

			if (double.TryParse(valStr, out val))
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
				double val = double.Parse(this.Value.ToString());
				lblText.Text = val.ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture) + "%";
				tbText1.Text = val.ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture);
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