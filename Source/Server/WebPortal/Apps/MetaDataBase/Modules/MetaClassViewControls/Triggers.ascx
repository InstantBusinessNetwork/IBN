<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Triggers.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls.Triggers" %>
<br />
<table width="100%">
	<tr>
		<td class="section"></td>
		<td align="right"><asp:HyperLink runat="server" ID="lnkNew"></asp:HyperLink></td>
	</tr>
</table>
<asp:GridView runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
	CellPadding="4" GridLines="both" AllowPaging="false" AllowSorting="false" OnRowCommand="grdMain_RowCommand">
	<Columns>
		<asp:TemplateField HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>">
			<ItemTemplate>
				<%# Eval("Name")%>
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" />
			<HeaderStyle CssClass="ibn-vh2"  />
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
				<asp:HyperLink id="ibEdit" runat="server" ImageUrl="~/Images/IbnFramework/edit.gif" NavigateUrl='<%# Eval("EditLink")%>' ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Edit%>"></asp:HyperLink>&nbsp;&nbsp;
				<asp:ImageButton ImageUrl="~/Images/IbnFramework/delete.gif" Runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName='<%# deleteCommand %>' CommandArgument='<%# Eval("Name") %>' ID="ibDelete"></asp:ImageButton> 
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" Width="50px" />
			<HeaderStyle CssClass="ibn-vh2" Width="50px" />
		</asp:TemplateField>
	</Columns>
</asp:GridView>
<asp:Button runat="server" ID="btnRefresh" OnClick="btnRefresh_Click" Text="Refresh" style="display:none" />
