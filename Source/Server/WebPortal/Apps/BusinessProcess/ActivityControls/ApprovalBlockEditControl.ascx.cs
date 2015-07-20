using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.ActivityControls
{
	public partial class ApprovalBlockEditControl : MCDataSavedControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			if (DataItem != null)
			{
				BlockActivityType type = WorkflowActivityWrapper.GetBlockActivityType(DataItem);
				CHelper.SafeSelect(BlockTypeList, ((int)type).ToString());
			}
		} 
		#endregion

		#region Save
		public override object Save(object dataItem)
		{
			BlockActivityType selectedType = (BlockActivityType)Enum.Parse(typeof(BlockActivityType), BlockTypeList.SelectedValue);
			WorkflowActivityWrapper.SetBlockActivityType(dataItem, selectedType);
			return base.Save(dataItem);
		} 
		#endregion
	}
}