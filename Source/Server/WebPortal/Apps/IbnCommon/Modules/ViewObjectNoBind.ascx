<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewObjectNoBind.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.IbnCommon.Modules.ViewObjectNoBind" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Reference VirtualPath="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<div id="mainDiv" style="overflow:auto;">
	<div style="height:100%">
		<ibn:XMLFormBuilder ID="xmlStruct" runat="server" />
	</div>
</div>
