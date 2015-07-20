<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ReportManagement.Modules.ReportView" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
	Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<script type="text/javascript">
function resizeTable()
{
	var panel1 = document.getElementById('trHeader');
	var panel2 = document.getElementById('trFilter');
	
	var obj = document.getElementById('<%=rvMain.ClientID %>');

	var intHeight = 0;
	if (typeof(window.innerWidth) == "number")
	{
		intHeight = window.innerHeight;
	}
	else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
	{
		intHeight = document.documentElement.clientHeight;
	}
	else if (document.body && (document.body.clientWidth || document.body.clientHeight))
	{
		intHeight = document.body.clientHeight;
	}

	if(obj && panel1 && panel2)
	{
		obj.style.height = (intHeight - panel1.offsetHeight - panel2.offsetHeight - <%=Request.Browser.Browser.IndexOf("IE")>=0 ? 15 : 50 %>) + "px";
	}
} 
window.onresize=resizeTable; 
window.onload=resizeTable; 
</script>
<table cellpadding="0" cellspacing="0" width="100%" class="ibn-propertysheet ibn-stylebox2">
	<tr id="trHeader">
		<td><mc:BlockHeader2 runat="server" ID="BlockHeaderMain" /></td>
	</tr>
	<tr id="trFilter">
		<td style="padding:5px;" class="ibn-alternating ibn-navline">
			<asp:PlaceHolder ID="phFilter" runat="server"></asp:PlaceHolder>
		</td>
	</tr>
	<tr>
		<td>
			<rsweb:ReportViewer ID="rvMain" runat="server" Width="100%" HyperlinkTarget="right">
			</rsweb:ReportViewer>		
		</td>
	</tr>
</table>