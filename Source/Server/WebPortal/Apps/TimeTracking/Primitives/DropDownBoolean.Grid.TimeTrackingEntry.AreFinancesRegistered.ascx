<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.TimeTracking.Primitives.DropDownBoolean_Grid_TimeTrackingEntry_AreFinancesRegistered" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.IbnNext.TimeTracking" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName].Value != null)
		{
			if (DataItem.Properties["ParentBlockId"] != null)
			{
				// Check User Right
				Mediachase.Ibn.Data.PrimaryKeyId? oParentBlockId = (Mediachase.Ibn.Data.PrimaryKeyId?)DataItem.Properties["ParentBlockId"].Value;
				if (oParentBlockId != null && (int)oParentBlockId > 0 && TimeTrackingBlock.CheckUserRight((int)oParentBlockId, "ViewFinances"))
				{
					TimeTrackingBlock blk = new TimeTrackingBlock(oParentBlockId.Value);
					
					if (blk.ProjectId.HasValue)
					{
						bool areFinancesActive = Mediachase.IBN.Business.SpreadSheet.ProjectSpreadSheet.IsActive(blk.ProjectId.Value);

						if (areFinancesActive)
						{
							if (!(bool)DataItem.Properties[FieldName].Value)
								retVal = String.Format(CultureInfo.InvariantCulture,
									"<img src='{0}' width='16px' height='16px' border='0' title='{1}'>",
									ResolveClientUrl("~/Images/IbnFramework/warning.png"),
									GetGlobalResourceObject("IbnFramework.TimeTracking", "FinancesAreNotRegistered").ToString());
						}
						else
						{
							retVal = String.Format(CultureInfo.InvariantCulture,
								"<img src='{0}' width='16px' height='16px' border='0' title='{1}'>",
								ResolveClientUrl("~/Images/IbnFramework/Warning2.png"),
								GetGlobalResourceObject("IbnFramework.TimeTracking", "FinancesAreNotActive").ToString());
						}
					}
				}
			}
		}

		return retVal;
	}
</script>
<div>
<%# GetValue(DataItem, FieldName)%>
</div>