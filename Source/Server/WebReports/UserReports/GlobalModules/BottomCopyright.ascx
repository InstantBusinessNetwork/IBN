<%@ Control Language="c#" Inherits="Mediachase.UI.Web.UserReports.GlobalModules.BottomCopyright" Codebehind="BottomCopyright.ascx.cs" %>
<script lang="JavaScript">
<!--
	function OpenPopUpWindow(sLink,sTitle)
		{
				var w = 650;
				var h = 400;
				var l = (screen.width - w) / 2;
				var t = (screen.height - h) / 2;
				winprops = 'scrollbars=1, resizable=0, height='+h+',width='+w+',top='+t+',left='+l+'w';
				var f = window.open(sLink, sTitle, winprops);
		}

//-->
</script>
<span id="loggedincopyright" runat=server Visible="false"><a  href='../Directory/UserView.aspx?UserID=<%=Mediachase.IBN.Business.Security.CurrentUser.UserID%>'>
		<%=LocRM.GetString("ManageYourProfile") %>
	</a>| <a href='../Home/ContactUs.aspx'>
		<%=LocRM.GetString("ContactUs") %>
	</a>| <a href='../Home/download.aspx'>
		<%=LocRM.GetString("Download") %>
	</a>| <a href='http://www.mediachase.com' target=_blank runat=server id="link1">
		<%=LocRM.GetString("MediachaseToday") %>
	</a>
	<br>
</span>
<asp:label ID="lblCopyright" Runat=server></asp:label>
<a runat=server id="link2">
	<asp:Label ID="lblTermsOfUse" Runat=server Visible=True></asp:Label>
</a><a runat=server id="link3">
	<asp:Label ID="lblPrivacy" Runat=server Visible=True></asp:Label>
</a><asp:Label ID="lblVersion" Runat=server></asp:Label>
