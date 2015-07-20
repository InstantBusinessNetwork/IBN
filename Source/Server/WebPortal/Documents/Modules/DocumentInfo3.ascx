<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.DocumentInfo3" Codebehind="DocumentInfo3.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;"><tr><td>
<ibn:blockheader id="tbInfo" runat="server"></ibn:blockheader>
</td></tr></table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="1" width="100%" border="0">
	<tr>
		<td style="padding:7">
			<table class="ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td width="100px"><b><%=LocRM.GetString("DocumentId")%>:</b></td>
					<td class="ibn-value" width="35%"><asp:label id="lblDocumentId" runat="server"></asp:label></td>
					<td width="130px"><b><%=LocRM.GetString("Manager")%>:</b></td>
					<td class="ibn-value"><asp:label id="lblManager" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("Title")%>:</b></td>
					<td class="ibn-value"><asp:label id="lblTitle" runat="server"></asp:label></td>
					<td><b><%=LocRM.GetString("Created")%>:</b></td>
					<td class="ibn-value"><asp:label id="lblCreated" runat="server"></asp:label></td>
				</tr>
				<tr id="trPriorityClient" runat="server">
					<td id="tdPriority" runat="server"><b><%=LocRM.GetString("Priority")%>:</b></td>
					<td class="ibn-value" id="tdPriority2" runat="server"><asp:label id="lblPriority" runat="server"></asp:label></td>
					<td id="tdClient" runat="server"><b><%=LocRM.GetString("tClient") %>:</b></td>
					<td class="ibn-value" id="tdClient2" runat="server"><asp:Label id="lblClient" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td id="tdPrjLabel" runat="server"><b><%=LocRM.GetString("Project")%>:</b></td>
					<td class="ibn-value" id="tdPrjName" runat="server"><asp:label id="lblProject" runat="server"></asp:label><A style="COLOR: red" href="Project.htm"></A></td>
					<td valign="top" id="tdCategories" runat="server"><b><%=LocRM.GetString("Category")%>:</b></td>
					<td class="ibn-value" valign="top" id="tdCats" runat="server"><asp:label id="lblCategories" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td valign="top" id="tdTaskTime" runat="server"><b><%=LocRM3.GetString("taskTime")%>:</b></td>
					<td class="ibn-value" valign="top" id="tdTaskTime2" runat="server"><asp:label id="lblTaskTime" runat="server"></asp:label></td>
					<td valign="top"><b><asp:Label runat="server" ID="SpentTimeLabel"></asp:Label></b></td>
					<td class="ibn-value" valign="top"><asp:Label id="lblSpentTime" runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("Description")%>:</b></td>
					<td colSpan="3" class="ibn-description" valign="top"><asp:label id="lblDescription" runat="server"></asp:label></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
