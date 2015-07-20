using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.Calendar.Primitives
{
	public partial class DateTime_Edit_CalendarEvent_End : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (AllowNulls || ReadOnly)
				dtcValue.DateIsRequired = false;
		}

		#region IEditControl Members

		#region AllowNulls
		public bool AllowNulls
		{
			set
			{
				dtcValue.DateIsRequired = !value;
			}
			get
			{
				return !dtcValue.DateIsRequired;
			}
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
		}
		#endregion

		#region FieldName
		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		}
		#endregion

		#region Label
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			set { dtcValue.Enabled = !value; }
			get { return !dtcValue.Enabled; }
		}
		#endregion

		#region RowCount
		public int RowCount
		{
			set { }
			get { return 1; }
		}
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region TabIndex
		public short TabIndex
		{
			set
			{
				dtcValue.TabIndex = value;
			}
		}
		#endregion

		#region Value
		public object Value
		{
			set
			{
				if (Request["EventEndDate"] != null)
				{
					DateTime date = DateTime.Now;
					string[] arr = Request["EventEndDate"].Split(new char[] { '.' });
					date = new DateTime(int.Parse(arr[0]), int.Parse(arr[1]),
												int.Parse(arr[2]), int.Parse(arr[3]), int.Parse(arr[4]),
												int.Parse(arr[5]));
					dtcValue.SelectedDate = date;
				}
				else if (Request["ObjectId"] == null)
				{
					DateTime dt = DateTime.Today.AddHours(DateTime.UtcNow.Hour + 2);
					dtcValue.SelectedDate = User.GetLocalDate(Mediachase.IBN.Business.Security.CurrentUser.TimeZoneId, dt);
				}
				else if (value != null)
					dtcValue.SelectedDate = (System.DateTime)value;
			}
			get
			{
				DateTime retVal = dtcValue.SelectedDate;
				if (AllowNulls && retVal == System.DateTime.MinValue)
					return null;
				else
					return retVal;
			}
		}
		#endregion

		#endregion
	}
}