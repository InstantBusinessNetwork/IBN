<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.TimeTracking.Primitives.Text_Grid_TimeTrackingEntry_StateFriendlyName" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType"%>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";

        if (DataItem != null && DataItem.Properties[FieldName].Value != null)
        {
            retVal = CHelper.GetResFileString(DataItem.Properties[FieldName].Value.ToString());
        }

        return retVal;
    }
</script>
<div>
<%# (DataItem == null) ? "null" : GetValue(DataItem, FieldName)%>
</div>