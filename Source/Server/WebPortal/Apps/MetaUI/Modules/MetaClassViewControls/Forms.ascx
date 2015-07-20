<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Forms.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Modules.MetaClassViewControls.Forms" %>
<br />
<table width="100%" class="ibn-bottomBorderLight">
	<tr>
		<td class="section"></td>
		<td align="right"><asp:HyperLink runat="server" ID="lnkNew"></asp:HyperLink></td>
	</tr>
</table>
<asp:DataGrid runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%"
	CellPadding="4" AllowPaging="false" AllowSorting="false" GridLines="None"
	OnItemCommand="grdMain_RowCommand" OnItemDataBound="grdMain_RowDataBound">
	<Columns>
		<asp:BoundColumn DataField="PublicFormName" Visible="false" />
		<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>">
			<ItemTemplate>
				<%# Eval("Name")%>
				<br /><asp:TextBox ID="txtLink" TextMode="MultiLine" Rows="5" Columns="80" runat="server" CssClass="text"></asp:TextBox>
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" />
			<HeaderStyle CssClass="ibn-vh2"  />
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemTemplate>
				<asp:HyperLink id="ibEdit" runat="server" ImageUrl="~/Images/IbnFramework/edit.gif" NavigateUrl='<%# Eval("EditLink")%>' ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Edit%>"></asp:HyperLink>&nbsp;
				<asp:ImageButton ImageUrl="~/Images/IbnFramework/newitem.gif" Runat="server" ToolTip="<%$Resources : IbnFramework.MetaForm, RecreateForm%>" Width="16" Height="16" CommandName='<%# recreateCommand %>' CommandArgument='<%# Eval("Id") %>' ID="ibRecreate" ImageAlign="AbsMiddle"></asp:ImageButton> 
				<asp:ImageButton ImageUrl="~/Images/IbnFramework/delete.gif" Runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName='<%# deleteCommand %>' CommandArgument='<%# Eval("Id") %>' ID="ibDelete" Visible='<%# Eval("CanDelete")%>' ImageAlign="AbsMiddle"></asp:ImageButton> 
				<asp:ImageButton ImageUrl="~/Images/IbnFramework/Undo.png" Runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, ResetToDefault%>" Width="16" Height="16" CommandName='<%# resetCommand %>' CommandArgument='<%# Eval("Id") %>' ID="ibReset" Visible='<%# Eval("CanReset")%>' ImageAlign="AbsMiddle"></asp:ImageButton> 
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" Width="80px" />
			<HeaderStyle CssClass="ibn-vh2" Width="80px" />
		</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>
<asp:Button runat="server" ID="btnRefresh" OnClick="btnRefresh_Click" Text="Refresh" style="display:none" />
