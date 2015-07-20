<%@ Reference Control="~/Wizards/Modules/WizardTemplate.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.CommonWizard" Codebehind="CommonWizard.ascx.cs" %>
<asp:Panel ID="basic" Runat="server">
	<BR>
	<table cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td width="250" class="SubHeader"><B><asp:Label id="lblTextHeader" Runat="server"></asp:Label></B><BR><br></td>
			<td vAlign="top" align="middle" width="60"></td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"></td>
		</tr>
		<tr>
			<td width="250">
				<asp:RadioButtonList id="rbActions" Runat="server" CssClass="text"></asp:RadioButtonList></td>
			<td vAlign="top" align="middle" width="60"><IMG alt="" src="../layouts/images/quicktip.gif" border="0">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"><%=GlobalString%></td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="upload" Runat="server" Visible="False">
	<div>
		</div><br>
	<table width="100%">
		<tr>
			<td vAlign="top" width="50%">
				<table width="100%">
					<tr>
						<td>
							
							<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px">
								<%=LocRM.GetString("UploadFileFor_" + ObjectName.ToLower())%></div>
							<cc1:McHtmlInputFile id="ffileUp" width="280px" CssClass="text" runat="server"></cc1:McHtmlInputFile>
							<asp:RequiredFieldValidator id="rffileUp" runat="server" ControlToValidate="ffileUp" ErrorMessage="*"></asp:RequiredFieldValidator>
						</td>
					</tr>
				</table>
			</td>
			<td vAlign="top" align="middle" width="60">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"></td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="categories" Runat="server" Visible="False">
	<div>
		<asp:Label id="lblDivCategories" Runat="server"></asp:Label></div><br>
	<table cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td vAlign="top" width="50%">
				<table>
					<tr>
						<td>
							<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("SelectCategories") %></div>
							<asp:ListBox id="lbCategories" runat="server" Rows="7" SelectionMode="Multiple" Width="260px"></asp:ListBox></td>
					</tr>
				</table>
			</td>
			<td vAlign="top" align="middle" width="60">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"></td>
		</tr>
	</table>
</asp:Panel>
<asp:Panel ID="comments" Runat="server" Visible="False">
	<div>
		</div><br>
	<table cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td vAlign="top" width="50%">
				<table>
					<tr>
						<td>
							<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("AddComments") %></div>
							<asp:TextBox id="txtComments" Runat="server" CssClass="text" Width="260px" Height="150px" TextMode="MultiLine"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfComments" ControlToValidate="txtComments" CssClass=text Display=Dynamic Runat=server ErrorMessage="*"></asp:RequiredFieldValidator>
 							</td>
					</tr>
				</table>
			</td>
			<td vAlign="top" align="middle" width="60">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"></td>
		</tr>
	</table>
</asp:Panel>
<script language="javascript">
	
	function ShowProgress()
	{
		if(document.forms[0].<%=ffileUp.ClientID%>.value!="")
		{
			var w = 300;
			var h = 140;
			var l = (screen.width - w) / 2;
			var t = (screen.height - h) / 2;
			winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
			var f = window.open('../External/Progress.aspx?ID='+document.forms[0].__MEDIACHASE_FORM_UNIQUEID.value, "_blank", winprops);
		}
	}
</script>