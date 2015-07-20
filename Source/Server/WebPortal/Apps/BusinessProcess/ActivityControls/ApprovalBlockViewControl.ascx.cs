using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Assignments;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.ActivityControls
{
	public partial class ApprovalBlockViewControl : MCDataBoundControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			BlockTypeLabel.Text = string.Empty;

			if (DataItem != null)
			{
				BlockActivityType type = WorkflowActivityWrapper.GetBlockActivityType(DataItem);
				if (type == BlockActivityType.All)
					BlockTypeLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "BlockActivityTypeAll").ToString();
				else if (type == BlockActivityType.Any)
					BlockTypeLabel.Text = GetGlobalResourceObject("IbnFramework.BusinessProcess", "BlockActivityTypeAny").ToString();
			}
		}
		#endregion
	}
}