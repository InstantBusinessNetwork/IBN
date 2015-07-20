<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.PrefsEdit2"
	CodeBehind="PrefsEdit2.ascx.cs" %>

<script type="text/javascript">
//<![CDATA[
function UpdateControls(isBatch)
{
	if(isBatch)
		document.forms[0].<%=hdnBatch.ClientID %>.value = "1";
	else
		document.forms[0].<%=hdnBatch.ClientID %>.value = "0";

	EnableDisable();
}

function EnableDisable()
{
	disable = !(document.forms[0].<%=cbSystemEvents.ClientID %>.checked);
	isBatch = (document.forms[0].<%=hdnBatch.ClientID %>.value == "1");
	
	disable2 = !(document.forms[0].<%=cbReminder.ClientID %>.checked);
	
	OcbEmail2 = document.forms[0].<%=cbRemEmail.ClientID %>;
	if(OcbEmail2 != null)
		OcbEmail2.disabled = disable2;

	OcbIBN2 = document.forms[0].<%=cbRemIBN.ClientID %>
	if(OcbIBN2 != null)	
		OcbIBN2.disabled = disable2;

	document.forms[0].<%=rbByBatch.ClientID %>.disabled = disable;
	document.forms[0].<%=rbInstantly.ClientID %>.disabled = disable;

	OcbEmail = document.forms[0].<%=cbEmail.ClientID %>;
	if(OcbEmail != null)
		OcbEmail.disabled = disable;

	OcbIBN = document.forms[0].<%=cbIBN.ClientID %>
	if(OcbIBN != null)	
		OcbIBN.disabled = disable;

	document.forms[0].<%=ddEvery.ClientID %>.disabled = (disable || !isBatch);
	document.forms[0].<%=ddFrom.ClientID %>.disabled = (disable || !isBatch);
	document.forms[0].<%=ddTo.ClientID %>.disabled = (disable || !isBatch);
}
//]]>
</script>

<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td valign="top" colspan="2" width="67%">
			<table cellpadding="3" cellspacing="3" border="0" width="100%">
				<tr>
					<td class="text" style="padding-left: 10px; width: 120px">
						<asp:Label ID="lblTimeZoneTitle" runat="server" CssClass="ibn-label"></asp:Label>
					</td>
					<td>
						<asp:DropDownList ID="lstTimeZone" runat="server" CssClass="text">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="text" style="padding-left: 10px">
						<asp:Label ID="lblLangTitle" runat="server" CssClass="ibn-label"></asp:Label>
					</td>
					<td>
						<asp:DropDownList ID="lstLang" runat="server" CssClass="text" Width="94px">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>
					</td>
					<td>
						<asp:CheckBox ID="cbMenuInAlerts" CssClass="text" runat="server"></asp:CheckBox>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<table class="text" cellspacing="0" cellpadding="4" width="100%" border="0">
	<tr>
		<td valign="top">
			<fieldset>
				<legend class="text">
					<asp:CheckBox ID="cbReminder" runat="server" CssClass="text" onclick="EnableDisable();" />
				</legend>
				<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td width="30">
							&nbsp;
						</td>
						<td>
							<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td colspan="2">
										<font style="color: #0046E3" class="text">
											<%=LocRM.GetString("NotifyBy")%>:</font><br/>
										<asp:CheckBox ID="cbRemEmail" runat="server" CssClass="text" /><br/>
										<asp:CheckBox ID="cbRemIBN" runat="server" CssClass="text" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</fieldset>
		</td>
	</tr>
	<tr>
		<td>
			<fieldset>
				<legend class="text">
					<asp:CheckBox ID="cbSystemEvents" runat="server" CssClass="text" onclick="EnableDisable();" />
				</legend>
				<table class="text" cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td width="30">
							&nbsp;
						</td>
						<td>
							<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td colspan="2" runat="server" id="fsNotification">
										<font style="color: #0046E3" class="text">
											<%=LocRM.GetString("NotifyBy")%>:</font><br/>
										<asp:CheckBox ID="cbEmail" runat="server" CssClass="text" /><br/>
										<asp:CheckBox ID="cbIBN" runat="server" CssClass="text" />
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<asp:RadioButton GroupName="Group1" ID="rbInstantly" runat="server" CssClass="text"
											onclick="UpdateControls(false);" />
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<asp:RadioButton GroupName="Group1" ID="rbByBatch" runat="server" CssClass="text"
											onclick="UpdateControls(true);" />
									</td>
								</tr>
								<tr>
									<td>
										&nbsp;
									</td>
									<td>
										<asp:Label ID="lblEvery" CssClass="text" runat="server"></asp:Label>
										<asp:DropDownList ID="ddEvery" CssClass="Text" runat="server" Width="140px" Enabled="False">
										</asp:DropDownList>
									</td>
								</tr>
								<tr>
									<td>
										&nbsp;
									</td>
									<td>
										<asp:Label ID="lblFrom" CssClass="text" runat="server"></asp:Label>
										<asp:DropDownList ID="ddFrom" runat="server" CssClass="Text" Enabled="False">
										</asp:DropDownList>
										&nbsp;
										<asp:Label ID="lblTo" CssClass="text" runat="server"></asp:Label>
										<asp:DropDownList ID="ddTo" runat="server" CssClass="Text" Enabled="False">
										</asp:DropDownList>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
				<br/>
			</fieldset>
		</td>
	</tr>
</table>
<input id="hdnBatch" type="hidden" runat="server" value="0" name="hdnBatch" />
<input type="hidden" id="oldLang" runat="server" />
<input type="hidden" id="oldTime" runat="server" />