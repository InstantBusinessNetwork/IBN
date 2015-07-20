<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.ImageFileValue" Codebehind="ImageFileValue.ascx.cs" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.FileUploader.Web.UI" Assembly = "Mediachase.FileUploader" %>
<table border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td valign="top" class="text">
			<mc:mchtmlinputfile id="fAssetFile" class="text" runat="server" Width="320px" />
			<asp:RequiredFieldValidator ID="rfPhoto" ControlToValidate="fAssetFile" Display="Dynamic" ErrorMessage="*" Runat="server" Visible="False"></asp:RequiredFieldValidator>
		</td>
		<td></td>
	</tr>
	<tr style="PADDING-Top:10px">
		<td align="center" valign="middle">
			<div align="left">
				<img alt="" id="imgPhoto" border="0" name="imgPhoto" runat="server" />
			</div>
		</td>
		<td width="25px" valign="top" style="PADDING-LEFT:10px">
			<asp:LinkButton ID="btnDelete" Runat="server" Visible="false" CausesValidation="False" onclick="btnDelete_Click"></asp:LinkButton>
		</td>
	</tr>
</table>
