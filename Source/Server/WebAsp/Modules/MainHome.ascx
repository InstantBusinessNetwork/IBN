<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.MainHome" CodeBehind="MainHome.ascx.cs" %>
<%@ Register TagPrefix="ctrl" TagName="TopTab" Src="../Modules/TopTabs.ascx" %>
<div>Use the links on this page to manage this Instant Business Network site.</div>
<ctrl:TopTab ID="ctrlTopTab" runat="server" BaseUrl="../Pages/ASPHome.aspx"></ctrl:TopTab>
<table cellpadding="7" cellspacing="0" border="0" width="100%" class="ibn-WPBorder">
	<tr>
		<td valign="top" style="padding-right: 8px">
			<asp:PlaceHolder ID="phItems" runat="server"></asp:PlaceHolder>
		</td>
	</tr>
</table>
