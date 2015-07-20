<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryFields.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls.HistoryFields" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<ibn:BlockHeaderLight2 id="MainBlockHeader" runat="server" Title="<%$Resources : IbnFramework.ListInfo, FieldList%>" />
<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
	<tr>
		<td>
			<asp:GridView runat="server" ID="MainGrid" AutoGenerateColumns="false" Width="100%" BorderWidth="1" BorderColor="lightgray"
				CellPadding="4" GridLines="Horizontal" AllowPaging="false" AllowSorting="false" OnRowCommand="MainGrid_RowCommand" EmptyDataText="&nbsp;">
				<Columns>
					<asp:BoundField DataField="Name" HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" HeaderStyle-Width="50%" ItemStyle-Width="50%" />
					<asp:BoundField DataField="Type" HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, Type%>" ItemStyle-CssClass="ibn-vb2" HeaderStyle-CssClass="ibn-vh2" />
					<asp:TemplateField>
						<ItemTemplate>
							<asp:ImageButton ImageUrl="~/Images/IbnFramework/delete.gif" Runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName='<%# deleteCommand %>' CommandArgument='<%# Eval("Id") %>' ID="DeleteButton"></asp:ImageButton> 
						</ItemTemplate>
						<ItemStyle CssClass="ibn-vb2" Width="25px" />
						<HeaderStyle CssClass="ibn-vh2" Width="25px" />
					</asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>
</table>
