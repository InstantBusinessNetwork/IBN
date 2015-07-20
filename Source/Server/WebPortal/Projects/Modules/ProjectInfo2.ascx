<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectInfo2" Codebehind="ProjectInfo2.ascx.cs" %>
<div runat="server" id="SynchronizationInfoDiv" style="text-align:center; padding:5px;margin:5px;" class="ibn-error" visible="false">
	<span style="width:50%; border: solid 1px red;padding:5px;">
		<%=LocRM2.GetString("SyncMode") %>
	</span>
</div>
<table class="ibn-propertysheet" width="100%" border="0" cellpadding="5" cellspacing="0" style="margin-bottom:0;">
	<colgroup>
		<col width="150px" />
		<col />
		<col width="85px" />
		<col />
		<col width="130px" />
	</colgroup>
	<tr>
		<td class="ibn-label" style="padding-left:10px; width:150px">
			<%= LocRM.GetString("title")%>:
		</td>
		<td class="ibn-value" colspan="3">
			<asp:Label Runat="server" ID="lblTitle"></asp:Label>
			<asp:HyperLink runat="server" ID="lnkMSProject"></asp:HyperLink>
		</td>
		<td align="right" style="width: 130px;">
			<asp:label id="lblState" runat="server" Font-Bold="True"></asp:label>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" style="padding-left:10px; width:150px"> 
			<nobr><%= LocRM.GetString("timeline")%>:</nobr>
		</td>
		<td class="ibn-legend-greyblack">
			<asp:label id="lblTimeline" runat="server"></asp:label>
		</td>
		<td class="ibn-label" style="width:85px;"> 
			<%= LocRM.GetString("manager")%>:
		</td>
		<td class="ibn-value">
			<asp:label id="lblManager" runat="server"></asp:label>
		</td>
		<td align="right" style="width: 130px;">
			<asp:label id="lblPriority" runat="server"></asp:label>
		</td>
	</tr>
	<tr>
		<td colspan="5" class="ibn-description" style="padding-left:10px" align="left">
			<asp:label id="lblDescription" runat="server"></asp:label>
		</td>
	</tr>
</table>