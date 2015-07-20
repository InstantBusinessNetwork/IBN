<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.TopTabs" CodeBehind="TopTabs.ascx.cs" %>

<script type="text/javascript">
	//<![CDATA[
	function copy(text) {
		if (window.clipboardData) {
			window.clipboardData.setData('Text', text);
		}
		else {
			var flashcopier = 'flashcopier';
			if (!document.getElementById(flashcopier)) {
				var divholder = document.createElement('div');
				divholder.id = flashcopier;
				document.body.appendChild(divholder);
			}
			document.getElementById(flashcopier).innerHTML = '';
			var link = '<%= ResolveClientUrl("~/Scripts/clipboard.swf") %>';
			var divinfo = '<embed src="' + link + '" FlashVars="clipboard=' + escape(text) + '" width="0" height="0" type="application/x-shockwave-flash"></embed>';
			document.getElementById(flashcopier).innerHTML = unescape(divinfo);
		}
	}
	//]]>
</script>

<table cellpadding="0" cellspacing="0" style="border:0; width:100%">
	<tr>
		<td>
			<table cellpadding="0" cellspacing="0" style="border:0; width:100%; margin-top: 5px;">
				<tr id="trTabs" runat="server"></tr>
			</table>
		</td>
	</tr>
	<tr id="tbl2" runat="server">
		<td>
			<table cellpadding="0" cellspacing="0" style="border:0; width:100%; margin-top: 5px;">
				<tr id="trTabs1" runat="server"></tr>
			</table>
		</td>
	</tr>
</table>
<asp:LinkButton CausesValidation="false" ID="lbSelectTab" runat="server" Style="display: none;"></asp:LinkButton>