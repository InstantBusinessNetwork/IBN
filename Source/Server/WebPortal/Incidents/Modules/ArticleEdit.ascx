<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Incidents.Modules.ArticleEdit" CodeBehind="ArticleEdit.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/FormHelper.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Register TagPrefix="ibn" TagName="FormHelper" Src="..\..\Modules\FormHelper.ascx" %>
<table class="ibn-stylebox text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<ibn:BlockHeader ID="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="padding-right: 15px; padding-left: 15px; padding-bottom: 15px; padding-top: 15px">
			<table class="ibn-propertysheet" cellspacing="3" cellpadding="3" width="100%" border="0" style="table-layout: fixed">
				<colgroup>
					<col width="80px" />
					<col />
					<col width="100px" />
					<col width="50px" />
				</colgroup>
				<tr>
					<td width="80" valign="top" class="ibn-label">
						<%= LocRM.GetString("atclQuestion")%>:
					</td>
					<td colspan="3">
						<asp:TextBox ID="txtQuestion" CssClass="text" runat="server" Width="98%" TextMode="MultiLine" Rows="4" MaxLength="1000"></asp:TextBox>
						<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="Text" ControlToValidate="txtQuestion" ErrorMessage=" * "></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td width="80" valign="top" class="ibn-label">
						<%= LocRM.GetString("atclAnswer")%>:
					</td>
					<td colspan="3">
						<FTB:FreeTextBox ID="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink" runat="Server" Width="98%" Height="250px" EnableHtmlMode="true" DropDownListCssClass="text" GutterBackColor="#F5F5F5" BreakMode="LineBreak" BackColor="#F5F5F5" ToolbarBackgroundImage="false" SupportFolder="~/Scripts/FreeTextBox/" JavaScriptLocation="ExternalFile" ButtonImagesLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" HtmlModeCssClass="text" DesignModeBodyTagCssClass="text" />
					</td>
				</tr>
				<tr>
					<td width="80" class="ibn-label">
						<%= LocRM.GetString("atclTags")%>:
					</td>
					<td>
						<asp:TextBox ID="txtTags" CssClass="text" runat="server" Width="80%" MaxLength="1000"></asp:TextBox>
						<ibn:FormHelper ID="fhTags" runat="server" ResKey="ArticleEdit_Tags" Width="250px" Position="TL" />
					</td>
					<td width="100px" class="ibn-label">
						<%= LocRM.GetString("atclDelimiter")%>:
					</td>
					<td width="50px">
						<asp:DropDownList runat="server" ID="ddlDelimiter">
							<asp:ListItem Text=" " Value=" "></asp:ListItem>
							<asp:ListItem Text="," Value=","></asp:ListItem>
							<asp:ListItem Text=";" Value=";"></asp:ListItem>
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td valign="bottom" align="right" height="40">
					</td>
					<td valign="bottom" align="left" height="40" colspan="3">
						<btn:IMButton class="text" ID="btnSave" style="width: 110px;" runat="server" OnServerClick="btnSave_ServerClick">
						</btn:IMButton>
						&nbsp;&nbsp;
						<btn:IMButton class="text" ID="btnCancel" style="width: 110px;" runat="server" Text="" IsDecline="true" CausesValidation="false" OnServerClick="btnCancel_ServerClick">
						</btn:IMButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<input id="iGroups" style="visibility: hidden" runat="server" name="iGroups" />
