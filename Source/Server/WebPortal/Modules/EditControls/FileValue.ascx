<%@ Reference Control="~/Modules/EditControls/ImageFileValue.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.FileValue" Codebehind="FileValue.ascx.cs" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.FileUploader.Web.UI" Assembly = "Mediachase.FileUploader" %>
<table border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td valign="top" class="text">
			<mc:mchtmlinputfile id="fAssetFile" class="text" runat="server" Width="320" />
			<asp:RequiredFieldValidator ID="rfFile" ControlToValidate="fAssetFile" Display="Dynamic" ErrorMessage="*" Runat="server" Visible="False"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr style="PADDING-Top:10px">
		<td align="left" valign=middle>
			<asp:Label ID="lblLink" Runat=server CssClass="text" Visible=False></asp:Label>&nbsp;&nbsp;
			<asp:LinkButton ID="btnDelete" Runat="server" Visible=False onclick="btnDelete_Click"></asp:LinkButton>
		</td>
	</tr>
</table>