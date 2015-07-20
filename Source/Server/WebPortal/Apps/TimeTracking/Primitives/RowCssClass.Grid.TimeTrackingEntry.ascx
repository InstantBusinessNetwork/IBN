<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.TimeTracking.Primitives.RowCssClass_Grid_TimeTrackingEntry" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.IbnNext.TimeTracking" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem)
	{
		string retVal = "";
		Mediachase.Ibn.Data.PrimaryKeyId? oParentBlockId = null;

		if (DataItem != null)
		{
			if (DataItem.Properties["ParentBlockId"] != null)
				oParentBlockId = (Mediachase.Ibn.Data.PrimaryKeyId?)DataItem.Properties["ParentBlockId"].Value;

			retVal = "x-grid-row";
			
			if (DataItem.Properties["MetaViewGroupByType"] == null)
				return retVal;
			
			if (DataItem.PrimaryKeyId != null)
			{
				if (oParentBlockId != null && !TimeTrackingBlock.CheckUserRight((int)oParentBlockId, Security.RightWrite))
				{
					retVal += " TimeSheetReadOnlyRow";
				}
			}
			else
			{
				if (oParentBlockId != null)
				{
					if (!TimeTrackingBlock.CheckUserRight((int)oParentBlockId, Security.RightWrite))
					{
						retVal += " TimeSheetReadOnlyRow";
					}
				}

				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Total.ToString())
				{
					if (DataItem.Properties["MetaViewGroupByScope"].OriginalValue.ToString() != MetaViewGroupByScope.Begin.ToString())
					{
						retVal += " TimeSheetSelectedTotalRow";
					}
				}

				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
				{
					if (DataItem.Properties["MetaViewGroupByScope"].OriginalValue.ToString() == MetaViewGroupByScope.Begin.ToString())
					{
						retVal += " TimeSheetSelectedRow";
					}
				}
				else if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
				{
					if (DataItem.Properties["MetaViewGroupByScope"].OriginalValue.ToString() == MetaViewGroupByScope.Begin.ToString())
					{
						retVal += " TimeSheetSelectedSecondaryRow";
					}
				}
			}
		}
		
		return retVal;
	}
</script>
<%# GetValue(DataItem) %>