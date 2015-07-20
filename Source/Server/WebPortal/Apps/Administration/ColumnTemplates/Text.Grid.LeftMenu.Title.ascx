<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.Administration.ColumnTemplates.Text_Grid_LeftMenu_Title" %>
<script type="text/C#" runat="server">
	protected string GetValue(string title, bool hidden, bool hiddenParent)
	{
		string retval = title;
		if (hidden || hiddenParent)
		{
			retval = String.Format(System.Globalization.CultureInfo.InvariantCulture,
				"<span style=\"color:#999999;\">{0}</span>",
				title);
		}
		return retval;
	}
</script>
<%# GetValue((string)Eval("Title"), (bool)Eval("Hidden"), (bool)Eval("HiddenParent"))%>