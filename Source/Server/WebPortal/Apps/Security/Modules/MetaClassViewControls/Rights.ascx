<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Rights.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls.Rights" %>
<br />
<table width="100%">
	<tr>
		<td class="section"><asp:Literal ID="Literal1" Text="<%$Resources : IbnFramework.Security, RightsList%>" runat="server"></asp:Literal></td>
		<td align="right"><asp:HyperLink runat="server" ID="lnkNew"></asp:HyperLink></td>
	</tr>
</table>
<asp:GridView runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
	CellPadding="4" GridLines="Horizontal" AllowPaging="false" AllowSorting="false" OnRowCommand="grdMain_RowCommand" OnRowDeleting="grdMain_RowDeleting">
	<Columns>
		<asp:TemplateField HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>">
			<ItemTemplate>
				<%# Eval("Name")%>
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb" />
			<HeaderStyle CssClass="ibn-vh"  />
		</asp:TemplateField>
		<asp:BoundField DataField="FriendlyName" HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, FriendlyName%>" ItemStyle-CssClass="ibn-vb" HeaderStyle-CssClass="ibn-vh" />
		<asp:TemplateField>
			<ItemTemplate>
				<asp:HyperLink id="ibEdit" runat="server" ImageUrl="~/Images/IbnFramework/edit.gif" NavigateUrl='<%# Eval("EditLink")%>' ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Edit%>"></asp:HyperLink>&nbsp;&nbsp;
				<asp:ImageButton ImageUrl="~/Images/IbnFramework/delete.gif" Runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName='<%# deleteCommand %>' CommandArgument='<%# Eval("Name") %>' ID="ibDelete" Visible='<%# Eval("CanDelete") %>'></asp:ImageButton> 
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb" Width="50px" />
			<HeaderStyle CssClass="ibn-vh" Width="50px" />
		</asp:TemplateField>
	</Columns>
</asp:GridView>
<asp:Button runat="server" ID="btnRefresh" OnClick="btnRefresh_Click" Text="Refresh" style="display:none" />
