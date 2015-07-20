<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.FieldEdit" Codebehind="FieldEdit.ascx.cs" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td colspan="2"><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td width="560px" valign=top>
			<table cellpadding="3" cellspacing="3" border="0" width="100%">
				<tr>
					<td colspan="2">
						<asp:Label Runat="server" ID="lblErrorMessage" CssClass="ibn-error"></asp:Label>
					</td>
				</tr>
				<tr runat="server" id="trElement">
					<td class="text ibn-label" align="right"><%=LocRM.GetString("ElementName") %>:</td>
					<td class="ibn-value">
						<asp:Label Runat="server" ID="lblElementName" CssClass="text"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="text ibn-label" align="right"><%=LocRM.GetString("FieldName") %>:</td>
					<td class="ibn-value" width="360px">
						<asp:TextBox Runat="server" ID="tbName" CssClass="text" Width="200px"></asp:TextBox>
						<asp:RegularExpressionValidator ID="reNameVal" ControlToValidate="tbName" Runat=server CssClass="text" Display=Dynamic ErrorMessage="*" ValidationExpression="^[A-Za-z0-9](\w|\x20)*$"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td class="text ibn-label" align="right"><%=LocRM.GetString("FieldFriendlyName") %>:</td>
					<td class="ibn-value">
						<asp:TextBox Runat="server" ID="tbFriendlyName" CssClass="text" Width="200px"></asp:TextBox>
						<asp:RequiredFieldValidator id="Requiredfieldvalidator2" runat="server" CssClass="text" ErrorMessage="*" ControlToValidate="tbFriendlyName"
							Display="Dynamic"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="text ibn-label" align="right" valign="top"><%=LocRM.GetString("FieldDescription") %>:</td>
					<td class="ibn-value"><asp:TextBox Runat="server" ID="tbDescription" CssClass="text" Width="350px" Rows="4" TextMode="MultiLine"></asp:TextBox></td>
				</tr>
				<tr>
					<td class="text ibn-label" align="right"><%=LocRM.GetString("FieldType") %>:</td>
					<td class="ibn-value">
						<asp:DropDownList Runat="server" ID="ddlType" Width="200" AutoPostBack="True" onselectedindexchanged="ddlType_SelectedIndexChanged"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:CheckBox Runat="server" ID="chkMultiline" CssClass="text"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:CheckBox Runat="server" ID="chkEditable" CssClass="text"></asp:CheckBox>
					</td>
				</tr>
				<tr id="trAllowNulls" runat="server">
					<td></td>
					<td>
						<asp:CheckBox Runat="server" ID="chkAllowNulls" CssClass="text" Checked="True"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:CheckBox Runat="server" ID="chkSaveHistory" CssClass="text" Checked="True"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:CheckBox Runat="server" ID="chkAllowSearch" CssClass="text" Checked="True"></asp:CheckBox>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top" style="padding-left:50px;padding-top:10px;">
			<fieldset style="width:300px;margin:0;padding:7px" runat="server" id="fsItems">
				<legend class="text" id="lgdDicItems" runat="server"></legend>
				<asp:DataGrid id="dgDic" runat="server" cellpadding="1" gridlines="Horizontal" borderwidth="0" autogeneratecolumns="False" width="100%" allowsorting="False">
					<ItemStyle Height="21px"></ItemStyle>
					<Columns>
						<asp:BoundColumn Visible="False" DataField="Id" ReadOnly="True"></asp:BoundColumn>
						<asp:TemplateColumn>
							<HeaderStyle CssClass="ibn-vh2" Width="50px"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2" Width="50px"></ItemStyle>
							<ItemTemplate>
								<%# ((int)DataBinder.Eval(Container.DataItem,"Index") + 1).ToString() %>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:DropDownList ID="ddIndex" Width="100%" Runat="server">
								</asp:DropDownList>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# DataBinder.Eval(Container.DataItem,"Value") %>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox ID="tbValue" Text='<%# DataBinder.Eval(Container.DataItem,"Value") %>' CssClass="text" Width="100%" Runat="server">
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<headerstyle cssclass="ibn-vh-right" Width="80"></headerstyle>
							<itemstyle cssclass="ibn-vb2" Width="80"></itemstyle>
							<HeaderTemplate>
								<asp:ImageButton id="btnSortAsc" Runat="server" CausesValidation="False" ImageUrl="~/Layouts/Images/Sort-Ascending.png" CommandName="SortAsc" title='<%#LocRMDict.GetString("tSortAsc") %>'>
								</asp:ImageButton>
								&nbsp;
								<asp:ImageButton id="btnSortDesc" Runat="server" CausesValidation="False" ImageUrl="~/Layouts/Images/Sort-Descending.png" CommandName="SortDesc" title='<%#LocRMDict.GetString("tSortDesc") %>'>
								</asp:ImageButton>
								&nbsp;
								<asp:ImageButton id="btnAdd" Runat="server" CausesValidation="False" ImageUrl="~/Layouts/Images/newitem.gif" CommandName="Add" title='<%#LocRM.GetString("tAddItem") %>'>
								</asp:ImageButton>
							</HeaderTemplate>
							<itemtemplate>
								&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
								<asp:imagebutton id="ibMove" runat="server" title='<%# LocRMDict.GetString("Edit")%>' imageurl="~/layouts/images/Edit.gif" commandname="Edit" causesvalidation="False">
								</asp:imagebutton>
								&nbsp;
								<asp:imagebutton id="ibDelete" runat="server" title='<%# LocRMDict.GetString("Delete")%>' imageurl="~/layouts/images/Delete.gif" commandname="Delete" causesvalidation="False" >
								</asp:imagebutton>
							</itemtemplate>
							<EditItemTemplate>
								&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
								<asp:imagebutton id="Imagebutton1" runat="server" title='<%# LocRMDict.GetString("Save")%>' imageurl="~/layouts/images/Saveitem.gif" commandname="Update" causesvalidation="False">
								</asp:imagebutton>
								&nbsp;
								<asp:imagebutton id="Imagebutton2" runat="server" imageurl="~/layouts/images/cancel.gif" title='<%# LocRMDict.GetString("Cancel")%>' commandname="Cancel" causesvalidation="False">
								</asp:imagebutton>
							</EditItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:DataGrid>
			</fieldset>
		</td>
	</tr>
	<tr>
		<td vAlign="bottom" align="right" height="40" style="padding:7px">
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton>
		</td>
	</tr>
</table>
