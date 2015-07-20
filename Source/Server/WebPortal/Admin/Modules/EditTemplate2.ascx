<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.EditTemplate2" Codebehind="EditTemplate2.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<script language="javascript">
function InsertHTML(arg)
{
	//FTB_InsertText('
	//fckEditor.ClientID
	//',"[%=" +arg + "=%]");
}
function InsertText(argTxt)
{
	var tbBodyEl = document.getElementById('<%=tbBody.ClientID%>');
	tbBodyEl.value += argTxt;
}
</script>
<table class="ibn-stylebox2 text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHdr" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<table class="text" cellspacing="3" cellpadding="3" width="100%" border="0">
				<tr id="trSubject" runat="server">
					<td vAlign="top" width="80" rowspan="2">
						<!-- Left Frame -->
						<asp:DataList Runat="server" id="dlSysVar" CellSpacing="2" ShowHeader="False" ShowFooter="False" RepeatColumns="1" Width="100%">
							<ItemStyle CssClass="text"></ItemStyle>
							<ItemTemplate>
								<a href='javascript:InsertText("<%# DataBinder.Eval(Container.DataItem, "Value") %>")' title='<%# DataBinder.Eval(Container.DataItem, "Description") %>'>
									<%# DataBinder.Eval(Container.DataItem, "Name") %>
								</a>
							</ItemTemplate>
						</asp:DataList>
						<!-- End Left Frame -->
					</td>
					<td align="left" width="60" class="ibn-label"><%=LocRM.GetString("Subject") %>:</td>
					<td align="right"><asp:textbox id="tbSubject" Runat="server" CssClass="text" Width="100%"></asp:textbox></td>
				</tr>
				<tr>
					
					<td vAlign="top" align="left" width="60" class="ibn-label"><%=LocRM.GetString("Body") %>:</td>
					<td align="right" valign="top">
						<asp:TextBox Runat="server" ID="tbBody" Width="100%" TextMode="MultiLine" Height="255px"></asp:TextBox>
					</td>
				</tr>
			</table>
			<table cellspacing="3" cellpadding="3" width="100%" align="right" border="0">
				<tr>
					<td width="100%"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' width="6" height="1" alt=""></td>
					<td align="right" id="diidSubmitsection">
						<btn:ImButton ID="btnSave" Runat="server" Class="text" style="width:110px;" onserverclick="btnSave_Click"></btn:ImButton>
					</td>
					<td width="6"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' width="6" height="1" alt=""></td>
					<td align="right"><btn:ImButton ID="btnCancel" CausesValidation="false" Runat="server" Class="text" style="width:110px;" IsDecline="true" onserverclick="btnCancel_Click"></btn:ImButton>
					</td>
					<td width="6"><img src='<%= ResolveClientUrl("~/layouts/images/blank.gif") %>' width="6" height="1" alt=""></td>
					<td align="right"><btn:ImButton ID="btnReset" CausesValidation="false" Runat="server" Class="text" style="width:110px;" IsDecline="true"></btn:ImButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
