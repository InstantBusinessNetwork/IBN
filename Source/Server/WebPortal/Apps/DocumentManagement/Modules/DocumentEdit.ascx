<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.DocumentManagement.Modules.DocumentEdit" %>
<%@ Reference VirtualPath="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx" %>
<%@ Register TagPrefix="ibn" TagName="formDocView" Src="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx "%>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-stylebox2">
	<tr id="trToolbar">
		<td>
			<mc:BlockHeader2 ID="MainBlockHeader" runat="server" />
		</td>
	</tr>
	<tr id="trDocType" runat="server">
		<td style="padding:10px;" class="ibn-navline ibn-alternating">
			<table>
				<tr>
					<td style="font-weight:bold; padding-right:15px;">
						<%=GetGlobalResourceObject("IbnFramework.Document", "DocumentType").ToString()%>:
					</td>
					<td>
						<asp:DropDownList ID="ddType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddType_SelectedIndexChanged" Width="250px"></asp:DropDownList>
					</td>
					<td style="font-weight:bold; padding-right:15px;padding-left:30px;">
						<%=GetGlobalResourceObject("IbnFramework.Document", "DocumentTemplate").ToString()%>:
					</td>
					<td>
						<asp:DropDownList ID="ddTemplate" runat="server" Width="250px"></asp:DropDownList>
					</td>
				</tr>
			</table>
			
		</td>
	</tr>
	<tr>
		<td valign="top">
			<div id="mainDiv" style="overflow:auto;">
				<ibn:formDocView ID="formView" runat="server" />
			</div>
		</td>
	</tr>
</table>
<asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" style="display:none;" />
