<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnumEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls.EnumEdit" %>
<%@ register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader.ascx" %>
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
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
    <td>
      <ibn:BlockHeader id="secHeader" runat="server" />
    </td>
  </tr>
	<tr>
		<td style="padding:5px">
			<table cellpadding="5" cellspacing="0" border="0" class="ibn-propertysheet" width="500">
				<tr>
					<td class="ibn-label" width="120px">
						<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, SystemName%>" />:
					</td>
					<td>
						<asp:TextBox Runat="server" ID="txtEnumName" Width="100%" MaxLength="50"></asp:TextBox>
					</td>
					<td width="20px">
						<asp:RequiredFieldValidator id="vldEnumName_Required" runat="server" ErrorMessage="*" ControlToValidate="txtEnumName" Display="Dynamic"></asp:RequiredFieldValidator>
						<asp:RegularExpressionValidator ID="vldEnumName_RegEx" ControlToValidate="txtEnumName" Runat="server" ValidationExpression="^[A-Za-z0-9](\w)*$"></asp:RegularExpressionValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" width="120px">
						<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, FriendlyName%>" />:
					</td>
					<td>
						<asp:TextBox Runat="server" ID="txtFriendlyName" Width="100%" MaxLength="100"></asp:TextBox>
					</td>
					<td style="width:16px" align="left">
						<img src='<%=ResolveUrl("~/images/IbnFramework/resource.gif")%>' title='<%=GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ResourceTooltip").ToString()%>' alt='' 
							onclick="SetText('<%=txtFriendlyName.ClientID%>','{ResourceName:ResourceKey}','<%=vldFriendlyName_Required.ClientID %>')" style="width:16px; height:16px" />
					</td>
					<td width="20px">
						<asp:RequiredFieldValidator id="vldFriendlyName_Required" runat="server" ErrorMessage="*" ControlToValidate="txtFriendlyName" Display="Dynamic"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td width="120px"></td>
					<td>
						<asp:CheckBox runat="server" ID="chkMultiValue" Text="<%$Resources : IbnFramework.GlobalMetaInfo, MultiValue%>" />
					</td>
					<td></td>
				</tr>
				<tr>
					<td width="120px"></td>
					<td style="text-align:right">
					 
					</td>
					<td></td>
				</tr>
			</table>
		</td>
	</tr>
</table>

<table border="0" cellpadding="0" cellspacing="0" width="450">
	<tr>
		<td align="right" style="padding-top:5px">
			<btn:IMButton runat="server" ID="imbtnSave" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Save%>" class='text' style="width:110px"></btn:IMButton>&nbsp;&nbsp;
			<btn:IMButton runat="server" ID="imbtnCancel" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel%>" CausesValidation="false" class='text' style="width:110px"></btn:IMButton>
		</td>
	</tr>
</table>