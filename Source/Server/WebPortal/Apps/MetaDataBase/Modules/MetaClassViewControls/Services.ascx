<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Services.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls.Services" %>
<table width="100%">
	<tr>
		<td class="section">
			<asp:Literal ID="Literal4" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Services%>" runat="server"></asp:Literal>
		</td>
	</tr>
</table>
<asp:GridView runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" BorderWidth="1" ShowHeader="false" 
	CellPadding="4" GridLines="Both" AllowPaging="false" AllowSorting="false" OnRowCommand="grdMain_RowCommand">
	<Columns>
		<asp:TemplateField>
			<ItemTemplate>
				<img align='absmiddle' border='0' src='<%# (bool)Eval("IsInstalled") ? ResolveUrl("~/Images/IbnFramework/allow.gif") : ResolveUrl("~/Images/IbnFramework/forbid.gif") %>' />&nbsp;<%#Eval("Name")%><br />
				<i><%#Eval("Description")%></i>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField ItemStyle-Width="100px">
			<ItemTemplate>
				<asp:LinkButton ID="lbInstall" runat="server" CommandName="Install" CommandArgument='<%#Eval("Name")%>' Text="<%$Resources : IbnFramework.GlobalMetaInfo, Install%>" Visible="false"></asp:LinkButton>
				<asp:LinkButton ID="lbUnInstall" runat="server" CommandName="Uninstall" CommandArgument='<%#Eval("Name")%>' Text="<%$Resources : IbnFramework.GlobalMetaInfo, Uninstall%>" Visible="false"></asp:LinkButton>
			</ItemTemplate>
		</asp:TemplateField>
	</Columns>
</asp:GridView>