<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnumMultiValue.Manage.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Primitives.EnumMultiValue_Manage " %>
<script type="text/javascript" language="javascript">
	function SetText(id, text, validatorid)
	{
		var input = document.getElementById(id);
		
		if(input!=null)
		{
			input.value = text;		
		}
		if(validatorid!=null)
		{
			var input1 = document.getElementById(validatorid);	
			if(input1!=null)
				input1.style.display = "none";
		}
		
	}
</script>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;">
	<tr>
		<td style="width:120px"></td>
		<td></td>
		<td style="width:16px"></td>
		<td style="width:20px"></td>
	</tr>
	<tr id="trName" runat="server">
		<td class="ibn-label">
			<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, EnumName %>" />:
		</td>
		<td>
			<asp:TextBox ID="txtEnumName" runat="server" Width="99%"></asp:TextBox>
		</td>
		<td></td>
		<td>
			<asp:RequiredFieldValidator id="vldEnumName_Required" runat="server" ErrorMessage="*" ControlToValidate="txtEnumName" Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:RegularExpressionValidator ID="vldEnumName_RegEx" ControlToValidate="txtEnumName" Runat="server" ValidationExpression="^[A-Za-z0-9](\w)*$"></asp:RegularExpressionValidator>
		</td>
	</tr>
	<tr id="trFriendlyName" runat="server">
		<td class="ibn-label">
			<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, EnumFriendlyName %>" />:
		</td>
		<td>
			<asp:TextBox ID="txtFriendlyName" runat="server" Width="99%"></asp:TextBox>
		</td>
		<td align="left">
			<img src='<%=ResolveUrl("~/images/IbnFramework/resource.gif")%>' title='<%=GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ResourceTooltip").ToString()%>' alt='' 
				onclick="SetText('<%=txtFriendlyName.ClientID%>','{ResourceName:ResourceKey}','<%=vldFriendlyName_Required.ClientID %>')" style="width:16px; height:16px" />
		</td>
		<td>
			<asp:RequiredFieldValidator id="vldFriendlyName_Required" runat="server" ErrorMessage="*" ControlToValidate="txtFriendlyName" Display="Dynamic"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr>
		<td colspan="4" style="border: 1px solid #aeaeae;">
			<asp:DataGrid runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" 
				CellPadding="4" GridLines="None" AllowPaging="false" AllowSorting="false" 
				OnCancelCommand="grdMain_CancelCommand" OnDeleteCommand="grdMain_DeleteCommand" 
				OnEditCommand="grdMain_EditCommand" OnUpdateCommand="grdMain_UpdateCommand"
				OnItemCommand="grdMain_ItemCommand">
				<Columns>
					<asp:TemplateColumn HeaderText="#" >
						<ItemStyle CssClass="ibn-vb2" Width="50px" />
						<HeaderStyle CssClass="ibn-vh2" Width="50px" />
						<ItemTemplate>
							<%# Eval("OrderId")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList runat="server" ID="ddlOrder" Width="50px"></asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
						<HeaderTemplate>
							<asp:Literal ID="headText" runat="server" Text='<%$Resources : IbnFramework.GlobalMetaInfo, ItemValue%>'></asp:Literal>
							<asp:ImageButton ID="btnAsc" runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, AscSort%>" CommandName="Asc" ImageAlign="AbsMiddle" CausesValidation="false" ImageUrl="~/Layouts/Images/Sort-Ascending.png" />&nbsp;&nbsp;
							<asp:ImageButton ID="btnDesc" runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, DescSort%>" CommandName="Desc" ImageAlign="AbsMiddle" CausesValidation="false" ImageUrl="~/Layouts/Images/Sort-Descending.png" />
						</HeaderTemplate>
						<ItemTemplate>
							<%# Eval("DisplayName")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="txtName" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' CssClass="text" Width="90%" Runat="server" MaxLength="255"></asp:TextBox>
							<img runat="server" id="imResourceTemplate" style="width:16px; height:16px" alt="" />
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.GlobalFieldManageControls, Default%>">
						<ItemStyle CssClass="ibn-vb2" Width="50px" HorizontalAlign="Center" />
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<img runat="server" id="imIsDefault" src="~/layouts/images/accept_1.gif" visible='<%# (bool)Eval("IsDefault") %>' style="width:16px; height:16px" alt="" />
						</ItemTemplate>
						<EditItemTemplate>
							<asp:Label ID="lblDefault" runat="server" Visible="false" Text='<%# ((bool)Eval("IsDefault")).ToString() %>'></asp:Label>
							<asp:CheckBox ID="cbDefault" runat="server" CssClass="text" />							
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="50px"/>
						<HeaderStyle CssClass="ibn-vh-right" Width="50px" />
						<HeaderTemplate>
							<asp:ImageButton id="ibAdd" runat="server" ImageUrl="~/Images/IbnFramework/newitem.gif" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, NewItem%>" Width="16" Height="16" CommandName="NewItem" causesvalidation="False" CommandArgument='<%# Eval("Id") %>'></asp:ImageButton>
							&nbsp;
						</HeaderTemplate>
						<ItemTemplate>
							<asp:ImageButton id="ibEdit" runat="server" ImageUrl="~/Images/IbnFramework/edit.gif" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Edit%>" Width="16" Height="16" CommandName="Edit" causesvalidation="False" CommandArgument='<%# Eval("Id") %>'></asp:ImageButton>
							&nbsp;
							<asp:ImageButton ID="ibDelete" Runat="server" ImageUrl="~/Images/IbnFramework/delete.gif" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName="Delete" CausesValidation="false" CommandArgument='<%# Eval("Id") %>'></asp:ImageButton> 
						</ItemTemplate>
						<EditItemTemplate>
							<asp:ImageButton id="ibSave" runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Save%>" ImageUrl="~/Images/IbnFramework/SaveItem.gif" commandname="Update" causesvalidation="False" CommandArgument='<%# Eval("Id") %>'></asp:ImageButton>
							&nbsp;
							<asp:imagebutton id="ibCancel" runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel%>" ImageUrl="~/Images/IbnFramework/cancel.gif" commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
	<tr>
		<td></td>
		<td>
			<asp:CheckBox runat="server" ID="chkEditable" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, EditableDictionary%>" Checked="false" />
		</td>
		<td></td>
		<td></td>
	</tr>
	<tr>
		<td></td>
		<td>
			<asp:CheckBox runat="server" ID="chkPublic" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, PublicDictionary%>" Checked="false" />
		</td>
		<td></td>
		<td></td>
	</tr>
</table>