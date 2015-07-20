<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetaClassInfo.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls.MetaClassInfo" %>
<div class="ibn-light">
<table class="ibn-propertysheet ibn-underline" width="100%" border="0" cellpadding="5" cellspacing="0">
	<tr>
		<td class="ibn-label" style="width:160px;"><asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>" />:</td>
		<td>
			<asp:HyperLink runat="server" ID="FriendlyNameLink"></asp:HyperLink>
			<asp:Label runat="server" ID="FriendlyNameLabel" Visible="false"></asp:Label>
		</td>
		<td class="ibn-label" style="width:140px;"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, SystemName%>" />:</td>
		<td class="ibn-value">
			<asp:Image runat="server" ID="ClassTypeImage" ImageAlign="AbsMiddle" Width="16px" Height="16px" />
			<asp:Label runat="server" ID="ClassNameLabel"></asp:Label>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" style="width:160px;"><asp:Literal ID="Literal4" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, PluralName%>" />:</td>
		<td>
			<asp:Label runat="server" ID="PluralNameLabel"></asp:Label>
		</td>
		<td class="ibn-label" style="width:140px;"><asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Type%>" />:</td>
		<td class="ibn-value">
			<asp:Label runat="server" ID="TypeLabel"></asp:Label>
		</td>
	</tr>
	<tr runat="server" id="MoreInfoRow">
		<td class="ibn-label" style="width:160px;" valign="top"><asp:Label runat="server" ID="MoreInfoLabel"></asp:Label></td>
		<td class="ibn-value" colspan="3">
			<asp:Label runat="server" ID="MoreInfoValue"></asp:Label>
		</td>
	</tr>
</table>
</div>