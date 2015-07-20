<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityView" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Reference VirtualPath="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>

<script type="text/javascript" src='<%= this.ResolveUrl("~/Apps/Common/Scripts/SelectPopupScript.js") %>'></script>
<script type="text/javascript">
	//<![CDATA[
	if (window.addEventListener)
		window.addEventListener("load", function(e) { ResizeContent(e); }, false);
	else
		window.attachEvent("onload", function(event) { ResizeContent(event); });

	function ResizeContent(sender, args) {
		var leftMenuCell = document.getElementById("<%=xmlStruct.LeftMenuCellId %>");
		var topTabCell = document.getElementById("<%=xmlStruct.TabCellId %>");
		var contentCell = document.getElementById("<%=xmlStruct.ContentCellId %>");

		if (leftMenuCell && topTabCell && contentCell) {
			var h1 = getObjectHeight(leftMenuCell);
			var h2 = getObjectHeight(topTabCell);
			var delta = 0;
			if (browseris.ie)
				delta = 1;
			if (h1 > 0 && h2 > 0)
				contentCell.style.height = (h1 - h2 - delta) + "px";
		}
	}
	//]]>
</script>

<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-stylebox2 ibn-propertysheet">
	<tr>
		<td valign="top">
			<div id="mainDiv" style="overflow: auto;">
				<div>
					<ibn:XmlFormBuilder ID="xmlStruct" runat="server" />
				</div>
			</div>
		</td>
	</tr>
</table>
