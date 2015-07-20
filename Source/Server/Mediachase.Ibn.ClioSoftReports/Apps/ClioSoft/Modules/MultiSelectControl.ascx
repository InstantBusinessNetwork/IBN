<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiSelectControl.ascx.cs" Inherits="Mediachase.Ibn.Apps.ClioSoft.Modules.MultiSelectControl" %>
<table width="250px" cellpadding="0" cellspacing="0" border="0" class="ibn-propertysheet" style="height:22px;table-layout:fixed" runat="server" id="MainTable">
	<tr>
		<td id="ValueCell" runat="server" style="height:22px;"><div class="dropstyle"><asp:Label runat="server" ID="SelectedLabel"></asp:Label></div></td>
		<td id="ButtonCell" runat="server" style="height:22px;" class="btndown" ><asp:Label ID="ChangeLabel" Runat="server"></asp:Label></td>
	</tr>
</table>
<div id="DropDownDiv" runat="server" style="position:absolute; top:30px;left:100px; z-index:10000;padding:0px;display:none; border: 1px solid #95b7f3; width:250px;" class="ibn-rtetoolbarmenu ibn-alternating" onclick="msc_SafeCancelBubble(event);">
	<table class="text" border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td class="ibn-navline" valign="top">
				<asp:CheckBox Runat="server" ID="AllItemsCheckBox" Font-Bold="True" CssClass="text"></asp:CheckBox>
			</td>
		</tr>
		<tr>
			<td class="ibn-navline">
				<div style="height:200px; overflow-y: auto;" runat="server" id="divBlock">
					<asp:DataGrid id="MainGrid" runat="server" allowsorting="False" allowpaging="False" width="100%" autogeneratecolumns="False" borderwidth="0" gridlines="None" cellpadding="0" CellSpacing="0" ShowFooter="False" ShowHeader="False">
						<columns>
							<asp:boundcolumn visible="false"></asp:boundcolumn>
							<asp:templatecolumn itemstyle-cssclass="text">
								<itemtemplate>
									<asp:checkbox runat="server" id="chkItem" Text='<%# Eval(DataTextField)%>'></asp:checkbox>
								</itemtemplate>
							</asp:templatecolumn>
						</columns>
					</asp:DataGrid>
				</div>
			</td>
		</tr>
		<tr>
			<td align="center" style="padding:5px;">
				<asp:Button runat="server" ID="SaveButton" OnClick="SaveButton_Click" Width="100"/>
			</td>
		</tr>
	</table>
</div>
