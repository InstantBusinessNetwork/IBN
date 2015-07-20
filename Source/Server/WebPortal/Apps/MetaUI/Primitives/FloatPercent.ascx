<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.UI.Web.Apps.MetaUI.Primitives.FloatPercent" %>
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
			double value = 0;
			if (!String.IsNullOrEmpty(DataItem.Properties[FieldName].Value.ToString()))
				value = Convert.ToDouble(DataItem.Properties[FieldName].Value.ToString());
			
			retVal = value.ToString("F02", System.Globalization.CultureInfo.InvariantCulture); //String.Format("{0:D2}:{1:D2}", hours, minutes);
			retVal = String.Format("{0}%", retVal);
        }
        
        return retVal;
    }
</script>
<%# GetValue(DataItem, FieldName) %>