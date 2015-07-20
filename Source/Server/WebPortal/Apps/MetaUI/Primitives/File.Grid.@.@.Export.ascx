<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.File_Grid_All_All_Export"%>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = String.Empty;
		
		if (DataItem != null) 
		{ 
			MetaObjectProperty prop = DataItem.Properties[FieldName];
			if (prop != null && prop.Value != null)
			{
				FileInfo fi = (FileInfo)prop.Value;
				retVal = fi.Name;
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>