<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.TimeTracking.Primitives.RowCssClass_Grid_TimeTrackingEntry_TT_MyGroupByWeekProjectAll" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<%@ Import Namespace="Mediachase.IbnNext.TimeTracking" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem)
	{
		string retVal = "";
		PrimaryKeyId? oParentBlockId = null;

		if (DataItem != null)
		{
			if (DataItem.Properties["ParentBlockId"] != null)
				oParentBlockId = (PrimaryKeyId?)DataItem.Properties["ParentBlockId"].Value;

			TimeTrackingBlock ttb = null;
			StateMachineService sms = null;
			bool isInitialState = true;

			if (oParentBlockId != null)
			{
				ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), (int)oParentBlockId);
//				ttb = new Mediachase.IbnNext.TimeTracking.TimeTrackingBlock((int)oParentBlockId);
				sms = ((BusinessObject)ttb).GetService<StateMachineService>();
				isInitialState = sms.StateMachine.GetStateIndex(sms.CurrentState.Name) == 0;
			}
			
			if (DataItem.PrimaryKeyId != null)
			{
				retVal = "x-grid-row";

                //if (ttb != null)
                //{
                //    if (ttb.ExtendedProperties.Length > 0)
                //    {
                //        retVal = "TimeSheetRejectedRow";
                //        return retVal;
                //    }                        
                //}

				if (ttb != null)
				{
					if (!TimeTrackingBlock.CheckUserRight(ttb, Security.RightWrite) || !isInitialState)
						retVal += " TimeSheetReadOnlyRow";
				}
				else if (oParentBlockId != null && (int)oParentBlockId > 0)
				{
					if (!TimeTrackingBlock.CheckUserRight((int)oParentBlockId, Security.RightWrite) || !isInitialState)
						retVal += " TimeSheetReadOnlyRow";
				}
			}
			else
			{

                if (ttb != null && ttb.Properties["IsRejected"].GetValue<bool>())
                {
                    retVal = "TimeSheetRejectedRow ";                    
                }
                                
				if (ttb != null)
				{
					if (!TimeTrackingBlock.CheckUserRight(ttb, Security.RightWrite) || !isInitialState)
						retVal += "TimeSheetReadOnlyRow ";
				}
				else if (oParentBlockId != null)
				{
					if (!TimeTrackingBlock.CheckUserRight((int)oParentBlockId, Security.RightWrite) || !isInitialState)
						retVal += "TimeSheetReadOnlyRow ";
				}
				
				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Total.ToString())
				{
					if (DataItem.Properties["MetaViewGroupByScope"].OriginalValue.ToString() == MetaViewGroupByScope.Begin.ToString())
					{
						retVal += "x-grid-row";
					}
					else
					{
						retVal += "TimeSheetSelectedTotalRow";
					}
				}
				
				if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
				{
					if (DataItem.Properties["MetaViewGroupByScope"].OriginalValue.ToString() == MetaViewGroupByScope.Begin.ToString())
					{
						retVal += "TimeSheetSelectedRow";
					}
					else
					{
						retVal += "x-grid-row";
					}
				}
				else if (DataItem.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
				{
					if (DataItem.Properties["MetaViewGroupByScope"].OriginalValue.ToString() == MetaViewGroupByScope.Begin.ToString())
					{
						retVal += "TimeSheetSelectedSecondaryRow";
					}
					else
					{
						retVal += "x-grid-row";
					}
				}
			}
		}
		
	return retVal;
	}
</script>
<%# GetValue(DataItem) %>