<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddFrameBuilder.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.Common.Layout.Modules.AddFrameBuilder" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<div id="mainDiv" style="overflow:auto;">
	<div style="height:100%">
		<ibn:XMLFormBuilder ID="xmlStruct" runat="server" LayoutMode="Default" />
	</div>
</div>