<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.CustomizeHomePage" Codebehind="CustomizeHomePage.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<script language="JavaScript">
	function cidentitychange()
	{
		var strFile = document.forms[0].<%=cidentity.ClientID%>.value;
		if (strFile != "")
		{
			document.forms[0].img_cidentity.src = "file:///" + strFile;
		}
	}

</script>
<table cellspacing="0" cellpadding="7" border="0" class="ibn-stylebox2 text" width="100%">
	<tr>
		<td style="padding:0px">
			<ibn:blockheader id="secHeader" runat="server" title="" />
		</td>
	</tr>
	<tr>
		<td>
			<table cellspacing="0" cellpadding="5" width="100%" border="0" class="text">
				<tr>
					<td><b><%=LocRM.GetString("HPTextTitle1") %>:</b></td>
					<td>
						<asp:textbox id="txtTextTitle1" cssclass="ibn-input" maxlength="100" width="300" runat="server"></asp:textbox>
					</td>
				</tr>
				<tr>
					<td valign="top" ><b><%=LocRM.GetString("HPText1") %>:</b>&nbsp;</td>
					<td>
						<FTB:FreeTextBox id="hpText1" 
							ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu" 
							runat="Server" Height="150px" EnableHtmlMode="true"
							DropDownListCssClass = "text"  StartMode="DesignMode"
							GutterBackColor="#F5F5F5" BreakMode = "LineBreak" BackColor="#F5F5F5"
							SupportFolder = "~/Scripts/FreeTextBox/"
							JavaScriptLocation="ExternalFile" 
							ButtonImagesLocation="ExternalFile"
							ToolBarImagesLocation="ExternalFile" />
					</td>
				</tr>
				<tr>
					<td><b><%=LocRM.GetString("HPTextTitle2") %>:</b>&nbsp;</td>
					<td>
						<asp:textbox id="txtTextTitle2" cssclass="ibn-input" maxlength="100" width="300" runat="server"></asp:textbox>
					</td>
				</tr>
				<tr>
					<td valign="top" ><b><%=LocRM.GetString("HPText2") %>:</b>&nbsp;</td>
					<td >
						<FTB:FreeTextBox id="hpText2" 
						ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu" 
						runat="Server" Height="150px" EnableHtmlMode="true" 
							DropDownListCssClass = "text"  StartMode="DesignMode"
							GutterBackColor="#F5F5F5" BreakMode = "LineBreak" BackColor="#F5F5F5"
							SupportFolder = "~/Scripts/FreeTextBox/"
							JavaScriptLocation="ExternalFile" 
							ButtonImagesLocation="ExternalFile"
							ToolBarImagesLocation="ExternalFile" />
					</td>
				</tr>
				<tr>
					<td valign="top"><b><%=LocRM.GetString("IdentityGraphic") %>:</b>&nbsp;</td>
					<td>
						<cc1:McHtmlInputFile id="cidentity" class="form" runat="server" size="40" onclick="cidentitychange();" onkeypress="cidentitychange();" onpropertychange="cidentitychange()" cssclass="text" />
						<asp:button runat="server" id="btnReset" cssclass="text" text="Reset" onclick="btnReset_Click"></asp:button>
						<div style="BORDER-RIGHT:1px solid; BORDER-TOP:1px solid; OVERFLOW-Y:auto; OVERFLOW-X:auto; BORDER-LEFT:1px solid; WIDTH:102px; BORDER-BOTTOM:1px solid; HEIGHT:52px"><img id="img_cidentity" name="img_cidentity" src='<%= ResolveClientUrl("~/Common/CompanyIdentity.aspx") %>' width="100" height="50"></div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<table cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td align="right" style="PADDING:5px">
						<btn:ImButton ID="btnSave" Runat="server" Class="text" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:ImButton>&nbsp;&nbsp;
						<btn:ImButton ID="btnCancel" CausesValidation="false" Runat="server" Class="text" IsDecline="true" style="width:110px;" onserverclick="btnCancel_Click"></btn:ImButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
