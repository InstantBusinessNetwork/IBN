<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.ActiveDirectory" Codebehind="ActiveDirectory.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td colspan="2">
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr runat="server" id="trWinLoginDisabled">
		<td style="padding:5px; background-color:#ffffe1;" class="text" colspan="2" valign="top">
			<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
				<%=LocRM.GetString("tWinLoginDisabledMessage")%>
				<div style="white-space:nowrap; text-align:center; padding-top:15px;">
					<asp:LinkButton runat="server" ID="lbEnableWinLogin" Font-Bold="true" Font-Underline="true" ForeColor="red"></asp:LinkButton>
				</div>
			</blockquote>
		</td>
	</tr>
	<tr runat="server" id="trWinLoginEnabled">
		<td style="padding:10px" class="text" valign="top">
			<%=LocRM.GetString("tIPRangeMessage")%><br /><br />
			<div style="width:380px;margin:0;" runat=server id="fsItems">
			<ibn:BlockHeaderLight id="secAddress" runat="server" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
				<tr><td>
					<asp:DataGrid Runat="server" ID="dgMain" AllowPaging="False" AllowSorting="False" 
					DataKeyField="RangeId" AutoGenerateColumns="False" GridLines="Horizontal" 
					CellPadding="2" BorderWidth="0">
						<Columns>
						<asp:TemplateColumn>
							<HeaderStyle CssClass="ibn-vh2" Width="150px"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2" Width="150px"></ItemStyle>
							<ItemTemplate>
								<%# DataBinder.Eval(Container.DataItem, "StartAddress").ToString() %>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox CssClass="text" Runat="server" ID="tbStartAddress" Text='<%#DataBinder.Eval(Container.DataItem, "StartAddress").ToString() %>' Width="100px" >
								</asp:TextBox>
								<asp:RequiredFieldValidator Runat="server" ID="rfvStartAddress" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbStartAddress">
								</asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator Runat="server" ID="revStartAddress" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbStartAddress"
									ValidationExpression="^([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$"></asp:RegularExpressionValidator>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn >
							<HeaderStyle CssClass="ibn-vh2" Width="150px"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2" Width="150px"></ItemStyle>
							<ItemTemplate>
								<%# DataBinder.Eval(Container.DataItem, "EndAddress").ToString() %>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox CssClass="text" Runat="server" ID="tbEndAddress" Text='<%#DataBinder.Eval(Container.DataItem, "EndAddress").ToString() %>' Width="100px">
								</asp:TextBox>
								<asp:RequiredFieldValidator Runat="server" ID="rfvEndAddress" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbEndAddress">
								</asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator Runat="server" ID="revEndAddress" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbEndAddress"
									ValidationExpression="^([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$"></asp:RegularExpressionValidator>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn>
							<HeaderStyle  Width="80px" HorizontalAlign="Right"></HeaderStyle>
							<HeaderTemplate>
								<asp:ImageButton CommandName="Add" Runat="server" ID="ibAddRange" ImageUrl="~/Layouts/Images/NEWITEM.GIF" title='<%#LocRM.GetString("tNewRange")%>' ></asp:ImageButton>
							</HeaderTemplate>
							<ItemStyle CssClass="ibn-vb2" Width="80px" HorizontalAlign="Right"></ItemStyle>
							<ItemTemplate>
								<asp:ImageButton Runat="server" ID="ibEdit" ImageUrl="~/Layouts/Images/EDIT.GIF" CommandName="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "RangeId")%>' title='<%#LocRM.GetString("Edit")%>'></asp:ImageButton>
								&nbsp;
								<asp:ImageButton Runat="server" ID="ibDelete" ImageUrl="~/Layouts/Images/Delete.gif" CommandName="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "RangeId")%>' title='<%#LocRM.GetString("Delete")%>'></asp:ImageButton>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:ImageButton Runat="server" ID="ibSave" ImageUrl="~/Layouts/Images/SaveItem.GIF" CommandName="Save" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "RangeId")%>' title='<%#LocRM.GetString("Save")%>'></asp:ImageButton>
								&nbsp;
								<asp:ImageButton Runat="server" ID="ibCancel" ImageUrl="~/Layouts/Images/Cancel.gif" CommandName="Cancel" CausesValidation="False" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "RangeId")%>' title='<%#LocRM.GetString("Cancel")%>'></asp:ImageButton>
							</EditItemTemplate>
						</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</td></tr>
			</table>
			</div>
		</td>
		<td style="padding:5px;" class="text">
			<div style="background-color:#ffffe1;border:1px solid #bbb;" class="text">
			<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
				<%=LocRM.GetString("tWinLoginEnabledMessage")%>
				<div style="white-space:nowrap; text-align:center; padding-top:15px;">
					<asp:LinkButton runat="server" ID="lbDisableWinLogin" Font-Bold="true" Font-Underline="true" ForeColor="red"></asp:LinkButton>
				</div>
			</blockquote>
			</div>
		</td>
	</tr>
</table>