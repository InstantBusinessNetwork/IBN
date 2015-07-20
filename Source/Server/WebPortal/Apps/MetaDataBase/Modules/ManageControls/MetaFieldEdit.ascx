<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetaFieldEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls.MetaFieldEdit" %>
<%@ register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaFormSelector" Src="~/Apps/MetaDataBase/Modules/ManageControls/MetaFormSelector.ascx" %>
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
<asp:UpdatePanel runat="server" ID="UpdatePanel1">
	<ContentTemplate>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td><ibn:BlockHeader id="MainBlockHeader" runat="server" /></td>
	</tr>
	<tr>
		<td>
			<table cellpadding="0" cellspacing="15" border="0" width="100%">
				<tr>
					<td colspan="2">
						<asp:Label Runat="server" ID="ErrorMessage" CssClass="ibn-error"></asp:Label>
					</td>
				</tr>
				<tr>
					<td valign="top" style="width:50%;">
						<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed;">
							<tr>
								<td class="ibn-label" style="width:130px">
									<asp:Label runat="server" ID="TableLabel"></asp:Label>:
								</td>
								<td>
									<asp:HyperLink runat="server" ID="TableLink"></asp:HyperLink>
								</td>
								<td style="width:20px"></td>
								<td style="width:20px"></td>
							</tr>
							<tr runat="server" id="NameRow">
								<td class="ibn-label" valign="top" style="padding-top:5px;">
									<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, FieldName%>" />:
								</td>
								<td>
									<asp:TextBox Runat="server" ID="NameTextBox" Width="99%" MaxLength="50"></asp:TextBox>
									<asp:RegularExpressionValidator ID="NameRegExValidator" ControlToValidate="NameTextBox" Runat="server" Display="Dynamic" ErrorMessage="<%$Resources : IbnFramework.GlobalMetaInfo, LatinOnlyError%>" ValidationExpression="^[A-Za-z0-9](\w)*$"></asp:RegularExpressionValidator>
								</td>
								<td colspan="2">
									<asp:RequiredFieldValidator id="NameRequiredValidator" runat="server" ErrorMessage="*" ControlToValidate="NameTextBox" Display="Dynamic"></asp:RequiredFieldValidator>
								</td>
							</tr>
							<tr>
								<td class="ibn-label">
									<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, FriendlyName%>" />:
								</td>
								<td>
									<asp:TextBox Runat="server" ID="FriendlyNameTextBox" Width="99%" MaxLength="100"></asp:TextBox>
								</td>
								<td>
										<img src='<%=ResolveClientUrl("~/images/IbnFramework/resource.gif")%>' title='<%=GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ResourceTooltip").ToString()%>' 
											onclick="SetText('<%=FriendlyNameTextBox.ClientID%>','{ResourceName:ResourceKey}','<%=FriendlyNameRequiredValidator.ClientID %>')" style="width:16px; height:16px" alt=""/>
								</td>
								<td>
									<asp:RequiredFieldValidator id="FriendlyNameRequiredValidator" runat="server" ErrorMessage="*" ControlToValidate="FriendlyNameTextBox" Display="Dynamic"></asp:RequiredFieldValidator>
								</td>
							</tr>
							<tr>
								<td></td>
								<td colspan="3">
									<asp:CheckBox Runat="server" ID="AllowNullsCheckBox" Checked="True" Text="<%$Resources : IbnFramework.GlobalMetaInfo, AllowNulls%>"></asp:CheckBox>
								</td>
							</tr>
							<tr id="trSelector" runat="server">
								<td class="ibn-label" valign="top">
									<asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, AddToForm%>" />:
								</td>
								<td valign="top">
									<ibn:MetaFormSelector ID="mfs" runat="server" />
								</td>
							</tr>
						</table>
						<br />
						<asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" Visible="false" CssClass="text"></asp:Label>
					</td>
					<td valign="top" style="width:50%;">
						<table cellpadding="3" cellspacing="1" border="0" width="100%" class="ibn-propertysheet">
							<tr>
								<td class="ibn-label" style="width:120px;">
									<asp:Literal ID="Literal4" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, FieldType%>" />:
								</td>
								<td>
									<asp:DropDownList Runat="server" ID="FieldTypeList" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="FieldTypeList_SelectedIndexChanged"></asp:DropDownList>
								</td>
								<td style="width:20px;"></td>
							</tr>
							<tr id="FormatRow" runat="server">
								<td class="ibn-label" style="width:120px;">
									<asp:Literal ID="FormatLabel" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Format%>" />:
								</td>
								<td>
									<asp:DropDownList Runat="server" ID="FormatList" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="FormatList_SelectedIndexChanged"></asp:DropDownList>
								</td>
								<td style="width:20px;"></td>
							</tr>
						</table>
						<asp:PlaceHolder runat="server" ID="MainPlaceHolder"></asp:PlaceHolder>
						
						<div style="padding: 20px 0px 10px 125px;">
							<btn:IMButton runat="server" class="text" style="width:110px" ID="SaveButton" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Save%>" OnServerClick="SaveButton_ServerClick"></btn:IMButton>&nbsp;&nbsp;
							<btn:IMButton runat="server" class="text" style="width:110px" ID="CancelButton" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel%>" CausesValidation="false" IsDecline="True"></btn:IMButton>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>