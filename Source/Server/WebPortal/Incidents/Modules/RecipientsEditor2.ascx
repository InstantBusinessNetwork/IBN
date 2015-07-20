<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecipientsEditor2.ascx.cs" Inherits="Mediachase.UI.Web.Incidents.Modules.RecipientsEditor2" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td class="ibn-alternating ibn-navline" style="height: 65px;">
			<table class="text" cellspacing="0" border="0" style="height: 100%;">
				<tr>
					<td style="padding: 5px; vertical-align: top;"><b><%=LocRM.GetString("tEMail")%>:</b></td>
					<td style="padding: 5px; vertical-align: top;" width="220px">
						<asp:TextBox ID="txtEMail" Runat="server" CssClass="text" Width="200px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rfEMail" Runat="server" CssClass="text" ErrorMessage="*" ControlToValidate="txtEMail" Display="Dynamic"></asp:RequiredFieldValidator>
						<asp:regularexpressionvalidator id="reEMail" runat="server" ErrorMessage="*" ControlToValidate="txtEMail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:regularexpressionvalidator>
					</td>
					<td style="vertical-align: top; padding: 5px;"><button ID="btnAdd" Runat="server" class="text" onserverclick="btnAdd_Click"></Button></td>
				</tr>
				<tr>
					<td colspan="3">
						<asp:LinkButton runat="server" ID="lbContact" Text="¬ыбрать внешнего получател€ из списка контактов" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td style="padding:5px;">
			<div style='OVERFLOW-Y:auto;HEIGHT:<%=Request.Browser.Browser.IndexOf("IE")>=0 ? "230" : "235" %>px'>
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
	<tr>
		<td class="ibn-alternating ibn-navline" style="height: 32px;">
			<table class="text" cellspacing="0" border="0" style="height: 100%; width: 100%;">
				<tr>
					<td align="right">
						<asp:Button runat="server" ID="btnSaveVisible" />
						<asp:Button runat="server" ID="btnCancelVisible" CausesValidation="false" />
					</td>
				</tr>
			</table>		
		</td>
	</tr>
</table>

<Button id="btnSave" runat="server" style="DISPLAY:none" onserverclick="btnSave_Click"></Button>
<input type="hidden" id="hdnCurrent" runat="server" />
<script type="text/javascript">
	function FuncSave()
	{
		<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>;
	}
	function FuncCancel()
	{
		window.close();
	}
</script>