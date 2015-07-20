<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnumMultiValue.Manage.@.@.ListInfoImport.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Primitives.EnumMultiValue_Manage_All_All_ListInfoImport" %>
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
</script>
<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
	<tr id="trName" runat="server">
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, tName %>" />:
		</td>
		<td>
			<asp:TextBox ID="txtEnumName" runat="server" Width="100%"></asp:TextBox>
		</td>
		<td></td>
		<td width="20px">
			<asp:RequiredFieldValidator id="vldEnumName_Required" runat="server" ErrorMessage="*" ControlToValidate="txtEnumName" Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:RegularExpressionValidator ID="vldEnumName_RegEx" ControlToValidate="txtEnumName" Runat="server" ValidationExpression="^[A-Za-z0-9](\w)*$"></asp:RegularExpressionValidator>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" width="120px">
		<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, FriendlyName %>" />:
		</td>
		<td>
			<asp:TextBox ID="txtFriendlyName" runat="server" Width="100%"></asp:TextBox>
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
		<td></td>
		<td>
			<asp:CheckBox runat="server" ID="chkEditable" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, EditableDictionary%>" Checked="false" />
		</td>
		<td></td>
		<td></td>
	</tr>
	<tr>
		<td></td>
		<td>
			<asp:CheckBox runat="server" ID="chkPublic" Text="<%$Resources : IbnFramework.GlobalFieldManageControls, PublicDictionary%>" Checked="false" />
		</td>
		<td></td>
		<td></td>
	</tr>
</table>