<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StateEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.StateMachine.Modules.ManageControls.StateEdit" %>
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
	
	function SetName(sourceid, targetid, validatorid)
	{
		var input = document.getElementById(sourceid);
		if(input!=null && input.value.length>0)
		{
			var input1 = document.getElementById(targetid);
			if(input1.value.length==0)
				input1.value = input.value;
			if(validatorid!=null)
			{	
				input1 = document.getElementById(validatorid);	
				if(input1!=null)
					input1.style.display = "none";
			}
		}
	}
</script>
<table style="width:100%;"><tr><td style="padding:10px;">
	<table cellpadding="3" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
		<tr>
			<td style='width:<%= labelColumnWidth%>'>
				<asp:Literal ID="Literal1" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:TextBox runat="server" ID="txtName" Width="100%"></asp:TextBox>
			</td>
			<td style="width:20px;">
				<asp:RequiredFieldValidator runat="server" ID="rfvName" ControlToValidate="txtName" Display="dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
				<asp:RegularExpressionValidator runat="server" ID="revName" ControlToValidate="txtName" Display="dynamic" ErrorMessage="*" ValidationExpression="^[A-Za-z0-9](\w)*$"></asp:RegularExpressionValidator>
			</td>
			<td style="width:20px"></td>
		</tr>
		<tr>
			<td style='width:<%= labelColumnWidth%>'>
				<asp:Literal ID="Literal5" Text="<%$Resources : IbnFramework.GlobalMetaInfo, FriendlyName%>" runat="server"></asp:Literal>:
			</td>
			<td>
				<asp:TextBox runat="server" ID="txtFriendlyName" Width="100%"></asp:TextBox>
			</td>
			<td>
				<img src='<%=ResolveUrl("~/images/IbnFramework/resource.gif")%>' title='<%=GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ResourceTooltip").ToString()%>' 
						onclick="SetText('<%=txtFriendlyName.ClientID%>','{ResourceName:ResourceKey}','<%=rfvFriendlyName.ClientID %>')" style="width:16px; height:16px" alt=""/>
			</td>
			<td>
				<asp:RequiredFieldValidator runat="server" ID="rfvFriendlyName" ControlToValidate="txtFriendlyName" Display="dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
			</td>
		</tr>
	</table>
	<br />
	<div style="padding-left: <%= labelColumnWidth%>; padding-top:10px; text-align:center; padding-right:65px;">
	<asp:Button runat="server" ID="btnSave" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="btnSave_Click" Width="80" />
	<asp:Button runat="server" ID="btnCancel" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" OnClientClick="window.close();" Width="80" style="margin-left:15px;" CausesValidation="false" />
	</div>
</td></tr></table>