<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmartTableLayoutItemProperties.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.SmartTableLayoutItemProperties" %>
<div style="padding: 15px;padding-top:7px; border-bottom: 1px solid #adadad" class="ibn-light text">
	<b><asp:Literal ID="Literal1" runat="server" Text='<%$Resources : IbnFramework.MetaForm, Field %>' />:</b>&nbsp;&nbsp;&nbsp;&nbsp;
	<asp:DropDownList ID="ddField" runat="server" Width="250px"></asp:DropDownList>
	<asp:Label ID="lblSourceName" runat="server"></asp:Label>
	<br /><br />
	<asp:CheckBox ID="cbReadOnly" runat="server" />
</div>