<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StateMachineEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.StateMachine.Modules.ManageControls.StateMachineEdit" %>
<script type="text/javascript" src='<%=ResolveUrl("~/Scripts/IbnFramework/List2List.js")%>'></script>

<script type="text/javascript">
function SaveStates()	
{
	var control = document.getElementById('<%=lstSelectedStates.ClientID%>');
	
	var str="";
	if(control != null)
	{
		for(var i=0;i<control.options.length;i++)
		{
			if (str != "")
				str += ",";
			str += control.options[i].value;
		}
	}
	document.getElementById('<%=hdnStates.ClientID%>').value = str;
}

function SelectedStatesValidate(source, args)
{
	var control = document.getElementById('<%=lstSelectedStates.ClientID%>');
	if (control.options.length > 0)
		args.IsValid = true;
	else
		args.IsValid = false;
}
</script>

<table style="width:100%;"><tr><td style="padding:10px;">
	<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
		<tr style="padding-bottom:10px;">
			<td style='width:<%= labelColumnWidth%>'>
				<asp:Literal ID="Literal1" Text="<%$Resources : IbnFramework.GlobalMetaInfo, StateMachineName%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:TextBox runat="server" ID="txtName" Width="100%"></asp:TextBox>
			</td>
			<td style="width:20px;">
				<asp:RequiredFieldValidator runat="server" ID="rfvName" ControlToValidate="txtName" Display="dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<asp:Literal ID="Literal4" Text="<%$Resources : IbnFramework.GlobalMetaInfo, StateList%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<table class="text" cellpadding="0" cellspacing="0" border="0" style="table-layout:fixed">
					<tr>
						<td valign="top" style="PADDING-RIGHT: 6px; PADDING-BOTTOM: 6px; width:276px;">
							<asp:Literal ID="Literal2" Text="<%$Resources : IbnFramework.GlobalMetaInfo, AvailableStates%>" runat="server"></asp:Literal>
							<br />
							<asp:listbox id="lstAvailableStates" runat="server" cssclass="text" Rows="8" Width="270"></asp:listbox>
						</td>
						<td style="PADDING-RIGHT: 6px; PADDING-LEFT: 6px; PADDING-BOTTOM: 6px" align="center">
							<br />
							<asp:button id="btnAddOne" style="MARGIN: 1px" runat="server" width="30px" cssclass="text" CausesValidation="False" text=">"></asp:button><br />
							<asp:button id="btnAddAll" style="MARGIN: 1px" runat="server" width="30px" cssclass="text" CausesValidation="False" text=">>"></asp:button><br /><br />
							<asp:button id="btnRemoveOne" style="MARGIN: 1px" runat="server" width="30px" cssclass="text" CausesValidation="False" text="<"></asp:button><br />
							<asp:button id="btnRemoveAll" style="MARGIN: 1px" runat="server" width="30px" cssclass="text" CausesValidation="False" text="<<"></asp:button>
						</td>
						<td valign="top" style="PADDING-LEFT: 6px; PADDING-BOTTOM: 6px; width:276px;">
							<asp:Literal ID="Literal3" Text="<%$Resources : IbnFramework.GlobalMetaInfo, SelectedStates%>" runat="server"></asp:Literal>
							<br />
							<asp:listbox id="lstSelectedStates" runat="server" cssclass="text" Rows="8" Width="270"></asp:listbox>
						</td>
						<td align="center">
							<asp:button id="btnUp" style="MARGIN: 1px" runat="server" width="30px" cssclass="text" CausesValidation="False" text="↑"></asp:button><br />
							<asp:button id="btnDn" style="MARGIN: 1px" runat="server" width="30px" cssclass="text" CausesValidation="False" text="↓"></asp:button><br />
						</td>
					</tr>
				</table>
			</td>
			<td valign="top" style="padding-top:15px;">
				<asp:CustomValidator runat="server" ID="vldSelectedStates" Display="dynamic" ErrorMessage="*" ClientValidationFunction="SelectedStatesValidate" OnServerValidate="vldSelectedStates_ServerValidate"></asp:CustomValidator>
			</td>
		</tr>
	</table>
	<br />
	<div style="padding-left: <%= labelColumnWidth%>; padding-top:10px; text-align:center; padding-right:65px;">
	<asp:Button runat="server" ID="btnSave" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClientClick="SaveStates();" OnClick="btnSave_Click" Width="80" />
	<asp:Button runat="server" ID="btnCancel" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" OnClientClick="window.close();" Width="80" style="margin-left:15px;" CausesValidation="false" />
	</div>
	<asp:HiddenField runat="server" ID="hdnStates" />
</td></tr></table>