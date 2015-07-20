<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EMailIssueBoxView2" Codebehind="EMailIssueBoxView2.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="7" cellspacing="0" border="0" class="text">
				<tr>
					<td><b><%= LocRM.GetString("tName")%>:</b></td>
					<td><asp:Label ID="lblIssBoxName" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%= LocRM.GetString("tManager")%>:</b></td>
					<td><asp:Label ID="lblManager" Runat="server"></asp:Label></td>
				</tr>
				<tr id="trControl" runat="server">
					<td><b><%= LocRM.GetString("tController")%>:</b></td>
					<td><asp:Label ID="lblController" Runat="server"></asp:Label></td>
				</tr>
				<tr id="trResponsible" runat="server">
					<td><b><%= LocRM.GetString("tResponsible")%>:</b></td>
					<td><asp:Label ID="lblResponsible" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("tExpDuration")%>:</b></td>
					<td><asp:Label ID="lblExpDuration" Runat="server" CssClass="text"></asp:Label></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("tExpRespTime")%>:</b></td>
					<td><asp:Label ID="lblExpRespTime" Runat="server" CssClass="text"></asp:Label></td>
				</tr>
			</table>
		</td>
	</tr>
</table>