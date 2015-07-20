<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormHelper.ascx.cs" Inherits="Mediachase.UI.Web.Modules.FormHelper" %>
<img alt="?" src="../Layouts/images/help2.gif" style="cursor:pointer;" onclick="onAction(this,'<%=Position%>',event);"/><div id="mainDiv" runat="server" style="position:absolute; width:350px; background-color: #FFE4B5; border: solid 1px #333333; display:none">
	<img alt="X" src="../Layouts/images/close.gif" id="imgClose" runat="server" hspace="0" vspace="0" align="right" class="borderWhite" style="cursor:pointer; border:0" onclick="offAction(event);" />
	<div style="padding:5px;"><asp:Label runat="server" ID="lblText"></asp:Label></div>
</div>
