<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.ProjectManagement.ColumnTemplates.ResourceWork_Duration" %>
<script language="c#" runat="server">
	protected string GetValue(object duration)
	{
		string retval = string.Empty;
		if (duration != DBNull.Value)
		{
			retval = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)duration);
		}
		return retval;
	}
</script>
<%# GetValue(Eval("Duration"))%>