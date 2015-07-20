<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagCloud.ascx.cs" Inherits="Mediachase.UI.Web.Modules.TagCloud" %>
<script language="javascript">
	function TagClk(obj)
	{
		var hdn = document.getElementById("<%= hdnTag.ClientID%>");
		var btn = document.getElementById("<%= btnTag.ClientID%>");
		if (hdn && btn)
		{
			hdn.value = obj;
			btn.click();
		}
	}
</script>
<asp:Repeater runat="server" ID="rptTags">
	<ItemTemplate>
		<a href='<%# "javascript:TagClk(&quot;" + Eval("Tag").ToString().Replace("\"", "\\\"").Replace("'", "&#039;") + "&quot;)"%>'
			style='<%# "font-size:" + ((int)Eval("TagSize")+6) + "pt;" %>'><%# Eval("Tag") %></a>
		&nbsp;
	</ItemTemplate>
</asp:Repeater>
<asp:HiddenField runat="server" ID="hdnTag" />
<asp:Button runat="server" ID="btnTag" OnClick="btnTag_Click" style="display:none" />