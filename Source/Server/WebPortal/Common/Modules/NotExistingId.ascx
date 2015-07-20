<%@ Reference Page="~/Common/NotExistingId.aspx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Common.Modules.NotExistingId" Codebehind="NotExistingId.ascx.cs" %>
<div id="divMessage" runat="server" class="text" style="vertical-align:middle; padding-top:10px; padding-bottom:10px; background-color:#ffffe1;border:1px solid #bbb;">
  <blockquote style="padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
    <div style="float:left;padding-right:10px;"><img src="../layouts/images/deleteuser.gif" align="absmiddle" /></div>
    <div style="float:left;padding-top:5px;"><asp:Label ID="lblMessage" runat="server"></asp:Label></div>
    <div style="clear:both;padding-top:10px;padding-left:45px;">
      <asp:LinkButton ID="btnSMTP" runat="server" Font-Bold="true" ForeColor="Red" Font-Underline="true" OnClick="btnSMTP_Click"></asp:LinkButton>
    </div>
  </blockquote>
</div>