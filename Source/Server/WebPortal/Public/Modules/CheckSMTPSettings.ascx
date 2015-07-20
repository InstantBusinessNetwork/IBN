<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Public.Modules.CheckSMTPSettings" CodeBehind="CheckSMTPSettings.ascx.cs" %>
<div class="text" style="vertical-align:middle; padding-top: 10px; padding-bottom: 10px; background-color: #ffffe1; border: 1px solid #bbb;">
	<blockquote style="border-left: solid 2px #CE3431; padding-left: 10px; margin: 5px; margin-left: 15px; padding-top: 3px; padding-bottom: 3px">
		<div id="labelCheckedTrue" runat="server" class="text"><%=LocRM.GetString("tIsCheckedTrue")%></div>
		<div id="labelCheckedFalse" runat="server" class="text"><%=LocRM.GetString("tIsCheckedFalse")%></div>
		<div style="text-align:center;font-weight:bold">
			<div id="divAdminCheckedTrue" runat="server">
				<a href="<%= ResolveUrl("~/Admin/SMTPList.aspx")%>" style="color:green"><%=LocRM.GetString("tReturnSMTP")%></a>
			</div>
			<div id="divAdminCheckedFalse" runat="server">
				<a href="<%= ResolveUrl("~/Admin/SMTPList.aspx")%>" style="color:red"><%=LocRM.GetString("tReturnSMTP")%></a>
			</div>
			<div id="divUserCheckedTrue" runat="server">
				<a href="<%= ResolveUrl("~/Public/default.aspx")%>" style="color:green"><%=LocRM.GetString("tInIBN")%></a>
			</div>
			<div id="divUserCheckedFalse" runat="server">
				<a href="<%= ResolveUrl("~/Public/default.aspx")%>" style="color:red"><%=LocRM.GetString("tInIBN")%></a>
			</div>
		</div>
	</blockquote>
</div>
