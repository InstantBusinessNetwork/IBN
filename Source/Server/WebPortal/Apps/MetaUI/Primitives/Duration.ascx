<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.MetaUI.Primitives.Duration" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";
        
        if (DataItem != null)
        {
			int hours, minutes;
			int value = 0;
			if (!String.IsNullOrEmpty(DataItem.Properties[FieldName].Value.ToString()))
				value = Convert.ToInt32(DataItem.Properties[FieldName].Value.ToString());
			
			hours = Convert.ToInt32(value / 60);
			minutes = value % 60;
			retVal = String.Format("{0:D2}:{1:D2}", hours, minutes);
						 
        }

		return String.Format("{0}", retVal);
    }
</script>
<div style="float: right;"><%# GetValue(DataItem, FieldName) %></div>