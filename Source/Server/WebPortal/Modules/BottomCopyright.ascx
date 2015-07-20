<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.BottomCopyright" Codebehind="BottomCopyright.ascx.cs" %>
<div Printable="0">
	<span id="loggedincopyright" runat="server" visible="false"><a  href='<%= ResolveClientUrl("~/Directory/UserView.aspx?UserID=" + Mediachase.Ibn.Data.Services.Security.CurrentUserId) %>'>
			<%=LocRM.GetString("ManageYourProfile") %>
		</a>| <a href='<%= ResolveClientUrl("~/Home/ContactUs.aspx") %>'>
			<%=LocRM.GetString("ContactUs") %>
		</a>| <a href='<%= ResolveClientUrl("~/Home/download.aspx") %>'>
			<%=LocRM.GetString("Download") %>
		</a>| <a href='http://www.mediachase.com' target="_blank" runat="server" id="link1">
			<%=LocRM.GetString("MediachaseToday") %>
		</a>
		<br />
	</span>
	<asp:label ID="lblCopyright" Runat="server"></asp:label>
	<a runat="server" id="link2">
		<asp:Label ID="lblTermsOfUse" Runat="server" Visible="True"></asp:Label>
	</a><a runat="server" id="link3">
		<asp:Label ID="lblPrivacy" Runat="server" Visible="True"></asp:Label>
	</a><asp:Label ID="lblVersion" Runat="server"></asp:Label>
</div>