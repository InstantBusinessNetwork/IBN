<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetaClassFields.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls.MetaClassFields" %>
<style type="text/css">
.titleField
{
	font-style:italic;
}
</style>

<br />
<table width="100%" class="ibn-bottomBorderLight">
	<tr>
		<td class="section"></td>
		<td align="right"><asp:HyperLink runat="server" ID="NewLink"></asp:HyperLink></td>
	</tr>
</table>
<asp:GridView runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" 
	CellPadding="4" GridLines="None" AllowPaging="false" AllowSorting="true" OnRowCommand="grdMain_RowCommand" OnSorting="grdMain_Sorting">
	<Columns>
		<asp:TemplateField HeaderText="">
			<ItemTemplate>
				<asp:Image runat="server" ID="imFieldType" ImageUrl='<%#Eval("FieldTypeImageUrl") %>' Width="16px" Height="16px" />
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" Width="20px" HorizontalAlign="Center" />
			<HeaderStyle CssClass="ibn-vh2" Width="20px" />
		</asp:TemplateField>
		<asp:TemplateField HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, SystemName%>" SortExpression="Name" >
			<ItemTemplate>
				<span class='<%# ((bool)Eval("IsTitle")) ? "titleField" : "text"%>'><%# Eval("Name")%></span>
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" Width="35%" />
			<HeaderStyle CssClass="ibn-vh2" Width="35%" />
		</asp:TemplateField>
		<asp:TemplateField HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, FriendlyName%>" SortExpression="FriendlyName">
			<ItemTemplate>
				<span class='<%# ((bool)Eval("IsTitle")) ? "titleField" : "text"%>'><%# Eval("FriendlyName")%></span>
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" Width="35%" />
			<HeaderStyle CssClass="ibn-vh2" Width="35%" />
		</asp:TemplateField>
		<asp:TemplateField HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, Type%>" SortExpression="TypeName" >
			<ItemTemplate>
				<span class='<%# ((bool)Eval("IsTitle")) ? "titleField" : "text"%>'><%# Eval("TypeName")%></span>
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" />
			<HeaderStyle CssClass="ibn-vh2" />
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
				<asp:HyperLink id="ibEdit" runat="server" ImageUrl="~/Images/IbnFramework/edit.gif" NavigateUrl='<%# Eval("EditLink")%>' ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Edit%>" Visible='<%# !((bool)Eval("IsSystem")) %>'></asp:HyperLink>&nbsp;&nbsp;
				<asp:ImageButton ImageUrl="~/Images/IbnFramework/delete.gif" Runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName='<%# deleteCommand %>' CommandArgument='<%# Eval("Name") %>' ID="ibDelete" Visible='<%# !((bool)Eval("IsSystem")) %>'></asp:ImageButton> 
			</ItemTemplate>
			<ItemStyle CssClass="ibn-vb2" Width="50px" />
			<HeaderStyle CssClass="ibn-vh2" Width="50px" />
		</asp:TemplateField>
	</Columns>
</asp:GridView>