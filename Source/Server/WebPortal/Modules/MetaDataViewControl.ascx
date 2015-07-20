<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.MetaDataViewControl" Codebehind="MetaDataViewControl.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="tbMetaInfo" runat="server" />
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5"	border="0" runat="server" id="tblCustomFields" width="100%">
</table>
