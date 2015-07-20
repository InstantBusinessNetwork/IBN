<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormDocumentView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.FormDocumentView" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.Controls.Util" Assembly="Mediachase.Ibn.Web.UI.Controls.Util" %>
<%@ Register TagPrefix="mc2" Namespace="Mediachase.Ibn.Web.UI" Assembly="Mediachase.UI.Web" %>
<div runat="server" id="OuterDiv">
<mc:FormRenderer ID="fRenderer" runat="server" TableLayoutMode="View">
	<SectionHeaderTemplate>
		<mc2:BlockHeaderLight HeaderCssClass="ibn-toolbar-light" id="bhl" runat="server" Title='<%# Eval("Title") %>'></mc2:BlockHeaderLight>
	</SectionHeaderTemplate>
</mc:FormRenderer>
</div>