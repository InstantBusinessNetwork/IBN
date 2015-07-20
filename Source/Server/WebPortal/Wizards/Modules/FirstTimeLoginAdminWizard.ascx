<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.FirstTimeLoginAdminWizard" Codebehind="FirstTimeLoginAdminWizard.ascx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<script language="javascript">
<!--
function SwitchAlerts()
{
	try
	{
	fc = document.forms[0];
	cb = fc.<%=cbEnableAlerts.ClientID %>
	
	fc.<%=tbAlertSenderFirstName.ClientID %>.disabled = !cb.checked;
	fc.<%=tbAlertSenderLastName.ClientID %>.disabled = !cb.checked;
	fc.<%=tbAlertSenderEmail.ClientID %>.disabled = !cb.checked;
	
	ValidatorEnable(<%=RequiredFieldValidator1.ClientID%>, cb.checked);
	ValidatorEnable(<%=RequiredFieldValidator2.ClientID%>, cb.checked);
	ValidatorEnable(<%=RequiredFieldValidator3.ClientID%>, cb.checked);
	}
	catch(e){}
}
//-->
</script>
<script language="javascript">
	function clogochange()
	{
		var strFile = document.forms[0].<%=clogo.ClientID%>.value;
		if (strFile != "")
		{
			document.forms[0].img_clogo.src = "file:///" + strFile;
		}
	}
	
	function cidentitychange()
	{
		var strFile = document.forms[0].<%=cidentity.ClientID%>.value;
		if (strFile != "")
		{
			document.forms[0].img_cidentity.src = "file:///" + strFile;
		}
	}
</script>
<asp:Panel ID="step1" Runat="server" Visible="False">
	<table cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td style="PADDING-RIGHT: 7px; PADDING-LEFT: 7px; PADDING-BOTTOM: 7px; PADDING-TOP: 7px" vAlign=top width="50%">
				<FIELDSET align=middle><LEGEND>
						<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
							<asp:CheckBox id=cbEnableAlerts onclick=SwitchAlerts() Runat="server" Text='<%#LocRM.GetString("s1EnableAlerts") %>'>
							</asp:CheckBox>&nbsp;</div>
					</LEGEND>
					<CENTER>
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td>
									<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 0px"><%=LocRM.GetString("s1AlertSenderFirstName") %></div>
									&nbsp;&nbsp;
									<asp:TextBox id=tbAlertSenderFirstName Runat="server"></asp:TextBox>
									<asp:RequiredFieldValidator id=RequiredFieldValidator1 ControlToValidate="tbAlertSenderFirstName" runat="server" ErrorMessage="&amp;nbsp;*"></asp:RequiredFieldValidator>
									<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px"><%=LocRM.GetString("s1AlertSenderLastName") %></div>
									&nbsp;&nbsp;
									<asp:TextBox id=tbAlertSenderLastName Runat="server"></asp:TextBox>
									<asp:RequiredFieldValidator id=RequiredFieldValidator2 ControlToValidate="tbAlertSenderLastName" runat="server" ErrorMessage="&amp;nbsp;*"></asp:RequiredFieldValidator>
									<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px"><%=LocRM.GetString("s1AlertSenderEmail") %></div>
									&nbsp;&nbsp;
									<asp:TextBox id=tbAlertSenderEmail Runat="server"></asp:TextBox>
									<asp:RequiredFieldValidator id=RequiredFieldValidator3 ControlToValidate="tbAlertSenderEmail" runat="server" ErrorMessage="&amp;nbsp;*" Display="Dynamic"></asp:RequiredFieldValidator>
									<asp:RegularExpressionValidator id=RegularExpressionValidator1 ControlToValidate="tbAlertSenderEmail" runat="server" ErrorMessage="&amp;nbsp;*" Display="Dynamic" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
									<asp:CustomValidator id=cvEmail Runat="server" Display="Dynamic"></asp:CustomValidator></td>
							</tr>
							<tr>
								<td height=10>&nbsp;</td>
							</tr>
						</table>
					</CENTER>
				</FIELDSET>
			</td>
			<td style="PADDING-TOP: 20px" vAlign=top align=middle width=60><IMG alt="" src="../layouts/images/quicktip.gif" border=0>
			</td>
			<td class=text style="PADDING-RIGHT: 15px; PADDING-TOP: 20px" vAlign=top><%=LocRM.GetString("s1QuickTip") %></td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="step3" Runat="server">
	<FIELDSET><LEGEND id="lgdCompanyInfo" class=subHeader runat="server">fff</legend>
	<table cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td vAlign=top width="50%">
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><%=LocRM.GetString("s3Title1") %></div>
				&nbsp;&nbsp;
				<asp:TextBox id=s3Title1 Runat="server" Width="260px"></asp:TextBox>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><%=LocRM.GetString("s3Text1") %></div>
				&nbsp;&nbsp;
				<asp:TextBox id=s3Text1 Runat="server" Width="260px" TextMode="MultiLine" Rows="3"></asp:TextBox>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><%=LocRM.GetString("s3Title2") %></div>
				&nbsp;&nbsp;
				<asp:TextBox id=s3Title2 Runat="server" Width="260px"></asp:TextBox>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><%=LocRM.GetString("s3Text2") %></div>
				&nbsp;&nbsp;
				<asp:TextBox id=s3Text2 Runat="server" Width="260px" TextMode="MultiLine" Rows="3"></asp:TextBox></td>
			<td vAlign=top align=middle width=5>&nbsp;
			</td>
			<td class=text style="PADDING-RIGHT: 15px" vAlign=top>
				<table cellspacing="0" cellpadding="0" width="100%" border=0>
					<tr>
						<td>
							<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><%=LocRM.GetString("Logo") %></div>
							<div style="BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; OVERFLOW-Y: auto; OVERFLOW-X: auto; BORDER-LEFT: 1px solid; WIDTH: 102px; BORDER-BOTTOM: 1px solid; HEIGHT: 52px"><img alt="" id="img_clogo" src="<%=Mediachase.UI.Web.Util.CommonHelper.GetCompanyLogoUrl(Page)%>"/></div>
							<cc1:McHtmlInputFile class=form onkeypress=clogochange(); id=clogo onpropertychange=clogochange() onclick=clogochange(); runat="server" cssclass="text" size="30"></cc1:McHtmlInputFile></td>
					</tr>
					<tr>
						<td>
							<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><%=LocRM.GetString("IdentityGraphic") %></div>
							<div style="BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; OVERFLOW-Y: auto; OVERFLOW-X: auto; BORDER-LEFT: 1px solid; WIDTH: 102px; BORDER-BOTTOM: 1px solid; HEIGHT: 52px"><IMG id=img_cidentity height=50 src="../common/CompanyIdentity.aspx" width=100 name=img_cidentity></div>
							<cc1:McHtmlInputFile class=form onkeypress=cidentitychange(); id=cidentity onpropertychange=cidentitychange() onclick=cidentitychange(); runat="server" cssclass="text" size="30"></cc1:McHtmlInputFile></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</fieldset>
	<FIELDSET id="fsFDOW" runat=server><LEGEND id="lgdFDOW" class=subHeader runat="server"></legend>
	<table cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td style="padding-left: 7px">
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 3px; PADDING-TOP: 3px"><%=LocRM.GetString("tFirstDayOfWeek") %>:</div>
			</td>
			<td style="padding-left: 7px">
				<asp:DropDownList ID="ddFDOW" Runat=server Width="150px"></asp:DropDownList>
			</td>
		</tr>
	</table>
	</fieldset>
</asp:Panel>
<asp:Panel ID="step4" Runat="server" Visible="False">
	<table height="100%" cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td vAlign=center width="100%">
				<table width="100%">
					<tr>
						<td width=40></td>
						<td vAlign=top width=80><IMG src="../layouts/images/check.gif" align=absMiddle></td>
						<td class=text vAlign=top><B><%=LocRM.GetString("s4Success") %></B><BR>
							<BR>
							<%=LocRM.GetString("s4SText") %>
						</td>
						<td width=40></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td vAlign=bottom>
				<asp:CheckBox id=cbShowNextTime Runat="server" Text='<%#LocRM.GetString("ShowOnNextLogin") %>' CssClass="subHeader" Checked="True">
				</asp:CheckBox></td>
		</tr>
	</table>
</asp:Panel>
