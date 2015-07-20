<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeTrackingBlockTypeList.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.ManageControls.TimeTrackingBlockTypeList" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<asp:UpdatePanel runat="server" ID="upMain">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td><ibn:BlockHeader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<asp:GridView runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" BorderWidth="1" 
				CellPadding="4" GridLines="Horizontal" AllowPaging="false" AllowSorting="false" OnRowCommand="grdMain_RowCommand">
				<Columns>
					<asp:BoundField DataField="Title" HeaderText="<%$Resources : IbnFramework.TimeTracking, BlockTypeName%>" ItemStyle-CssClass="ibn-vb" HeaderStyle-CssClass="ibn-vh" />
					<asp:BoundField DataField="SuperType" HeaderText="<%$Resources : IbnFramework.TimeTracking, SuperType%>" ItemStyle-CssClass="ibn-vb" HeaderStyle-CssClass="ibn-vh" />
					<asp:BoundField DataField="StateMachine" HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, StateMachine%>" ItemStyle-CssClass="ibn-vb" HeaderStyle-CssClass="ibn-vh"/>
					<asp:BoundField DataField="BlockCard" HeaderText="<%$Resources : IbnFramework.TimeTracking, BlockCard%>" ItemStyle-CssClass="ibn-vb" HeaderStyle-CssClass="ibn-vh" />
					<asp:BoundField DataField="EntryCard" HeaderText="<%$Resources : IbnFramework.TimeTracking, EntryCard%>" ItemStyle-CssClass="ibn-vb" HeaderStyle-CssClass="ibn-vh" />
					<asp:TemplateField>
						<ItemTemplate>
							<asp:ImageButton ImageUrl="~/images/IbnFramework/delete.gif" Runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName='<%# deleteCommand %>' CommandArgument='<%# Eval("Id") %>' ID="ibDelete"></asp:ImageButton>
						</ItemTemplate>
						<ItemStyle CssClass="ibn-vb" Width="20px" />
						<HeaderStyle CssClass="ibn-vh" Width="20px" />
					</asp:TemplateField>
				</Columns>
			</asp:GridView>
			<asp:Button runat="server" ID="btnRefresh" OnClick="btnRefresh_Click" Text="Refresh" style="display:none" />
		</td>
	</tr>
</table>
</ContentTemplate></asp:UpdatePanel>