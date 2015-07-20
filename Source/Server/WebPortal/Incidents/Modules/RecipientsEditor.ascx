<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.RecipientsEditor" Codebehind="RecipientsEditor.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeader.ascx" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<table class="text" cellpadding="7" cellspacing="0" border="0">
				<tr>
					<td style="padding:7px;"><b><%=LocRM.GetString("tEMail")%>:</b></td>
					<td width="220px" style="padding:7px;">
						<asp:TextBox ID="txtEMail" Runat="server" CssClass="text" Width="200px"></asp:TextBox>
						<asp:regularexpressionvalidator id="reEMail" runat="server" ErrorMessage="*" ControlToValidate="txtEMail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:regularexpressionvalidator>
					</td>
					<td style="padding:7px;"><button ID="btnAdd" Runat="server" class="text" onserverclick="btnAdd_Click"></Button></td>
				</tr>
				<tr>
					<td colspan="3" style="padding:7px;" class="ibn-propertysheet">
						<asp:HyperLink ID="lblAddContacts" runat="server"></asp:HyperLink>
						<asp:LinkButton ID="lbAddClient" runat="server" Visible="false" OnClick="lbAddClient_Click"></asp:LinkButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding:5px;">
			<div style='OVERFLOW-Y:auto;HEIGHT:<%=Request.Browser.Browser.IndexOf("IE")>=0 ? "255" : "260" %>px'>
				<asp:DataGrid Runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" 
						cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%">
					<Columns>
						<asp:BoundColumn DataField="EMailIssueExternalRecipientId" Visible="False"></asp:BoundColumn>
						<asp:TemplateColumn>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<ItemTemplate>
								<%# DataBinder.Eval(Container.DataItem, "EMail")%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:templatecolumn>
							<ItemStyle CssClass="ibn-vb2" Width="26px" HorizontalAlign="Center"></ItemStyle>
							<HeaderStyle CssClass="ibn-vh2" Width="26px"></HeaderStyle>
							<itemtemplate>
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" commandname="Delete" ImageAlign="AbsMiddle" causesvalidation="False"></asp:imagebutton>
							</itemtemplate>
						</asp:templatecolumn>
					</Columns>
				</asp:DataGrid>
			</div>
		</td>
	</tr>
</table>
<Button id="btnSave" runat="server" style="DISPLAY:none" onserverclick="btnSave_Click"></Button>
<input type="hidden" id="hdnCurrent" runat="server" />
<script language=javascript>
	function FuncSave()
	{
		<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>;
	}
	function FuncCancel()
	{
		window.close();
	}
</script>