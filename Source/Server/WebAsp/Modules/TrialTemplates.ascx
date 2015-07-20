<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.TrialTemplates" CodeBehind="TrialTemplates.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<table class="ibn-stylebox text" width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top: 0px">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td align="left" style="padding-left: 15px; padding-bottom: 15px; padding-top: 15px">
			<table id="table2" cellspacing="0" cellpadding="0" border="0" width="100%">
				<tr>
					<td>
						<img alt="" src="../layouts/images/blank.gif" width="1" height="9" />
					</td>
				</tr>
				<tr>
					<td class="ibn-sectionheader" colspan="3">
						Customer Templates
					</td>
				</tr>
				<tr>
					<td>
						<img alt="" src="../layouts/images/blank.gif" width="1" height="2" />
					</td>
				</tr>
				<tr>
					<td class="ibn-sectionline" colspan="3">
						<img alt="" src="../layouts/images/blank.gif" width="1" height="1" />
					</td>
				</tr>
				<tr>
					<td class="ibn-propertysheet" valign="top">
						<asp:DataGrid ID="dgCustomer" runat="server" AllowPaging="False" AllowSorting="False" CellPadding="1" GridLines="Horizontal" CellSpacing="3" BorderWidth="0px" AutoGenerateColumns="False" Width="100%">
							<Columns>
								<asp:BoundColumn DataField="TemplateName" HeaderText="Template Name">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Subject" HeaderText="Subject">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Attachments" HeaderText="Attachments">
									<HeaderStyle CssClass="ibn-vh2" Width="200px"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2" Width="200px"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="ActionEdit" HeaderStyle-Width="25" ItemStyle-Width="25"></asp:BoundColumn>
							</Columns>
						</asp:DataGrid>
					</td>
				</tr>
				<tr>
					<td>
						<img alt="" src="../layouts/images/blank.gif" width="1" height="9" />
					</td>
				</tr>
				<tr>
					<td class="ibn-sectionheader" colspan="3">
						Operator Notifications
					</td>
				</tr>
				<tr>
					<td>
						<img alt="" src="../layouts/images/blank.gif" width="1" height="2" />
					</td>
				</tr>
				<tr>
					<td class="ibn-sectionline" colspan="3">
						<img alt="" src="../layouts/images/blank.gif" width="1" height="1" />
					</td>
				</tr>
				<tr>
					<td class="ibn-propertysheet" valign="top">
						<asp:DataGrid ID="dgTrial" runat="server" AllowPaging="False" AllowSorting="False" CellPadding="1" GridLines="Horizontal" CellSpacing="3" BorderWidth="0px" AutoGenerateColumns="False" Width="100%">
							<Columns>
								<asp:BoundColumn DataField="TemplateName" HeaderText="Template Name">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Subject" HeaderText="Subject">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Attachments" HeaderText="Attachments">
									<HeaderStyle CssClass="ibn-vh2" Width="200px"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2" Width="200px"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="ActionEdit" HeaderStyle-Width="25" ItemStyle-Width="25"></asp:BoundColumn>
							</Columns>
						</asp:DataGrid>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
