<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.DirectoryBrowser" Codebehind="DirectoryBrowser.ascx.cs" %>
<%@ Register TagPrefix="ctrl" TagName="TopTab" src="../../Modules/TopTabs.ascx" %>
<ctrl:TopTab id="ctrlTopTab" runat="server" BaseUrl="Directory.aspx" />
<asp:PlaceHolder ID="phItems" Runat="server" />
