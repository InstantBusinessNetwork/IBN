<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ie6updatePopup.ascx.cs" Inherits="Mediachase.UI.Web.Modules.ie6updatePopup" %>
<script type="text/javascript">
function hideAllDropDown()
{
	var list = document.getElementsByTagName('select');
	for (var i = 0; i < list.length; i++)
	{
		list[i].style.display = 'none';
	}
}

function showAllDropDown()
{
	var list = document.getElementsByTagName('select');
	for (var i = 0; i < list.length; i++)
	{
		list[i].style.display = 'inline';
	}
}
</script>
<div runat="server" id="divPopupIe" style="background-color: #FFFFE1; position: absolute; display: none; height: 75px; width: 95%; left: 2%; border: solid 1px #BBBBBB; z-index: 100000;">
	<div style="position: relative; width: 100%; height: 100%;">
		<img style="float: right; cursor: pointer;" alt='delete' src='<%= this.ResolveUrl("~/Images/IbnFramework/closeTab.gif") %>' border='0' onclick="document.getElementById('<%= divPopupIe.ClientID %>').style.display = 'none'; showAllDropDown();"/>
		<div style="position: absolute; top: 20px;"><%= Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_ie6update}")%></div>
	</div>
</div>