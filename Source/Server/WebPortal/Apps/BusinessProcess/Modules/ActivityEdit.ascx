<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.ActivityEdit" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="0" width="600px">
	<tr>
		<td style="padding:10px;">
			<table width="100%" border="0" class="ibn-propertysheet pad5" width="100%">
				<tr>
					<td class="ibn-label" style="width:150px;">
						<asp:Literal runat="server" ID="ActivityTypeLabel" Text="<%$ Resources: IbnFramework.BusinessProcess, ActivityType %>"></asp:Literal>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="ActivityTypeList" Width="350" AutoPostBack="true" 
							onselectedindexchanged="ActivityTypeList_SelectedIndexChanged"></asp:DropDownList>
					</td>
				</tr>
			</table>
			<asp:PlaceHolder runat="server" ID="MainPlaceHolder"></asp:PlaceHolder>
		</td>
	</tr>
	<tr>
		<td style="padding-top:10px; text-align:center; ">
			<btn:imbutton class="text" id="SaveButton" style="width:110px;" 
				Runat="server" onserverclick="SaveButton_ServerClick" />&nbsp;&nbsp;
			<btn:imbutton class="text" id="CancelButton" style="width:110px;" 
				Runat="server" IsDecline="true" CausesValidation="false" />
		</td>
	</tr>
</table>
