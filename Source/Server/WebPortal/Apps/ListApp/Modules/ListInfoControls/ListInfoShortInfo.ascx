<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListInfoShortInfo.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls.ListInfoShortInfo" %>
<div class="ibn-light">
	<table class="ibn-propertysheet ibn-underline tablePadding5" width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td class="ibn-label" style="width:140px;">
				<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>" />:
			</td>
			<td class="ibn-value">
				<asp:HyperLink runat="server" ID="ListDataLink"></asp:HyperLink>
				<asp:Label runat="server" ID="ListTemplate"></asp:Label>
			</td>
			<td class="ibn-label" style="width:140px;">
				<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.ListInfo, RecordCount%>" />:
			</td>
			<td style="width:30%;">
				<asp:Label runat="server" ID="RecordCountLabel"></asp:Label>
			</td>
		</tr>
		<tr>
			<td class="ibn-label" >
				<asp:Literal ID="Literal6" runat="server" Text="<%$Resources : IbnFramework.ListInfo, ListStatus%>" />:
			</td>
			<td class="ibn-value">
				<asp:Label runat="server" ID="StatusLabel"></asp:Label>
			</td>
			<td class="ibn-label">
				<asp:Literal ID="Literal7" runat="server" Text="<%$Resources : IbnFramework.ListInfo, ListType%>" />:
			</td>
			<td class="ibn-value">
				<asp:Label runat="server" ID="TypeLabel"></asp:Label>
			</td>
		</tr>
		<tr>
			<td class="ibn-label" >
				<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.ListInfo, CreatedBy%>" />:
			</td>
			<td class="ibn-value">
				<asp:Label runat="server" ID="CreatedByLabel"></asp:Label>
			</td>
			<td class="ibn-label">
				<asp:Literal ID="Literal4" runat="server" Text="<%$Resources : IbnFramework.ListInfo, Created%>" />:
			</td>
			<td class="ibn-value">
				<asp:Label runat="server" ID="CreatedLabel"></asp:Label>
			</td>
		</tr>
		<tr style="height:19px;">
			<td class="ibn-label" >
				<asp:Literal ID="Literal5" runat="server" Text="<%$Resources : IbnFramework.ListInfo, DefaultField %>" />:
			</td>
			<td class="ibn-value">
				<asp:UpdatePanel runat="server" ID="DefaultFieldPanel" RenderMode="block" UpdateMode="Conditional" ChildrenAsTriggers="true">
					<ContentTemplate>
						<asp:Label runat="server" ID="DefaultFieldLabel"></asp:Label>
						<asp:LinkButton runat="server" ID="DefaultFieldButton" ToolTip="<%$Resources : IbnFramework.ListInfo, DefaultFieldChange %>" OnClick="DefaultFieldButton_Click"></asp:LinkButton>
						<asp:DropDownList runat="server" ID="FieldsList"></asp:DropDownList>
						<asp:ImageButton runat="server" ID="SaveButton" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Save %>" AlternateText="<%$Resources : IbnFramework.GlobalMetaInfo, Save %>" ImageUrl="~/Images/IbnFramework/saveitem.gif" ImageAlign="AbsMiddle" OnClick="SaveButton_Click" />
						<asp:ImageButton runat="server" ID="CancelButton" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel %>" AlternateText="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel %>" ImageUrl="~/Images/IbnFramework/cancel.gif" ImageAlign="AbsMiddle" OnClick="CancelButton_Click" />
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
			<td></td>
			<td></td>
		</tr>
	</table>
</div>