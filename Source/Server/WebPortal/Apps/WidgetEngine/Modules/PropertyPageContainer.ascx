<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PropertyPageContainer.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules.PropertyPageContainer" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
function getInternetExplorerVersion() 
    {
        var rv = -1; // Return value assumes failure.
        if (navigator.appName == 'Microsoft Internet Explorer') 
        {
            var ua = navigator.userAgent;
            var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
            if (re.exec(ua) != null)
                rv = parseFloat(RegExp.$1);
        }
        return rv;
    }
	var resizeFlag = false;
	function LayoutResizeHandler(sender, eventArgs)
	{
	    if (getInternetExplorerVersion() >= 8)
	    {
	        document.getElementById('<%= mainContainer.ClientID %>').style.height = eventArgs._height - parseInt(sender._containerId.style.marginBottom) - parseInt(sender._containerId.style.marginTop) + 20 + 'px';
	        document.getElementById('<%= mainContainer.ClientID %>').style.width = eventArgs._width - parseInt(sender._containerId.style.marginBottom) - parseInt(sender._containerId.style.marginTop) + 20 + 'px';
	        window.setTimeout(function() { sender._containerId.style.height = eventArgs._height - parseInt(sender._containerId.style.marginBottom) - parseInt(sender._containerId.style.marginTop)  + 'px'; }, 10);
	    }
	}
</script>
<style type="text/css" rel="Stylesheet">
.LayoutBase
{
	background-color: White ! important;
}
</style>
<div runat="server" id="generalContainer" style="background-color: White; ">
	<div runat="server" id="mainContainer" style="background-color: White; padding: 5px;" class="text">
	</div>
</div>
<mc2:McDock ID="DockBottom" runat="server" Anchor="Bottom" EnableSplitter="false" DefaultSize="35" InnerPadding="0" CssClass="propertyPageBottom" InnerCssClass="propertyPageBottom">
	<DockItems>
		<div runat="server" id="buttonContainer" style="border-top: solid 1px #ccc; padding-top: 5px; background-color: #f0f0f0;">
			<div style="float:right; margin-right: 15px; background-color: #f0f0f0;">
				<asp:Button runat="server" id="btnSaveReal" Text="Save" />
				<input type="button" runat="server" id="btnCancel" value="Cancel" />
			</div>
		</div>
	</DockItems>
</mc2:McDock>
