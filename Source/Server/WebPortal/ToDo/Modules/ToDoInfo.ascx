<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.ToDoInfo" Codebehind="ToDoInfo.ascx.cs" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="padding:5px">
	<tr>
		<td>
			<table class="ibn-propertysheet" width="100%" border="0" cellpadding="5" cellspacing="0">
				<colgroup>
					<col width="170px"/>
					<col/>
					<col width="170px"/>
				</colgroup>
				<tr>
					<td class="ibn-label" width="170px"><%= LocRM.GetString("Title")%>:</td>
					<td class="ibn-value">
						<asp:Label Runat="server" ID="lblTitle"></asp:Label>
					</td>
					<td align="right" width="170px">
						<asp:label id="lblState" runat="server" Font-Bold="True"></asp:label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" style="white-space: nowrap">
						<%= LocRM.GetString("timeline")%>:
					</td>
					<td class="ibn-legend-greyblack">
						<asp:label id="lblTimeline" runat="server"></asp:label>
					</td>
					<td align="right">
						<asp:label id="lblPriority" runat="server"></asp:label>
					</td>
				</tr>
				<tr>
					<td colspan="3" class="ibn-description" align="left">
						<asp:label id="lblDescription" runat="server"></asp:label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
