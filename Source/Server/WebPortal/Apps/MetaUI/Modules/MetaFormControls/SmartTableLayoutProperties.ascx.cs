using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class SmartTableLayoutProperties : System.Web.UI.UserControl
	{
		#region Columns
		public string Columns
		{
			get
			{
				string retVal = String.Empty;
				if (!rb11.Disabled && rb11.Checked)
					retVal = "50%;*";

				if (!rb12.Disabled && rb12.Checked)
					retVal = "35%;*";

				if (!rb21.Disabled && rb21.Checked)
					retVal = "*;35%";

				if (!rb1.Disabled && rb1.Checked)
					retVal = "*";
				return retVal;
			}
			set
			{
				switch (value)
				{
					case "*":
					case "100%":
						rb1.Checked = true;
						break;
					case "50%;*":
					case "50%;50%":
					case "*;50%":
						rb11.Checked = true;
						rb1.Disabled = true;
						break;
					case "35%;*":
						rb12.Checked = true;
						rb1.Disabled = true;
						break;
					case "*;35%":
						rb21.Checked = true;
						rb1.Disabled = true;
						break;
					default:
						rb1.Disabled = true;
						rb11.Disabled = true;
						rb12.Disabled = true;
						rb21.Disabled = true;

						rb1.Checked = false;
						break;
				}
			}
		}
		#endregion

		#region CellPadding
		public int CellPadding
		{
			get
			{
				int retVal = 5;
				try
				{
					retVal = int.Parse(txtCellPadding.Text);
				}
				catch { }
				return retVal;
			}
			set
			{
				txtCellPadding.Text = value.ToString();
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(txtCellPadding.Text))
				txtCellPadding.Text = "5";
		}
	}
}