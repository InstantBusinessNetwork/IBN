<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.AddBroadcastAlert" Codebehind="AddBroadcastAlert.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ftb" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox"
	style="margin-top: 0px">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server" Title="" />
		</td>
	</tr>
	<tr>
		<td class="ibn-propertysheet">
			<table cellpadding="3" class="text" cellspacing="3" border="0">
				<tr>
					<td valign="top" width="120px" class="text ibn-label" style="padding-left: 10px">
						<%=LocRM.GetString("tMessageText")%>:
					</td>
					<td style="width: 400px">
						<ftb:FreeTextBox ID="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline,createlink" runat="Server" Width="100%" Height="120px" EnableHtmlMode="true" DropDownListCssClass="text" GutterBackColor="#F5F5F5" BreakMode="LineBreak" BackColor="#F5F5F5" ToolbarBackgroundImage="false" SupportFolder="~/Scripts/FreeTextBox/" JavaScriptLocation="ExternalFile" ButtonImagesLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" DesignModeBodyTagCssClass="pNoMargin" DesignModeCss="~/styles/IbnFramework/ibn.css" RemoveServerNameFromUrls="false" FormatHtmlTagsToXhtml="true" />
						<asp:RequiredFieldValidator ID="rfText" runat="server" CssClass="text" Display="Dynamic" ErrorMessage="*" ControlToValidate="fckEditor"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td valign="top" class="text ibn-label" style="padding-left: 10px">
						<%=LocRM.GetString("tSelectGroups")%>:
					</td>
					<td>
						<asp:ListBox ID="lbGroups" runat="server" CssClass="text" Rows="7" SelectionMode="Multiple" Width="250px"></asp:ListBox><br>
						<asp:CustomValidator ID="cvGroup" runat="server" CssClass="text" Display="Static"></asp:CustomValidator>
					</td>
				</tr>
				<tr>
					<td class="text ibn-label" style="padding-left: 10px">
						<%=LocRM.GetString("tExpDate")%>:
					</td>
					<td>
						<mc:Picker ID="dtcExDate" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" DateIsRequired="true" />
					</td>
				</tr>
				<tr>
					<td valign="bottom" align="right" height="40" colspan="2" class="text" style="padding-left: 10px">
						<div style="white-space: nowrap">
							<btn:imbutton class="text" id="btnSave" Runat="server" Text="" style="width: 115px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
							<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" Text="" IsDecline="true" style="width: 110px;" onserverclick="btnCancel_ServerClick"></btn:imbutton>
						</div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
