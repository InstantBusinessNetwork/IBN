using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class Duration_Manage : System.Web.UI.UserControl, IManageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members

		public void BindData(MetaField mf)
		{
			txtDefaultValue.Text = mf.DefaultValue;
		}

		public void BindData(MetaClass mc, string FieldType)
		{
			txtDefaultValue.Text = "0";
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				return Attr;
			}
		}

		public string GetDefaultValue(bool AllowNulls)
		{
			return txtDefaultValue.Text;
		}

		#endregion
	}
}