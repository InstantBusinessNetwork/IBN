<%@ Reference Control="~/Modules/ControlPlace/CustomizeView.ascx" %>
<%@ Reference Control="~/Modules/MetaDataBlockViewControl.ascx" %>
<%@ Reference Control="~/Modules/ControlPlace/BlockMenuHeader.ascx" %>
<%@ Control Language="c#" Inherits="ControlPlaceApplication.ControlPlace" CodeBehind="ControlPlace.ascx.cs" %>
<%@ Register TagPrefix="mc" TagName="BlockMenuHeader" Src="BlockMenuHeader.ascx" %>
<table id="tbMain" runat="server" cellpadding="0" cellspacing="0" width="100%">
</table>
<mc:BlockMenuHeader ID="tbView" runat="server" />
<input id="txtSourceNewControl" type="hidden" runat="server">
<input id="txtSourceControl" type="hidden" runat="server">
<input id="txtSourceElement" type="hidden" runat="server">
<input id="txtActiveElement" type="hidden" runat="server">
<input id="txtImgSubmit" type="hidden" runat="server">
<asp:Button ID="ImgSubmit" runat="server" Visible="False" OnClick="ImgSubmit_Click" />
<input id="txtHideSubmit" type="hidden" runat="server">
<asp:Button ID="HideSubmit" runat="server" Visible="False" OnClick="HideSubmit_Click" />