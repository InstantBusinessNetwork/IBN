<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.Administration.ColumnTemplates.RowCssClass_Grid_LeftMenu" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<script type="text/C#" runat="server">
	protected string GetValue(bool odd)
	{
		string retval = "";
		if (!odd)
			retval = "ibn-alternating";
		return retval;
	}
</script>
<%# GetValue((bool)Eval("Odd"))%>