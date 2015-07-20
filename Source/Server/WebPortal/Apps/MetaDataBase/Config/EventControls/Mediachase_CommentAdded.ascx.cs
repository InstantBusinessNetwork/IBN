using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.Config.EventControls
{
	public partial class Mediachase_CommentAdded : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		public override void DataBind()
		{
			object bindObject = DataBinder.GetDataItem(this.Parent);
			string retVal = "";
			if (bindObject != null && bindObject is MetaObject)
			{
				if (((MetaObject)bindObject).Properties["Description"] != null &&
					((MetaObject)bindObject).Properties["Description"].Value != null)
					retVal = ((MetaObject)bindObject).Properties["Description"].Value.ToString();
				
				if (String.IsNullOrEmpty(retVal))
					retVal = CHelper.GetEventResourceString((MetaObject)bindObject);

				lblCommentValue.Text = retVal;
			}
			base.DataBind();
		}
	}
}