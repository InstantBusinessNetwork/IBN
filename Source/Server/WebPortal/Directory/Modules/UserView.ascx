<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.UserView" Codebehind="UserView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="Sharing" src="..\..\Directory\Modules\Sharing.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td width="60%" valign="top" id="tdPrefs" runat="server" style="padding-right:7px;">
			<div style="margin-top:5px">
				<ibn:blockheader id="tbPreferences" title="User Preferences" runat="server"></ibn:blockheader>
			</div>
			<table class="ibn-stylebox-light" id="tblPreferences" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server">
				<tr>
					<td style="padding-left: 7px">
						<table class="text" id="tbl" cellspacing="0" cellpadding="5" width="100%" border="0">
							<tr id="trTimeZone">
								<td valign="top" class="ibn-label"><asp:label id="lblTimeZoneTitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblTimeZone" runat="server"></asp:label></td>
							</tr>
							<tr>
								<td valign="top" class="ibn-label"><asp:label id="lblLangTitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblLang" runat="server"></asp:label></td>
							</tr>
							<tr>
								<td valign="top" class="ibn-label"><asp:label id="lblMIATitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblMenuInAlerts" runat="server"></asp:label></td>
							</tr>
							<tr>
								<td valign="top" class="ibn-label"><asp:label id="lblNotifyEnabledTitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblNotifyEnabled" Runat="server"></asp:label></td>
							</tr>
							<tr id="trNotifyDelay" runat="server">
								<td valign="top" class="ibn-label"><asp:label id="lblNotifyDelayTitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblNotifyDelay" Runat="server"></asp:label></td>
							</tr>
							<tr id="trNotifyBy" runat="server">
								<td valign="top" class="ibn-label"><asp:label id="lblNotifyByTitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblNotifyBy" Runat="server"></asp:label></td>
							</tr>
							<tr id="trBatchLast" runat="server">
								<td valign="top" class="ibn-label"><asp:label id="lblLastBatchTitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblLastBatch" Runat="server"></asp:label></td>
							</tr>
							<tr id="trBatchNext" runat="server">
								<td valign="top" class="ibn-label"><asp:label id="lblNextBatchTitle" Runat="server"></asp:label>:</td>
								<td valign="top" class="ibn-value"><asp:label id="lblNextBatch" Runat="server"></asp:label></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<td width="40%" valign="top">
			<div style="margin-top:5px">
				<ibn:blockheader id="tbEMails" runat="server"></ibn:blockheader>
			</div>
			<table class="ibn-stylebox-light" id="tblEMails" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td style="padding-right: 7px; padding-left:7px" valign="top" align="left" width="100%" rowspan="14">
						<asp:datagrid id="dgEmails" Runat="server" AutoGenerateColumns="False" AllowSorting="False"
							cellpadding="3" gridlines="None" CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
							<Columns>
								<asp:BoundColumn Visible="False" DataField="EmailId"></asp:BoundColumn>
								<asp:TemplateColumn>
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
									<ItemTemplate>
										<a href='mailto:<%# DataBinder.Eval(Container.DataItem,"Email")%>'>
											<%# DataBinder.Eval(Container.DataItem,"Email") %>
										</a>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="tbEmail" Text='<%# DataBinder.Eval(Container.DataItem,"Email") %>' CssClass="text" Width="90%" Runat="server">
										</asp:TextBox>
										<asp:RequiredFieldValidator ID="rfEmail" ControlToValidate="tbEmail" ErrorMessage='*' Display="Dynamic" Runat="server"/>
										<asp:regularexpressionvalidator id="reEmail" runat="server" ErrorMessage="*" ControlToValidate="tbEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:regularexpressionvalidator>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Right" Width="78px" CssClass="ibn-vh-right"></HeaderStyle>
									<ItemStyle Width="78px" CssClass="ibn-vb2"></ItemStyle>
									<ItemTemplate>
										<asp:imagebutton id="Imagebutton3" title='<%# LocRM.GetString("tSetPrimary")%>' runat="server" borderwidth="0" imageurl="../../layouts/images/icon-key.gif" commandname="SetPrimary" causesvalidation="False">
										</asp:imagebutton>
										&nbsp;
										<asp:imagebutton id="ibMove" title='<%# LocRM.GetString("tbEditEdit")%>' runat="server" borderwidth="0" imageurl="../../layouts/images/edit.gif" commandname="Edit" causesvalidation="False">
										</asp:imagebutton>
										&nbsp;
										<asp:imagebutton id="ibDelete" title='<%# LocRM.GetString("tDelete")%>' runat="server" borderwidth="0" imageurl="../../layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
										</asp:imagebutton>
									</ItemTemplate>
									<EditItemTemplate>
										&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										<asp:imagebutton id="Imagebutton1" title='<%# LocRM.GetString("tSave")%>' runat="server" borderwidth="0" imageurl="../../layouts/images/Saveitem.gif" commandname="Update" causesvalidation="True">
										</asp:imagebutton>
										&nbsp;
										<asp:imagebutton id="Imagebutton2" title='<%# LocRM.GetString("tCancel")%>' runat="server" borderwidth="0" imageurl="../../layouts/images/cancel.gif" commandname="Cancel" causesvalidation="False">
										</asp:imagebutton>
									</EditItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:datagrid>
						<asp:Label ID="lblEmailDuplication" Runat="server" CssClass="ibn-error" Visible="False"></asp:Label>
					</td>
				</tr>
			</table>
			
			<ibn:sharing id="Sharing" runat="server"></ibn:sharing>
		</td>
	</tr>
</table>
<asp:Button CssClass="text" ID="btnAddNewItem" Runat="server" CausesValidation="False" style="DISPLAY:none" onclick="btnAddNewItem_Click"></asp:Button>