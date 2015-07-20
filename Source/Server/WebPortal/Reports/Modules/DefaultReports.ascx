<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.DefaultReports" Codebehind="DefaultReports.ascx.cs" %>
<%@ Register TagPrefix="ctrl" TagName="TopTab" src="../../Modules/TopTabs.ascx" %>
<ctrl:TopTab id="ctrlTopTab" runat="server" BaseUrl="DefaultAdmin.aspx" />
<asp:PlaceHolder ID="phItems" Runat="server" />