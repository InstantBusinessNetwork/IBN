<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.ManageDictionaries" Codebehind="ManageDictionaries.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
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
<table class="ibn-stylebox2 text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="PADDING:14px">
			<%=LocRM.GetString("SelectDictionary") %>:&nbsp;
			<asp:DropDownList Width="200px" id="ddDictionaries" runat="server" AutoPostBack="True" onselectedindexchanged="ddDic_ChangeDictionary"></asp:DropDownList>
			<asp:Button CssClass="text" ID="btnAddNewItem" Runat="server" CausesValidation="False" style="display:none" onclick="btnAddNewItem_Click"></asp:Button>
			<asp:Label Runat="server" ID="lblError" CssClass="ibn-error"></asp:Label>
		</td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid id="dgDic" runat="server" cellpadding="1" gridlines="Horizontal" borderwidth="0" autogeneratecolumns="False" width="100%" allowsorting="False">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ItemId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"ItemName") %>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="tbName" Text='<%# DataBinder.Eval(Container.DataItem,"ItemName") %>' CssClass="text" Width="100%" Runat="server">
							</asp:TextBox>
							<asp:RequiredFieldValidator ID="rfName" ControlToValidate="tbName" ErrorMessage='<%#LocRM.GetString("Required") %>' Display="Dynamic" Runat="server"/>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2" Width="80"></headerstyle>
						<itemstyle cssclass="ibn-vb2" Width="80"></itemstyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"Weight") %>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="tbWeight" Text='<%# DataBinder.Eval(Container.DataItem,"Weight") %>' CssClass="text" Width="60px" Runat="server"></asp:TextBox>
							<asp:RangeValidator ID="rvWeight" ErrorMessage="*" CssClass="text" Runat=server ControlToValidate="tbWeight" Display=Dynamic MinimumValue="0" MaximumValue="1000" Type=Integer></asp:RangeValidator>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2" Width="50"></headerstyle>
						<itemstyle cssclass="ibn-vb2" Width="50"></itemstyle>
						<itemtemplate>
							<asp:imagebutton id="ibMove" runat="server" borderwidth="0" title='<%# LocRM.GetString("Edit")%>' imageurl="~/layouts/images/Edit.gif" commandname="Edit" causesvalidation="False">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="~/layouts/images/Delete.gif" commandname="Delete" causesvalidation="False" Visible='<%# GetVisibleStatus(DataBinder.Eval(Container.DataItem,"CanDelete")) %>' >
							</asp:imagebutton>
						</itemtemplate>
						<EditItemTemplate>
							<asp:imagebutton id="Imagebutton1" runat="server" borderwidth="0" title='<%# LocRM.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="Imagebutton2" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" title='<%# LocRM.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			
			<asp:DataGrid id="dgScore" runat="server" cellpadding="1" gridlines="Horizontal" borderwidth="0" autogeneratecolumns="False" width="100%" allowsorting="False">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="BusinessScoreId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"Key") %>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="tbKey" Text='<%# DataBinder.Eval(Container.DataItem,"Key") %>' CssClass="text" Width="95%" Runat="server">
							</asp:TextBox>
							<asp:RequiredFieldValidator ID="rfKey" ControlToValidate="tbKey" ErrorMessage='<%#LocRM.GetString("Required") %>' Display="Dynamic" Runat="server"/>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"Name") %>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="tbName2" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' CssClass="text" Width="95%" Runat="server">
							</asp:TextBox>
							<asp:RequiredFieldValidator ID="rfName2" ControlToValidate="tbName2" ErrorMessage='<%#LocRM.GetString("Required") %>' Display="Dynamic" Runat="server"/>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2" Width="50"></headerstyle>
						<itemstyle cssclass="ibn-vb2" Width="50"></itemstyle>
						<itemtemplate>
							<asp:imagebutton id="Imagebutton3" runat="server" borderwidth="0" title='<%# LocRM.GetString("Edit")%>' imageurl="~/layouts/images/Edit.gif" commandname="Edit" causesvalidation="False">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="ibDelete2" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="~/layouts/images/Delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</itemtemplate>
						<EditItemTemplate>
							<asp:imagebutton id="Imagebutton5" runat="server" borderwidth="0" title='<%# LocRM.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="Imagebutton6" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" title='<%# LocRM.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			
			<asp:DataGrid id="dgPlanSlot" runat="server" cellpadding="1" gridlines="Horizontal" borderwidth="0" autogeneratecolumns="False" width="100%" allowsorting="False">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="BasePlanSlotId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem,"Name") %>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="tbName3" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' CssClass="text" Width="95%" Runat="server">
							</asp:TextBox>
							<asp:RequiredFieldValidator ID="rfName3" ControlToValidate="tbName3" ErrorMessage='<%#LocRM.GetString("Required") %>' Display="Dynamic" Runat="server"/>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# (bool)DataBinder.Eval(Container.DataItem,"IsDefault")? 
								"<img align='absmiddle' src = '" + ResolveClientUrl("~/layouts/images/accept_1.gif") + "' />" :
								"<img align='absmiddle' src = '" + ResolveClientUrl("~/layouts/images/deny_1.gif") + "' />"
							%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:CheckBox ID="cbIsDef" Runat="server" CssClass="text" Checked='<%# (bool)DataBinder.Eval(Container.DataItem,"IsDefault")%>'></asp:CheckBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<headerstyle cssclass="ibn-vh2" Width="50"></headerstyle>
						<itemstyle cssclass="ibn-vb2" Width="50"></itemstyle>
						<itemtemplate>
							<asp:imagebutton id="Imagebutton4" runat="server" borderwidth="0" title='<%# LocRM.GetString("Edit")%>' imageurl="~/layouts/images/Edit.gif" commandname="Edit" causesvalidation="False">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="ibDelete3" runat="server" borderwidth="0" title='<%# LocRM.GetString("Delete")%>' imageurl="~/layouts/images/Delete.gif" commandname="Delete" causesvalidation="False">
							</asp:imagebutton>
						</itemtemplate>
						<EditItemTemplate>
							<asp:imagebutton id="Imagebutton8" runat="server" borderwidth="0" title='<%# LocRM.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
							</asp:imagebutton>
							&nbsp;&nbsp;
							<asp:imagebutton id="Imagebutton9" runat="server" borderwidth="0" imageurl="~/layouts/images/cancel.gif" title='<%# LocRM.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
			
			<asp:DataGrid runat="server" ID="dgEnum" AutoGenerateColumns="false" Width="100%" 
				CellPadding="1" GridLines="Horizontal" AllowPaging="false" AllowSorting="false" BorderWidth="0">
				<Columns>
					<asp:TemplateColumn HeaderText="¹" >
						<ItemStyle CssClass="ibn-vb2" Width="50px" />
						<HeaderStyle CssClass="ibn-vh2" Width="50px" />
						<ItemTemplate>
							<%# Eval("OrderId")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList runat="server" ID="ddlOrder" Width="50px"></asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, ItemValue%>" >
						<ItemStyle CssClass="ibn-vb2" Width="40%" />
						<HeaderStyle CssClass="ibn-vh2" Width="40%" />
						<ItemTemplate>
							<%# Eval("Name")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="txtName" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' CssClass="text" Width="90%" Runat="server" MaxLength="255"></asp:TextBox>
							<img runat="server" id="imResourceTemplate" style="width:16px; height:16px" alt="" src="~/images/IbnFramework/resource.gif" title="<%$Resources: IbnFramework.GlobalMetaInfo, ResourceTooltip %>" />
							<asp:RequiredFieldValidator ID="rfName" ControlToValidate="txtName" ErrorMessage="*" Display="Dynamic" Runat="server"/>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="<%$Resources : IbnFramework.GlobalMetaInfo, ItemDisplayValue%>" >
						<ItemStyle CssClass="ibn-vb2" />
						<HeaderStyle CssClass="ibn-vh2" />
						<ItemTemplate>
							<%# Eval("DisplayName")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemStyle CssClass="ibn-vb2" Width="50px"/>
						<HeaderStyle CssClass="ibn-vh2" Width="50px" />
						<ItemTemplate>
							<asp:ImageButton id="ibEdit" runat="server" ImageUrl="~/Images/IbnFramework/edit.gif" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Edit%>" Width="16" Height="16" CommandName="Edit" causesvalidation="False" CommandArgument='<%# Eval("Id") %>'></asp:ImageButton>
							&nbsp;
							<asp:ImageButton ID="ibDelete" Runat="server" ImageUrl="~/Images/IbnFramework/delete.gif" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Delete%>" Width="16" Height="16" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'></asp:ImageButton> 
						</ItemTemplate>
						<EditItemTemplate>
							<asp:ImageButton id="ibSave" runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Save%>" ImageUrl="~/Images/IbnFramework/SaveItem.gif" commandname="Update" causesvalidation="True" CommandArgument='<%# Eval("Id") %>'></asp:ImageButton>
							&nbsp;
							<asp:imagebutton id="ibCancel" runat="server" ToolTip="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel%>" ImageUrl="~/Images/IbnFramework/cancel.gif" commandname="Cancel" causesvalidation="False">
							</asp:imagebutton>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
