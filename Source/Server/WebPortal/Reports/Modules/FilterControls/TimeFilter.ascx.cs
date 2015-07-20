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
using System.Resources;
using System.Reflection;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.Interfaces;

namespace Mediachase.UI.Web.Reports.Modules
{
	public partial class TimeFilter : System.Web.UI.UserControl, IFilterControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strXMLReport", Assembly.GetExecutingAssembly());
		public HtmlTableCell tdTitle
		{
			get { return Migrated_tdTitle; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IFilterControl Members

		public object Value
		{
			get
			{
				object retval = null;
				if (int.Parse(ddType.SelectedValue) < 3 && dtcFromDate.Value.Ticks > 0)
				{
					TimeSpan ts = new TimeSpan(dtcFromDate.Value.Ticks);
					retval = new TimeFilterValue(ddType.SelectedValue, ((int)ts.TotalMinutes).ToString(), "");
				}
				else if (ddType.SelectedValue == "3" && dtcFromDate.Value.Ticks > 0 && dtcToDate.Value.Ticks > 0)
				{
					TimeSpan ts1 = new TimeSpan(dtcFromDate.Value.Ticks);
					TimeSpan ts2 = new TimeSpan(dtcToDate.Value.Ticks);
					retval = new TimeFilterValue(ddType.SelectedValue, ((int)ts1.TotalMinutes).ToString(), ((int)ts2.TotalMinutes).ToString());
				}
				return retval;
			}
			set
			{
				if (value != null)
				{
					TimeFilterValue tfValue = (TimeFilterValue)value;
					if (tfValue.TypeValue == "3")
					{
						dtcToDate.Value = DateTime.MinValue.AddMinutes(int.Parse(tfValue.SecondValue));
						tdSecond.Style.Add("display", "");
					}
					else
						tdSecond.Style.Add("display", "none");
					CommonHelper.SafeSelect(ddType, tfValue.TypeValue);
					dtcFromDate.Value = DateTime.MinValue.AddMinutes(int.Parse(tfValue.FirstValue));
				}
				else
				{
					CommonHelper.SafeSelect(ddType, "0");
					tdSecond.Style.Add("display", "none");
					dtcFromDate.Value = DateTime.MinValue;
					dtcToDate.Value = DateTime.MinValue.AddHours(3);
				}
			}
		}

		public string FilterTitle
		{

			get
			{
				return lblTitle.Text;
			}
			set
			{
				lblTitle.Text = value;
			}
		}

		private string filterField = "";
		public string FilterField
		{
			get
			{
				return filterField;

			}
			set
			{
				filterField = value;
			}
		}

		public string FilterType
		{
			get
			{
				return "Time";
			}
		}

		public void InitControl(object reader)
		{
			ddType.Items.Clear();
			ddType.Items.Add(new ListItem(LocRM.GetString("tEqual"), "0"));
			ddType.Items.Add(new ListItem(LocRM.GetString("tGreaterNum"), "1"));
			ddType.Items.Add(new ListItem(LocRM.GetString("tLessNum"), "2"));
			ddType.Items.Add(new ListItem(LocRM.GetString("tBetween"), "3"));
			dtcFromDate.Value = DateTime.MinValue;
			dtcToDate.Value = DateTime.MinValue.AddHours(3);
			tdSecond.Style.Add("display", "none");
		}

		#endregion
	}
}