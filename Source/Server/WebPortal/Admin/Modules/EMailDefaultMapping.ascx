<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailDefaultMapping" Codebehind="EMailDefaultMapping.ascx.cs" %>
<table cellpadding="0" cellspacing="0" width="100%" border="0">
	<colgroup>
		<col width="110px"/>
		<col width="310px"/>
		<col />
	</colgroup>
	<tr class="text">
		<td style="padding:10px 0px 7px 10px" class="ibn-alternating ibn-navline"><b><%= LocRM.GetString("tName")%>:</b></td>
		<td style="padding:10px 0px 7px 10px" class="ibn-alternating ibn-navline"><asp:DropDownList ID="ddSource" Runat="server" Width="300px" AutoPostBack="True"></asp:DropDownList></td>
		<td class="ibn-alternating ibn-navline">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="3" valign="top">
			<asp:PlaceHolder ID="phMap" Runat="server"></asp:PlaceHolder>
		</td>
	</tr>
</table>