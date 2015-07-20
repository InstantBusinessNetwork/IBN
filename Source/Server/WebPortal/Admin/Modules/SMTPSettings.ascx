<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.SMTPSettings" CodeBehind="SMTPSettings.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script type="text/javascript">
	//<![CDATA[
	function SetVisible(obj_id) {
		var obj = document.getElementById(obj_id);
		var obj1 = document.getElementById('<%=txtUser.ClientID%>');
		var obj2 = document.getElementById('<%=txtPassword.ClientID%>');
		if (obj && obj1 && obj2) {
			obj1.disabled = !obj.checked;
			obj2.disabled = !obj.checked;
		}
	}
	//]]>
</script>

<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<colgroup>
		<col width="450px" />
		<col />
	</colgroup>
	<tr>
		<td colspan="2">
			<ibn:BlockHeader ID="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="padding: 10px 0px 15px 15px;">
			<table cellpadding="7" cellspacing="0" class="text">
				<colgroup>
					<col width="140px" />
					<col width="300px" />
				</colgroup>
				<tr>
					<td>
						<b>
							<%=LocRM.GetString("tTitle")%>:</b>
					</td>
					<td>
						<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="250px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rfTitle" runat="server" CssClass="text" ErrorMessage="*" ControlToValidate="txtTitle" Display="Dynamic"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td>
						<b>
							<%=LocRM.GetString("tServer")%>:</b>
					</td>
					<td>
						<asp:TextBox ID="txtServer" runat="server" CssClass="text" Width="250px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<b>
							<%=LocRM.GetString("tPort")%>:</b>
					</td>
					<td>
						<asp:TextBox ID="txtPort" runat="server" CssClass="text" Width="250px"></asp:TextBox>
						<asp:RequiredFieldValidator ID="rfPort" runat="server" Display="Dynamic" ErrorMessage="*"
							ControlToValidate="txtPort"></asp:RequiredFieldValidator>
						<asp:CompareValidator ID="cvPort" runat="server" Display="Dynamic" ErrorMessage="*"
							ControlToValidate="txtPort" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer"></asp:CompareValidator>
					</td>
				</tr>
				<tr>
					<td>
						<b>
							<%=LocRM.GetString("tUseSecureConnection")%>:</b>
					</td>
					<td>
						<asp:RadioButtonList CssClass="text" ID="rbSecurity" runat="server" RepeatDirection="Horizontal">
						</asp:RadioButtonList>
					</td>
				</tr>
				<tr>
					<td>
					</td>
					<td>
						<asp:CheckBox ID="cbAuthenticate" onclick="SetVisible(this.id);" runat="server" CssClass="text">
						</asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td>
						<b>
							<%=LocRM.GetString("tUser")%>:</b>
					</td>
					<td>
						<asp:TextBox ID="txtUser" runat="server" CssClass="text" Width="250px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<b>
							<%=LocRM.GetString("tPassword")%>:</b>
					</td>
					<td>
						<asp:TextBox ID="txtPassword" runat="server" CssClass="text"
							Width="250px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td width="80px">
						<asp:Label runat="server" ID="lbSettingsValid" Visible="false" ForeColor="Red"></asp:Label>
					</td>
					<td align="right" style="padding-right: 40px">
						<asp:Button runat="server" CssClass="text" ID="btCheckSettings" Text="Check"></asp:Button>
					</td>
				</tr>
				<tr>
					<td>
					</td>
					<td>
						<asp:CheckBox ID="cbIsDefault" runat="server" />
					</td>
				</tr>
				<tr valign="bottom" style="height: 50px;">
					<td align="right" colspan="2" style="padding-right: 40px">
						<mc:IMButton runat="server" class="text" ID="imbSave" style="width: 115px">
						</mc:IMButton>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top" style="padding: 10px 50px 50px 50px;">
			<div class="text" style="vertical-align: middle; padding-top: 10px; padding-bottom: 10px;
				background-color: #ffffe1; border: 1px solid #bbb;" id="divCheck" runat="server">
				<blockquote style="border-left: solid 2px #CE3431; padding-left: 10px; margin: 5px;
					margin-left: 15px; padding-top: 3px; padding-bottom: 3px">
					<asp:Label ID="lblMessage" runat="server"></asp:Label>
					<div style="text-align: center; padding-top: 10px">
						<asp:LinkButton ID="lbCheck" runat="server" Font-Bold="true" Font-Underline="true"></asp:LinkButton>
					</div>
				</blockquote>
			</div>
		</td>
	</tr>
</table>
