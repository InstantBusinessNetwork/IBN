<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetaFieldListEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.MetaFieldListEdit" %>
<%@ register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>
<script type="text/javascript" language="javascript">
	function SetText(id, text)
	{
		var input = document.getElementById(id);
		
		if(input!=null)
		{
			input.value = text;		
		}
	}
	
	function SetName(sourceid, targetid)
	{
		var input = document.getElementById(sourceid);
		if(input!=null && input.value.length>0)
		{
			var input1 = document.getElementById(targetid);
			if(input1.value.length==0)
				input1.value = input.value;
		}
	}
	
	function MFListAdd_CheckName()
	{
		var input = document.getElementById('<%=NameTextBox.ClientID %>');
		if(input==null || input.value.length>0)
		{
			<%= this.Page.ClientScript.GetPostBackEventReference(btnUpdate, "") %>;
		}
		else
		{
			var error = document.getElementById('<%=lblNotEmptyName.ClientID %>');
			if(error)
				error.style.display = "";
			return false;
		}
	}
</script>
<style type="text/css">
	table.padTable5 tbody tr td
	{
		padding: 5px ! important;
	}
	table.padTable3 tbody tr td
	{
		padding: 3px ! important;
	}
</style>
<table cellpadding="0" border="0" width="100%" class="ibn-propertysheet padTable5">
	<tr>
		<td colspan="2">
			<asp:Label Runat="server" ID="ErrorMessage" CssClass="ibn-error"></asp:Label>
		</td>
	</tr>
	<tr>
		<td valign="top">
			<table border="0" width="100%" class="ibn-propertysheet padTable3" style="table-layout:fixed;">
				<tr>
					<td class="ibn-label" style="width:130px">
						<asp:Label runat="server" ID="TableLabel"></asp:Label>:
					</td>
					<td>
						<asp:Label ID="tblName" runat="server" CssClass="text" Font-Bold="true"></asp:Label>
					</td>
					<td style="width:20px"></td>
					<td style="width:20px"></td>
				</tr>
				<tr runat="server" id="NameRow">
					<td class="ibn-label">
						<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, FieldName%>" />:
					</td>
					<td>
						<asp:TextBox Runat="server" ID="NameTextBox" Width="99%" MaxLength="50"></asp:TextBox>
					</td>
					<td colspan="2">
						<asp:Label ID="lblNotEmptyName" runat="server" ForeColor="red" Text="*"></asp:Label>
						<asp:RegularExpressionValidator ID="NameRegExValidator" ControlToValidate="NameTextBox" Runat="server" Display="Dynamic" ErrorMessage="*" ValidationExpression="^[A-Za-z0-9](\w)*$"></asp:RegularExpressionValidator>
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
								onclick="SetText('<%=FriendlyNameTextBox.ClientID%>','{ResourceName:ResourceKey}')" style="width:16px; height:16px" alt=""/>
					</td>
					<td></td>
				</tr>
				<tr>
					<td></td>
					<td colspan="3">
						<asp:CheckBox Runat="server" ID="AllowNullsCheckBox" Checked="True"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" style="width:120px;">
						<asp:Literal ID="Literal4" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, FieldType%>" />:
					</td>
					<td>
						<asp:DropDownList Runat="server" ID="FieldTypeList" Width="100%" CssClass="text" AutoPostBack="True" OnSelectedIndexChanged="FieldTypeList_SelectedIndexChanged"></asp:DropDownList>
					</td>
					<td style="width:20px;"></td>
				</tr>
				<tr id="FormatRow" runat="server">
					<td class="ibn-label" style="width:120px;">
						<asp:Literal ID="Literal5" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Format%>" />:
					</td>
					<td>
						<asp:DropDownList Runat="server" ID="FormatList" Width="100%" CssClass="text" AutoPostBack="True" OnSelectedIndexChanged="FormatList_SelectedIndexChanged"></asp:DropDownList>
					</td>
					<td style="width:20px;"></td>
				</tr>
			</table>
			<br />
			<asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" Visible="false" CssClass="text"></asp:Label>
			<asp:PlaceHolder runat="server" ID="MainPlaceHolder"></asp:PlaceHolder>
		</td>
	</tr>
	<tr>
		<td align="right">
			<div style="padding: 10px 0px 10px 125px;">
				<btn:IMButton runat="server" class="text" style="width:110px" ID="SaveButton" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Save%>"></btn:IMButton>&nbsp;&nbsp;
				<btn:IMButton runat="server" class="text" style="width:110px" ID="CancelButton" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel%>" CausesValidation="false" IsDecline="True"></btn:IMButton>
			</div>
		</td>
	</tr>
</table>
<asp:Button ID="btnUpdate" runat="server" OnClick="SaveButton_ServerClick" Visible="false" />