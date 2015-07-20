<%@ Control Language="c#" Inherits="ControlPlaceApplication.BlockMenuHeader" Codebehind="BlockMenuHeader.ascx.cs" %>
<div id="divBMenu" onmouseover="CancelClosing()" onmouseout="PrepareClosing(this.id)" runat=server style="background-color:white;padding:2px;border:solid 1px #6486D4;border-bottom:solid 2px #7E7E81;border-right:2px solid #7E7E81;position:absolute;top:80px;left:100px;width:100px;z-index:255;display:none;">
<table id="tblBMenu" class="text" runat=server width="100%" height="100%" cellpadding="2" cellspacing="0" border="0">
</table>
</div>